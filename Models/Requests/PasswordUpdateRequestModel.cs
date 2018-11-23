namespace Models.Requests
{
    public class PasswordUpdateRequestModel : AccountRequestModel
    {
        public string OldPassword { get; set; }
    }
}