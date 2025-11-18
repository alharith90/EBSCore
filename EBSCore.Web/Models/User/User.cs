namespace EBSCore.Web.Models
{
    public class User
    {
        public string? UserID { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? RePassword { get; set; }
        public string? UserFullName { get; set; }
        public string? CompanyID { get; set; }
        public string? CategoryID { get; set; }
        public UserType UserType { get; set; }
        public string? Job_Desc { get; set; }
        public string? SecNo { get; set; }
        public string? ManagerNo { get; set; }
        public string? Mobile { get; set; }
        public string? UserName { get; set; }
        public string? UserImage { get; set; }
        public string? ResetPasswordKey { get; set; }
        public string? CompanyName { get; set; }

        public string? BirthDate { get; set; }
        public string? UserStatus { get; set; }
        public string? ExpiryDate { get; set; }

    }
    public enum UserType
    {
        Manager = 1,
        TeamLeader = 2,
        Developer = 3,
        Analysit = 4,
        QA = 4
    }
}
