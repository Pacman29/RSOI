using Newtonsoft.Json;
using System;
using System.Collections.Generic;


namespace Models.Responses
{
    [Serializable]
    public class JobInfo : ImagesResponseModel, IEquatable<JobInfo>
    {
        public string JobId { get; set; }
        public string JobStatus { get; set; }
        public string PdfPath { get; set; } = "";
        public int PdfId { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as JobInfo);
        }

        public bool Equals(JobInfo other)
        {
            return other != null &&
                   JobId == other.JobId &&
                   JobStatus == other.JobStatus &&
                   PdfPath == other.PdfPath &&
                   PdfId == other.PdfId;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}