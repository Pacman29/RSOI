using Newtonsoft.Json;

namespace Models.Requests
{
    public class AccountRequestModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static AccountRequestModel fromJson(string json)
        {
            return JsonConvert.DeserializeObject<AccountRequestModel>(json);
        }
    }
}