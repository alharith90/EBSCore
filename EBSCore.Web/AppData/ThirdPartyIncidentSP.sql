USE [EBS]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[ThirdPartyIncidentSP]
    @Operation NVARCHAR(100) = NULL,
    @UserID BIGINT = NULL,
    @CompanyID INT = NULL,
    @IssueIncidentID INT = NULL,
    @ThirdPartyID INT = NULL,
    @Date DATETIME = NULL,
    @IssueType NVARCHAR(250) = NULL,
    @Description NVARCHAR(MAX) = NULL,
    @ImpactOnBusiness NVARCHAR(MAX) = NULL,
    @Severity NVARCHAR(250) = NULL,
    @RelatedSLABreach NVARCHAR(250) = NULL,
    @ActionsTakenByVendor NVARCHAR(MAX) = NULL,
    @ActionsTakenInternally NVARCHAR(MAX) = NULL,
    @Status NVARCHAR(50) = NULL,
    @LinkedRiskEvent NVARCHAR(250) = NULL,
    @Notes NVARCHAR(MAX) = NULL,
    @CreatedBy INT = NULL,
    @UpdatedBy INT = NULL
AS
BEGIN
    DECLARE @CurrentUserType AS INT = (SELECT UserType FROM [User] WHERE UserID = @UserID)
    DECLARE @CurrentCompanyID AS INT = (SELECT CompanyID FROM [User] WHERE UserID = @UserID)

    IF @CurrentUserType <> 1 OR @CurrentCompanyID <> @CompanyID
        THROW 51000, 'You''re not allowed to do the action', 1;

    IF @Operation = 'SaveThirdPartyIncident'
    BEGIN
        IF EXISTS(SELECT 1 FROM ThirdPartyIncident WHERE IssueIncidentID = @IssueIncidentID AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'ThirdPartyIncident', @IssueIncidentID,
                    (SELECT * FROM ThirdPartyIncident WHERE IssueIncidentID = @IssueIncidentID FOR XML AUTO), GETDATE())

            UPDATE [dbo].[ThirdPartyIncident]
            SET ThirdPartyID = @ThirdPartyID,
                Date = @Date,
                IssueType = @IssueType,
                Description = @Description,
                ImpactOnBusiness = @ImpactOnBusiness,
                Severity = @Severity,
                RelatedSLABreach = @RelatedSLABreach,
                ActionsTakenByVendor = @ActionsTakenByVendor,
                ActionsTakenInternally = @ActionsTakenInternally,
                Status = @Status,
                LinkedRiskEvent = @LinkedRiskEvent,
                Notes = @Notes,
                UpdatedBy = @UserID,
                UpdatedAt = GETUTCDATE()
            WHERE IssueIncidentID = @IssueIncidentID
              AND CompanyID = @CompanyID
        END
        ELSE
        BEGIN
            INSERT INTO [dbo].[ThirdPartyIncident]
            (
                CompanyID,
                ThirdPartyID,
                Date,
                IssueType,
                Description,
                ImpactOnBusiness,
                Severity,
                RelatedSLABreach,
                ActionsTakenByVendor,
                ActionsTakenInternally,
                Status,
                LinkedRiskEvent,
                Notes,
                CreatedBy,
                CreatedAt,
                UpdatedAt
            )
            VALUES
            (
                @CompanyID,
                @ThirdPartyID,
                @Date,
                @IssueType,
                @Description,
                @ImpactOnBusiness,
                @Severity,
                @RelatedSLABreach,
                @ActionsTakenByVendor,
                @ActionsTakenInternally,
                @Status,
                @LinkedRiskEvent,
                @Notes,
                @UserID,
                GETUTCDATE(),
                GETUTCDATE()
            )

            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'INSERT', 'ThirdPartyIncident', @IssueIncidentID,
                    (SELECT * FROM ThirdPartyIncident WHERE IssueIncidentID = (SELECT TOP 1 @@IDENTITY FROM ThirdPartyIncident) FOR XML AUTO), GETDATE())
        END
    END
    ELSE IF @Operation = 'rtvThirdPartyIncidents'
    BEGIN
        SELECT IssueIncidentID, CompanyID, ThirdPartyID, Date, IssueType, Description, ImpactOnBusiness, Severity, RelatedSLABreach,
               ActionsTakenByVendor, ActionsTakenInternally, Status, LinkedRiskEvent, Notes, CreatedBy, UpdatedBy, CreatedAt, UpdatedAt
        FROM [ThirdPartyIncident]
        WHERE CompanyID = @CompanyID

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'ThirdPartyIncident', NULL,
                (SELECT * FROM ThirdPartyIncident FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'rtvThirdPartyIncident'
    BEGIN
        SELECT IssueIncidentID, CompanyID, ThirdPartyID, Date, IssueType, Description, ImpactOnBusiness, Severity, RelatedSLABreach,
               ActionsTakenByVendor, ActionsTakenInternally, Status, LinkedRiskEvent, Notes, CreatedBy, UpdatedBy, CreatedAt, UpdatedAt
        FROM [ThirdPartyIncident]
        WHERE IssueIncidentID = @IssueIncidentID
          AND CompanyID = @CompanyID

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select One', 'ThirdPartyIncident', @IssueIncidentID,
                (SELECT * FROM ThirdPartyIncident WHERE IssueIncidentID = @IssueIncidentID FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'DeleteThirdPartyIncident'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'ThirdPartyIncident', @IssueIncidentID,
                (SELECT * FROM ThirdPartyIncident WHERE IssueIncidentID = @IssueIncidentID FOR XML AUTO), GETDATE())

        DELETE FROM [ThirdPartyIncident]
        WHERE IssueIncidentID = @IssueIncidentID
          AND CompanyID = @CompanyID
    END
END
GO
