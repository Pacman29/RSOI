using System;

namespace Models.Responses
{
    [Serializable]
    public class JobInfoBase : IEquatable<JobInfoBase>
    {
        public string JobId { get; set; }
        public string JobStatus { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as JobInfoBase);
        }

        public bool Equals(JobInfoBase other)
        {
            return other != null &&
                   JobId == other.JobId &&
                   JobStatus == other.JobStatus;
        }
    }
}