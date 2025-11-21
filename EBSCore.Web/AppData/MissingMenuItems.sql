-- Auto-generated insert statements for missing menu items

SET NOCOUNT ON;


INSERT INTO sec.MenuItems (ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy, CreatedAt)
VALUES (NULL, N'AccessDenied', N'Access Denied', N'MenuAccessDenied', N'/access-denied', NULL, NULL, 2, 1, 1, 0, 1, GETDATE());

INSERT INTO sec.MenuItems (ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy, CreatedAt)
VALUES (2, N'Bcm.BiaIndex', N'Bia', N'MenuBia', N'/BCM/S7SBIA', NULL, NULL, 2, 1, 1, 0, 1, GETDATE());

INSERT INTO sec.MenuItems (ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy, CreatedAt)
VALUES (2, N'Bcm.Committees', N'Committees', N'MenuCommittees', N'/governance/committees', NULL, NULL, 3, 1, 1, 0, 1, GETDATE());

INSERT INTO sec.MenuItems (ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy, CreatedAt)
VALUES (2, N'Bcm.CompetencyGaps', N'Competency Gaps', N'MenuCompetencyGaps', N'/governance/competency-gaps', NULL, NULL, 4, 1, 1, 0, 1, GETDATE());

INSERT INTO sec.MenuItems (ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy, CreatedAt)
VALUES (2, N'Bcm.GovernanceIndex', N'Governance', N'MenuGovernance', N'/governance', NULL, NULL, 5, 1, 1, 0, 1, GETDATE());

INSERT INTO sec.MenuItems (ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy, CreatedAt)
VALUES (2, N'Bcm.RoleMatrix', N'Role Matrix', N'MenuRoleMatrix', N'/governance/roles', NULL, NULL, 6, 1, 1, 0, 1, GETDATE());

INSERT INTO sec.MenuItems (ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy, CreatedAt)
VALUES (2, N'Bcm.ExerciseTracker', N'Exercise Tracker', N'MenuExerciseTracker', N'/bcm/plans/{PlanId:int}/exercises', NULL, NULL, 7, 1, 1, 0, 1, GETDATE());

INSERT INTO sec.MenuItems (ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy, CreatedAt)
VALUES (2, N'Bcm.PIRForm', N'PIRForm', N'MenuPirform', N'/bcm/plans/{PlanId:int}/pir', NULL, NULL, 8, 1, 1, 0, 1, GETDATE());

INSERT INTO sec.MenuItems (ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy, CreatedAt)
VALUES (2, N'Bcm.PlanDetails', N'Plan Details', N'MenuPlanDetails', N'/bcm/plans/{PlanId:int}', NULL, NULL, 9, 1, 1, 0, 1, GETDATE());

INSERT INTO sec.MenuItems (ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy, CreatedAt)
VALUES (2, N'Bcm.PlanEditor', N'Plan Editor', N'MenuPlanEditor', N'/bcm/plans/edit/{PlanId:int?}', NULL, NULL, 10, 1, 1, 0, 1, GETDATE());

INSERT INTO sec.MenuItems (ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy, CreatedAt)
VALUES (2, N'Bcm.PlansIndex', N'Plans', N'MenuPlans', N'/bcm/plans', NULL, NULL, 11, 1, 1, 0, 1, GETDATE());

INSERT INTO sec.MenuItems (ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy, CreatedAt)
VALUES (2, N'Bcm.RiskForm', N'Risk Form', N'MenuRiskForm', N'/BCM/RiskForm', NULL, NULL, 12, 1, 1, 0, 1, GETDATE());

INSERT INTO sec.MenuItems (ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy, CreatedAt)
VALUES (2, N'Bcm.RiskForm', N'Risk Form', N'MenuRiskForm', N'/BCM/RiskForm/{RiskId:int}', NULL, NULL, 13, 1, 1, 0, 1, GETDATE());

INSERT INTO sec.MenuItems (ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy, CreatedAt)
VALUES (2, N'Bcm.RiskIndex', N'Risk', N'MenuRisk', N'/BCM/RiskIndex', NULL, NULL, 14, 1, 1, 0, 1, GETDATE());

INSERT INTO sec.MenuItems (ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy, CreatedAt)
VALUES (2, N'Bcm.RiskMatrixConfiguration', N'Risk Matrix Configuration', N'MenuRiskMatrixConfiguration', N'/BCM/RiskMatrixConfiguration', NULL, NULL, 15, 1, 1, 0, 1, GETDATE());

INSERT INTO sec.MenuItems (ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy, CreatedAt)
VALUES (4, N'Config.InformationSystems-Copy', N'Information Systems-Copy', N'MenuInformationSystemsCopy', N'/Config1/InformationSystems-Orignial', NULL, NULL, 2, 1, 1, 0, 1, GETDATE());

INSERT INTO sec.MenuItems (ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy, CreatedAt)
VALUES (NULL, N'Documents.CommunicationLogs', N'Communication Logs', N'MenuCommunicationLogs', N'/Documents/Communications', NULL, NULL, 3, 1, 1, 0, 1, GETDATE());

INSERT INTO sec.MenuItems (ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy, CreatedAt)
VALUES (NULL, N'Documents.DocumentHistory', N'Document History', N'MenuDocumentHistory', N'/Documents/History/{DocumentId:int}', NULL, NULL, 4, 1, 1, 0, 1, GETDATE());

INSERT INTO sec.MenuItems (ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy, CreatedAt)
VALUES (NULL, N'Documents.DocumentLibrary', N'Document Library', N'MenuDocumentLibrary', N'/Documents/Library', NULL, NULL, 5, 1, 1, 0, 1, GETDATE());

INSERT INTO sec.MenuItems (ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy, CreatedAt)
VALUES (NULL, N'Documents.DocumentReview', N'Document Review', N'MenuDocumentReview', N'/Documents/Review/{DocumentId:int}', NULL, NULL, 6, 1, 1, 0, 1, GETDATE());

INSERT INTO sec.MenuItems (ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy, CreatedAt)
VALUES (NULL, N'Login', N'Login', N'MenuLogin', N'/login', NULL, NULL, 7, 1, 1, 0, 1, GETDATE());

INSERT INTO sec.MenuItems (ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy, CreatedAt)
VALUES (NULL, N'Notification.S7SNotificationConnections', N'S7S Notification Connections', N'MenuS7SNotificationConnections', N'/Notification/Connections', NULL, NULL, 8, 1, 1, 0, 1, GETDATE());

INSERT INTO sec.MenuItems (ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy, CreatedAt)
VALUES (NULL, N'Notification.S7SNotificationIndex', N'S7SNotification', N'MenuS7Snotification', N'/Notification', NULL, NULL, 9, 1, 1, 0, 1, GETDATE());

INSERT INTO sec.MenuItems (ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy, CreatedAt)
VALUES (NULL, N'Notification.S7SNotificationTemplates', N'S7S Notification Templates', N'MenuS7SNotificationTemplates', N'/Notification/Templates', NULL, NULL, 10, 1, 1, 0, 1, GETDATE());

INSERT INTO sec.MenuItems (ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy, CreatedAt)
VALUES (NULL, N'Notification.S7SNotifications', N'S7S Notifications', N'MenuS7SNotifications', N'/Notification/S7SNotifications', NULL, NULL, 11, 1, 1, 0, 1, GETDATE());

INSERT INTO sec.MenuItems (ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy, CreatedAt)
VALUES (NULL, N'ResetPassword', N'Reset Password', N'MenuResetPassword', N'/reset-password', NULL, NULL, 12, 1, 1, 0, 1, GETDATE());

INSERT INTO sec.MenuItems (ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy, CreatedAt)
VALUES (NULL, N'ResetPasswordConfirm', N'Reset Password Confirm', N'MenuResetPasswordConfirm', N'/reset-password-confirm', NULL, NULL, 13, 1, 1, 0, 1, GETDATE());

INSERT INTO sec.MenuItems (ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy, CreatedAt)
VALUES (NULL, N'Security.Assignments', N'Assignments', N'MenuAssignments', N'/Security/Assignments', NULL, NULL, 14, 1, 1, 0, 1, GETDATE());

INSERT INTO sec.MenuItems (ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy, CreatedAt)
VALUES (NULL, N'Security.Groups', N'Groups', N'MenuGroups', N'/Security/Groups', NULL, NULL, 15, 1, 1, 0, 1, GETDATE());

INSERT INTO sec.MenuItems (ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy, CreatedAt)
VALUES (NULL, N'Security.RolePermissions', N'Role Permissions', N'MenuRolePermissions', N'/Security/RolePermissions', NULL, NULL, 16, 1, 1, 0, 1, GETDATE());

INSERT INTO sec.MenuItems (ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy, CreatedAt)
VALUES (NULL, N'Security.Roles', N'Roles', N'MenuRoles', N'/Security/Roles', NULL, NULL, 17, 1, 1, 0, 1, GETDATE());

INSERT INTO sec.MenuItems (ParentMenuItemID, MenuCode, MenuName, MenuNameKey, Url, ApiRoute, IconCssClass, DisplayOrder, IsVisible, StatusID, IsDeleted, CreatedBy, CreatedAt)
VALUES (NULL, N'Workflow.Designer', N'Designer', N'MenuDesigner', N'/Workflow/Designer', NULL, NULL, 18, 1, 1, 0, 1, GETDATE());
