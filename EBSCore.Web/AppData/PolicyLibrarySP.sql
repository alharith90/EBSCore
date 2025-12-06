USE [EBS]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[PolicyLibrarySP]
    @Operation NVARCHAR(100) = NULL,
    @UserID BIGINT = NULL,
    @CompanyID INT = NULL,
    @PolicyID BIGINT = NULL,
    @PolicyCode NVARCHAR(50) = NULL,
    @PolicyNameEN NVARCHAR(250) = NULL,
    @PolicyNameAR NVARCHAR(250) = NULL,
    @PolicyType NVARCHAR(100) = NULL,
    @CategoryEN NVARCHAR(150) = NULL,
    @CategoryAR NVARCHAR(150) = NULL,
    @DescriptionEN NVARCHAR(MAX) = NULL,
    @DescriptionAR NVARCHAR(MAX) = NULL,
    @EffectiveDate DATETIME = NULL,
    @ReviewDate DATETIME = NULL,
    @OwnerUserID BIGINT = NULL,
    @ApproverUserID BIGINT = NULL,
    @StatusID NVARCHAR(50) = NULL,
    @RelatedRegulationIDs NVARCHAR(MAX) = NULL,
    @RelatedControlIDs NVARCHAR(MAX) = NULL,
    @VersionNumber NVARCHAR(50) = NULL,
    @DocumentPath NVARCHAR(500) = NULL,
    @IsMandatory BIT = NULL,
    @AppliesToRoles NVARCHAR(MAX) = NULL,
    @CreatedAt DATETIME = NULL,
    @UpdatedBy BIGINT = NULL,
    @UpdatedAt DATETIME = NULL,
    @CreatedBy INT = NULL,
    @ModifiedBy INT = NULL
AS
BEGIN
    DECLARE @CurrentUserType AS INT = (SELECT UserType FROM [User] WHERE UserID = @UserID)
    IF @CurrentUserType <> 1
        THROW 51000, 'You''re not allowed to do the action', 1;

    IF @Operation = 'SavePolicy'
    BEGIN
        IF EXISTS(SELECT 1 FROM PolicyLibrary WHERE PolicyID = @PolicyID AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'PolicyLibrary', @PolicyID,
                    (SELECT * FROM PolicyLibrary WHERE PolicyID = @PolicyID FOR XML AUTO), GETDATE())

            UPDATE [PolicyLibrary]
            SET PolicyCode = @PolicyCode,
                PolicyNameEN = @PolicyNameEN,
                PolicyNameAR = @PolicyNameAR,
                PolicyType = @PolicyType,
                CategoryEN = @CategoryEN,
                CategoryAR = @CategoryAR,
                DescriptionEN = @DescriptionEN,
                DescriptionAR = @DescriptionAR,
                EffectiveDate = @EffectiveDate,
                ReviewDate = @ReviewDate,
                OwnerUserID = @OwnerUserID,
                ApproverUserID = @ApproverUserID,
                StatusID = @StatusID,
                RelatedRegulationIDs = @RelatedRegulationIDs,
                RelatedControlIDs = @RelatedControlIDs,
                VersionNumber = @VersionNumber,
                DocumentPath = @DocumentPath,
                IsMandatory = @IsMandatory,
                AppliesToRoles = @AppliesToRoles,
                UpdatedBy = @UpdatedBy,
                UpdatedAt = ISNULL(@UpdatedAt, GETUTCDATE()),
                ModifiedBy = @UserID
            WHERE PolicyID = @PolicyID AND CompanyID = @CompanyID
        END
        ELSE
        BEGIN
            INSERT INTO [PolicyLibrary]
            (
                CompanyID, PolicyCode, PolicyNameEN, PolicyNameAR, PolicyType, CategoryEN, CategoryAR, DescriptionEN, DescriptionAR, EffectiveDate, ReviewDate, OwnerUserID, ApproverUserID, StatusID, RelatedRegulationIDs, RelatedControlIDs, VersionNumber, DocumentPath, IsMandatory, AppliesToRoles, CreatedBy, CreatedAt, UpdatedAt
            )
            VALUES
            (
                @CompanyID, @PolicyCode, @PolicyNameEN, @PolicyNameAR, @PolicyType, @CategoryEN, @CategoryAR, @DescriptionEN, @DescriptionAR, @EffectiveDate, @ReviewDate, @OwnerUserID, @ApproverUserID, @StatusID, @RelatedRegulationIDs, @RelatedControlIDs, @VersionNumber, @DocumentPath, @IsMandatory, @AppliesToRoles, @UserID, ISNULL(@CreatedAt, GETUTCDATE()), GETUTCDATE()
            )

            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'Insert', 'PolicyLibrary', @PolicyID,
                    (SELECT * FROM PolicyLibrary WHERE PolicyID = (SELECT TOP 1 @@IDENTITY FROM PolicyLibrary) FOR XML AUTO), GETDATE())
        END
    END
    ELSE IF @Operation = 'rtvPolicies'
    BEGIN
        SELECT PolicyID, CompanyID, PolicyCode, PolicyNameEN, PolicyNameAR, PolicyType, CategoryEN, CategoryAR, DescriptionEN, DescriptionAR, EffectiveDate, ReviewDate, OwnerUserID, ApproverUserID, StatusID, RelatedRegulationIDs, RelatedControlIDs, VersionNumber, DocumentPath, IsMandatory, AppliesToRoles, CreatedBy, UpdatedBy, CreatedAt, UpdatedAt
        FROM PolicyLibrary
        WHERE CompanyID = @CompanyID

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'PolicyLibrary', NULL,
                (SELECT * FROM PolicyLibrary FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'rtvPolicy'
    BEGIN
        SELECT PolicyID, CompanyID, PolicyCode, PolicyNameEN, PolicyNameAR, PolicyType, CategoryEN, CategoryAR, DescriptionEN, DescriptionAR, EffectiveDate, ReviewDate, OwnerUserID, ApproverUserID, StatusID, RelatedRegulationIDs, RelatedControlIDs, VersionNumber, DocumentPath, IsMandatory, AppliesToRoles, CreatedBy, UpdatedBy, CreatedAt, UpdatedAt
        FROM PolicyLibrary
        WHERE PolicyID = @PolicyID AND CompanyID = @CompanyID

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select One', 'PolicyLibrary', @PolicyID,
                (SELECT * FROM PolicyLibrary WHERE PolicyID = @PolicyID FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'DeletePolicy'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'PolicyLibrary', @PolicyID,
                (SELECT * FROM PolicyLibrary WHERE PolicyID = @PolicyID FOR XML AUTO), GETDATE())

        DELETE FROM PolicyLibrary WHERE PolicyID = @PolicyID AND CompanyID = @CompanyID
    END
END
GO
