USE [EBS]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[SustainabilityInitiativeSP]
    @Operation NVARCHAR(100) = NULL,
    @UserID BIGINT = NULL,
    @CompanyID INT = NULL,
    @InitiativeID BIGINT = NULL,
    @InitiativeName NVARCHAR(250) = NULL,
    @Description NVARCHAR(MAX) = NULL,
    @ESGCategory NVARCHAR(50) = NULL,
    @StartDate DATETIME = NULL,
    @EndDate DATETIME = NULL,
    @ResponsibleDepartment NVARCHAR(250) = NULL,
    @KeyMetrics NVARCHAR(MAX) = NULL,
    @BudgetAllocated NVARCHAR(100) = NULL,
    @Outcome NVARCHAR(MAX) = NULL,
    @Status NVARCHAR(100) = NULL,
    @CreatedBy BIGINT = NULL,
    @CreatedAt DATETIME = NULL,
    @UpdatedBy BIGINT = NULL,
    @UpdatedAt DATETIME = NULL
AS
BEGIN
    DECLARE @CurrentUserType AS INT = (SELECT UserType FROM [User] WHERE UserID = @UserID)
    IF @CurrentUserType <> 1
        THROW 51000, 'You''re not allowed to do the action', 1;

    IF @Operation = 'SaveSustainabilityInitiative'
    BEGIN
        IF EXISTS(SELECT 1 FROM SustainabilityInitiative WHERE InitiativeID = @InitiativeID AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'SustainabilityInitiative', @InitiativeID,
                    (SELECT * FROM SustainabilityInitiative WHERE InitiativeID = @InitiativeID FOR XML AUTO), GETDATE())

            UPDATE SustainabilityInitiative
            SET InitiativeName = @InitiativeName,
                Description = @Description,
                ESGCategory = @ESGCategory,
                StartDate = @StartDate,
                EndDate = @EndDate,
                ResponsibleDepartment = @ResponsibleDepartment,
                KeyMetrics = @KeyMetrics,
                BudgetAllocated = @BudgetAllocated,
                Outcome = @Outcome,
                Status = @Status,
                UpdatedBy = @UpdatedBy,
                UpdatedAt = ISNULL(@UpdatedAt, GETUTCDATE())
            WHERE InitiativeID = @InitiativeID AND CompanyID = @CompanyID
        END
        ELSE
        BEGIN
            INSERT INTO SustainabilityInitiative
            (CompanyID, InitiativeName, Description, ESGCategory, StartDate, EndDate, ResponsibleDepartment, KeyMetrics, BudgetAllocated, Outcome, Status, CreatedBy, CreatedAt, UpdatedAt)
            VALUES
            (@CompanyID, @InitiativeName, @Description, @ESGCategory, @StartDate, @EndDate, @ResponsibleDepartment, @KeyMetrics, @BudgetAllocated, @Outcome, @Status, @UserID, ISNULL(@CreatedAt, GETUTCDATE()), GETUTCDATE())

            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'Insert', 'SustainabilityInitiative', @InitiativeID,
                    (SELECT * FROM SustainabilityInitiative WHERE InitiativeID = (SELECT TOP 1 @@IDENTITY FROM SustainabilityInitiative) FOR XML AUTO), GETDATE())
        END
    END
    ELSE IF @Operation = 'rtvSustainabilityInitiatives'
    BEGIN
        SELECT InitiativeID, CompanyID, InitiativeName, Description, ESGCategory, StartDate, EndDate, ResponsibleDepartment, KeyMetrics, BudgetAllocated, Outcome, Status, CreatedBy, UpdatedBy, CreatedAt, UpdatedAt
        FROM SustainabilityInitiative
        WHERE CompanyID = @CompanyID

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'SustainabilityInitiative', NULL,
                (SELECT * FROM SustainabilityInitiative FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'rtvSustainabilityInitiative'
    BEGIN
        SELECT InitiativeID, CompanyID, InitiativeName, Description, ESGCategory, StartDate, EndDate, ResponsibleDepartment, KeyMetrics, BudgetAllocated, Outcome, Status, CreatedBy, UpdatedBy, CreatedAt, UpdatedAt
        FROM SustainabilityInitiative
        WHERE InitiativeID = @InitiativeID AND CompanyID = @CompanyID

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select One', 'SustainabilityInitiative', @InitiativeID,
                (SELECT * FROM SustainabilityInitiative WHERE InitiativeID = @InitiativeID FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'DeleteSustainabilityInitiative'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'SustainabilityInitiative', @InitiativeID,
                (SELECT * FROM SustainabilityInitiative WHERE InitiativeID = @InitiativeID FOR XML AUTO), GETDATE())

        DELETE FROM SustainabilityInitiative WHERE InitiativeID = @InitiativeID AND CompanyID = @CompanyID
    END
END
GO
