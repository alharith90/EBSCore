using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Data;

namespace EBSCore.AdoClass
{
    /// <summary>
    /// Stored procedure wrapper for governance tables covering committees, roles, competencies and training.
    /// </summary>
    public class DBS7SGovernance_SP : DBParentStoredProcedureClass
    {
        public TableField Operation = new("Operation", SqlDbType.NVarChar);
        public TableField UserID = new("UserID", SqlDbType.BigInt);
        public TableField CompanyID = new("CompanyID", SqlDbType.Int);
        public TableField CommitteeID = new("CommitteeID", SqlDbType.Int);
        public TableField RoleID = new("RoleID", SqlDbType.Int);
        public TableField CompetencyID = new("CompetencyID", SqlDbType.Int);
        public TableField PlanID = new("PlanID", SqlDbType.Int);
        public TableField EmployeeID = new("EmployeeID", SqlDbType.Int);
        public TableField SkillRequired = new("SkillRequired", SqlDbType.NVarChar);
        public TableField RequiredLevel = new("RequiredLevel", SqlDbType.NVarChar);
        public TableField AssessedLevel = new("AssessedLevel", SqlDbType.NVarChar);
        public TableField TrainingCourse = new("TrainingCourse", SqlDbType.NVarChar);
        public TableField TargetDate = new("TargetDate", SqlDbType.DateTime);
        public TableField CompletionDate = new("CompletionDate", SqlDbType.DateTime);
        public TableField Notes = new("Notes", SqlDbType.NVarChar);

        public DBS7SGovernance_SP(IConfiguration configuration) : base(configuration)
        {
            base.SPName = "S7SGovernance_SP";
        }

        public new object QueryDatabase(
            SqlQueryType queryType,
            string Operation = "",
            string UserID = "",
            string CompanyID = "",
            string CommitteeID = "",
            string RoleID = "",
            string CompetencyID = "",
            string PlanID = "",
            string EmployeeID = "",
            string SkillRequired = "",
            string RequiredLevel = "",
            string AssessedLevel = "",
            string TrainingCourse = "",
            string TargetDate = "",
            string CompletionDate = "",
            string Notes = ""
        )
        {
            FieldsArrayList = new ArrayList();

            this.Operation.SetValue(Operation, ref FieldsArrayList);
            this.UserID.SetValue(UserID, ref FieldsArrayList);
            this.CompanyID.SetValue(CompanyID, ref FieldsArrayList);
            this.CommitteeID.SetValue(CommitteeID, ref FieldsArrayList);
            this.RoleID.SetValue(RoleID, ref FieldsArrayList);
            this.CompetencyID.SetValue(CompetencyID, ref FieldsArrayList);
            this.PlanID.SetValue(PlanID, ref FieldsArrayList);
            this.EmployeeID.SetValue(EmployeeID, ref FieldsArrayList);
            this.SkillRequired.SetValue(SkillRequired, ref FieldsArrayList);
            this.RequiredLevel.SetValue(RequiredLevel, ref FieldsArrayList);
            this.AssessedLevel.SetValue(AssessedLevel, ref FieldsArrayList);
            this.TrainingCourse.SetValue(TrainingCourse, ref FieldsArrayList);
            this.TargetDate.SetValue(TargetDate, ref FieldsArrayList);
            this.CompletionDate.SetValue(CompletionDate, ref FieldsArrayList);
            this.Notes.SetValue(Notes, ref FieldsArrayList);

            return base.QueryDatabase(queryType);
        }
    }
}
