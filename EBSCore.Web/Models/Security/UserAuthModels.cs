using System;
using System.ComponentModel.DataAnnotations;

namespace EBSCore.Web.Models.Security
{
    public class LoginRequest
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        public bool KeepMeSignedIn { get; set; }
    }

    public class ResetPasswordRequest
    {
        [Required]
        public string UserNameOrEmail { get; set; }
    }

    public class ResetPasswordConfirmRequest
    {
        [Required]
        public Guid Token { get; set; }

        [Required]
        [MinLength(8)]
        public string NewPassword { get; set; }

        [Compare(nameof(NewPassword))]
        public string ConfirmNewPassword { get; set; }
    }

    public class PasswordResetTokenModel
    {
        public int TokenID { get; set; }
        public int UserID { get; set; }
        public Guid Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
