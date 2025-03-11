using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SanfordTest.Models;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using SanfordTest.Configurations;
using Microsoft.Extensions.Options;
using System.Linq;
using DarkLoop.Azure.Functions.Authorization;

namespace SanfordTest
{
    [FunctionAuthorize]
    public class DispatchFunction
    {
        private readonly ILogger<DispatchFunction> _logger;
        private readonly AppSettings _appSettings;
        private readonly ICsvWriter _csvWriterService;

        public DispatchFunction(ILogger<DispatchFunction> logger, AppSettings appSettings, ICsvWriter csvWriterService)
        {
            _logger = logger;
            _appSettings = appSettings;
            _csvWriterService = csvWriterService;
        }

        [Function("DispatchFunction")]
        public async Task<HttpResponseData> RunAsync(
            [HttpTrigger("get", "post", Route = null)] HttpRequestData req, FunctionContext functionContext)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                _logger.LogInformation($"Received request body of size {requestBody.Length} bytes.");

                if (requestBody.Length > _appSettings.MaxRequestBodySize)
                {
                    _logger.LogWarning("Request body exceeds the maximum size of 800 KB.");
                    var badResponse = req.CreateResponse(System.Net.HttpStatusCode.RequestEntityTooLarge);
                    await badResponse.WriteStringAsync("Request body exceeds the maximum size of 800 KB.");
                    return badResponse;
                }

                ReadyForDispatch data = JsonConvert.DeserializeObject<ReadyForDispatch>(requestBody);

                if (data == null)
                {
                    _logger.LogError("Failed to deserialize request body into ReadyForDispatch object.");
                    var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                    await errorResponse.WriteStringAsync("Invalid request body format.");
                    return errorResponse;
                }

                _logger.LogInformation($"Deserialized data for SalesOrder {data.SalesOrder}. Containers count: {data.Containers.Count}");

                var csvRecords = new List<BlueCorpDispatchRequest>();

                foreach (var container in data.Containers)
                {
                    foreach (var item in container.Items)
                    {
                        var csvRecord = new BlueCorpDispatchRequest
                        {
                            SalesOrder = data.SalesOrder,
                            CustomerReference = data.SalesOrder,
                            LoadId = container.LoadId,
                            ContainerType = BlueCorpDispatchRequest.ConvertContainerType(container.ContainerType),
                            ItemCode = item.ItemCode,
                            ItemQuantity = item.Quantity.ToString(),
                            ItemWeight = item.CartonWeight.ToString(),
                            Street = data.DeliveryAddress.Street,
                            City = data.DeliveryAddress.City,
                            State = data.DeliveryAddress.State,
                            PostalCode = data.DeliveryAddress.PostalCode,
                            Country = data.DeliveryAddress.Country
                        };
                        csvRecords.Add(csvRecord);
                    }
                }

                _logger.LogInformation($"CSV file generation completed with {csvRecords.Count} records.");

                string csvContent = await _csvWriterService.GetCsvString(csvRecords);
                var fileContent = Encoding.UTF8.GetBytes(csvContent);
                var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "text/csv");
                response.Headers.Add("Content-Disposition", $"attachment; filename={BlueCorpDispatchRequest.FileName}");
                await response.WriteStringAsync(csvContent);

                _logger.LogInformation("DispatchFunction completed successfully.");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}");
                _logger.LogError($"Stack Trace: {ex.StackTrace}");

                var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync("An unexpected error occurred. Please try again later.");
                return errorResponse;
            }
         }
    }
}