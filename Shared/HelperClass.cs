using CsvHelper;
using SanfordTest.Models;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SanfordTest.Shared
{
    public class HelperClass
    {
        public static string WriteCsvFile<T>(List<T> csvRecords)
        {
            using (var writer = new StringWriter())
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(csvRecords);
                return writer.ToString();
            }

            //Console.WriteLine($"CSV file has been generated at {filePath}");
        }

        //private static async Task UploadFileToBlobAsync(byte[] fileContent, string fileName)
        //{
        //    // Use DefaultAzureCredential, which automatically picks the correct authentication method (Managed Identity, Environment Variables, etc.)
        //    var blobServiceClient = new BlobServiceClient(new Uri("https://<your-storage-account-name>.blob.core.windows.net"), new DefaultAzureCredential());

        //    // Get a reference to the container
        //    var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

        //    // Create the container if it doesn't exist
        //    await blobContainerClient.CreateIfNotExistsAsync();

        //    // Get a reference to the blob (file) to upload
        //    var blobClient = blobContainerClient.GetBlobClient(fileName);

        //    // Upload the file to Blob Storage
        //    using (var memoryStream = new MemoryStream(fileContent))
        //    {
        //        await blobClient.UploadAsync(memoryStream, overwrite: true);
        //    }
        //}
    }
}
