using System;
using System.Collections.Generic;

namespace Models.Responses
{
    [Serializable]
    public class JobInfo : ImagesResponseModel
    {
        public string JobId { get; set; }
        public string JobStatus { get; set; }
        public string PdfPath { get; set; } = "";
    }
}