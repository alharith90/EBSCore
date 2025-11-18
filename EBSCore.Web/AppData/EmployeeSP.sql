/****** Object:  StoredProcedure [dbo].[EmployeeSP] ******/
IF OBJECT_ID(N'[dbo].[EmployeeSP]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[EmployeeSP];
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[EmployeeSP]
    @Operation NVARCHAR(50),
    @CurrentUserID INT = NULL,
    @EmployeeId INT = NULL,

    @FullName NVARCHAR(250) = NULL,
    @Position NVARCHAR(250) = NULL,
    @Email NVARCHAR(250) = NULL,
    @Phone NVARCHAR(50) = NULL,
    @OrganizationUnitId INT = NULL,
    @SourceId NVARCHAR(100) = NULL,
    @SourceSystem NVARCHAR(100) = NULL,
    @JobTitle NVARCHAR(250) = NULL,
    @JobFamily NVARCHAR(100) = NULL,
    @SupervisorId INT = NULL,
    @EmploymentType NVARCHAR(100) = NULL,
    @Location NVARCHAR(250) = NULL,
    @Department NVARCHAR(250) = NULL,
    @IsActive BIT = NULL,

    @PageSize INT = 10,
    @PageNumber INT = 1,
    @SortColumn NVARCHAR(50) = NULL,
    @SortDirection NVARCHAR(10) = 'ASC',
    @SearchFields NVARCHAR(MAX) = NULL,
    @SearchQuery NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Operation = 'Save'
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM [dbo].[Employee] WHERE EmployeeId = @EmployeeId)
        BEGIN
            INSERT INTO [dbo].[Employee]
            (
                [FullName], [Position], [Email], [Phone], [OrganizationUnitId], [SourceId], [SourceSystem],
                [JobTitle], [JobFamily], [SupervisorId], [EmploymentType], [Location], [Department], [IsActive],
                [CreatedDate]
            )
            VALUES
            (
                @FullName, @Position, @Email, @Phone, @OrganizationUnitId, @SourceId, @SourceSystem,
                @JobTitle, @JobFamily, @SupervisorId, @EmploymentType, @Location, @Department, ISNULL(@IsActive, 1),
                GETUTCDATE()
            );

            SELECT SCOPE_IDENTITY() AS NewEmployeeId;
        END
        ELSE
        BEGIN
            UPDATE [dbo].[Employee]
            SET [FullName] = @FullName,
                [Position] = @Position,
                [Email] = @Email,
                [Phone] = @Phone,
                [OrganizationUnitId] = @OrganizationUnitId,
                [SourceId] = @SourceId,
                [SourceSystem] = @SourceSystem,
                [JobTitle] = @JobTitle,
                [JobFamily] = @JobFamily,
                [SupervisorId] = @SupervisorId,
                [EmploymentType] = @EmploymentType,
                [Location] = @Location,
                [Department] = @Department,
                [IsActive] = ISNULL(@IsActive, [IsActive]),
                [UpdatedDate] = GETUTCDATE()
            WHERE [EmployeeId] = @EmployeeId;
        END
    END

    ELSE IF @Operation = 'rtvEmployee'
    BEGIN
        SELECT [EmployeeId], [FullName], [Position], [Email], [Phone], [OrganizationUnitId], [SourceId], [SourceSystem],
               [JobTitle], [JobFamily], [SupervisorId], [EmploymentType], [Location], [Department], [IsActive],
               [CreatedDate], [UpdatedDate]
          FROM [dbo].[Employee]
         WHERE [EmployeeId] = @EmployeeId;
    END

    ELSE IF @Operation = 'rtvEmployees'
    BEGIN
        SELECT ROW_NUMBER() OVER(ORDER BY EmployeeId) AS SerialNo,
               [EmployeeId], [FullName], [Position], [Email], [Phone], [OrganizationUnitId], [SourceId], [SourceSystem],
               [JobTitle], [JobFamily], [SupervisorId], [EmploymentType], [Location], [Department], [IsActive],
               [CreatedDate], [UpdatedDate]
          FROM [dbo].[Employee]
         WHERE (@SearchQuery IS NULL OR @SearchQuery = '' OR
               (ISNULL(@SearchFields,'') = '' AND (
                    [FullName] LIKE '%' + @SearchQuery + '%' OR [Email] LIKE '%' + @SearchQuery + '%' OR [Department] LIKE '%' + @SearchQuery + '%' OR [JobTitle] LIKE '%' + @SearchQuery + '%'
               )) OR
               (@SearchFields = 'FullName' AND [FullName] LIKE '%' + @SearchQuery + '%') OR
               (@SearchFields = 'Email' AND [Email] LIKE '%' + @SearchQuery + '%') OR
               (@SearchFields = 'Department' AND [Department] LIKE '%' + @SearchQuery + '%') OR
               (@SearchFields = 'JobTitle' AND [JobTitle] LIKE '%' + @SearchQuery + '%')
         )
         ORDER BY 
            CASE WHEN @SortColumn = 'FullName' AND @SortDirection = 'ASC' THEN [FullName] END ASC,
            CASE WHEN @SortColumn = 'FullName' AND @SortDirection = 'DESC' THEN [FullName] END DESC,
            [EmployeeId] DESC
         OFFSET (@PageSize * (@PageNumber - 1)) ROWS
         FETCH NEXT @PageSize ROWS ONLY;

        SELECT CEILING(COUNT(1) * 1.0 / @PageSize) AS PageCount,
               @PageNumber AS CurrentPage
          FROM [dbo].[Employee]
         WHERE (@SearchQuery IS NULL OR @SearchQuery = '' OR
               (ISNULL(@SearchFields,'') = '' AND (
                    [FullName] LIKE '%' + @SearchQuery + '%' OR [Email] LIKE '%' + @SearchQuery + '%' OR [Department] LIKE '%' + @SearchQuery + '%' OR [JobTitle] LIKE '%' + @SearchQuery + '%'
               )) OR
               (@SearchFields = 'FullName' AND [FullName] LIKE '%' + @SearchQuery + '%') OR
               (@SearchFields = 'Email' AND [Email] LIKE '%' + @SearchQuery + '%') OR
               (@SearchFields = 'Department' AND [Department] LIKE '%' + @SearchQuery + '%') OR
               (@SearchFields = 'JobTitle' AND [JobTitle] LIKE '%' + @SearchQuery + '%')
         );
    END

    ELSE IF @Operation = 'Delete'
    BEGIN
        DELETE FROM [dbo].[Employee]
         WHERE [EmployeeId] = @EmployeeId;
    END
END
GO
