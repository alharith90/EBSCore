using EBSCore.AdoClass;
using Microsoft.Extensions.Configuration;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;
using System.Collections;
using System.Data;

public class DBSupplierSP : DBParentStoredProcedureClass
{
    public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
    public TableField UserID = new TableField("UserID", SqlDbType.BigInt);
    public TableField CompanyID = new TableField("CompanyID", SqlDbType.Int);
    public TableField UnitID = new TableField("UnitID", SqlDbType.BigInt);
    public TableField SupplierID = new TableField("SupplierID", SqlDbType.Int);
    public TableField SupplierType = new TableField("SupplierType", SqlDbType.NVarChar);
    public TableField SupplierName = new TableField("SupplierName", SqlDbType.NVarChar);
    public TableField Services = new TableField("Services", SqlDbType.NVarChar);
    public TableField MainContactName = new TableField("MainContactName", SqlDbType.NVarChar);
    public TableField MainContactEmail = new TableField("MainContactEmail", SqlDbType.NVarChar);
    public TableField MainContactPhone = new TableField("MainContactPhone", SqlDbType.NVarChar);
    public TableField SecondaryContactName = new TableField("SecondaryContactName", SqlDbType.NVarChar);
    public TableField SecondaryContactEmail = new TableField("SecondaryContactEmail", SqlDbType.NVarChar);
    public TableField SecondaryContactPhone = new TableField("SecondaryContactPhone", SqlDbType.NVarChar);
    public TableField SLAInPlace = new TableField("SLAInPlace", SqlDbType.Bit);
    public TableField RTOHours = new TableField("RTOHours", SqlDbType.Int);
    public TableField RPOHours = new TableField("RPOHours", SqlDbType.Int);
    public TableField Notes = new TableField("Notes", SqlDbType.NVarChar);
    public TableField CreatedBy = new TableField("CreatedBy", SqlDbType.Int);
    public TableField ModifiedBy = new TableField("ModifiedBy", SqlDbType.Int);
    public TableField CreatedAt = new TableField("CreatedAt", SqlDbType.DateTime);
    public TableField UpdatedAt = new TableField("UpdatedAt", SqlDbType.DateTime);

    public DBSupplierSP(IConfiguration configuration) : base(configuration)
    {
        base.SPName = "SupplierSP";
    }

    public new object QueryDatabase(
        SqlQueryType QueryType,
        string Operation = "",
        string UserID = "",
        string CompanyID = "",
        string UnitID = "",
        string SupplierID = "",
        string SupplierType = "",
        string SupplierName = "",
        string Services = "",
        string MainContactName = "",
        string MainContactEmail = "",
        string MainContactPhone = "",
        string SecondaryContactName = "",
        string SecondaryContactEmail = "",
        string SecondaryContactPhone = "",
        string SLAInPlace = "",
        string RTOHours = "",
        string RPOHours = "",
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
        this.SupplierID.SetValue(SupplierID, ref FieldsArrayList);
        this.SupplierType.SetValue(SupplierType, ref FieldsArrayList);
        this.SupplierName.SetValue(SupplierName, ref FieldsArrayList);
        this.Services.SetValue(Services, ref FieldsArrayList);
        this.MainContactName.SetValue(MainContactName, ref FieldsArrayList);
        this.MainContactEmail.SetValue(MainContactEmail, ref FieldsArrayList);
        this.MainContactPhone.SetValue(MainContactPhone, ref FieldsArrayList);
        this.SecondaryContactName.SetValue(SecondaryContactName, ref FieldsArrayList);
        this.SecondaryContactEmail.SetValue(SecondaryContactEmail, ref FieldsArrayList);
        this.SecondaryContactPhone.SetValue(SecondaryContactPhone, ref FieldsArrayList);
        this.SLAInPlace.SetValue(SLAInPlace, ref FieldsArrayList);
        this.RTOHours.SetValue(RTOHours, ref FieldsArrayList);
        this.RPOHours.SetValue(RPOHours, ref FieldsArrayList);
        this.Notes.SetValue(Notes, ref FieldsArrayList);
        this.CreatedBy.SetValue(CreatedBy, ref FieldsArrayList);
        this.ModifiedBy.SetValue(ModifiedBy, ref FieldsArrayList);
        this.CreatedAt.SetValue(CreatedAt, ref FieldsArrayList);
        this.UpdatedAt.SetValue(UpdatedAt, ref FieldsArrayList);

        return base.QueryDatabase(QueryType);
    }
}
