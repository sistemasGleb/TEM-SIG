using System.Collections.Generic;


namespace BusinessEntity
{
    public class UploadFile
    {
        public string error { get; set; }
        public List<string> initialPreview { get; set; }
        public List<UploadFilePreviewConfig> initialPreviewConfig { get; set; }
        public bool initialPreviewAsData { get; set; }
        public string ResultData { get; set; }
        public string downloadUrlBase64 { get; set; }
        public string fileName { get; set; }
        public int fileRowsAfected { get; set; }
        public string message { get; set; }
    }

    public class UploadFilePreviewConfig
    {
        public string key { get; set; }
        public string caption { get; set; }
        public float size { get; set; }
        public string downloadUrl { get; set; }
        public string url { get; set; }

    }

    public class UploadFileNames
    {
        public int file_id { get; set; }
        public string file_nom { get; set; }
        public string file_desc { get; set; }
        public string file_contains { get; set; }
        public bool file_vig { get; set; }
    }
}
