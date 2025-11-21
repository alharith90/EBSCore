/****** Object:  Table [dbo].[S7SRoleCompetency] ******/
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = 'S7SRoleCompetency')
BEGIN
    CREATE TABLE [dbo].[S7SRoleCompetency](
        [RoleCompetencyID] INT IDENTITY(1,1) NOT NULL,
        [RoleID] INT NOT NULL,
        [CompetencyID] INT NOT NULL,
        [RequiredLevel] NVARCHAR(100) NULL,
        [AssessedLevel] NVARCHAR(100) NULL,
        [GapAnalysis] NVARCHAR(MAX) NULL,
        [TrainingRequired] BIT NOT NULL DEFAULT(0),
    CONSTRAINT PK_S7SRoleCompetency PRIMARY KEY CLUSTERED(RoleCompetencyID)
    );
END
GO

/****** Object:  Table [dbo].[S7SCommitteePlanLink] ******/
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = 'S7SCommitteePlanLink')
BEGIN
    CREATE TABLE [dbo].[S7SCommitteePlanLink](
        [CommitteePlanLinkID] INT IDENTITY(1,1) NOT NULL,
        [CommitteeID] INT NOT NULL,
        [PlanID] INT NOT NULL,
        [LinkageType] NVARCHAR(50) NULL,
        [PlanName] NVARCHAR(255) NULL,
    CONSTRAINT PK_S7SCommitteePlanLink PRIMARY KEY CLUSTERED(CommitteePlanLinkID)
    );
END
GO

/****** Object:  Table [dbo].[S7STrainingRequirement] ******/
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = 'S7STrainingRequirement')
BEGIN
    CREATE TABLE [dbo].[S7STrainingRequirement](
        [TrainingRequirementID] INT IDENTITY(1,1) NOT NULL,
        [RoleID] INT NULL,
        [CompetencyID] INT NULL,
        [TrainingCourse] NVARCHAR(255) NULL,
        [TargetDate] DATETIME NULL,
        [CompletionDate] DATETIME NULL,
        [Notes] NVARCHAR(MAX) NULL,
    CONSTRAINT PK_S7STrainingRequirement PRIMARY KEY CLUSTERED(TrainingRequirementID)
    );
END
GO

/****** Object:  StoredProcedure [dbo].[S7SGovernance_SP] ******/
IF EXISTS (SELECT 1 FROM sys.objects WHERE type = 'P' AND name = 'S7SGovernance_SP')
    DROP PROCEDURE [dbo].[S7SGovernance_SP];
GO
CREATE PROCEDURE [dbo].[S7SGovernance_SP]
    @Operation NVARCHAR(50),
    @UserID BIGINT = NULL,
    @CompanyID INT = NULL,
    @CommitteeID INT = NULL,
    @RoleID INT = NULL,
    @CompetencyID INT = NULL,
    @PlanID INT = NULL,
    @EmployeeID INT = NULL,
    @SkillRequired NVARCHAR(255) = NULL,
    @RequiredLevel NVARCHAR(100) = NULL,
    @AssessedLevel NVARCHAR(100) = NULL,
    @TrainingCourse NVARCHAR(255) = NULL,
    @TargetDate DATETIME = NULL,
    @CompletionDate DATETIME = NULL,
    @Notes NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Operation = 'rtvGovernance'
    BEGIN
        SELECT CommitteeID, Name, Charter, EstablishedDate, Chairperson, Status FROM BCMCommittee;
        SELECT RoleID, RoleName, Responsibilities, AssignedTo FROM BCMSRole;
        SELECT CompetencyID, RoleID, SkillRequired, RequiredLevel, AssessedLevel, GapAnalysis, TrainingRequired FROM CompetencyMatrix;
        SELECT CommitteeID, PlanID, NULL AS PlanName, 'Mapped' AS LinkageType FROM S7SCommitteePlanLink;
        SELECT TrainingRequirementID, RoleID, CompetencyID, TrainingCourse, TargetDate, CompletionDate, Notes FROM S7STrainingRequirement;
        RETURN;
    END

    IF @Operation = 'upsertCompetency'
    BEGIN
        IF EXISTS(SELECT 1 FROM CompetencyMatrix WHERE CompetencyID = @CompetencyID)
        BEGIN
            UPDATE CompetencyMatrix
            SET SkillRequired = ISNULL(@SkillRequired, SkillRequired),
                RequiredLevel = ISNULL(@RequiredLevel, RequiredLevel),
                AssessedLevel = ISNULL(@AssessedLevel, AssessedLevel),
                GapAnalysis = ISNULL(@Notes, GapAnalysis)
            WHERE CompetencyID = @CompetencyID;
        END
        ELSE
        BEGIN
            INSERT INTO CompetencyMatrix(RoleID, SkillRequired, RequiredLevel, AssessedLevel, GapAnalysis, TrainingRequired)
            VALUES(@RoleID, @SkillRequired, @RequiredLevel, @AssessedLevel, @Notes, 1);
        END
        RETURN;
    END
END
GO
