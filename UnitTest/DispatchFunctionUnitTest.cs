using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using SanfordTest;
using Moq;
using Microsoft.Azure.Functions.Worker.Http;
using SanfordTest.Models;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using SanfordTest.Configurations;
using System.Net.Mail;

namespace UnitTest
{
    public class DispatchFunctionUnitTest
    {
        private readonly Mock<ILogger<DispatchFunction>> _loggerMock;
        private readonly AppSettings _appSettings;
        private readonly DispatchFunction _function;
        private readonly ICsvWriter _csvWriter;

        public DispatchFunctionUnitTest()
        {
            _loggerMock = new Mock<ILogger<DispatchFunction>>();
            _csvWriter = new CsvWriterService(); ;
            _appSettings = new AppSettings
            {
                MaxRequestBodySize = 819200  // Example value
            };
            _function = new DispatchFunction(_loggerMock.Object, _appSettings, _csvWriter);
        }

        [Fact]
        public async Task RunAsync_ShouldReturnPayloadTooLarge_WhenRequestBodyExceedsSizeLimit()
        {
            var requestBody = new string('a', 819201);  // 800 KB + 1 byte (to exceed the size limit)

            var context = new Mock<FunctionContext>();
            var mockBodyStream = new MemoryStream(Encoding.UTF8.GetBytes(requestBody));

            var request = new Mock<HttpRequestData>(context.Object);
            request.Setup(r => r.Body).Returns(mockBodyStream);
            request.Setup(r => r.CreateResponse()).Returns(() =>
            {
                var response = new Mock<HttpResponseData>(context.Object);
                response.SetupProperty(r => r.Headers, new HttpHeadersCollection());
                response.SetupProperty(r => r.StatusCode);
                response.SetupProperty(r => r.Body, new MemoryStream());
                return response.Object;
            });

            var result = await _function.RunAsync(request.Object, context.Object);
            result.StatusCode.Should().Be(HttpStatusCode.RequestEntityTooLarge);
        }

        [Fact]
        public async Task RunAsync_ShouldReturnBadRequest_WhenRequestBodyCannotBeDeserialized()
        {
            // Arrange
            var invalidRequestBody = "{}";
            var mockBodyStream = new MemoryStream(Encoding.UTF8.GetBytes(invalidRequestBody));
            var context = new Mock<FunctionContext>();

            var request = new Mock<HttpRequestData>(context.Object);
            request.Setup(r => r.Body).Returns(mockBodyStream);
            request.Setup(r => r.CreateResponse()).Returns(() =>
            {
                var response = new Mock<HttpResponseData>(context.Object);
                response.SetupProperty(r => r.Headers, new HttpHeadersCollection());
                response.SetupProperty(r => r.StatusCode);
                response.SetupProperty(r => r.Body, new MemoryStream());
                return response.Object;
            });

            var response = await _function.RunAsync(request.Object, context.Object);

            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task RunAsync_ShouldReturnCsvFile_WhenValidRequestIsProcessed()
        {
            string path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "../../../TestData/bluecorp-ready-for-dispatch-event.json"));
            byte[] fileBytes = await File.ReadAllBytesAsync(path);
            var mockBodyStream = new MemoryStream(fileBytes);
            var context = new Mock<FunctionContext>();


            var request = new Mock<HttpRequestData>(context.Object);
            request.Setup(r => r.Body).Returns(mockBodyStream);
            request.Setup(r => r.CreateResponse()).Returns(() =>
            {
                var response = new Mock<HttpResponseData>(context.Object);
                response.SetupProperty(r => r.Headers, new HttpHeadersCollection());
                response.SetupProperty(r => r.StatusCode);
                response.SetupProperty(r => r.Body, new MemoryStream());
                return response.Object;
            });

            var response = await _function.RunAsync(request.Object, context.Object);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Headers.Should().ContainKey("Content-Type").WhoseValue.Should().Contain("text/csv");
        }
    }
}