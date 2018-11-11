using System;

namespace Models.Responses
{
    [Serializable]
    public class ImageResponseModel : IEquatable<ImageResponseModel>
    {
        public long PageNo { get; set; }
        public string Path { get; set; }
        public int ImgId { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as ImageResponseModel);
        }

        public bool Equals(ImageResponseModel other)
        {
            return other != null &&
                   PageNo == other.PageNo &&
                   Path == other.Path &&
                   ImgId == other.ImgId;
        }
    }
}