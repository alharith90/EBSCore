/****** KRI register aligned to ISO-style monitoring and ProcessSP pattern ******/
IF OBJECT_ID('dbo.KeyRiskIndicator', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[KeyRiskIndicator]
    (
        [IndicatorID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [CompanyID] INT NOT NULL,
        [IndicatorName] NVARCHAR(250) NOT NULL,
        [RelatedRisk] NVARCHAR(250) NULL,
        [MeasurementFrequency] NVARCHAR(100) NULL,
        [DataSource] NVARCHAR(200) NULL,
        [ThresholdValue] NVARCHAR(100) NULL,
        [CurrentValue] NVARCHAR(100) NULL,
        [Status] NVARCHAR(50) NULL,
        [Owner] NVARCHAR(200) NULL,
        [LastUpdateDate] DATETIME NULL,
        [CreatedBy] BIGINT NULL,
        [ModifiedBy] BIGINT NULL,
        [CreatedAt] DATETIME2 NOT NULL CONSTRAINT DF_KRI_CreatedAt DEFAULT (GETUTCDATE()),
        [UpdatedAt] DATETIME2 NOT NULL CONSTRAINT DF_KRI_UpdatedAt DEFAULT (GETUTCDATE())
    );
END
GO

CREATE OR ALTER PROCEDURE [dbo].[KeyRiskIndicatorSP]
    @Operation NVARCHAR(100) = NULL,
    @UserID BIGINT = NULL,
    @CompanyID INT = NULL,
    @IndicatorID INT = NULL,
    @IndicatorName NVARCHAR(250) = NULL,
    @RelatedRisk NVARCHAR(250) = NULL,
    @MeasurementFrequency NVARCHAR(100) = NULL,
    @DataSource NVARCHAR(200) = NULL,
    @ThresholdValue NVARCHAR(100) = NULL,
    @CurrentValue NVARCHAR(100) = NULL,
    @Status NVARCHAR(50) = NULL,
    @Owner NVARCHAR(200) = NULL,
    @LastUpdateDate DATETIME = NULL,
    @CreatedBy INT = NULL,
    @ModifiedBy INT = NULL
AS
BEGIN
    DECLARE @CurrentUserType AS INT = (SELECT UserType FROM [User] WHERE UserID = @UserID);
    IF @CurrentUserType <> 1
        THROW 51000, 'You''re not allowed to do the action', 1;

    IF @Operation = 'SaveKRI'
    BEGIN
        IF EXISTS(SELECT 1 FROM KeyRiskIndicator WHERE IndicatorID = @IndicatorID AND CompanyID = @CompanyID)
        BEGIN
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'UPDATE', 'KeyRiskIndicator', @IndicatorID,
                    (SELECT * FROM KeyRiskIndicator WHERE IndicatorID = @IndicatorID FOR XML AUTO), GETDATE());

            UPDATE [KeyRiskIndicator]
            SET IndicatorName = @IndicatorName,
                RelatedRisk = @RelatedRisk,
                MeasurementFrequency = @MeasurementFrequency,
                DataSource = @DataSource,
                ThresholdValue = @ThresholdValue,
                CurrentValue = @CurrentValue,
                Status = @Status,
                Owner = @Owner,
                LastUpdateDate = @LastUpdateDate,
                ModifiedBy = @UserID,
                UpdatedAt = GETUTCDATE()
            WHERE IndicatorID = @IndicatorID AND CompanyID = @CompanyID;
        END
        ELSE
        BEGIN
            INSERT INTO [KeyRiskIndicator]
            (CompanyID, IndicatorName, RelatedRisk, MeasurementFrequency, DataSource, ThresholdValue, CurrentValue, Status, Owner, LastUpdateDate, CreatedBy, CreatedAt, UpdatedAt)
            VALUES
            (@CompanyID, @IndicatorName, @RelatedRisk, @MeasurementFrequency, @DataSource, @ThresholdValue, @CurrentValue, @Status, @Owner, @LastUpdateDate, @UserID, GETUTCDATE(), GETUTCDATE());

            DECLARE @NewIndicatorID INT = SCOPE_IDENTITY();
            INSERT INTO [ScreenActionHistory]
            VALUES (@UserID, 'Insert', 'KeyRiskIndicator', @NewIndicatorID,
                    (SELECT * FROM KeyRiskIndicator WHERE IndicatorID = @NewIndicatorID FOR XML AUTO), GETDATE());
        END
    END
    ELSE IF @Operation = 'rtvKRIs'
    BEGIN
        SELECT IndicatorID, CompanyID, IndicatorName, RelatedRisk, MeasurementFrequency, DataSource, ThresholdValue, CurrentValue, Status, Owner, LastUpdateDate, CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM KeyRiskIndicator
        WHERE CompanyID = @CompanyID;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select All', 'KeyRiskIndicator', NULL,
                (SELECT * FROM KeyRiskIndicator WHERE CompanyID = @CompanyID FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'rtvKRI'
    BEGIN
        SELECT IndicatorID, CompanyID, IndicatorName, RelatedRisk, MeasurementFrequency, DataSource, ThresholdValue, CurrentValue, Status, Owner, LastUpdateDate, CreatedBy, ModifiedBy, CreatedAt, UpdatedAt
        FROM KeyRiskIndicator
        WHERE IndicatorID = @IndicatorID AND CompanyID = @CompanyID;

        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'Select One', 'KeyRiskIndicator', @IndicatorID,
                (SELECT * FROM KeyRiskIndicator WHERE IndicatorID = @IndicatorID FOR XML AUTO), GETDATE());
    END
    ELSE IF @Operation = 'DeleteKRI'
    BEGIN
        INSERT INTO [ScreenActionHistory]
        VALUES (@UserID, 'DELETE', 'KeyRiskIndicator', @IndicatorID,
                (SELECT * FROM KeyRiskIndicator WHERE IndicatorID = @IndicatorID FOR XML AUTO), GETDATE());

        DELETE FROM KeyRiskIndicator WHERE IndicatorID = @IndicatorID AND CompanyID = @CompanyID;
    END
END
GO
