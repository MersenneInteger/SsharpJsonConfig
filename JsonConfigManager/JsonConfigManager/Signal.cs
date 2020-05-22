using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;

namespace JsonConfigManager
{
    public partial class Signal
    {
        [JsonProperty("Key")]
        public string Key { get; set; }
        [JsonProperty("Type")]
        public string Type { get; set; }
        [JsonProperty("Value")]
        public object Value { get; set; }
    }
}