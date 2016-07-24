
ALTER TABLE [Transactions]
	ADD COLUMN [RecordDate] TEXT(10) NOT NULL DEFAULT "0001-01-01";


ALTER TABLE [Transactions]
	ADD COLUMN [Comment] TEXT(200)  NOT NULL DEFaULT "";

UPDATE [Transactions] 
	SET [RecordDate] = [Transactions].[TransactionDate],
	    [Comment] = (SELECT [Aquisitions].[Comment] FROM [Aquisitions] WHERE [Aquisitions].[Id] = [Transactions].[Id]) 
	WHERE [Transactions].[Type] = 0;

UPDATE [Transactions] 
	SET [RecordDate] = [Transactions].[TransactionDate],
	    [Comment] = (SELECT [Disposals].[Comment] FROM [Disposals] WHERE [Disposals].[Id] = [Transactions].[Id]) 
	WHERE [Transactions].[Type] = 1;

UPDATE [Transactions] 
	SET [RecordDate] = (SELECT [CostBaseAdjustments].[RecordDate] FROM [CostBaseAdjustments] WHERE [CostBaseAdjustments].[Id] = [Transactions].[Id]),
	    [Comment] = (SELECT [CostBaseAdjustments].[Comment] FROM [CostBaseAdjustments] WHERE [CostBaseAdjustments].[Id] = [Transactions].[Id]) 
	WHERE [Transactions].[Type] = 2;
	
UPDATE [Transactions] 
	SET [RecordDate] = [Transactions].[TransactionDate],
	    [Comment] = (SELECT [OpeningBalances].[Comment] FROM [OpeningBalances] WHERE [OpeningBalances].[Id] = [Transactions].[Id]) 
	WHERE [Transactions].[Type] = 3;

UPDATE [Transactions] 
	SET [RecordDate] = (SELECT [ReturnsOfCapital].[RecordDate] FROM [ReturnsOfCapital] WHERE [ReturnsOfCapital].[Id] = [Transactions].[Id]),
	    [Comment] = (SELECT [ReturnsOfCapital].[Comment] FROM [ReturnsOfCapital] WHERE [ReturnsOfCapital].[Id] = [Transactions].[Id]) 
	WHERE [Transactions].[Type] = 4;

UPDATE [Transactions] 
	SET [RecordDate] = (SELECT [IncomeReceived].[RecordDate] FROM [IncomeReceived] WHERE [IncomeReceived].[Id] = [Transactions].[Id]),
	[Comment] = (SELECT [IncomeReceived].[Comment] FROM [IncomeReceived] WHERE [IncomeReceived].[Id] = [Transactions].[Id]) 
	WHERE [Transactions].[Type] = 5;
            
UPDATE [Transactions] 
	SET [RecordDate] = [Transactions].[TransactionDate],
	    [Comment] = (SELECT [UnitCountAdjustments].[Comment] FROM [UnitCountAdjustments] WHERE [UnitCountAdjustments].[Id] = [Transactions].[Id]) 
	WHERE [Transactions].[Type] = 6;

ALTER TABLE [Aquisitions] RENAME TO [Aquisitions_Backup];
CREATE TABLE [Aquisitions]
(
	[Id] TEXT(36) NOT NULL,
	[Units] INTEGER NOT NULL,
	[AveragePrice] INTEGER NOT NULL,
	[TransactionCosts] INTEGER NOT NULL,
	
	PRIMARY KEY ([Id])
);
INSERT INTO [Aquisitions] SELECT [Id], [Units], [AveragePrice], [TransactionCosts] FROM [Aquisitions_Backup];		

ALTER TABLE [CostBaseAdjustments] RENAME TO [CostBaseAdjustments_Backup];
CREATE TABLE [CostBaseAdjustments]
(
	[Id] TEXT(36) NOT NULL,
	[Percentage] INTEGER NOT NULL,
	
	PRIMARY KEY ([Id])
);
INSERT INTO [CostBaseAdjustments] SELECT [Id], [Percentage] FROM [CostBaseAdjustments_Backup];

ALTER TABLE [Disposals] RENAME TO [Disposals_Backup];
CREATE TABLE [Disposals]
(
	[Id] TEXT(36) NOT NULL,
	[Units] INTEGER NOT NULL,
	[AveragePrice] INTEGER NOT NULL,
	[TransactionCosts] INTEGER NOT NULL,
	[CGTMethod] INTEGER NOT NULL,
	
	PRIMARY KEY ([Id])
);
INSERT INTO [Disposals] SELECT [Id], [Units], [AveragePrice], [TransactionCosts], [CGTMethod] FROM [Disposals_Backup];


ALTER TABLE [IncomeReceived] RENAME TO [IncomeReceived_Backup];
CREATE TABLE [IncomeReceived]
(
	[Id] TEXT(36) NOT NULL,
	[FrankedAmount] INTEGER NOT NULL,
	[UnfrankedAmount] INTEGER NOT NULL,
	[FrankingCredits] INTEGER NOT NULL,
	[Interest] INTEGER NOT NULL,
	[TaxDeferred] INTEGER  NOT NULL,
	
	PRIMARY KEY ([Id])
);
INSERT INTO [IncomeReceived] SELECT [Id], [FrankedAmount], [UnfrankedAmount], [FrankingCredits], [Interest], [TaxDeferred] FROM [IncomeReceived_Backup];

ALTER TABLE [OpeningBalances] RENAME TO [OpeningBalances_Backup];
CREATE TABLE [OpeningBalances]
(
	[Id] TEXT(36) NOT NULL,
	[Units] INTEGER NOT NULL,
	[CostBase] INTEGER NOT NULL,
	[AquisitionDate] TEXT(10) NOT NULL,
	
	PRIMARY KEY ([Id])
);
INSERT INTO [OpeningBalances] SELECT [Id], [Units], [CostBase], [AquisitionDate] FROM [OpeningBalances_Backup];

ALTER TABLE [ReturnsOfCapital] RENAME TO [ReturnsOfCapital_Backup];
CREATE TABLE [ReturnsOfCapital]
(
	[Id] TEXT(36) NOT NULL,
	[Amount] INTEGER NOT NULL,
	
	PRIMARY KEY ([Id])
);
INSERT INTO [ReturnsOfCapital] SELECT [Id], [Amount] FROM [ReturnsOfCapital_Backup];


ALTER TABLE [UnitCountAdjustments] RENAME TO [UnitCountAdjustments_Backup];
CREATE TABLE [UnitCountAdjustments]
(
	[Id] TEXT(36) NOT NULL,
	[OriginalUnits] INTEGER NOT NULL,
	[NewUnits] INTEGER NOT NULL,
	
	PRIMARY KEY ([Id])
);
INSERT INTO [UnitCountAdjustments] SELECT [Id], [OriginalUnits], [NewUnits] FROM [UnitCountAdjustments_Backup];
