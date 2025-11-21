namespace EBSCore.Web.Models
{
    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool KeepSignedIn { get; set; }
    }
}
