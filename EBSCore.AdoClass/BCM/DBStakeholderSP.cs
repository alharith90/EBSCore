using EBSCore.AdoClass;
using Microsoft.Extensions.Configuration;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;
using System.Collections;
using System.Data;

public class DBStakeholderSP : DBParentStoredProcedureClass
{
    public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
    public TableField UserID = new TableField("UserID", SqlDbType.BigInt);
    public TableField CompanyID = new TableField("CompanyID", SqlDbType.Int);
    public TableField UnitID = new TableField("UnitID", SqlDbType.BigInt);
    public TableField StakeholderID = new TableField("StakeholderID", SqlDbType.Int);
    public TableField StakeholderName = new TableField("StakeholderName", SqlDbType.NVarChar);
    public TableField StakeholderType = new TableField("StakeholderType", SqlDbType.NVarChar);
    public TableField Role = new TableField("Role", SqlDbType.NVarChar);
    public TableField ContactEmail = new TableField("ContactEmail", SqlDbType.NVarChar);
    public TableField ContactPhone = new TableField("ContactPhone", SqlDbType.NVarChar);
    public TableField IsCritical = new TableField("IsCritical", SqlDbType.Bit);
    public TableField Notes = new TableField("Notes", SqlDbType.NVarChar);
    public TableField CreatedBy = new TableField("CreatedBy", SqlDbType.Int);
    public TableField ModifiedBy = new TableField("ModifiedBy", SqlDbType.Int);
    public TableField CreatedAt = new TableField("CreatedAt", SqlDbType.DateTime);
    public TableField UpdatedAt = new TableField("UpdatedAt", SqlDbType.DateTime);

    public DBStakeholderSP(IConfiguration configuration) : base(configuration)
    {
        base.SPName = "StakeholderSP";
    }

    public new object QueryDatabase(
        SqlQueryType QueryType,
        string Operation = "",
        string UserID = "",
        string CompanyID = "",
        string UnitID = "",
        string StakeholderID = "",
        string StakeholderName = "",
        string StakeholderType = "",
        string Role = "",
        string ContactEmail = "",
        string ContactPhone = "",
        string IsCritical = "",
        string Notes = "",
        string CreatedBy = "",
        string ModifiedBy = "",
        string CreatedAt = "",
        string UpdatedAt = ""
    )
    {
        FieldsArrayList = new ArrayList();

        this.Operation.SetValue(Operation, ref FieldsArrayList);
        this.UserID.SetValue(UserID, ref FieldsArrayList);
        this.CompanyID.SetValue(CompanyID, ref FieldsArrayList);
        this.UnitID.SetValue(UnitID, ref FieldsArrayList);
        this.StakeholderID.SetValue(StakeholderID, ref FieldsArrayList);
        this.StakeholderName.SetValue(StakeholderName, ref FieldsArrayList);
        this.StakeholderType.SetValue(StakeholderType, ref FieldsArrayList);
        this.Role.SetValue(Role, ref FieldsArrayList);
        this.ContactEmail.SetValue(ContactEmail, ref FieldsArrayList);
        this.ContactPhone.SetValue(ContactPhone, ref FieldsArrayList);
        this.IsCritical.SetValue(IsCritical, ref FieldsArrayList);
        this.Notes.SetValue(Notes, ref FieldsArrayList);
        this.CreatedBy.SetValue(CreatedBy, ref FieldsArrayList);
        this.ModifiedBy.SetValue(ModifiedBy, ref FieldsArrayList);
        this.CreatedAt.SetValue(CreatedAt, ref FieldsArrayList);
        this.UpdatedAt.SetValue(UpdatedAt, ref FieldsArrayList);

        return base.QueryDatabase(QueryType);
    }
}
