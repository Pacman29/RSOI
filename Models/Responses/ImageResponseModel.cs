using System;

namespace Models.Responses
{
    [Serializable]
    public class ImageResponseModel
    {
        public long PageNo { get; set; }
        public string Path { get; set; }
        public int ImgId { get; set; }
    }
}