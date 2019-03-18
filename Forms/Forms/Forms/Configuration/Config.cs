using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Forms.Configuration
{
    public class Config : IConfiguration
    {
        [JsonConstructor]
        public Config()
        {
        }

        public string ApiBaseAddress { get; set; }
    }
}
