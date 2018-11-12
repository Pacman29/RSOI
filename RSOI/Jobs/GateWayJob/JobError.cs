using Newtonsoft.Json;

namespace RSOI.Jobs
{
    public class JobError
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}