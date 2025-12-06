USE [EBS]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[ThirdPartyProfileSP]
    @Operation NVARCHAR(100) = NULL,
    @UserID BIGINT = NULL,
    @CompanyID INT = NULL,
    @ThirdPartyID INT = NULL,
    @ThirdPartyName NVARCHAR(250) = NULL,
    @ServiceType NVARCHAR(250) = NULL,
    @CriticalityTier NVARCHAR(250) = NULL,
    @InherentRiskRating NVARCHAR(250) = NULL,
    @CountryRegion NVARCHAR(250) = NULL,
    @BusinessOwner NVARCHAR(250) = NULL,
    @ContractValue NVARCHAR(250) = NULL,
    @ContractExpiryDate DATETIME = NULL,
    @KeySLAKPIRequirements NVARCHAR(MAX) = NULL,
    @ComplianceRequirements NVARCHAR(MAX) = NULL,
    @PrivacyDataProcessing BIT = NULL,
    @RelatedAssetProcess NVARCHAR(250) = NULL,
    @ContingencyPlan NVARCHAR(MAX) = NULL,
    @LastAssessmentDate DATETIME = NULL,
    @NextReviewDate DATETIME = NULL,
    @Status NVARCHAR(50) = NULL,
    @CreatedBy INT = NULL,
    @UpdatedBy INT = NULL
AS
BEGIN
    DECLARE @CurrentUserType AS INT = (SELECT UserType FROM [User] WHERE UserID = @UserID)
    DECLARE @CurrentCompanyID AS INT = (SELECT CompanyID FROM [User] WHERE UserID = @UserID)

    IF @CurrentUserType <> 1 OR @CurrentCompanyID <> @CompanyID
        THROW 51000, 'You''re not allowed to do the action', 1;

    IF @Operation = 'SaveThirdPartyProfile'
    BEGIN
        IF EXISTS(SELECT 1 FROM ThirdPartyProfile WHERE ThirdPartyID = @ThirdPartyID AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'ThirdPartyProfile', @ThirdPartyID,
                    (SELECT * FROM ThirdPartyProfile WHERE ThirdPartyID = @ThirdPartyID FOR XML AUTO), GETDATE())

            UPDATE [dbo].[ThirdPartyProfile]
            SET ThirdPartyName = @ThirdPartyName,
                ServiceType = @ServiceType,
                CriticalityTier = @CriticalityTier,
                InherentRiskRating = @InherentRiskRating,
                CountryRegion = @CountryRegion,
                BusinessOwner = @BusinessOwner,
                ContractValue = @ContractValue,
                ContractExpiryDate = @ContractExpiryDate,
                KeySLAKPIRequirements = @KeySLAKPIRequirements,
                ComplianceRequirements = @ComplianceRequirements,
                PrivacyDataProcessing = @PrivacyDataProcessing,
                RelatedAssetProcess = @RelatedAssetProcess,
                ContingencyPlan = @ContingencyPlan,
                LastAssessmentDate = @LastAssessmentDate,
                NextReviewDate = @NextReviewDate,
                Status = @Status,
                UpdatedBy = @UserID,
                UpdatedAt = GETUTCDATE()
            WHERE ThirdPartyID = @ThirdPartyID
              AND CompanyID = @CompanyID
        END
        ELSE
        BEGIN
            INSERT INTO [dbo].[ThirdPartyProfile]
            (
                CompanyID,
                ThirdPartyName,
                ServiceType,
                CriticalityTier,
                InherentRiskRating,
                CountryRegion,
                BusinessOwner,
                ContractValue,
                ContractExpiryDate,
                KeySLAKPIRequirements,
                ComplianceRequirements,
                PrivacyDataProcessing,
                RelatedAssetProcess,
                ContingencyPlan,
                LastAssessmentDate,
                NextReviewDate,
                Status,
                CreatedBy,
                CreatedAt,
                UpdatedAt
            )
            VALUES
            (
                @CompanyID,
                @ThirdPartyName,
                @ServiceType,
                @CriticalityTier,
                @InherentRiskRating,
                @CountryRegion,
                @BusinessOwner,
                @ContractValue,
                @ContractExpiryDate,
                @KeySLAKPIRequirements,
                @ComplianceRequirements,
                @PrivacyDataProcessing,
                @RelatedAssetProcess,
                @ContingencyPlan,
                @LastAssessmentDate,
                @NextReviewDate,
                @Status,
                @UserID,
                GETUTCDATE(),
                GETUTCDATE()
            )

            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'INSERT', 'ThirdPartyProfile', @ThirdPartyID,
                    (SELECT * FROM ThirdPartyProfile WHERE ThirdPartyID = (SELECT TOP 1 @@IDENTITY FROM ThirdPartyProfile) FOR XML AUTO), GETDATE())
        END
    END
    ELSE IF @Operation = 'rtvThirdPartyProfiles'
    BEGIN
        SELECT ThirdPartyID, CompanyID, ThirdPartyName, ServiceType, CriticalityTier, InherentRiskRating, CountryRegion, BusinessOwner,
               ContractValue, ContractExpiryDate, KeySLAKPIRequirements, ComplianceRequirements, PrivacyDataProcessing, RelatedAssetProcess,
               ContingencyPlan, LastAssessmentDate, NextReviewDate, Status, CreatedBy, UpdatedBy, CreatedAt, UpdatedAt
        FROM [ThirdPartyProfile]
        WHERE CompanyID = @CompanyID

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'ThirdPartyProfile', NULL,
                (SELECT * FROM ThirdPartyProfile FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'rtvThirdPartyProfile'
    BEGIN
        SELECT ThirdPartyID, CompanyID, ThirdPartyName, ServiceType, CriticalityTier, InherentRiskRating, CountryRegion, BusinessOwner,
               ContractValue, ContractExpiryDate, KeySLAKPIRequirements, ComplianceRequirements, PrivacyDataProcessing, RelatedAssetProcess,
               ContingencyPlan, LastAssessmentDate, NextReviewDate, Status, CreatedBy, UpdatedBy, CreatedAt, UpdatedAt
        FROM [ThirdPartyProfile]
        WHERE ThirdPartyID = @ThirdPartyID
          AND CompanyID = @CompanyID

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select One', 'ThirdPartyProfile', @ThirdPartyID,
                (SELECT * FROM ThirdPartyProfile WHERE ThirdPartyID = @ThirdPartyID FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'DeleteThirdPartyProfile'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'ThirdPartyProfile', @ThirdPartyID,
                (SELECT * FROM ThirdPartyProfile WHERE ThirdPartyID = @ThirdPartyID FOR XML AUTO), GETDATE())

        DELETE FROM [ThirdPartyProfile]
        WHERE ThirdPartyID = @ThirdPartyID
          AND CompanyID = @CompanyID
    END
END
GO
