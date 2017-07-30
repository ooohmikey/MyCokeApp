using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Tabs.DataModels
{
    public class MyCokeModel
    {
        [JsonProperty(PropertyName = "Id")]
        public string ID { get; set; }
        
        [JsonProperty(PropertyName = "Tag")]
        public string Tag { get; set; }

    }
}
