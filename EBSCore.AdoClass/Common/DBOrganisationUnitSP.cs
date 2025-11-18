using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace EBSCore.AdoClass
{
    public class DBOrganisationUnitSP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField UserID = new TableField("UserID", SqlDbType.BigInt);
        public TableField CompanyID = new TableField("CompanyID", SqlDbType.Int);
        public TableField UnitID = new TableField("UnitID", SqlDbType.BigInt);
        public TableField UnitCode = new TableField("UnitCode", SqlDbType.NVarChar);
        public TableField UnitName = new TableField("UnitName", SqlDbType.NVarChar);
        public TableField UnitTypeID = new TableField("UnitTypeID", SqlDbType.Int);
        public TableField ColorCode = new TableField("ColorCode", SqlDbType.NVarChar);
        public TableField ParentUnitID = new TableField("ParentUnitID", SqlDbType.BigInt);
        public TableField Description = new TableField("Description", SqlDbType.NVarChar);
        public TableField Status = new TableField("Status", SqlDbType.Int);
        public TableField ExternalID = new TableField("ExternalID", SqlDbType.NVarChar);
        public TableField Location = new TableField("Location", SqlDbType.NVarChar);
        public TableField HierarchyLevel = new TableField("HierarchyLevel", SqlDbType.Int);
        public TableField IsActive = new TableField("IsActive", SqlDbType.Bit);
        public TableField CreatedBy = new TableField("CreatedBy", SqlDbType.Int);
        public TableField ModifiedBy = new TableField("ModifiedBy", SqlDbType.Int);
        public TableField CreatedAt = new TableField("CreatedAt", SqlDbType.DateTime);
        public TableField UpdatedAt = new TableField("UpdatedAt", SqlDbType.DateTime);

        public DBOrganisationUnitSP(IConfiguration configuration) : base(configuration)
        {
            base.SPName = "OrganisationUnitSP";
        }

        public new object QueryDatabase(SqlQueryType QueryType,
            string Operation = "", string UserID = "", string CompanyID = "", string UnitID = "",
            string UnitCode = "", string UnitName = "", string UnitTypeID = "", string ColorCode = "",
            string ParentUnitID = "", string Description = "", string Status = "", string ExternalID = "",
            string Location = "", string HierarchyLevel = "", string IsActive = "", string CreatedBy = "",
            string ModifiedBy = "", string CreatedAt = "", string UpdatedAt = "")
        {
            FieldsArrayList = new ArrayList();
            this.Operation.SetValue(Operation, ref FieldsArrayList);
            this.UserID.SetValue(UserID, ref FieldsArrayList);
            this.CompanyID.SetValue(CompanyID, ref FieldsArrayList);
            this.UnitID.SetValue(UnitID, ref FieldsArrayList);
            this.UnitCode.SetValue(UnitCode, ref FieldsArrayList);
            this.UnitName.SetValue(UnitName, ref FieldsArrayList);
            this.UnitTypeID.SetValue(UnitTypeID, ref FieldsArrayList);
            this.ColorCode.SetValue(ColorCode, ref FieldsArrayList);
            this.ParentUnitID.SetValue(ParentUnitID, ref FieldsArrayList);
            this.Description.SetValue(Description, ref FieldsArrayList);
            this.Status.SetValue(Status, ref FieldsArrayList);
            this.ExternalID.SetValue(ExternalID, ref FieldsArrayList);
            this.Location.SetValue(Location, ref FieldsArrayList);
            this.HierarchyLevel.SetValue(HierarchyLevel, ref FieldsArrayList);
            this.IsActive.SetValue(IsActive, ref FieldsArrayList);
            this.CreatedBy.SetValue(CreatedBy, ref FieldsArrayList);
            this.ModifiedBy.SetValue(ModifiedBy, ref FieldsArrayList);
            this.CreatedAt.SetValue(CreatedAt, ref FieldsArrayList);
            this.UpdatedAt.SetValue(UpdatedAt, ref FieldsArrayList);

            return base.QueryDatabase(QueryType);
        }
    }
}
