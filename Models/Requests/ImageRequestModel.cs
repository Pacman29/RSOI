using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Models.Requests
{
    public class ImageRequestModel
    {
        [FromQuery(Name = "PageNo")]
        public long PageNo { get; set; } = 0;

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

}