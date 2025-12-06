USE [EBS]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[RiskCategorySP]
    @Operation NVARCHAR(100) = NULL,
    @UserID BIGINT = NULL,
    @CompanyID INT = NULL,
    @CategoryID INT = NULL,
    @CategoryNameEN NVARCHAR(250) = NULL,
    @CategoryNameAR NVARCHAR(250) = NULL,
    @DescriptionEN NVARCHAR(MAX) = NULL,
    @DescriptionAR NVARCHAR(MAX) = NULL,
    @ParentCategoryID INT = NULL,
    @StatusID NVARCHAR(50) = NULL,
    @CreatedBy INT = NULL,
    @UpdatedBy INT = NULL
AS
BEGIN
    DECLARE @CurrentUserType AS INT = (SELECT UserType FROM [User] WHERE UserID = @UserID)
    DECLARE @CurrentCompanyID AS INT = (SELECT CompanyID FROM [User] WHERE UserID = @UserID)

    IF @CurrentUserType <> 1 OR @CurrentCompanyID <> @CompanyID
        THROW 51000, 'You''re not allowed to do the action', 1;

    IF @Operation = 'SaveRiskCategory'
    BEGIN
        IF EXISTS(SELECT 1 FROM RiskCategory WHERE CategoryID = @CategoryID AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'RiskCategory', @CategoryID,
            (SELECT * FROM RiskCategory WHERE CategoryID = @CategoryID FOR XML AUTO), GETDATE())

            UPDATE [dbo].[RiskCategory]
            SET CategoryNameEN = @CategoryNameEN,
                CategoryNameAR = @CategoryNameAR,
                DescriptionEN = @DescriptionEN,
                DescriptionAR = @DescriptionAR,
                ParentCategoryID = @ParentCategoryID,
                StatusID = @StatusID,
                UpdatedBy = @UserID,
                UpdatedAt = GETUTCDATE()
            WHERE CategoryID = @CategoryID
              AND CompanyID = @CompanyID
        END
        ELSE
        BEGIN
            INSERT INTO [dbo].[RiskCategory]
            (
                CompanyID,
                CategoryNameEN,
                CategoryNameAR,
                DescriptionEN,
                DescriptionAR,
                ParentCategoryID,
                StatusID,
                CreatedBy,
                CreatedAt,
                UpdatedAt
            )
            VALUES
            (
                @CompanyID,
                @CategoryNameEN,
                @CategoryNameAR,
                @DescriptionEN,
                @DescriptionAR,
                @ParentCategoryID,
                @StatusID,
                @UserID,
                GETUTCDATE(),
                GETUTCDATE()
            )

            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'INSERT', 'RiskCategory', @CategoryID,
            (SELECT * FROM RiskCategory WHERE CategoryID = (SELECT TOP 1 @@IDENTITY FROM RiskCategory) FOR XML AUTO), GETDATE())
        END
    END
    ELSE IF @Operation = 'rtvRiskCategories'
    BEGIN
        SELECT CategoryID, CompanyID, CategoryNameEN, CategoryNameAR, DescriptionEN, DescriptionAR, ParentCategoryID, StatusID, CreatedBy, UpdatedBy, CreatedAt, UpdatedAt
        FROM [RiskCategory]
        WHERE CompanyID = @CompanyID

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'RiskCategory', NULL,
        (SELECT * FROM RiskCategory FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'rtvRiskCategory'
    BEGIN
        SELECT CategoryID, CompanyID, CategoryNameEN, CategoryNameAR, DescriptionEN, DescriptionAR, ParentCategoryID, StatusID, CreatedBy, UpdatedBy, CreatedAt, UpdatedAt
        FROM [RiskCategory]
        WHERE CategoryID = @CategoryID
          AND CompanyID = @CompanyID

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select One', 'RiskCategory', @CategoryID,
        (SELECT * FROM RiskCategory WHERE CategoryID = @CategoryID FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'DeleteRiskCategory'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'RiskCategory', @CategoryID,
        (SELECT * FROM RiskCategory WHERE CategoryID = @CategoryID FOR XML AUTO), GETDATE())

        DELETE FROM [RiskCategory]
        WHERE CategoryID = @CategoryID
          AND CompanyID = @CompanyID
    END
END
GO
