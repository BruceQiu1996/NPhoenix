using LeagueOfLegendsBoxer.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LeagueOfLegendsBoxer.Helpers
{
    public class RuneHelper
    {
        private readonly string _folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "heroDatas");
        private readonly string _customer_folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "customerheroDatas");

        public async Task<HeroRecommandModule> GetRuneAsync(int champId)
        {
            if (!Directory.Exists(_folder))
                return null;

            DirectoryInfo directoryInfo = new DirectoryInfo(_folder);
            var file = directoryInfo.GetFiles().FirstOrDefault(x => x.Name == $"{champId}_data.json");
            if (file == null)
                return null;

            var content = await File.ReadAllTextAsync(file.FullName);
            return JsonConvert.DeserializeObject<HeroRecommandModule>(content);
        }

        public async Task WriteSystemRuneAsync(int champId, HeroRecommandModule module)
        {
            if (!Directory.Exists(_folder))
                return;

            DirectoryInfo directoryInfo = new DirectoryInfo(_folder);
            var file = directoryInfo.GetFiles().FirstOrDefault(x => x.Name == $"{champId}_data.json");
            if (file == null)
                return;

            await File.WriteAllTextAsync(file.FullName, JsonConvert.SerializeObject(module));
        }

        public async Task<RuneModule> ReadCustomerRuneAsync(int champId)
        {
            if (!Directory.Exists(_customer_folder))
                return null;

            DirectoryInfo directoryInfo = new DirectoryInfo(_customer_folder);
            var file = directoryInfo.GetFiles().FirstOrDefault(x => x.Name == $"{champId}_data.json");
            if (file == null)
                return null;

            var content = await File.ReadAllTextAsync(file.FullName);
            return JsonConvert.DeserializeObject<RuneModule>(content);
        }

        public async Task WriteCustomerRuneAsync(int champId, RuneModule module)
        {
            if (!Directory.Exists(_customer_folder))
                Directory.CreateDirectory(_customer_folder);

            DirectoryInfo directoryInfo = new DirectoryInfo(_folder);
            var file = directoryInfo.GetFiles().FirstOrDefault(x => x.Name == $"{champId}_data.json");
            if (file == null)
                File.Create(Path.Combine(_customer_folder, $"{champId}_data.json"));

            await File.WriteAllTextAsync(Path.Combine(_customer_folder, $"{champId}_data.json"), JsonConvert.SerializeObject(module));
        }
    }
}
