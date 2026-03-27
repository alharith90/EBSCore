/*
    Enterprise admin navigation seed aligned with modern module architecture.
    Safe to run multiple times.
*/

DECLARE @Now DATETIME = GETUTCDATE();
DECLARE @EnterpriseRootId INT;

IF NOT EXISTS (SELECT 1 FROM dbo.MenuItems WHERE Url = '/Enterprise/CommandCenter')
BEGIN
    INSERT INTO dbo.MenuItems (ParentID, LabelAR, LabelEN, Url, Icon, [Order], IsActive, CreatedBy, CreatedAt)
    VALUES (NULL, N'قيادة المؤسسة', N'Enterprise Command Center', '/Enterprise/CommandCenter', 'fa-solid fa-building', 200, 1, 1, @Now);
END;

SELECT TOP 1 @EnterpriseRootId = MenuItemID FROM dbo.MenuItems WHERE Url = '/Enterprise/CommandCenter' ORDER BY MenuItemID DESC;

IF @EnterpriseRootId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.MenuItems WHERE Url = '/Enterprise/FinanceBudget')
        INSERT INTO dbo.MenuItems (ParentID, LabelAR, LabelEN, Url, Icon, [Order], IsActive, CreatedBy, CreatedAt)
        VALUES (@EnterpriseRootId, N'المالية والميزانية', N'Finance & Budget Management', '/Enterprise/FinanceBudget', 'fa-solid fa-wallet', 1, 1, 1, @Now);

    IF NOT EXISTS (SELECT 1 FROM dbo.MenuItems WHERE LabelEN = 'HR and Workforce Analytics')
        INSERT INTO dbo.MenuItems (ParentID, LabelAR, LabelEN, Url, Icon, [Order], IsActive, CreatedBy, CreatedAt)
        VALUES (@EnterpriseRootId, N'تحليلات الموارد البشرية', N'HR and Workforce Analytics', '/Config/Employees', 'fa-solid fa-people-group', 2, 1, 1, @Now);

    IF NOT EXISTS (SELECT 1 FROM dbo.MenuItems WHERE LabelEN = 'Sales and Revenue Performance')
        INSERT INTO dbo.MenuItems (ParentID, LabelAR, LabelEN, Url, Icon, [Order], IsActive, CreatedBy, CreatedAt)
        VALUES (@EnterpriseRootId, N'أداء المبيعات والإيرادات', N'Sales and Revenue Performance', '/Operations/Dashboard', 'fa-solid fa-chart-line', 3, 1, 1, @Now);

    IF NOT EXISTS (SELECT 1 FROM dbo.MenuItems WHERE LabelEN = 'Project and PMO Portfolio')
        INSERT INTO dbo.MenuItems (ParentID, LabelAR, LabelEN, Url, Icon, [Order], IsActive, CreatedBy, CreatedAt)
        VALUES (@EnterpriseRootId, N'محفظة المشاريع ومكتب إدارة المشاريع', N'Project and PMO Portfolio', '/Workflow/List', 'fa-solid fa-diagram-project', 4, 1, 1, @Now);

    IF NOT EXISTS (SELECT 1 FROM dbo.MenuItems WHERE LabelEN = 'Risk, Compliance, and Audit Management')
        INSERT INTO dbo.MenuItems (ParentID, LabelAR, LabelEN, Url, Icon, [Order], IsActive, CreatedBy, CreatedAt)
        VALUES (@EnterpriseRootId, N'إدارة المخاطر والامتثال والتدقيق', N'Risk, Compliance, and Audit Management', '/GRC/GovernanceDashboard', 'fa-solid fa-shield-halved', 5, 1, 1, @Now);

    IF NOT EXISTS (SELECT 1 FROM dbo.MenuItems WHERE LabelEN = 'Supply Chain and Inventory Performance')
        INSERT INTO dbo.MenuItems (ParentID, LabelAR, LabelEN, Url, Icon, [Order], IsActive, CreatedBy, CreatedAt)
        VALUES (@EnterpriseRootId, N'أداء سلاسل الإمداد والمخزون', N'Supply Chain and Inventory Performance', '/Operations/SupplyChainDashboard', 'fa-solid fa-boxes-stacked', 6, 1, 1, @Now);

    IF NOT EXISTS (SELECT 1 FROM dbo.MenuItems WHERE LabelEN = 'Customer Service and SLA Management')
        INSERT INTO dbo.MenuItems (ParentID, LabelAR, LabelEN, Url, Icon, [Order], IsActive, CreatedBy, CreatedAt)
        VALUES (@EnterpriseRootId, N'إدارة خدمة العملاء واتفاقيات مستوى الخدمة', N'Customer Service and SLA Management', '/Notification/S7SNotifications', 'fa-solid fa-headset', 7, 1, 1, @Now);

    IF NOT EXISTS (SELECT 1 FROM dbo.MenuItems WHERE LabelEN = 'Data Quality and Master Data Governance')
        INSERT INTO dbo.MenuItems (ParentID, LabelAR, LabelEN, Url, Icon, [Order], IsActive, CreatedBy, CreatedAt)
        VALUES (@EnterpriseRootId, N'جودة البيانات وحوكمة البيانات الرئيسية', N'Data Quality and Master Data Governance', '/Sys/Lookup', 'fa-solid fa-database', 8, 1, 1, @Now);

    IF NOT EXISTS (SELECT 1 FROM dbo.MenuItems WHERE LabelEN = 'ESG and Sustainability Management')
        INSERT INTO dbo.MenuItems (ParentID, LabelAR, LabelEN, Url, Icon, [Order], IsActive, CreatedBy, CreatedAt)
        VALUES (@EnterpriseRootId, N'إدارة الاستدامة وESG', N'ESG and Sustainability Management', '/GRC/ESGMetrics', 'fa-solid fa-leaf', 9, 1, 1, @Now);
END;
