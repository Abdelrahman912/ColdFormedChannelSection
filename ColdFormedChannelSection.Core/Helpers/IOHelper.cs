using CSharp.Functional.Constructs;
using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static ColdFormedChannelSection.Core.Errors.Errors;
using static CSharp.Functional.Extensions.ValidationExtension;

namespace ColdFormedChannelSection.Core.Helpers
{
    public static class IOHelper
    {
        public static  async Task<Validation<List<T>>> ReadAsCsv<T>(this string filePath)
        {
            if (!File.Exists(filePath))
            {
               await Task.Run(() =>
                {
                    Validation<List<T>> lst = FileNotFound(filePath);
                    return lst;
                });
            }

            try
            {
                var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture);
                config.HeaderValidated = null;
                config.MissingFieldFound = null;
                using (var streamReader = new StreamReader(filePath))
                using (var csvRedaer = new CsvReader(streamReader, config))
                {
                    return await Task.Run(() =>
                     {
                         return Valid(csvRedaer.GetRecords<T>().ToList());
                     });
                }
            }
            catch (IOException)
            {
                return await Task.Run(()=> {
                     Validation<List<T>> lst =  FileUsedByAnotherProcess(filePath);
                        return lst;
                    });
            }
        }
    }
}
