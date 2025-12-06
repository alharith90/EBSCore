USE [EBS]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[InformationSystemSP]
    @Operation NVARCHAR(100),
    @UserID INT = NULL,
    @SystemID INT = NULL,
    @CompanyID INT = NULL,
    @UnitID BIGINT = NULL,
    @SystemName NVARCHAR(255) = NULL,
    @RPO NVARCHAR(255) = NULL,
    @ApplicationLifecycleStatus NVARCHAR(255) = NULL,
    @Type NVARCHAR(255) = NULL,
    @RequiredFor NVARCHAR(MAX) = NULL,
    @SystemDescription NVARCHAR(MAX) = NULL,
    @PrimaryOwnerId INT = NULL,
    @SecondaryOwner NVARCHAR(255) = NULL,
    @BusinessOwner NVARCHAR(255) = NULL,
    @InternetFacing BIT = NULL,
    @ThirdPartyAccess BIT = NULL,
    @NumberOfUsers INT = NULL,
    @LicenseType NVARCHAR(255) = NULL,
    @Infrastructure BIT = NULL,
    @MFAEnabled BIT = NULL,
    @MFAStatusDetails NVARCHAR(MAX) = NULL,
    @AssociatedInformationSystems NVARCHAR(MAX) = NULL,
    @Confidentiality NVARCHAR(255) = NULL,
    @Integrity NVARCHAR(255) = NULL,
    @Availability NVARCHAR(255) = NULL,
    @OverallCategorizationRating NVARCHAR(255) = NULL,
    @HighestInformationClassification NVARCHAR(255) = NULL,
    @RiskHighlightedByIT NVARCHAR(MAX) = NULL,
    @AdditionalNote NVARCHAR(MAX) = NULL,
    @Logo VARBINARY(MAX) = NULL,
    @PageSize INT = 10,
    @PageNumber INT = 1,
    @SortColumn NVARCHAR(50) = NULL,
    @SortDirection NVARCHAR(10) = 'ASC',
    @CreatedBy INT = NULL,
    @UpdatedBy INT = NULL
AS
BEGIN
    IF @Operation = 'SaveInformationSystem'
    BEGIN
        IF EXISTS(SELECT 1 FROM InformationSystems WHERE SystemID = @SystemID AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'InformationSystems', @SystemID,
                    (SELECT * FROM InformationSystems WHERE SystemID = @SystemID FOR XML AUTO), GETDATE());

            UPDATE [dbo].[InformationSystems]
            SET [SystemName] = @SystemName,
                [RPO] = @RPO,
                [ApplicationLifecycleStatus] = @ApplicationLifecycleStatus,
                [Type] = @Type,
                [RequiredFor] = @RequiredFor,
                [SystemDescription] = @SystemDescription,
                [PrimaryOwnerId] = @PrimaryOwnerId,
                [SecondaryOwner] = @SecondaryOwner,
                [BusinessOwner] = @BusinessOwner,
                [InternetFacing] = @InternetFacing,
                [ThirdPartyAccess] = @ThirdPartyAccess,
                [NumberOfUsers] = @NumberOfUsers,
                [LicenseType] = @LicenseType,
                [Infrastructure] = @Infrastructure,
                [MFAEnabled] = @MFAEnabled,
                [MFAStatusDetails] = @MFAStatusDetails,
                [AssociatedInformationSystems] = @AssociatedInformationSystems,
                [Confidentiality] = @Confidentiality,
                [Integrity] = @Integrity,
                [Availability] = @Availability,
                [OverallCategorizationRating] = @OverallCategorizationRating,
                [HighestInformationClassification] = @HighestInformationClassification,
                [RiskHighlightedByIT] = @RiskHighlightedByIT,
                [AdditionalNote] = @AdditionalNote,
                [Logo] = @Logo,
                [UpdatedBy] = @UpdatedBy,
                [UpdatedAt] = GETDATE()
            WHERE SystemID = @SystemID
              AND CompanyID = @CompanyID;
        END
        ELSE
        BEGIN
            INSERT INTO [dbo].[InformationSystems]
            (
                [CompanyID], [UnitID], [SystemName], [RPO], [ApplicationLifecycleStatus], [Type], [RequiredFor],
                [SystemDescription], [PrimaryOwnerId], [SecondaryOwner], [BusinessOwner], [InternetFacing],
                [ThirdPartyAccess], [NumberOfUsers], [LicenseType], [Infrastructure], [MFAEnabled],
                [MFAStatusDetails], [AssociatedInformationSystems], [Confidentiality], [Integrity],
                [Availability], [OverallCategorizationRating], [HighestInformationClassification], [RiskHighlightedByIT],
                [AdditionalNote], [Logo], [CreatedBy], [CreatedAt], [UpdatedAt]
            )
            VALUES
            (
                @CompanyID, @UnitID, @SystemName, @RPO, @ApplicationLifecycleStatus, @Type, @RequiredFor,
                @SystemDescription, @PrimaryOwnerId, @SecondaryOwner, @BusinessOwner, @InternetFacing,
                @ThirdPartyAccess, @NumberOfUsers, @LicenseType, @Infrastructure, @MFAEnabled,
                @MFAStatusDetails, @AssociatedInformationSystems, @Confidentiality, @Integrity,
                @Availability, @OverallCategorizationRating, @HighestInformationClassification, @RiskHighlightedByIT,
                @AdditionalNote, @Logo, @CreatedBy, GETDATE(), GETDATE()
            );

            DECLARE @NewSystemID INT = SCOPE_IDENTITY();

            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'INSERT', 'InformationSystems', @NewSystemID,
                    (SELECT * FROM InformationSystems WHERE SystemID = @NewSystemID FOR XML AUTO), GETDATE());
        END
    END
    ELSE IF @Operation = 'rtvInformationSystems'
    BEGIN
        SELECT SystemID,
               SystemName,
               RPO,
               Type,
               BusinessOwner,
               NumberOfUsers
        FROM [InformationSystems]
        WHERE CompanyID = @CompanyID
        ORDER BY
            CASE WHEN @SortColumn = 'SystemName' AND @SortDirection = 'ASC' THEN SystemName END ASC,
            CASE WHEN @SortColumn = 'SystemName' AND @SortDirection = 'DESC' THEN SystemName END DESC,
            CASE WHEN @SortColumn = 'CreatedAt' AND @SortDirection = 'ASC' THEN CreatedAt END ASC,
            CASE WHEN @SortColumn = 'CreatedAt' AND @SortDirection = 'DESC' THEN CreatedAt END DESC,
            CASE WHEN @SortColumn = 'SystemID' AND @SortDirection = 'ASC' THEN SystemID END ASC,
            CASE WHEN @SortColumn = 'SystemID' AND @SortDirection = 'DESC' THEN SystemID END DESC,
            SystemID ASC
        OFFSET (@PageSize * (@PageNumber - 1)) ROWS
        FETCH NEXT @PageSize ROWS ONLY;

        SELECT CEILING(COUNT(0) * 1.0 / (@PageSize))  AS PageCount,
               @PageNumber AS CurrentPage
        FROM [InformationSystems]
        WHERE CompanyID = @CompanyID;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'SELECT ALL', 'InformationSystems', NULL,
                (SELECT * FROM InformationSystems WHERE CompanyID = @CompanyID FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'DeleteInformationSystem'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'InformationSystems', @SystemID,
                (SELECT * FROM InformationSystems WHERE SystemID = @SystemID FOR XML AUTO), GETDATE());

        DELETE FROM [InformationSystems]
        WHERE SystemID = @SystemID
          AND CompanyID = @CompanyID;
    END
END
GO
