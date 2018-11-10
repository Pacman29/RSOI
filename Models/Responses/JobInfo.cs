using System;

namespace Models.Responses
{
    [Serializable]
    public class JobInfo
    {
        public string JobId { get; set; }
        public string JobStatus { get; set; }
        public string PdfPath { get; set; } = "";
        public string[] ImagePath { get; set; } = new string[0];
    }
}