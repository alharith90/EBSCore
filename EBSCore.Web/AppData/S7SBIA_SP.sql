CREATE OR ALTER PROCEDURE [dbo].[S7SBIA_SP]
    @Operation NVARCHAR(50),
    @CompanyID INT = NULL,
    @UserID BIGINT = NULL,
    @BIAID INT = NULL,
    @BIACode NVARCHAR(50) = NULL,
    @UnitID NVARCHAR(50) = NULL,
    @ProcessID NVARCHAR(50) = NULL,
    @ProcessName NVARCHAR(255) = NULL,
    @ProcessDescription NVARCHAR(MAX) = NULL,
    @Frequency NVARCHAR(100) = NULL,
    @Criticality NVARCHAR(50) = NULL,
    @RTO NVARCHAR(50) = NULL,
    @RPO NVARCHAR(50) = NULL,
    @MTPD NVARCHAR(50) = NULL,
    @MAO NVARCHAR(50) = NULL,
    @MBCO NVARCHAR(50) = NULL,
    @Priority NVARCHAR(50) = NULL,
    @RequiredCompetencies NVARCHAR(MAX) = NULL,
    @AlternativeWorkLocation NVARCHAR(255) = NULL,
    @RegulatoryRequirements NVARCHAR(MAX) = NULL,
    @PrimaryStaff NVARCHAR(255) = NULL,
    @BackupStaff NVARCHAR(255) = NULL,
    @RTOJustification NVARCHAR(MAX) = NULL,
    @MBCODetails NVARCHAR(MAX) = NULL,
    @RevenueLossPerHour NVARCHAR(50) = NULL,
    @CostOfDowntime NVARCHAR(50) = NULL,
    @Remarks NVARCHAR(MAX) = NULL,
    @LastComment NVARCHAR(MAX) = NULL,
    @ReviewDate NVARCHAR(50) = NULL,
    @WorkFlowStatus NVARCHAR(50) = NULL,
    @IsDeleted BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    IF(@Operation = 'rtvList')
    BEGIN
        SELECT * FROM BIA WHERE IsDeleted = 0 AND (@CompanyID IS NULL OR CompanyID = @CompanyID);
        RETURN;
    END

    IF(@Operation = 'rtvItem')
    BEGIN
        SELECT * FROM BIA WHERE BIAID = @BIAID;
        RETURN;
    END

    IF(@Operation = 'Delete')
    BEGIN
        UPDATE BIA SET IsDeleted = 1, UpdatedBy = @UserID, UpdatedAt = GETDATE() WHERE BIAID = @BIAID;
        RETURN;
    END

    IF(@Operation = 'Save')
    BEGIN
        IF EXISTS(SELECT 1 FROM BIA WHERE BIAID = @BIAID)
        BEGIN
            UPDATE BIA
            SET BIACode = @BIACode,
                UnitID = @UnitID,
                ProcessID = @ProcessID,
                ProcessName = @ProcessName,
                ProcessDescription = @ProcessDescription,
                Frequency = @Frequency,
                Criticality = @Criticality,
                RTO = @RTO,
                RPO = @RPO,
                MAO = @MAO,
                MTPD = @MTPD,
                MTD = @MTPD,
                MBCO = @MBCO,
                Priority = @Priority,
                RequiredCompetencies = @RequiredCompetencies,
                AlternativeWorkLocation = @AlternativeWorkLocation,
                RegulatoryRequirements = @RegulatoryRequirements,
                PrimaryStaff = @PrimaryStaff,
                BackupStaff = @BackupStaff,
                RTOJustification = @RTOJustification,
                MBCODetails = @MBCODetails,
                RevenueLossPerHour = @RevenueLossPerHour,
                CostOfDowntime = @CostOfDowntime,
                Remarks = @Remarks,
                LastComment = @LastComment,
                ReviewDate = @ReviewDate,
                WorkFlowStatus = @WorkFlowStatus,
                UpdatedBy = @UserID,
                UpdatedAt = GETDATE()
            WHERE BIAID = @BIAID;
        END
        ELSE
        BEGIN
            INSERT INTO BIA (
                CompanyID, BIACode, UnitID, ProcessID, ProcessName, ProcessDescription, Frequency, Criticality, RTO, RPO, MAO, MTPD, MTD,
                MBCO, Priority, RequiredCompetencies, AlternativeWorkLocation, RegulatoryRequirements, PrimaryStaff, BackupStaff,
                RTOJustification, MBCODetails, RevenueLossPerHour, CostOfDowntime, Remarks, LastComment, ReviewDate, WorkFlowStatus,
                IsDeleted, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
            VALUES (
                @CompanyID, @BIACode, @UnitID, @ProcessID, @ProcessName, @ProcessDescription, @Frequency, @Criticality, @RTO, @RPO, @MAO, @MTPD, @MTPD,
                @MBCO, @Priority, @RequiredCompetencies, @AlternativeWorkLocation, @RegulatoryRequirements, @PrimaryStaff, @BackupStaff,
                @RTOJustification, @MBCODetails, @RevenueLossPerHour, @CostOfDowntime, @Remarks, @LastComment, @ReviewDate, @WorkFlowStatus,
                @IsDeleted, @UserID, GETDATE(), @UserID, GETDATE());
        END
        RETURN;
    END
END
