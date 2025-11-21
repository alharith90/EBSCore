EXEC dbo.S7SAddColumnIfMissing 'dbo.BIA', 'AlternativeWorkLocation', 'NVARCHAR(255) NULL';
EXEC dbo.S7SAddColumnIfMissing 'dbo.BIA', 'RegulatoryRequirements', 'NVARCHAR(MAX) NULL';
