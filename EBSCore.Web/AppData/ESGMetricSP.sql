USE [EBS]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[ESGMetricSP]
    @Operation NVARCHAR(100) = NULL,
    @UserID BIGINT = NULL,
    @CompanyID INT = NULL,
    @MetricID BIGINT = NULL,
    @MetricName NVARCHAR(250) = NULL,
    @Category NVARCHAR(50) = NULL,
    @Description NVARCHAR(MAX) = NULL,
    @UnitOfMeasure NVARCHAR(50) = NULL,
    @DataSource NVARCHAR(250) = NULL,
    @ReportingFrequency NVARCHAR(100) = NULL,
    @TargetValue NVARCHAR(100) = NULL,
    @LatestValue NVARCHAR(100) = NULL,
    @MeasurementDate DATETIME = NULL,
    @Owner NVARCHAR(250) = NULL,
    @RelatedObjective NVARCHAR(250) = NULL,
    @RelatedRisk NVARCHAR(250) = NULL,
    @Trend NVARCHAR(100) = NULL,
    @Comments NVARCHAR(MAX) = NULL,
    @CreatedBy BIGINT = NULL,
    @CreatedAt DATETIME = NULL,
    @UpdatedBy BIGINT = NULL,
    @UpdatedAt DATETIME = NULL
AS
BEGIN
    DECLARE @CurrentUserType AS INT = (SELECT UserType FROM [User] WHERE UserID = @UserID)
    IF @CurrentUserType <> 1
        THROW 51000, 'You''re not allowed to do the action', 1;

    IF @Operation = 'SaveESGMetric'
    BEGIN
        IF EXISTS(SELECT 1 FROM ESGMetric WHERE MetricID = @MetricID AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'ESGMetric', @MetricID,
                    (SELECT * FROM ESGMetric WHERE MetricID = @MetricID FOR XML AUTO), GETDATE())

            UPDATE ESGMetric
            SET MetricName = @MetricName,
                Category = @Category,
                Description = @Description,
                UnitOfMeasure = @UnitOfMeasure,
                DataSource = @DataSource,
                ReportingFrequency = @ReportingFrequency,
                TargetValue = @TargetValue,
                LatestValue = @LatestValue,
                MeasurementDate = @MeasurementDate,
                Owner = @Owner,
                RelatedObjective = @RelatedObjective,
                RelatedRisk = @RelatedRisk,
                Trend = @Trend,
                Comments = @Comments,
                UpdatedBy = @UpdatedBy,
                UpdatedAt = ISNULL(@UpdatedAt, GETUTCDATE())
            WHERE MetricID = @MetricID AND CompanyID = @CompanyID
        END
        ELSE
        BEGIN
            INSERT INTO ESGMetric
            (CompanyID, MetricName, Category, Description, UnitOfMeasure, DataSource, ReportingFrequency, TargetValue, LatestValue, MeasurementDate, Owner, RelatedObjective, RelatedRisk, Trend, Comments, CreatedBy, CreatedAt, UpdatedAt)
            VALUES
            (@CompanyID, @MetricName, @Category, @Description, @UnitOfMeasure, @DataSource, @ReportingFrequency, @TargetValue, @LatestValue, @MeasurementDate, @Owner, @RelatedObjective, @RelatedRisk, @Trend, @Comments, @UserID, ISNULL(@CreatedAt, GETUTCDATE()), GETUTCDATE())

            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'Insert', 'ESGMetric', @MetricID,
                    (SELECT * FROM ESGMetric WHERE MetricID = (SELECT TOP 1 @@IDENTITY FROM ESGMetric) FOR XML AUTO), GETDATE())
        END
    END
    ELSE IF @Operation = 'rtvESGMetrics'
    BEGIN
        SELECT MetricID, CompanyID, MetricName, Category, Description, UnitOfMeasure, DataSource, ReportingFrequency, TargetValue, LatestValue, MeasurementDate, Owner, RelatedObjective, RelatedRisk, Trend, Comments, CreatedBy, UpdatedBy, CreatedAt, UpdatedAt
        FROM ESGMetric
        WHERE CompanyID = @CompanyID

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'ESGMetric', NULL,
                (SELECT * FROM ESGMetric FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'rtvESGMetric'
    BEGIN
        SELECT MetricID, CompanyID, MetricName, Category, Description, UnitOfMeasure, DataSource, ReportingFrequency, TargetValue, LatestValue, MeasurementDate, Owner, RelatedObjective, RelatedRisk, Trend, Comments, CreatedBy, UpdatedBy, CreatedAt, UpdatedAt
        FROM ESGMetric
        WHERE MetricID = @MetricID AND CompanyID = @CompanyID

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select One', 'ESGMetric', @MetricID,
                (SELECT * FROM ESGMetric WHERE MetricID = @MetricID FOR XML AUTO), GETDATE())
    END
    ELSE IF @Operation = 'DeleteESGMetric'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'ESGMetric', @MetricID,
                (SELECT * FROM ESGMetric WHERE MetricID = @MetricID FOR XML AUTO), GETDATE())

        DELETE FROM ESGMetric WHERE MetricID = @MetricID AND CompanyID = @CompanyID
    END
END
GO
