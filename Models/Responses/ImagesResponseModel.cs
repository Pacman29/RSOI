using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Models.Responses
{
    [Serializable]
    public class ImagesResponseModel
    {
        public List<ImageResponseModel> Images { get; set; } = new List<ImageResponseModel>();

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}