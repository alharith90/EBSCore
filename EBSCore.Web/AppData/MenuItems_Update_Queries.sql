-- MenuItems URL fixes to align with Razor @page routes
UPDATE [dbo].[MenuItems]
SET [Url] = '/access-denied'
WHERE [MenuItemID] = 90105 AND [Url] = '/AccessDenied';

UPDATE [dbo].[MenuItems]
SET [Url] = '/BCM/S7SBIA'
WHERE [MenuItemID] = 20101 AND [Url] = '/BCM/BiaIndex';

UPDATE [dbo].[MenuItems]
SET [Url] = '/governance'
WHERE [MenuItemID] = 10201 AND [Url] = '/BCM/Governance/GovernanceIndex';

UPDATE [dbo].[MenuItems]
SET [Url] = '/governance/roles'
WHERE [MenuItemID] = 10202 AND [Url] = '/BCM/Governance/RoleMatrix';

UPDATE [dbo].[MenuItems]
SET [Url] = '/BCM/PositionsResponsibilities'
WHERE [MenuItemID] = 10203 AND [Url] = '/BCM/Governance/PositionsResponsibilities';

UPDATE [dbo].[MenuItems]
SET [Url] = '/governance/committees'
WHERE [MenuItemID] = 10204 AND [Url] = '/BCM/Governance/Committees';

UPDATE [dbo].[MenuItems]
SET [Url] = '/governance/competency-gaps'
WHERE [MenuItemID] = 10205 AND [Url] = '/BCM/Governance/CompetencyGaps';

UPDATE [dbo].[MenuItems]
SET [Url] = '/bcm/plans'
WHERE [MenuItemID] = 20301 AND [Url] = '/BCM/Plans';

UPDATE [dbo].[MenuItems]
SET [Url] = '/bcm/plans/{PlanId:int}'
WHERE [MenuItemID] = 20302 AND [Url] = '/BCM/Plans/{PlanId:int}';

UPDATE [dbo].[MenuItems]
SET [Url] = '/bcm/plans/edit/{PlanId:int?}'
WHERE [MenuItemID] = 20303 AND [Url] = '/BCM/Plans/Edit/{PlanId:int?}';

UPDATE [dbo].[MenuItems]
SET [Url] = '/bcm/plans/{PlanId:int}/exercises'
WHERE [MenuItemID] = 20304 AND [Url] = '/BCM/Plans/{PlanId:int}/exercises';

UPDATE [dbo].[MenuItems]
SET [Url] = '/bcm/plans/{PlanId:int}/pir'
WHERE [MenuItemID] = 20305 AND [Url] = '/BCM/Plans/{PlanId:int}/pir';

UPDATE [dbo].[MenuItems]
SET [Url] = '/Documents/Communications'
WHERE [MenuItemID] = 30301 AND [Url] = '/Documents/CommunicationLogs';

UPDATE [dbo].[MenuItems]
SET [Url] = '/Documents/History/{DocumentId:int}'
WHERE [MenuItemID] = 30102 AND [Url] = '/Documents/DocumentHistory';

UPDATE [dbo].[MenuItems]
SET [Url] = '/Documents/Library'
WHERE [MenuItemID] = 30101 AND [Url] = '/Documents/DocumentLibrary';

UPDATE [dbo].[MenuItems]
SET [Url] = '/Documents/Review/{DocumentId:int}'
WHERE [MenuItemID] = 30103 AND [Url] = '/Documents/DocumentReview/{DocumentId:int}';

UPDATE [dbo].[MenuItems]
SET [Url] = '/Notification'
WHERE [MenuItemID] = 60101 AND [Url] = '/Notification/S7SNotificationIndex';

UPDATE [dbo].[MenuItems]
SET [Url] = '/Notification/Connections'
WHERE [MenuItemID] = 60103 AND [Url] = '/Notification/S7SNotificationConnections';

UPDATE [dbo].[MenuItems]
SET [Url] = '/Notification/Templates'
WHERE [MenuItemID] = 60104 AND [Url] = '/Notification/S7SNotificationTemplates';

UPDATE [dbo].[MenuItems]
SET [Url] = '/Workflow/Builder/{WorkflowId:int?}'
WHERE [MenuItemID] = 70101 AND [Url] = '/Workflow/WorkflowBuilder';

UPDATE [dbo].[MenuItems]
SET [Url] = '/Workflow/List'
WHERE [MenuItemID] = 70102 AND [Url] = '/Workflow/WorkflowList';

UPDATE [dbo].[MenuItems]
SET [Url] = '/Workflow/Executions/{WorkflowId:int?}'
WHERE [MenuItemID] = 70103 AND [Url] = '/Workflow/WorkflowExecutions';

-- MenuItems inserts for Razor routes not present in MenuItems
IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItems] WHERE [Url] = '/BCM/RiskForm/{RiskId:int}')
BEGIN
    INSERT INTO [dbo].[MenuItems]
        ([ParentID], [LabelAR], [LabelEN], [DescriptionAR], [DescriptionEn], [Url], [Icon], [Order], [IsActive], [Permission], [Type], [CreatedBy], [UpdatedBy], [CreatedAt], [UpdatedAt])
    VALUES
        (202, N'Risk Form Details', N'Risk Form Details', NULL, NULL, '/BCM/RiskForm/{RiskId:int}', NULL, 4, 1, NULL, NULL, 1, NULL, GETUTCDATE(), NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItems] WHERE [Url] = '/Config1/InformationSystems-Orignial')
BEGIN
    INSERT INTO [dbo].[MenuItems]
        ([ParentID], [LabelAR], [LabelEN], [DescriptionAR], [DescriptionEn], [Url], [Icon], [Order], [IsActive], [Permission], [Type], [CreatedBy], [UpdatedBy], [CreatedAt], [UpdatedAt])
    VALUES
        (501, N'Information Systems Copy', N'Information Systems Copy', NULL, NULL, '/Config1/InformationSystems-Orignial', NULL, 4, 1, NULL, NULL, 1, NULL, GETUTCDATE(), NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItems] WHERE [Url] = '/Config1/InformationSystemsDT')
BEGIN
    INSERT INTO [dbo].[MenuItems]
        ([ParentID], [LabelAR], [LabelEN], [DescriptionAR], [DescriptionEn], [Url], [Icon], [Order], [IsActive], [Permission], [Type], [CreatedBy], [UpdatedBy], [CreatedAt], [UpdatedAt])
    VALUES
        (501, N'Information Systems Copy 2', N'Information Systems Copy 2', NULL, NULL, '/Config1/InformationSystemsDT', NULL, 5, 1, NULL, NULL, 1, NULL, GETUTCDATE(), NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItems] WHERE [Url] = '/GRC/AuditFindings')
BEGIN
    INSERT INTO [dbo].[MenuItems]
        ([ParentID], [LabelAR], [LabelEN], [DescriptionAR], [DescriptionEn], [Url], [Icon], [Order], [IsActive], [Permission], [Type], [CreatedBy], [UpdatedBy], [CreatedAt], [UpdatedAt])
    VALUES
        (3, N'Audit Findings', N'Audit Findings', NULL, NULL, '/GRC/AuditFindings', NULL, 4, 1, NULL, NULL, 1, NULL, GETUTCDATE(), NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItems] WHERE [Url] = '/GRC/AuditPlans')
BEGIN
    INSERT INTO [dbo].[MenuItems]
        ([ParentID], [LabelAR], [LabelEN], [DescriptionAR], [DescriptionEn], [Url], [Icon], [Order], [IsActive], [Permission], [Type], [CreatedBy], [UpdatedBy], [CreatedAt], [UpdatedAt])
    VALUES
        (3, N'Audit Plans', N'Audit Plans', NULL, NULL, '/GRC/AuditPlans', NULL, 5, 1, NULL, NULL, 1, NULL, GETUTCDATE(), NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItems] WHERE [Url] = '/GRC/AuditUniverse')
BEGIN
    INSERT INTO [dbo].[MenuItems]
        ([ParentID], [LabelAR], [LabelEN], [DescriptionAR], [DescriptionEn], [Url], [Icon], [Order], [IsActive], [Permission], [Type], [CreatedBy], [UpdatedBy], [CreatedAt], [UpdatedAt])
    VALUES
        (3, N'Audit Universe', N'Audit Universe', NULL, NULL, '/GRC/AuditUniverse', NULL, 6, 1, NULL, NULL, 1, NULL, GETUTCDATE(), NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItems] WHERE [Url] = '/GRC/BCPPlans')
BEGIN
    INSERT INTO [dbo].[MenuItems]
        ([ParentID], [LabelAR], [LabelEN], [DescriptionAR], [DescriptionEn], [Url], [Icon], [Order], [IsActive], [Permission], [Type], [CreatedBy], [UpdatedBy], [CreatedAt], [UpdatedAt])
    VALUES
        (3, N'BCP Plans', N'BCP Plans', NULL, NULL, '/GRC/BCPPlans', NULL, 7, 1, NULL, NULL, 1, NULL, GETUTCDATE(), NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItems] WHERE [Url] = '/GRC/BIAProcess')
BEGIN
    INSERT INTO [dbo].[MenuItems]
        ([ParentID], [LabelAR], [LabelEN], [DescriptionAR], [DescriptionEn], [Url], [Icon], [Order], [IsActive], [Permission], [Type], [CreatedBy], [UpdatedBy], [CreatedAt], [UpdatedAt])
    VALUES
        (3, N'BIA Process', N'BIA Process', NULL, NULL, '/GRC/BIAProcess', NULL, 8, 1, NULL, NULL, 1, NULL, GETUTCDATE(), NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItems] WHERE [Url] = '/GRC/ComplianceAssessments')
BEGIN
    INSERT INTO [dbo].[MenuItems]
        ([ParentID], [LabelAR], [LabelEN], [DescriptionAR], [DescriptionEn], [Url], [Icon], [Order], [IsActive], [Permission], [Type], [CreatedBy], [UpdatedBy], [CreatedAt], [UpdatedAt])
    VALUES
        (3, N'Compliance Assessments', N'Compliance Assessments', NULL, NULL, '/GRC/ComplianceAssessments', NULL, 9, 1, NULL, NULL, 1, NULL, GETUTCDATE(), NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItems] WHERE [Url] = '/GRC/ComplianceIssues')
BEGIN
    INSERT INTO [dbo].[MenuItems]
        ([ParentID], [LabelAR], [LabelEN], [DescriptionAR], [DescriptionEn], [Url], [Icon], [Order], [IsActive], [Permission], [Type], [CreatedBy], [UpdatedBy], [CreatedAt], [UpdatedAt])
    VALUES
        (3, N'Compliance Issues', N'Compliance Issues', NULL, NULL, '/GRC/ComplianceIssues', NULL, 10, 1, NULL, NULL, 1, NULL, GETUTCDATE(), NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItems] WHERE [Url] = '/GRC/ComplianceObligations')
BEGIN
    INSERT INTO [dbo].[MenuItems]
        ([ParentID], [LabelAR], [LabelEN], [DescriptionAR], [DescriptionEn], [Url], [Icon], [Order], [IsActive], [Permission], [Type], [CreatedBy], [UpdatedBy], [CreatedAt], [UpdatedAt])
    VALUES
        (3, N'Compliance Obligations', N'Compliance Obligations', NULL, NULL, '/GRC/ComplianceObligations', NULL, 11, 1, NULL, NULL, 1, NULL, GETUTCDATE(), NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItems] WHERE [Url] = '/GRC/ControlLibrary')
BEGIN
    INSERT INTO [dbo].[MenuItems]
        ([ParentID], [LabelAR], [LabelEN], [DescriptionAR], [DescriptionEn], [Url], [Icon], [Order], [IsActive], [Permission], [Type], [CreatedBy], [UpdatedBy], [CreatedAt], [UpdatedAt])
    VALUES
        (3, N'Control Library', N'Control Library', NULL, NULL, '/GRC/ControlLibrary', NULL, 12, 1, NULL, NULL, 1, NULL, GETUTCDATE(), NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItems] WHERE [Url] = '/GRC/ESGMetrics')
BEGIN
    INSERT INTO [dbo].[MenuItems]
        ([ParentID], [LabelAR], [LabelEN], [DescriptionAR], [DescriptionEn], [Url], [Icon], [Order], [IsActive], [Permission], [Type], [CreatedBy], [UpdatedBy], [CreatedAt], [UpdatedAt])
    VALUES
        (3, N'ESG Metrics', N'ESG Metrics', NULL, NULL, '/GRC/ESGMetrics', NULL, 13, 1, NULL, NULL, 1, NULL, GETUTCDATE(), NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItems] WHERE [Url] = '/GRC/EnvironmentalAspects')
BEGIN
    INSERT INTO [dbo].[MenuItems]
        ([ParentID], [LabelAR], [LabelEN], [DescriptionAR], [DescriptionEn], [Url], [Icon], [Order], [IsActive], [Permission], [Type], [CreatedBy], [UpdatedBy], [CreatedAt], [UpdatedAt])
    VALUES
        (3, N'Environmental Aspects', N'Environmental Aspects', NULL, NULL, '/GRC/EnvironmentalAspects', NULL, 14, 1, NULL, NULL, 1, NULL, GETUTCDATE(), NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItems] WHERE [Url] = '/GRC/EnvironmentalObjectives')
BEGIN
    INSERT INTO [dbo].[MenuItems]
        ([ParentID], [LabelAR], [LabelEN], [DescriptionAR], [DescriptionEn], [Url], [Icon], [Order], [IsActive], [Permission], [Type], [CreatedBy], [UpdatedBy], [CreatedAt], [UpdatedAt])
    VALUES
        (3, N'Environmental Objectives', N'Environmental Objectives', NULL, NULL, '/GRC/EnvironmentalObjectives', NULL, 15, 1, NULL, NULL, 1, NULL, GETUTCDATE(), NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItems] WHERE [Url] = '/GRC/HSIncidents')
BEGIN
    INSERT INTO [dbo].[MenuItems]
        ([ParentID], [LabelAR], [LabelEN], [DescriptionAR], [DescriptionEn], [Url], [Icon], [Order], [IsActive], [Permission], [Type], [CreatedBy], [UpdatedBy], [CreatedAt], [UpdatedAt])
    VALUES
        (3, N'HS Incidents', N'HS Incidents', NULL, NULL, '/GRC/HSIncidents', NULL, 16, 1, NULL, NULL, 1, NULL, GETUTCDATE(), NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItems] WHERE [Url] = '/GRC/HSRiskAssessments')
BEGIN
    INSERT INTO [dbo].[MenuItems]
        ([ParentID], [LabelAR], [LabelEN], [DescriptionAR], [DescriptionEn], [Url], [Icon], [Order], [IsActive], [Permission], [Type], [CreatedBy], [UpdatedBy], [CreatedAt], [UpdatedAt])
    VALUES
        (3, N'HS Risk Assessments', N'HS Risk Assessments', NULL, NULL, '/GRC/HSRiskAssessments', NULL, 17, 1, NULL, NULL, 1, NULL, GETUTCDATE(), NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItems] WHERE [Url] = '/GRC/IncidentRegister')
BEGIN
    INSERT INTO [dbo].[MenuItems]
        ([ParentID], [LabelAR], [LabelEN], [DescriptionAR], [DescriptionEn], [Url], [Icon], [Order], [IsActive], [Permission], [Type], [CreatedBy], [UpdatedBy], [CreatedAt], [UpdatedAt])
    VALUES
        (3, N'Incident Register', N'Incident Register', NULL, NULL, '/GRC/IncidentRegister', NULL, 18, 1, NULL, NULL, 1, NULL, GETUTCDATE(), NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItems] WHERE [Url] = '/GRC/InformationSystems')
BEGIN
    INSERT INTO [dbo].[MenuItems]
        ([ParentID], [LabelAR], [LabelEN], [DescriptionAR], [DescriptionEn], [Url], [Icon], [Order], [IsActive], [Permission], [Type], [CreatedBy], [UpdatedBy], [CreatedAt], [UpdatedAt])
    VALUES
        (3, N'Information Systems', N'Information Systems', NULL, NULL, '/GRC/InformationSystems', NULL, 19, 1, NULL, NULL, 1, NULL, GETUTCDATE(), NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItems] WHERE [Url] = '/GRC/KeyRiskIndicators')
BEGIN
    INSERT INTO [dbo].[MenuItems]
        ([ParentID], [LabelAR], [LabelEN], [DescriptionAR], [DescriptionEn], [Url], [Icon], [Order], [IsActive], [Permission], [Type], [CreatedBy], [UpdatedBy], [CreatedAt], [UpdatedAt])
    VALUES
        (3, N'Key Risk Indicators', N'Key Risk Indicators', NULL, NULL, '/GRC/KeyRiskIndicators', NULL, 20, 1, NULL, NULL, 1, NULL, GETUTCDATE(), NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItems] WHERE [Url] = '/GRC/PolicyLibrary')
BEGIN
    INSERT INTO [dbo].[MenuItems]
        ([ParentID], [LabelAR], [LabelEN], [DescriptionAR], [DescriptionEn], [Url], [Icon], [Order], [IsActive], [Permission], [Type], [CreatedBy], [UpdatedBy], [CreatedAt], [UpdatedAt])
    VALUES
        (3, N'Policy Library', N'Policy Library', NULL, NULL, '/GRC/PolicyLibrary', NULL, 21, 1, NULL, NULL, 1, NULL, GETUTCDATE(), NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItems] WHERE [Url] = '/GRC/RiskCategories')
BEGIN
    INSERT INTO [dbo].[MenuItems]
        ([ParentID], [LabelAR], [LabelEN], [DescriptionAR], [DescriptionEn], [Url], [Icon], [Order], [IsActive], [Permission], [Type], [CreatedBy], [UpdatedBy], [CreatedAt], [UpdatedAt])
    VALUES
        (3, N'Risk Categories', N'Risk Categories', NULL, NULL, '/GRC/RiskCategories', NULL, 22, 1, NULL, NULL, 1, NULL, GETUTCDATE(), NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItems] WHERE [Url] = '/GRC/RiskRegister')
BEGIN
    INSERT INTO [dbo].[MenuItems]
        ([ParentID], [LabelAR], [LabelEN], [DescriptionAR], [DescriptionEn], [Url], [Icon], [Order], [IsActive], [Permission], [Type], [CreatedBy], [UpdatedBy], [CreatedAt], [UpdatedAt])
    VALUES
        (3, N'Risk Register', N'Risk Register', NULL, NULL, '/GRC/RiskRegister', NULL, 23, 1, NULL, NULL, 1, NULL, GETUTCDATE(), NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItems] WHERE [Url] = '/GRC/RiskTemplates')
BEGIN
    INSERT INTO [dbo].[MenuItems]
        ([ParentID], [LabelAR], [LabelEN], [DescriptionAR], [DescriptionEn], [Url], [Icon], [Order], [IsActive], [Permission], [Type], [CreatedBy], [UpdatedBy], [CreatedAt], [UpdatedAt])
    VALUES
        (3, N'Risk Templates', N'Risk Templates', NULL, NULL, '/GRC/RiskTemplates', NULL, 24, 1, NULL, NULL, 1, NULL, GETUTCDATE(), NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItems] WHERE [Url] = '/GRC/RiskTreatmentPlan')
BEGIN
    INSERT INTO [dbo].[MenuItems]
        ([ParentID], [LabelAR], [LabelEN], [DescriptionAR], [DescriptionEn], [Url], [Icon], [Order], [IsActive], [Permission], [Type], [CreatedBy], [UpdatedBy], [CreatedAt], [UpdatedAt])
    VALUES
        (3, N'Risk Treatment Plan', N'Risk Treatment Plan', NULL, NULL, '/GRC/RiskTreatmentPlan', NULL, 25, 1, NULL, NULL, 1, NULL, GETUTCDATE(), NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItems] WHERE [Url] = '/GRC/StrategicObjectives')
BEGIN
    INSERT INTO [dbo].[MenuItems]
        ([ParentID], [LabelAR], [LabelEN], [DescriptionAR], [DescriptionEn], [Url], [Icon], [Order], [IsActive], [Permission], [Type], [CreatedBy], [UpdatedBy], [CreatedAt], [UpdatedAt])
    VALUES
        (3, N'Strategic Objectives', N'Strategic Objectives', NULL, NULL, '/GRC/StrategicObjectives', NULL, 26, 1, NULL, NULL, 1, NULL, GETUTCDATE(), NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItems] WHERE [Url] = '/GRC/SustainabilityInitiatives')
BEGIN
    INSERT INTO [dbo].[MenuItems]
        ([ParentID], [LabelAR], [LabelEN], [DescriptionAR], [DescriptionEn], [Url], [Icon], [Order], [IsActive], [Permission], [Type], [CreatedBy], [UpdatedBy], [CreatedAt], [UpdatedAt])
    VALUES
        (3, N'Sustainability Initiatives', N'Sustainability Initiatives', NULL, NULL, '/GRC/SustainabilityInitiatives', NULL, 27, 1, NULL, NULL, 1, NULL, GETUTCDATE(), NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItems] WHERE [Url] = '/GRC/ThirdPartyAssessments')
BEGIN
    INSERT INTO [dbo].[MenuItems]
        ([ParentID], [LabelAR], [LabelEN], [DescriptionAR], [DescriptionEn], [Url], [Icon], [Order], [IsActive], [Permission], [Type], [CreatedBy], [UpdatedBy], [CreatedAt], [UpdatedAt])
    VALUES
        (3, N'Third Party Assessments', N'Third Party Assessments', NULL, NULL, '/GRC/ThirdPartyAssessments', NULL, 28, 1, NULL, NULL, 1, NULL, GETUTCDATE(), NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItems] WHERE [Url] = '/GRC/ThirdPartyIncidents')
BEGIN
    INSERT INTO [dbo].[MenuItems]
        ([ParentID], [LabelAR], [LabelEN], [DescriptionAR], [DescriptionEn], [Url], [Icon], [Order], [IsActive], [Permission], [Type], [CreatedBy], [UpdatedBy], [CreatedAt], [UpdatedAt])
    VALUES
        (3, N'Third Party Incidents', N'Third Party Incidents', NULL, NULL, '/GRC/ThirdPartyIncidents', NULL, 29, 1, NULL, NULL, 1, NULL, GETUTCDATE(), NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItems] WHERE [Url] = '/GRC/ThirdPartyProfiles')
BEGIN
    INSERT INTO [dbo].[MenuItems]
        ([ParentID], [LabelAR], [LabelEN], [DescriptionAR], [DescriptionEn], [Url], [Icon], [Order], [IsActive], [Permission], [Type], [CreatedBy], [UpdatedBy], [CreatedAt], [UpdatedAt])
    VALUES
        (3, N'Third Party Profiles', N'Third Party Profiles', NULL, NULL, '/GRC/ThirdPartyProfiles', NULL, 30, 1, NULL, NULL, 1, NULL, GETUTCDATE(), NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[MenuItems] WHERE [Url] = '/test')
BEGIN
    INSERT INTO [dbo].[MenuItems]
        ([ParentID], [LabelAR], [LabelEN], [DescriptionAR], [DescriptionEn], [Url], [Icon], [Order], [IsActive], [Permission], [Type], [CreatedBy], [UpdatedBy], [CreatedAt], [UpdatedAt])
    VALUES
        (901, N'Test', N'Test', NULL, NULL, '/test', NULL, 6, 0, NULL, NULL, 1, NULL, GETUTCDATE(), NULL);
END;
