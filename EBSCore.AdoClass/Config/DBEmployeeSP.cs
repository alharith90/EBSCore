using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Data;

namespace EBSCore.AdoClass
{
    public class DBEmployeeSP : DBParentStoredProcedureClass
    {
        // Parameters
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField CurrentUserID = new TableField("CurrentUserID", SqlDbType.Int);

        public TableField EmployeeId = new TableField("EmployeeId", SqlDbType.Int);
        public TableField FullName = new TableField("FullName", SqlDbType.NVarChar);
        public TableField Position = new TableField("Position", SqlDbType.NVarChar);
        public TableField Email = new TableField("Email", SqlDbType.NVarChar);
        public TableField Phone = new TableField("Phone", SqlDbType.NVarChar);
        public TableField OrganizationUnitId = new TableField("OrganizationUnitId", SqlDbType.Int);
        public TableField SourceId = new TableField("SourceId", SqlDbType.NVarChar);
        public TableField SourceSystem = new TableField("SourceSystem", SqlDbType.NVarChar);
        public TableField JobTitle = new TableField("JobTitle", SqlDbType.NVarChar);
        public TableField JobFamily = new TableField("JobFamily", SqlDbType.NVarChar);
        public TableField SupervisorId = new TableField("SupervisorId", SqlDbType.Int);
        public TableField EmploymentType = new TableField("EmploymentType", SqlDbType.NVarChar);
        public TableField Location = new TableField("Location", SqlDbType.NVarChar);
        public TableField Department = new TableField("Department", SqlDbType.NVarChar);
        public TableField IsActive = new TableField("IsActive", SqlDbType.Bit);

        public TableField PageSize = new TableField("PageSize", SqlDbType.Int);
        public TableField PageNumber = new TableField("PageNumber", SqlDbType.Int);
        public TableField SortColumn = new TableField("SortColumn", SqlDbType.NVarChar);
        public TableField SortDirection = new TableField("SortDirection", SqlDbType.NVarChar);
        public TableField SearchFields = new TableField("SearchFields", SqlDbType.NVarChar);
        public TableField SearchQuery = new TableField("SearchQuery", SqlDbType.NVarChar);

        public DBEmployeeSP(IConfiguration configuration) : base(configuration)
        {
            base.SPName = "EmployeeSP";
        }

        public new object QueryDatabase(
            SqlQueryType QueryType,
            string Operation = "",
            string CurrentUserID = "",
            string EmployeeId = "",
            string FullName = "",
            string Position = "",
            string Email = "",
            string Phone = "",
            string OrganizationUnitId = "",
            string SourceId = "",
            string SourceSystem = "",
            string JobTitle = "",
            string JobFamily = "",
            string SupervisorId = "",
            string EmploymentType = "",
            string Location = "",
            string Department = "",
            string IsActive = "",
            string PageNumber = "",
            string PageSize = "",
            string SortColumn = "",
            string SortDirection = "",
            string SearchFields = "",
            string SearchQuery = ""
        )
        {
            FieldsArrayList = new ArrayList();

            this.Operation.SetValue(Operation, ref FieldsArrayList);
            this.CurrentUserID.SetValue(CurrentUserID, ref FieldsArrayList);
            this.EmployeeId.SetValue(EmployeeId, ref FieldsArrayList);
            this.FullName.SetValue(FullName, ref FieldsArrayList);
            this.Position.SetValue(Position, ref FieldsArrayList);
            this.Email.SetValue(Email, ref FieldsArrayList);
            this.Phone.SetValue(Phone, ref FieldsArrayList);
            this.OrganizationUnitId.SetValue(OrganizationUnitId, ref FieldsArrayList);
            this.SourceId.SetValue(SourceId, ref FieldsArrayList);
            this.SourceSystem.SetValue(SourceSystem, ref FieldsArrayList);
            this.JobTitle.SetValue(JobTitle, ref FieldsArrayList);
            this.JobFamily.SetValue(JobFamily, ref FieldsArrayList);
            this.SupervisorId.SetValue(SupervisorId, ref FieldsArrayList);
            this.EmploymentType.SetValue(EmploymentType, ref FieldsArrayList);
            this.Location.SetValue(Location, ref FieldsArrayList);
            this.Department.SetValue(Department, ref FieldsArrayList);
            this.IsActive.SetValue(IsActive, ref FieldsArrayList);

            this.PageNumber.SetValue(PageNumber, ref FieldsArrayList);
            this.PageSize.SetValue(PageSize, ref FieldsArrayList);
            this.SortColumn.SetValue(SortColumn, ref FieldsArrayList);
            this.SortDirection.SetValue(SortDirection, ref FieldsArrayList);
            this.SearchFields.SetValue(SearchFields, ref FieldsArrayList);
            this.SearchQuery.SetValue(SearchQuery, ref FieldsArrayList);

            return base.QueryDatabase(QueryType);
        }
    }
}
