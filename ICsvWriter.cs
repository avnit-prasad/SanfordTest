using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanfordTest
{
    public interface ICsvWriter
    {
        Task<string> GetCsvString<T>(IList<T> records);
    }
}
