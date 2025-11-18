using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EBSCore.AdoClass.Common
{
    
    public class ErrorHandler : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", System.Data.SqlDbType.NVarChar);
        public TableField ID = new TableField("ID", System.Data.SqlDbType.Int);
        public TableField ExDate = new TableField("ExDate", System.Data.SqlDbType.DateTime);
        public TableField Message = new TableField("Message", System.Data.SqlDbType.NVarChar);
        public TableField Source = new TableField("Source", System.Data.SqlDbType.NVarChar);
        public TableField Form = new TableField("Form", System.Data.SqlDbType.NVarChar);
        public TableField TargetSite = new TableField("TargetSite", System.Data.SqlDbType.NVarChar);
        public TableField StackTrace = new TableField("StackTrace", System.Data.SqlDbType.NVarChar);
        public TableField UserName = new TableField("UserName", System.Data.SqlDbType.NVarChar);
        public TableField RemoteAdders = new TableField("RemoteAdders", System.Data.SqlDbType.NVarChar);
        public TableField ToExDate = new TableField("ToExDate", System.Data.SqlDbType.DateTime);
        public TableField ApplicationID = new TableField("ApplicationID", System.Data.SqlDbType.Int);

        public ErrorHandler(IConfiguration configuration) : base(configuration)
        {
            base.SPName = "ErrorHandlerSP";
        }

        public new object QueryDatabase(SqlQueryType QueryType, string Operation = "", string ID = "", string ExDate = "", string Message = "", string Source = "", string Form = "", string TargetSite = "", string StackTrace = "", string UserName = "", string RemoteAdders = "", string ToExDate = "", string ApplicationID = "")
        {
            base.FieldsArrayList = new ArrayList();
            this.Operation.SetValue(Operation, ref base.FieldsArrayList);
            this.ID.SetValue(ID, ref base.FieldsArrayList);
            this.ExDate.SetValue(ExDate, ref base.FieldsArrayList);
            this.Message.SetValue(Message, ref base.FieldsArrayList);
            this.Source.SetValue(Source, ref base.FieldsArrayList);
            this.Form.SetValue(Form, ref base.FieldsArrayList);
            this.TargetSite.SetValue(TargetSite, ref base.FieldsArrayList);
            this.StackTrace.SetValue(StackTrace, ref base.FieldsArrayList);
            this.UserName.SetValue(UserName, ref base.FieldsArrayList);
            this.RemoteAdders.SetValue(getIPAddres(), ref base.FieldsArrayList);
            this.ToExDate.SetValue(ToExDate, ref base.FieldsArrayList);
            this.ApplicationID.SetValue("1", ref base.FieldsArrayList);
            return base.QueryDatabase(QueryType);
        }
        public string getIPAddres()
        {
            try
            {
                // return HttpContext.Current.Request.UserHostAddress;
                return "";
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}
