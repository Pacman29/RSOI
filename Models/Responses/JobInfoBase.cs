using System;

namespace Models.Responses
{
    [Serializable]
    public class JobInfoBase
    {
        public string JobId { get; set; }
        public string JobStatus { get; set; }
    }
}