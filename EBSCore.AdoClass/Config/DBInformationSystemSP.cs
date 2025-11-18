using EBSCore.AdoClass;
using Microsoft.Extensions.Configuration;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;
using System.Collections;
using System.Data;

public class DBInformationSystemSP : DBParentStoredProcedureClass
{
    public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
    public TableField UserID = new TableField("UserID", SqlDbType.Int);
    public TableField SystemID = new TableField("SystemID", SqlDbType.Int);
    public TableField CompanyID = new TableField("CompanyID", SqlDbType.Int);
    public TableField UnitID = new TableField("UnitID", SqlDbType.BigInt);
    public TableField SystemName = new TableField("SystemName", SqlDbType.NVarChar);
    public TableField RPO = new TableField("RPO", SqlDbType.NVarChar);
    public TableField ApplicationLifecycleStatus = new TableField("ApplicationLifecycleStatus", SqlDbType.NVarChar);
    public TableField Type = new TableField("Type", SqlDbType.NVarChar);
    public TableField RequiredFor = new TableField("RequiredFor", SqlDbType.NVarChar);
    public TableField SystemDescription = new TableField("SystemDescription", SqlDbType.NVarChar);
    public TableField PrimaryOwnerId = new TableField("PrimaryOwnerId", SqlDbType.Int);
    public TableField SecondaryOwner = new TableField("SecondaryOwner", SqlDbType.NVarChar);
    public TableField BusinessOwner = new TableField("BusinessOwner", SqlDbType.NVarChar);
    public TableField InternetFacing = new TableField("InternetFacing", SqlDbType.Bit);
    public TableField ThirdPartyAccess = new TableField("ThirdPartyAccess", SqlDbType.Bit);
    public TableField NumberOfUsers = new TableField("NumberOfUsers", SqlDbType.Int);
    public TableField LicenseType = new TableField("LicenseType", SqlDbType.NVarChar);
    public TableField Infrastructure = new TableField("Infrastructure", SqlDbType.Bit);
    public TableField MFAEnabled = new TableField("MFAEnabled", SqlDbType.Bit);
    public TableField MFAStatusDetails = new TableField("MFAStatusDetails", SqlDbType.NVarChar);
    public TableField AssociatedInformationSystems = new TableField("AssociatedInformationSystems", SqlDbType.NVarChar);
    public TableField Confidentiality = new TableField("Confidentiality", SqlDbType.NVarChar);
    public TableField Integrity = new TableField("Integrity", SqlDbType.NVarChar);
    public TableField Availability = new TableField("Availability", SqlDbType.NVarChar);
    public TableField OverallCategorizationRating = new TableField("OverallCategorizationRating", SqlDbType.NVarChar);
    public TableField HighestInformationClassification = new TableField("HighestInformationClassification", SqlDbType.NVarChar);
    public TableField RiskHighlightedByIT = new TableField("RiskHighlightedByIT", SqlDbType.NVarChar);
    public TableField AdditionalNote = new TableField("AdditionalNote", SqlDbType.NVarChar);
    public TableField Logo = new TableField("Logo", SqlDbType.VarBinary);
    public TableField CreatedBy = new TableField("CreatedBy", SqlDbType.Int);
    public TableField UpdatedBy = new TableField("UpdatedBy", SqlDbType.Int);

    // ➕ New Pagination and Sorting Fields
    public TableField PageSize = new TableField("PageSize", SqlDbType.Int);
    public TableField PageNumber = new TableField("PageNumber", SqlDbType.Int);
    public TableField SortColumn = new TableField("SortColumn", SqlDbType.NVarChar);
    public TableField SortDirection = new TableField("SortDirection", SqlDbType.NVarChar);

    public DBInformationSystemSP(IConfiguration configuration) : base(configuration)
    {
        base.SPName = "InformationSystemSP";
    }

    public new object QueryDatabase(
        SqlQueryType QueryType,
        string Operation = "", string UserID = "", string SystemID = "", string CompanyID = "",
        string UnitID = "", string SystemName = "", string RPO = "", string ApplicationLifecycleStatus = "",
        string Type = "", string RequiredFor = "", string SystemDescription = "", string PrimaryOwnerId = "",
        string SecondaryOwner = "", string BusinessOwner = "", string InternetFacing = "", string ThirdPartyAccess = "",
        string NumberOfUsers = "", string LicenseType = "", string Infrastructure = "", string MFAEnabled = "",
        string MFAStatusDetails = "", string AssociatedInformationSystems = "", string Confidentiality = "",
        string Integrity = "", string Availability = "", string OverallCategorizationRating = "",
        string HighestInformationClassification = "", string RiskHighlightedByIT = "", string AdditionalNote = "",
        byte[] Logo = null, string CreatedBy = "", string UpdatedBy = "",
        string PageSize = "", string PageNumber = "", string SortColumn = "", string SortDirection = ""
    )
    {
        FieldsArrayList = new ArrayList();

        this.Operation.SetValue(Operation, ref FieldsArrayList);
        this.UserID.SetValue(UserID, ref FieldsArrayList);
        this.SystemID.SetValue(SystemID, ref FieldsArrayList);
        this.CompanyID.SetValue(CompanyID, ref FieldsArrayList);
        this.UnitID.SetValue(UnitID, ref FieldsArrayList);
        this.SystemName.SetValue(SystemName, ref FieldsArrayList);
        this.RPO.SetValue(RPO, ref FieldsArrayList);
        this.ApplicationLifecycleStatus.SetValue(ApplicationLifecycleStatus, ref FieldsArrayList);
        this.Type.SetValue(Type, ref FieldsArrayList);
        this.RequiredFor.SetValue(RequiredFor, ref FieldsArrayList);
        this.SystemDescription.SetValue(SystemDescription, ref FieldsArrayList);
        this.PrimaryOwnerId.SetValue(PrimaryOwnerId, ref FieldsArrayList);
        this.SecondaryOwner.SetValue(SecondaryOwner, ref FieldsArrayList);
        this.BusinessOwner.SetValue(BusinessOwner, ref FieldsArrayList);
        this.InternetFacing.SetValue(InternetFacing, ref FieldsArrayList);
        this.ThirdPartyAccess.SetValue(ThirdPartyAccess, ref FieldsArrayList);
        this.NumberOfUsers.SetValue(NumberOfUsers, ref FieldsArrayList);
        this.LicenseType.SetValue(LicenseType, ref FieldsArrayList);
        this.Infrastructure.SetValue(Infrastructure, ref FieldsArrayList);
        this.MFAEnabled.SetValue(MFAEnabled, ref FieldsArrayList);
        this.MFAStatusDetails.SetValue(MFAStatusDetails, ref FieldsArrayList);
        this.AssociatedInformationSystems.SetValue(AssociatedInformationSystems, ref FieldsArrayList);
        this.Confidentiality.SetValue(Confidentiality, ref FieldsArrayList);
        this.Integrity.SetValue(Integrity, ref FieldsArrayList);
        this.Availability.SetValue(Availability, ref FieldsArrayList);
        this.OverallCategorizationRating.SetValue(OverallCategorizationRating, ref FieldsArrayList);
        this.HighestInformationClassification.SetValue(HighestInformationClassification, ref FieldsArrayList);
        this.RiskHighlightedByIT.SetValue(RiskHighlightedByIT, ref FieldsArrayList);
        this.AdditionalNote.SetValue(AdditionalNote, ref FieldsArrayList);
        this.Logo.SetBytes(Logo, ref FieldsArrayList);
        this.CreatedBy.SetValue(CreatedBy, ref FieldsArrayList);
        this.UpdatedBy.SetValue(UpdatedBy, ref FieldsArrayList);

        // ➕ New Pagination and Sorting Parameters
        this.PageSize.SetValue(PageSize, ref FieldsArrayList);
        this.PageNumber.SetValue(PageNumber, ref FieldsArrayList);
        this.SortColumn.SetValue(SortColumn, ref FieldsArrayList);
        this.SortDirection.SetValue(SortDirection, ref FieldsArrayList);

        return base.QueryDatabase(QueryType);
    }
}
