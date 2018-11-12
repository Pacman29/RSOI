using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Models.Requests
{
    public class ImagesRequestModel
    {
        [FromQuery(Name = "FirstPage")] 
        public int FirstPage { get; set; } = 0;
        
        [FromQuery(Name = "Count")]
        public int Count { get; set; } = 10;

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}