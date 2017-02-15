using Newtonsoft.Json;
using System;
using System.IO;

namespace JustTheBasics
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CoreConfig
    {
        [JsonProperty("token")]
        public string Token { get; set; } = "YOUR TOKEN HERE";
        [JsonProperty("prefix")]
        public string Prefix { get; set; } = "!";
        [JsonProperty("owner-ids")]
        public ulong[] OwnerIds { get; set; } = new ulong[0];

        public static CoreConfig ReadConfig()
        {
            if (!File.Exists("config.json"))
            {
                File.WriteAllText("config.json", JsonConvert.SerializeObject(new CoreConfig(), Formatting.Indented));
                throw new FileNotFoundException("Config File Not Found - Generating a template at 'config.json'");
            }

            var jsonString = File.ReadAllText("config.json");
            
            return JsonConvert.DeserializeObject<CoreConfig>(jsonString);
        }
    }
}