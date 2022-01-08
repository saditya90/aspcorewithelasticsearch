using System.Collections.Generic;

namespace WebElasticSearch.Models
{
    public class IndexDetail
    {
        public string Key { get; set; }
        public Dictionary<string, object> Values { get; set; }
    }
}
