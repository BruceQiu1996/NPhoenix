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
        private readonly string _folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "runes");
        public async Task<RuneModule> GetRuneAsync(int champId)
        {
            if (!Directory.Exists(_folder))
                return null;

            DirectoryInfo directoryInfo = new DirectoryInfo(_folder);
            var file = directoryInfo.GetFiles().FirstOrDefault(x => x.Name == $"{champId}_rune.json");
            if (file == null)
                return null;

            var content = await File.ReadAllTextAsync(file.FullName);
            return JsonConvert.DeserializeObject<RuneModule>(content);
        }
    }
}
