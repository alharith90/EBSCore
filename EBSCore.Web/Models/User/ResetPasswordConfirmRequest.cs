namespace EBSCore.Web.Models
{
    public class ResetPasswordConfirmRequest
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}
