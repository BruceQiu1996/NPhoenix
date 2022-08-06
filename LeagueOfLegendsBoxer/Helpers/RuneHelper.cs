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
    }
}
