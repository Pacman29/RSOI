using Microsoft.AspNetCore.Mvc;

namespace Models.Requests
{
    public class ImageRequestModel
    {
        [FromQuery(Name = "PageNo")]
        public long PageNo { get; set; } = 0;
    }
}