using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBSCore.AdoClass
{
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections;
    public class DBUserSP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", System.Data.SqlDbType.NVarChar);
        public TableField CurrentUserID = new TableField("CurrentUserID", System.Data.SqlDbType.Int);
        public TableField UserID = new TableField("UserID", System.Data.SqlDbType.Int);
        public TableField Email = new TableField("Email", System.Data.SqlDbType.NVarChar);
        public TableField Password = new TableField("Password", System.Data.SqlDbType.NVarChar);
        public TableField UserFullName = new TableField("UserFullName", System.Data.SqlDbType.NVarChar);
        public TableField CompanyID = new TableField("CompanyID", System.Data.SqlDbType.Int);
        public TableField CategoryID = new TableField("CategoryID", System.Data.SqlDbType.Int);
        public TableField UserType = new TableField("UserType", System.Data.SqlDbType.Int);
        public TableField UserName = new TableField("UserName", System.Data.SqlDbType.NVarChar);
        public TableField UserImage = new TableField("UserImage", System.Data.SqlDbType.VarBinary);
        public TableField UserImageMeta = new TableField("UserImageMeta", System.Data.SqlDbType.NVarChar);
        public TableField Mobile = new TableField("Mobile", System.Data.SqlDbType.NVarChar);
        public TableField RequireNewPassword = new TableField("RequireNewPassword", System.Data.SqlDbType.Bit);

        public TableField BirthDate = new TableField("BirthDate", System.Data.SqlDbType.DateTime);
        public TableField UserStatus = new TableField("UserStatus", System.Data.SqlDbType.Int);
        public TableField ExpiryDate = new TableField("ExpiryDate", System.Data.SqlDbType.DateTime);



        public TableField FullLink = new TableField("FullLink", System.Data.SqlDbType.NVarChar);
        public TableField IPAddress = new TableField("IPAddress", System.Data.SqlDbType.NVarChar);
        public TableField ReferURL = new TableField("ReferURL", System.Data.SqlDbType.NVarChar);
        public TableField Browser = new TableField("Browser", System.Data.SqlDbType.NVarChar);
        public TableField DeviceType = new TableField("DeviceType", System.Data.SqlDbType.NVarChar);
        public TableField OperatingSystem = new TableField("OperatingSystem", System.Data.SqlDbType.NVarChar);
        public TableField IsMobile = new TableField("IsMobile", System.Data.SqlDbType.Bit);
        public TableField SessionID = new TableField("SessionID", System.Data.SqlDbType.NVarChar);

        public TableField ResetPasswordKey = new TableField("ResetPasswordKey", System.Data.SqlDbType.NVarChar);
        public DBUserSP(IConfiguration configuration) : base(configuration)
        {
            base.SPName = "UserSP"; 
        }
        public new object QueryDatabase(SqlQueryType QueryType,
        string Operation = "", string CurrentUserID = "",
        string UserID = "", string Email = "",
        string Password = "", string UserFullName = "",
        string CompanyID = "", string CategoryID = "", string UserType = "",
        string UserName = "", Byte[] UserImage = null,
        string UserImageMeta = "",
        string Mobile = "", string RequireNewPassword = "",
        string BirthDate = "", string UserStatus = "", string ExpiryDate = "",
        string FullLink = "",
        string IPAddress = "", string ReferURL = "",
        string Browser = "", string DeviceType = "",
        string OperatingSystem = "", string IsMobile = "",
        string SessionID = "", string ResetPasswordKey = "")
        {
            FieldsArrayList = new ArrayList();
            this.Operation.SetValue(Operation, ref FieldsArrayList);
            this.CurrentUserID.SetValue(CurrentUserID, ref FieldsArrayList);
            this.UserID.SetValue(UserID, ref FieldsArrayList);
            this.Email.SetValue(Email, ref FieldsArrayList);
            this.Password.SetValue(Password, ref FieldsArrayList);
            this.UserFullName.SetValue(UserFullName, ref FieldsArrayList);
            this.CompanyID.SetValue(CompanyID, ref FieldsArrayList);
            this.CategoryID.SetValue(CategoryID, ref FieldsArrayList);
            this.UserType.SetValue(UserType, ref FieldsArrayList);
            this.UserName.SetValue(UserName, ref FieldsArrayList);
            this.UserImage.SetBytes(UserImage, ref FieldsArrayList);
            this.UserImageMeta.SetValue(UserImageMeta, ref FieldsArrayList);
            this.Mobile.SetValue(Mobile, ref FieldsArrayList);
            this.RequireNewPassword.SetValue(RequireNewPassword, ref FieldsArrayList);

            this.BirthDate.SetValue(BirthDate, ref FieldsArrayList);
            this.UserStatus.SetValue(UserStatus, ref FieldsArrayList);
            this.ExpiryDate.SetValue(ExpiryDate, ref FieldsArrayList);

            this.FullLink.SetValue(FullLink, ref FieldsArrayList);
            this.IPAddress.SetValue(IPAddress, ref FieldsArrayList);
            this.ReferURL.SetValue(ReferURL, ref FieldsArrayList);
            this.Browser.SetValue(Browser, ref FieldsArrayList);
            this.DeviceType.SetValue(DeviceType, ref FieldsArrayList);
            this.OperatingSystem.SetValue(OperatingSystem, ref FieldsArrayList);
            this.IsMobile.SetValue(IsMobile, ref FieldsArrayList);
            this.SessionID.SetValue(SessionID, ref FieldsArrayList);
            this.ResetPasswordKey.SetValue(ResetPasswordKey, ref FieldsArrayList);
            return base.QueryDatabase(QueryType);
        }
    }
}
