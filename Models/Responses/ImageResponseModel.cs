using System;

namespace Models.Responses
{
    [Serializable]
    public class ImageResponseModel
    {
        public long PageNo { get; set; }
        public string Path { get; set; }
    }
}