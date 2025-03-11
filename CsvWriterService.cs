using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanfordTest
{
    public class CsvWriterService : ICsvWriter
    {
        public async Task<string> GetCsvString<T>(IList<T> records)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            };

            await using var memoryStream = new MemoryStream();
            await using var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8);
            await using var csv = new CsvWriter(streamWriter, config);
            await csv.WriteRecordsAsync(records);

            await streamWriter.FlushAsync();
            memoryStream.Position = 0;

            using var reader = new StreamReader(memoryStream, Encoding.UTF8);
            return await reader.ReadToEndAsync();
        }
    }
}
