USE [EBS]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[EnvironmentalObjectiveSP]
    @Operation NVARCHAR(100) = NULL,
    @UserID BIGINT = NULL,
    @CompanyID INT = NULL,
    @ObjectiveID BIGINT = NULL,
    @UnitID BIGINT = NULL,
    @ObjectiveDescription NVARCHAR(500) = NULL,
    @TargetValue NVARCHAR(100) = NULL,
    @Unit NVARCHAR(50) = NULL,
    @BaselineValue NVARCHAR(100) = NULL,
    @CurrentValue NVARCHAR(100) = NULL,
    @TargetDate DATETIME = NULL,
    @ResponsibleOwner NVARCHAR(250) = NULL,
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

    IF @Operation = 'SaveEnvironmentalObjective'
    BEGIN
        IF EXISTS(SELECT 1 FROM EnvironmentalObjective WHERE ObjectiveID = @ObjectiveID AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'EnvironmentalObjective', @ObjectiveID,
                    (SELECT * FROM EnvironmentalObjective WHERE ObjectiveID = @ObjectiveID FOR XML AUTO), GETDATE())

            UPDATE EnvironmentalObjective
            SET UnitID = @UnitID,
                ObjectiveDescription = @ObjectiveDescription,
                TargetValue = @TargetValue,
                Unit = @Unit,
                BaselineValue = @BaselineValue,
                CurrentValue = @CurrentValue,
                TargetDate = @TargetDate,
                ResponsibleOwner = @ResponsibleOwner,
                Status = @Status,
                UpdatedBy = @UpdatedBy,
                UpdatedAt = ISNULL(@UpdatedAt, GETUTCDATE())
            WHERE ObjectiveID = @ObjectiveID AND CompanyID = @CompanyID
        END
        ELSE
        BEGIN
            INSERT INTO EnvironmentalObjective
            (CompanyID, UnitID, ObjectiveDescription, TargetValue, Unit, BaselineValue, CurrentValue, TargetDate, ResponsibleOwner, Status, CreatedBy, CreatedAt, UpdatedAt)
            VALUES
            (@CompanyID, @UnitID, @ObjectiveDescription, @TargetValue, @Unit, @BaselineValue, @CurrentValue, @TargetDate, @ResponsibleOwner, @Status, @UserID, ISNULL(@CreatedAt, GETUTCDATE()), GETUTCDATE())

            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'Insert', 'EnvironmentalObjective', @ObjectiveID,
                    (SELECT * FROM EnvironmentalObjective WHERE ObjectiveID = (SELECT TOP 1 @@IDENTITY FROM EnvironmentalObjective) FOR XML AUTO), GETDATE())
        END
    END
    ELSE IF @Operation = 'rtvEnvironmentalObjectives'
    BEGIN
        SELECT ObjectiveID, CompanyID, UnitID, ObjectiveDescription, TargetValue, Unit, BaselineValue, CurrentValue, TargetDate, ResponsibleOwner, Status, CreatedBy, UpdatedBy, CreatedAt, UpdatedAt
        FROM EnvironmentalObjective
        WHERE CompanyID = @CompanyID

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'EnvironmentalObjective', NULL,
                (SELECT * FROM EnvironmentalObjective FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'rtvEnvironmentalObjective'
    BEGIN
        SELECT ObjectiveID, CompanyID, UnitID, ObjectiveDescription, TargetValue, Unit, BaselineValue, CurrentValue, TargetDate, ResponsibleOwner, Status, CreatedBy, UpdatedBy, CreatedAt, UpdatedAt
        FROM EnvironmentalObjective
        WHERE ObjectiveID = @ObjectiveID AND CompanyID = @CompanyID

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select One', 'EnvironmentalObjective', @ObjectiveID,
                (SELECT * FROM EnvironmentalObjective WHERE ObjectiveID = @ObjectiveID FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'DeleteEnvironmentalObjective'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'EnvironmentalObjective', @ObjectiveID,
                (SELECT * FROM EnvironmentalObjective WHERE ObjectiveID = @ObjectiveID FOR XML AUTO), GETDATE())

        DELETE FROM EnvironmentalObjective WHERE ObjectiveID = @ObjectiveID AND CompanyID = @CompanyID
    END
END
GO
