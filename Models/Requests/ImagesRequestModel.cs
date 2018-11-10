using Microsoft.AspNetCore.Mvc;

namespace Models.Requests
{
    public class ImagesRequestModel
    {
        [FromQuery(Name = "FirstPage")] 
        public int FirstPage { get; set; } = 0;
        
        [FromQuery(Name = "Count")]
        public int Count { get; set; } = 10;
    }
}