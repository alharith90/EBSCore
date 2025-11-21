-- BCM Plan extensions

CREATE TABLE IF NOT EXISTS BCMPlanApprovalWorkflow (
    ApprovalWorkflowID INT IDENTITY(1,1) PRIMARY KEY,
    PlanID INT NOT NULL,
    ApproverRole NVARCHAR(128) NOT NULL,
    Sequence INT NOT NULL,
    Status NVARCHAR(64) NOT NULL DEFAULT 'Pending',
    DecisionDate DATETIMEOFFSET(7) NULL,
    Comments NVARCHAR(MAX) NULL,
    CreatedBy BIGINT NOT NULL,
    CreatedAt DATETIMEOFFSET(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedBy BIGINT NULL,
    UpdatedAt DATETIMEOFFSET(7) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);
GO

CREATE TABLE IF NOT EXISTS BCMPlanVersion (
    PlanVersionID INT IDENTITY(1,1) PRIMARY KEY,
    PlanID INT NOT NULL,
    VersionNumber NVARCHAR(32) NOT NULL,
    ChangeSummary NVARCHAR(MAX) NULL,
    PublishedBy BIGINT NULL,
    PublishedAt DATETIMEOFFSET(7) NULL,
    CreatedBy BIGINT NOT NULL,
    CreatedAt DATETIMEOFFSET(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    IsPublished BIT NOT NULL DEFAULT 0,
    IsDeleted BIT NOT NULL DEFAULT 0
);
GO

CREATE TABLE IF NOT EXISTS BCMReviewCycle (
    ReviewCycleID INT IDENTITY(1,1) PRIMARY KEY,
    PlanID INT NOT NULL,
    NextReviewDate DATETIMEOFFSET(7) NOT NULL,
    FrequencyMonths INT NOT NULL DEFAULT 12,
    LastReviewedAt DATETIMEOFFSET(7) NULL,
    ReviewerID BIGINT NULL,
    CreatedBy BIGINT NOT NULL,
    CreatedAt DATETIMEOFFSET(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedBy BIGINT NULL,
    UpdatedAt DATETIMEOFFSET(7) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);
GO

CREATE TABLE IF NOT EXISTS BCMExerciseType (
    ExerciseTypeID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(128) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    CreatedAt DATETIMEOFFSET(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    IsDeleted BIT NOT NULL DEFAULT 0
);
GO

CREATE TABLE IF NOT EXISTS BCMEvaluationCriteria (
    EvaluationCriteriaID INT IDENTITY(1,1) PRIMARY KEY,
    ExerciseID INT NOT NULL,
    Criteria NVARCHAR(256) NOT NULL,
    Score INT NULL,
    Notes NVARCHAR(MAX) NULL,
    CreatedAt DATETIMEOFFSET(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    IsDeleted BIT NOT NULL DEFAULT 0
);
GO

CREATE PROCEDURE S7SPlan_SP
    @Operation NVARCHAR(50),
    @PlanID INT = NULL,
    @PlanCode NVARCHAR(50) = NULL,
    @PlanName NVARCHAR(200) = NULL,
    @CompanyID INT = NULL,
    @UnitID BIGINT = NULL,
    @NextReviewDate DATETIMEOFFSET(7) = NULL,
    @FrequencyMonths INT = NULL,
    @ExerciseTypeID INT = NULL,
    @PostIncidentSummary NVARCHAR(MAX) = NULL,
    @MandatoryControls NVARCHAR(MAX) = NULL,
    @AttachmentID BIGINT = NULL,
    @UserID BIGINT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF (@Operation = 'Get')
    BEGIN
        SELECT p.PlanID, p.PlanCode, p.PlanName, p.CompanyID, p.UnitID,
               rc.NextReviewDate, rc.FrequencyMonths,
               ex.ExerciseID, ex.ExerciseTypeID, et.Name AS ExerciseTypeName,
               pir.PostIncidentReportID, pir.Summary AS PostIncidentSummary, pir.MandatoryControls,
               f.FileID AS AttachmentID, f.FileName
          FROM BCMPlan p
          LEFT JOIN BCMReviewCycle rc ON rc.PlanID = p.PlanID AND rc.IsDeleted = 0
          LEFT JOIN BCMExercise ex ON ex.PlanID = p.PlanID AND ex.IsDeleted = 0
          LEFT JOIN BCMExerciseType et ON et.ExerciseTypeID = ex.ExerciseTypeID AND et.IsDeleted = 0
          LEFT JOIN BCMPostIncidentReport pir ON pir.PlanID = p.PlanID AND pir.IsDeleted = 0
          LEFT JOIN [File] f ON f.FileID = pir.FileID
         WHERE p.PlanID = ISNULL(@PlanID, p.PlanID) AND p.IsDeleted = 0;
        RETURN;
    END

    IF (@Operation = 'Upsert')
    BEGIN
        IF EXISTS(SELECT 1 FROM BCMPlan WHERE PlanID = @PlanID)
        BEGIN
            UPDATE BCMPlan SET PlanCode = @PlanCode, PlanName = @PlanName, UpdatedBy = @UserID, UpdatedAt = SYSUTCDATETIME()
            WHERE PlanID = @PlanID;
        END
        ELSE
        BEGIN
            INSERT INTO BCMPlan(PlanCode, PlanName, CompanyID, UnitID, CreatedBy, CreatedAt, IsDeleted)
            VALUES(@PlanCode, @PlanName, @CompanyID, @UnitID, @UserID, SYSUTCDATETIME(), 0);
            SET @PlanID = SCOPE_IDENTITY();
        END

        MERGE BCMReviewCycle AS target
        USING (SELECT @PlanID AS PlanID) AS source
        ON target.PlanID = source.PlanID AND target.IsDeleted = 0
        WHEN MATCHED THEN UPDATE SET FrequencyMonths = ISNULL(@FrequencyMonths, target.FrequencyMonths), NextReviewDate = ISNULL(@NextReviewDate, target.NextReviewDate), UpdatedAt = SYSUTCDATETIME(), UpdatedBy = @UserID
        WHEN NOT MATCHED THEN INSERT(PlanID, NextReviewDate, FrequencyMonths, CreatedBy) VALUES(@PlanID, ISNULL(@NextReviewDate, DATEADD(MONTH, ISNULL(@FrequencyMonths,12), SYSUTCDATETIME())), ISNULL(@FrequencyMonths,12), @UserID);

        IF @ExerciseTypeID IS NOT NULL
        BEGIN
            MERGE BCMExercise AS target
            USING (SELECT @PlanID AS PlanID) AS source
            ON target.PlanID = source.PlanID AND target.IsDeleted = 0
            WHEN MATCHED THEN UPDATE SET ExerciseTypeID = @ExerciseTypeID, UpdatedAt = SYSUTCDATETIME(), UpdatedBy = @UserID
            WHEN NOT MATCHED THEN INSERT(PlanID, ExerciseTypeID, CreatedBy, CreatedAt, IsDeleted) VALUES(@PlanID, @ExerciseTypeID, @UserID, SYSUTCDATETIME(), 0);
        END

        MERGE BCMPostIncidentReport AS target
        USING (SELECT @PlanID AS PlanID) AS source
        ON target.PlanID = source.PlanID AND target.IsDeleted = 0
        WHEN MATCHED THEN UPDATE SET Summary = @PostIncidentSummary, MandatoryControls = @MandatoryControls, FileID = @AttachmentID, UpdatedAt = SYSUTCDATETIME(), UpdatedBy = @UserID
        WHEN NOT MATCHED THEN INSERT(PlanID, Summary, MandatoryControls, FileID, CreatedBy, CreatedAt, IsDeleted) VALUES(@PlanID, @PostIncidentSummary, @MandatoryControls, @AttachmentID, @UserID, SYSUTCDATETIME(), 0);
    END
END
GO
