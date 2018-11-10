using System;
using System.Collections.Generic;

namespace Models.Responses
{
    [Serializable]
    public class ImagesResponseModel
    {
        public List<ImageResponseModel> Images { get; set; } = new List<ImageResponseModel>();
    }
}