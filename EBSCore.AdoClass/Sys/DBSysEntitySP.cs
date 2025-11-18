using EBSCore.AdoClass;
using Microsoft.Extensions.Configuration;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;
using System.Collections;
using System.Data;

public class DBSysEntitySP : DBParentStoredProcedureClass
{
    public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
    public TableField UserID = new TableField("UserID", SqlDbType.Int);
    public TableField EntityID = new TableField("EntityID", SqlDbType.Int);
    public TableField Name = new TableField("Name", SqlDbType.NVarChar);
    public TableField Sector = new TableField("Sector", SqlDbType.NVarChar);
    public TableField Description = new TableField("Description", SqlDbType.NVarChar);
    public TableField Location = new TableField("Location", SqlDbType.NVarChar);
    public TableField ContactPerson = new TableField("ContactPerson", SqlDbType.NVarChar);
    public TableField Email = new TableField("Email", SqlDbType.NVarChar);
    public TableField Phone = new TableField("Phone", SqlDbType.NVarChar);
    public TableField CreatedBy = new TableField("CreatedBy", SqlDbType.Int);
    public TableField UpdatedBy = new TableField("UpdatedBy", SqlDbType.Int);

    // ➕ Pagination and Sorting Fields
    public TableField PageSize = new TableField("PageSize", SqlDbType.Int);
    public TableField PageNumber = new TableField("PageNumber", SqlDbType.Int);
    public TableField SortColumn = new TableField("SortColumn", SqlDbType.NVarChar);
    public TableField SortDirection = new TableField("SortDirection", SqlDbType.NVarChar);

    public DBSysEntitySP(IConfiguration configuration) : base(configuration)
    {
        base.SPName = "SysEntitySP";
    }

    public new object QueryDatabase(
        SqlQueryType QueryType,
        string Operation = "", string UserID = "", string EntityID = "",
        string Name = "", string Sector = "", string Description = "", string Location = "",
        string ContactPerson = "", string Email = "", string Phone = "",
        string CreatedBy = "", string UpdatedBy = "",
        string PageSize = "", string PageNumber = "", string SortColumn = "", string SortDirection = ""
    )
    {
        FieldsArrayList = new ArrayList();

        this.Operation.SetValue(Operation, ref FieldsArrayList);
        this.UserID.SetValue(UserID, ref FieldsArrayList);
        this.EntityID.SetValue(EntityID, ref FieldsArrayList);
        this.Name.SetValue(Name, ref FieldsArrayList);
        this.Sector.SetValue(Sector, ref FieldsArrayList);
        this.Description.SetValue(Description, ref FieldsArrayList);
        this.Location.SetValue(Location, ref FieldsArrayList);
        this.ContactPerson.SetValue(ContactPerson, ref FieldsArrayList);
        this.Email.SetValue(Email, ref FieldsArrayList);
        this.Phone.SetValue(Phone, ref FieldsArrayList);
        this.CreatedBy.SetValue(CreatedBy, ref FieldsArrayList);
        this.UpdatedBy.SetValue(UpdatedBy, ref FieldsArrayList);

        this.PageSize.SetValue(PageSize, ref FieldsArrayList);
        this.PageNumber.SetValue(PageNumber, ref FieldsArrayList);
        this.SortColumn.SetValue(SortColumn, ref FieldsArrayList);
        this.SortDirection.SetValue(SortDirection, ref FieldsArrayList);

        return base.QueryDatabase(QueryType);
    }
}
