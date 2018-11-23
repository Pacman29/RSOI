using Newtonsoft.Json;

namespace Models.Responses
{
    public class AccountResponseModel
    {
        public string Token { get; set; }
        public string UserName { get; set; }
        
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}