using System.Collections.Generic;

namespace BusinessImpl
{
    public class TreeJSON
    {
        public string title { get; set; }
        public string key { get; set; }
        public object data { get; set; }
        public bool partsel { get; set; }
        public bool selected { get; set; }
        public bool expanded { get; set; }
        public List<TreeJSON> children { get; set; }
        public short row { get; set; }
    }
}
