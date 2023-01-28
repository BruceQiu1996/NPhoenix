using LeagueOfLegendsBoxer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LeagueOfLegendsBoxer.Helpers
{
    public class RuneHelper
    {
        private readonly string _customer_folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "newCustomerheroDatas");

        public async Task<IEnumerable<RuneModule>> ReadCustomerRuneAsync(int champId)
        {
            if (!Directory.Exists(_customer_folder))
                return null;

            DirectoryInfo directoryInfo = new DirectoryInfo(_customer_folder);
            var file = directoryInfo.GetFiles().FirstOrDefault(x => x.Name == $"{champId}_data.json");
            if (file == null)
                return null;

            var content = await File.ReadAllTextAsync(file.FullName);
            return JsonConvert.DeserializeObject<IEnumerable<RuneModule>>(content);
        }

        public async Task WriteCustomerRuneAsync(int champId, IEnumerable<RuneModule> module)
        {
            if (!Directory.Exists(_customer_folder))
                Directory.CreateDirectory(_customer_folder);

            DirectoryInfo directoryInfo = new DirectoryInfo(_customer_folder);
            var file = directoryInfo.GetFiles().FirstOrDefault(x => x.Name == $"{champId}_data.json");
            if (file == null)
                File.Create(Path.Combine(_customer_folder, $"{champId}_data.json"));

            await File.WriteAllTextAsync(Path.Combine(_customer_folder, $"{champId}_data.json"), JsonConvert.SerializeObject(module));
        }
    }
}
