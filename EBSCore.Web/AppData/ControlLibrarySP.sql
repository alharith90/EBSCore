USE [EBS]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[ControlLibrarySP]
    @Operation NVARCHAR(100) = NULL,
    @UserID BIGINT = NULL,
    @CompanyID INT = NULL,
    @ControlID BIGINT = NULL,
    @ControlName NVARCHAR(250) = NULL,
    @Description NVARCHAR(MAX) = NULL,
    @ControlType NVARCHAR(100) = NULL,
    @ControlCategory NVARCHAR(100) = NULL,
    @ControlOwner NVARCHAR(250) = NULL,
    @Frequency NVARCHAR(100) = NULL,
    @IsKeyControl BIT = NULL,
    @RelatedRisks NVARCHAR(MAX) = NULL,
    @RelatedObligations NVARCHAR(MAX) = NULL,
    @ImplementationStatus NVARCHAR(100) = NULL,
    @LastTestDate DATETIME = NULL,
    @LastTestResult NVARCHAR(250) = NULL,
    @DocumentationReference NVARCHAR(500) = NULL,
    @CreatedBy INT = NULL,
    @ModifiedBy INT = NULL
AS
BEGIN
    DECLARE @CurrentUserType AS INT = (SELECT UserType FROM [User] WHERE UserID = @UserID)
    IF @CurrentUserType <> 1
        THROW 51000, 'You''re not allowed to do the action', 1;

    IF @Operation = 'SaveControl'
    BEGIN
        IF EXISTS(SELECT 1 FROM ControlLibrary WHERE ControlID = @ControlID AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'ControlLibrary', @ControlID,
                    (SELECT * FROM ControlLibrary WHERE ControlID = @ControlID FOR XML AUTO), GETDATE())

            UPDATE [ControlLibrary]
            SET ControlName = @ControlName,
                Description = @Description,
                ControlType = @ControlType,
                ControlCategory = @ControlCategory,
                ControlOwner = @ControlOwner,
                Frequency = @Frequency,
                IsKeyControl = @IsKeyControl,
                RelatedRisks = @RelatedRisks,
                RelatedObligations = @RelatedObligations,
                ImplementationStatus = @ImplementationStatus,
                LastTestDate = @LastTestDate,
                LastTestResult = @LastTestResult,
                DocumentationReference = @DocumentationReference,
                ModifiedBy = @UserID,
                UpdatedAt = GETUTCDATE()
            WHERE ControlID = @ControlID AND CompanyID = @CompanyID
        END
        ELSE
        BEGIN
            INSERT INTO [ControlLibrary]
            (
                CompanyID, ControlName, Description, ControlType, ControlCategory, ControlOwner, Frequency, IsKeyControl, RelatedRisks, RelatedObligations, ImplementationStatus, LastTestDate, LastTestResult, DocumentationReference, CreatedBy, CreatedAt, UpdatedAt
            )
            VALUES
            (
                @CompanyID, @ControlName, @Description, @ControlType, @ControlCategory, @ControlOwner, @Frequency, @IsKeyControl, @RelatedRisks, @RelatedObligations, @ImplementationStatus, @LastTestDate, @LastTestResult, @DocumentationReference, @UserID, GETUTCDATE(), GETUTCDATE()
            )

            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'Insert', 'ControlLibrary', @ControlID,
                    (SELECT * FROM ControlLibrary WHERE ControlID = (SELECT TOP 1 @@IDENTITY FROM ControlLibrary) FOR XML AUTO), GETDATE())
        END
    END
    ELSE IF @Operation = 'rtvControls'
    BEGIN
        SELECT ControlID, CompanyID, ControlName, Description, ControlType, ControlCategory, ControlOwner, Frequency, IsKeyControl, RelatedRisks, RelatedObligations, ImplementationStatus, LastTestDate, LastTestResult, DocumentationReference, CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM ControlLibrary
        WHERE CompanyID = @CompanyID

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'ControlLibrary', NULL,
                (SELECT * FROM ControlLibrary FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'rtvControl'
    BEGIN
        SELECT ControlID, CompanyID, ControlName, Description, ControlType, ControlCategory, ControlOwner, Frequency, IsKeyControl, RelatedRisks, RelatedObligations, ImplementationStatus, LastTestDate, LastTestResult, DocumentationReference, CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM ControlLibrary
        WHERE ControlID = @ControlID AND CompanyID = @CompanyID

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select One', 'ControlLibrary', @ControlID,
                (SELECT * FROM ControlLibrary WHERE ControlID = @ControlID FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'DeleteControl'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'ControlLibrary', @ControlID,
                (SELECT * FROM ControlLibrary WHERE ControlID = @ControlID FOR XML AUTO), GETDATE())

        DELETE FROM ControlLibrary WHERE ControlID = @ControlID AND CompanyID = @CompanyID
    END
END
