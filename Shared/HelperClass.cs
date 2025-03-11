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
        }
    }
}
