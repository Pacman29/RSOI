using Newtonsoft.Json;

namespace Models.Requests
{
    public class PasswordUpdateRequestModel : AccountRequestModel
    {
        public string OldPassword { get; set; }
        
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static PasswordUpdateRequestModel fromJson(string json)
        {
            return JsonConvert.DeserializeObject<PasswordUpdateRequestModel>(json);
        }
    }
    
}