USE [EBS]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[EnvironmentalAspectSP]
    @Operation NVARCHAR(100) = NULL,
    @UserID BIGINT = NULL,
    @CompanyID INT = NULL,
    @AspectID BIGINT = NULL,
    @UnitID BIGINT = NULL,
    @AspectDescription NVARCHAR(500) = NULL,
    @EnvironmentalImpact NVARCHAR(MAX) = NULL,
    @ImpactSeverity NVARCHAR(100) = NULL,
    @FrequencyLikelihood NVARCHAR(100) = NULL,
    @SignificanceRating NVARCHAR(100) = NULL,
    @ControlsInPlace NVARCHAR(MAX) = NULL,
    @LegalRequirement NVARCHAR(MAX) = NULL,
    @ImprovementActions NVARCHAR(MAX) = NULL,
    @AspectOwner NVARCHAR(250) = NULL,
    @MonitoringMetric NVARCHAR(250) = NULL,
    @LastEvaluationDate DATETIME = NULL,
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

    IF @Operation = 'SaveAspect'
    BEGIN
        IF EXISTS(SELECT 1 FROM EnvironmentalAspect WHERE AspectID = @AspectID AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'EnvironmentalAspect', @AspectID,
                    (SELECT * FROM EnvironmentalAspect WHERE AspectID = @AspectID FOR XML AUTO), GETDATE())

            UPDATE EnvironmentalAspect
            SET UnitID = @UnitID,
                AspectDescription = @AspectDescription,
                EnvironmentalImpact = @EnvironmentalImpact,
                ImpactSeverity = @ImpactSeverity,
                FrequencyLikelihood = @FrequencyLikelihood,
                SignificanceRating = @SignificanceRating,
                ControlsInPlace = @ControlsInPlace,
                LegalRequirement = @LegalRequirement,
                ImprovementActions = @ImprovementActions,
                AspectOwner = @AspectOwner,
                MonitoringMetric = @MonitoringMetric,
                LastEvaluationDate = @LastEvaluationDate,
                Status = @Status,
                UpdatedBy = @UpdatedBy,
                UpdatedAt = ISNULL(@UpdatedAt, GETUTCDATE())
            WHERE AspectID = @AspectID AND CompanyID = @CompanyID
        END
        ELSE
        BEGIN
            INSERT INTO EnvironmentalAspect
            (CompanyID, UnitID, AspectDescription, EnvironmentalImpact, ImpactSeverity, FrequencyLikelihood, SignificanceRating, ControlsInPlace, LegalRequirement, ImprovementActions, AspectOwner, MonitoringMetric, LastEvaluationDate, Status, CreatedBy, CreatedAt, UpdatedAt)
            VALUES
            (@CompanyID, @UnitID, @AspectDescription, @EnvironmentalImpact, @ImpactSeverity, @FrequencyLikelihood, @SignificanceRating, @ControlsInPlace, @LegalRequirement, @ImprovementActions, @AspectOwner, @MonitoringMetric, @LastEvaluationDate, @Status, @UserID, ISNULL(@CreatedAt, GETUTCDATE()), GETUTCDATE())

            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'Insert', 'EnvironmentalAspect', @AspectID,
                    (SELECT * FROM EnvironmentalAspect WHERE AspectID = (SELECT TOP 1 @@IDENTITY FROM EnvironmentalAspect) FOR XML AUTO), GETDATE())
        END
    END
    ELSE IF @Operation = 'rtvAspects'
    BEGIN
        SELECT AspectID, CompanyID, UnitID, AspectDescription, EnvironmentalImpact, ImpactSeverity, FrequencyLikelihood, SignificanceRating, ControlsInPlace, LegalRequirement, ImprovementActions, AspectOwner, MonitoringMetric, LastEvaluationDate, Status, CreatedBy, UpdatedBy, CreatedAt, UpdatedAt
        FROM EnvironmentalAspect
        WHERE CompanyID = @CompanyID

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'EnvironmentalAspect', NULL,
                (SELECT * FROM EnvironmentalAspect FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'rtvAspect'
    BEGIN
        SELECT AspectID, CompanyID, UnitID, AspectDescription, EnvironmentalImpact, ImpactSeverity, FrequencyLikelihood, SignificanceRating, ControlsInPlace, LegalRequirement, ImprovementActions, AspectOwner, MonitoringMetric, LastEvaluationDate, Status, CreatedBy, UpdatedBy, CreatedAt, UpdatedAt
        FROM EnvironmentalAspect
        WHERE AspectID = @AspectID AND CompanyID = @CompanyID

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select One', 'EnvironmentalAspect', @AspectID,
                (SELECT * FROM EnvironmentalAspect WHERE AspectID = @AspectID FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'DeleteAspect'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'EnvironmentalAspect', @AspectID,
                (SELECT * FROM EnvironmentalAspect WHERE AspectID = @AspectID FOR XML AUTO), GETDATE())

        DELETE FROM EnvironmentalAspect WHERE AspectID = @AspectID AND CompanyID = @CompanyID
    END
END
GO
