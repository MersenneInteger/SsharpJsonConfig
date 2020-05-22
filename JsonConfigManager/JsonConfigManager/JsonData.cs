using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace JsonConfigManager
{
    public partial class JsonData
    {
        public List<Signal> Signals { get; set; }
        public JsonData()
        {
            Signals = new List<Signal>();
        }
    }
}