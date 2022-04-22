using CSharp.Functional.Constructs;
using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using static CSharp.Functional.Extensions.ValidationExtension;
using static ColdFormedChannelSection.Core.Errors.Errors;

namespace ColdFormedChannelSection.Core.Helpers
{
    public static class IOHelper
    {
        public static Validation<List<T>> ReadAsCsv<T>(this string filePath)
        {
            if (!File.Exists(filePath))
                return FileNotFound(filePath);

            try
            {
                using (var streamReader = new StreamReader(filePath))
                using (var csvRedaer = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                {
                    return Valid(csvRedaer.GetRecords<T>().ToList());
                }
            }
            catch (IOException)
            {
                return FileUsedByAnotherProcess(filePath);
            }
        }
    }
}
