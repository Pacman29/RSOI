using System;
using System.Collections.Generic;

namespace Models.Responses
{
    [Serializable]
    public class ImagesResponseModel
    {
        [Serializable]
        public class ImageResponseModel
        {
            public byte[] Data { get; set; }
            public long PageNo { get; set; }
            public string Path { get; set; }
        }

        public List<ImageResponseModel> Images { get; set; } = new List<ImageResponseModel>();
    }
}