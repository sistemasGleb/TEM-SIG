using System.Collections.Generic;


namespace BusinessEntity
{
    public class Root
    {
        public bool Expanded { get; set; }
        public string Key { get; set; }
        public string Title { get; set; }
        public List<Child> Children { get; set; }
    }

    public class Child
    {
        public bool Expanded { get; set; }
        public long Key { get; set; }
        public bool Partsel { get; set; }
        public bool Selected { get; set; }
        public string Title { get; set; }
        public List<Child> Children { get; set; }
    }

    public class ListChild
    {
        public int RootChild { get; set; }
        public bool Expanded { get; set; }
        public long ChildKey { get; set; }
        public bool Selected { get; set; }
        public string Title { get; set; }
    }


}
