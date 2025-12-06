USE [EBS]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[StrategicObjectiveSP]
    @Operation NVARCHAR(100) = NULL,
    @UserID BIGINT = NULL,
    @CompanyID INT = NULL,
    @ObjectiveID BIGINT = NULL,
    @ObjectiveCode NVARCHAR(50) = NULL,
    @ObjectiveNameEN NVARCHAR(250) = NULL,
    @ObjectiveNameAR NVARCHAR(250) = NULL,
    @DescriptionEN NVARCHAR(MAX) = NULL,
    @DescriptionAR NVARCHAR(MAX) = NULL,
    @Category NVARCHAR(150) = NULL,
    @Perspective NVARCHAR(150) = NULL,
    @OwnerUserID BIGINT = NULL,
    @DepartmentID BIGINT = NULL,
    @StartDate DATETIME = NULL,
    @EndDate DATETIME = NULL,
    @TargetValue DECIMAL(18,2) = NULL,
    @UnitEN NVARCHAR(100) = NULL,
    @UnitAR NVARCHAR(100) = NULL,
    @RiskAppetiteThreshold NVARCHAR(100) = NULL,
    @StatusID NVARCHAR(50) = NULL,
    @CreatedBy INT = NULL,
    @ModifiedBy INT = NULL
AS
BEGIN
    DECLARE @CurrentUserType AS INT = (SELECT UserType FROM [User] WHERE UserID = @UserID)
    IF @CurrentUserType <> 1
        THROW 51000, 'You''re not allowed to do the action', 1;

    IF @Operation = 'SaveObjective'
    BEGIN
        IF EXISTS(SELECT 1 FROM StrategicObjective WHERE ObjectiveID = @ObjectiveID AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'StrategicObjective', @ObjectiveID,
                    (SELECT * FROM StrategicObjective WHERE ObjectiveID = @ObjectiveID FOR XML AUTO), GETDATE())

            UPDATE [StrategicObjective]
            SET ObjectiveCode = @ObjectiveCode,
                ObjectiveNameEN = @ObjectiveNameEN,
                ObjectiveNameAR = @ObjectiveNameAR,
                DescriptionEN = @DescriptionEN,
                DescriptionAR = @DescriptionAR,
                Category = @Category,
                Perspective = @Perspective,
                OwnerUserID = @OwnerUserID,
                DepartmentID = @DepartmentID,
                StartDate = @StartDate,
                EndDate = @EndDate,
                TargetValue = @TargetValue,
                UnitEN = @UnitEN,
                UnitAR = @UnitAR,
                RiskAppetiteThreshold = @RiskAppetiteThreshold,
                StatusID = @StatusID,
                ModifiedBy = @UserID,
                UpdatedAt = GETUTCDATE()
            WHERE ObjectiveID = @ObjectiveID AND CompanyID = @CompanyID
        END
        ELSE
        BEGIN
            INSERT INTO [StrategicObjective]
            (
                CompanyID, ObjectiveCode, ObjectiveNameEN, ObjectiveNameAR, DescriptionEN, DescriptionAR, Category, Perspective, OwnerUserID, DepartmentID, StartDate, EndDate, TargetValue, UnitEN, UnitAR, RiskAppetiteThreshold, StatusID, CreatedBy, CreatedAt, UpdatedAt
            )
            VALUES
            (
                @CompanyID, @ObjectiveCode, @ObjectiveNameEN, @ObjectiveNameAR, @DescriptionEN, @DescriptionAR, @Category, @Perspective, @OwnerUserID, @DepartmentID, @StartDate, @EndDate, @TargetValue, @UnitEN, @UnitAR, @RiskAppetiteThreshold, @StatusID, @UserID, GETUTCDATE(), GETUTCDATE()
            )

            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'Insert', 'StrategicObjective', @ObjectiveID,
                    (SELECT * FROM StrategicObjective WHERE ObjectiveID = (SELECT TOP 1 @@IDENTITY FROM StrategicObjective) FOR XML AUTO), GETDATE())
        END
    END
    ELSE IF @Operation = 'rtvObjectives'
    BEGIN
        SELECT ObjectiveID, CompanyID, ObjectiveCode, ObjectiveNameEN, ObjectiveNameAR, DescriptionEN, DescriptionAR, Category, Perspective, OwnerUserID, DepartmentID, StartDate, EndDate, TargetValue, UnitEN, UnitAR, RiskAppetiteThreshold, StatusID, CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM StrategicObjective
        WHERE CompanyID = @CompanyID

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'StrategicObjective', NULL,
                (SELECT * FROM StrategicObjective FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'rtvObjective'
    BEGIN
        SELECT ObjectiveID, CompanyID, ObjectiveCode, ObjectiveNameEN, ObjectiveNameAR, DescriptionEN, DescriptionAR, Category, Perspective, OwnerUserID, DepartmentID, StartDate, EndDate, TargetValue, UnitEN, UnitAR, RiskAppetiteThreshold, StatusID, CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM StrategicObjective
        WHERE ObjectiveID = @ObjectiveID AND CompanyID = @CompanyID

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select One', 'StrategicObjective', @ObjectiveID,
                (SELECT * FROM StrategicObjective WHERE ObjectiveID = @ObjectiveID FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'DeleteObjective'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'StrategicObjective', @ObjectiveID,
                (SELECT * FROM StrategicObjective WHERE ObjectiveID = @ObjectiveID FOR XML AUTO), GETDATE())

        DELETE FROM StrategicObjective WHERE ObjectiveID = @ObjectiveID AND CompanyID = @CompanyID
    END
END
GO
