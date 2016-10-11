
CREATE TABLE [DbVersion]
(
	[Version] INTEGER NOT NULL,
	[CreationTime] TEXT(19) NOT NULL,
	[UpgradeTime] TEXT(19) NOT NULL
);

CREATE TABLE [Transactions]
(
	[Id] TEXT(36) NOT NULL,
	[TransactionDate] TEXT(10) NOT NULL,
	[Sequence] INTEGER PRIMARY KEY AUTOINCREMENT,
	[Type] INTEGER NOT NULL,
	[ASXCode] TEXT(6) NOT NULL,
	[Description] TEXT(200)  NOT NULL,
	[Attachment] TEXT(36) NOT NULL,
	[RecordDate] TEXT(10) NOT NULL,
	[Comment] TEXT(200)  NOT NULL
);

CREATE UNIQUE INDEX [Index_Transactions_Id] ON[Transactions]
(
		[Id]
);

CREATE INDEX [Index_Transactions_ByDate] ON [Transactions]
(
		[TransactionDate] ASC,
		[Sequence] ASC
);

CREATE UNIQUE INDEX [Index_Transactions_ByASXCode] ON [Transactions]
(
        [ASXCode]  ASC,
		[TransactionDate] ASC,
		[Sequence] ASC
);

CREATE TABLE [Aquisitions]
(
	[Id] TEXT(36) NOT NULL,
	[Units] INTEGER NOT NULL,
	[AveragePrice] INTEGER NOT NULL,
	[TransactionCosts] INTEGER NOT NULL,
	[CreateCashTransaction] TEXT(1) NOT NULL DEFAULT "N",
	
	PRIMARY KEY ([Id])
);

CREATE TABLE [CostBaseAdjustments]
(
	[Id] TEXT(36) NOT NULL,
	[Percentage] INTEGER NOT NULL,
	
	PRIMARY KEY ([Id])
);


CREATE TABLE [Disposals]
(
	[Id] TEXT(36) NOT NULL,
	[Units] INTEGER NOT NULL,
	[AveragePrice] INTEGER NOT NULL,
	[TransactionCosts] INTEGER NOT NULL,
	[CGTMethod] INTEGER NOT NULL,
	[CreateCashTransaction] TEXT(1) NOT NULL DEFAULT "N",
	
	PRIMARY KEY ([Id])
);


CREATE TABLE [IncomeReceived]
(
	[Id] TEXT(36) NOT NULL,
	[FrankedAmount] INTEGER NOT NULL,
	[UnfrankedAmount] INTEGER NOT NULL,
	[FrankingCredits] INTEGER NOT NULL,
	[Interest] INTEGER NOT NULL,
	[TaxDeferred] INTEGER  NOT NULL,
	[CreateCashTransaction] TEXT(1) NOT NULL DEFAULT "N",
	
	PRIMARY KEY ([Id])
);


CREATE TABLE [OpeningBalances]
(
	[Id] TEXT(36) NOT NULL,
	[Units] INTEGER NOT NULL,
	[CostBase] INTEGER NOT NULL,
	[AquisitionDate] TEXT(10) NOT NULL,
	[PurchaseId] TEXT(36) NOT NULL,
	
	PRIMARY KEY ([Id])
);


CREATE TABLE [ReturnsOfCapital]
(
	[Id] TEXT(36) NOT NULL,
	[Amount] INTEGER NOT NULL,
	[CreateCashTransaction] TEXT(1) NOT NULL DEFAULT "N",
	
	PRIMARY KEY ([Id])
);


CREATE TABLE [UnitCountAdjustments]
(
	[Id] TEXT(36) NOT NULL,
	[OriginalUnits] INTEGER NOT NULL,
	[NewUnits] INTEGER NOT NULL,
	
	PRIMARY KEY ([Id])
);

CREATE TABLE [CashTransactions]
(
	[Id] TEXT(36) NOT NULL,
	[Type] INTEGER NOT NULL,
	[Amount] INTEGER NOT NULL,
	
	PRIMARY KEY ([Id])
);

CREATE TABLE [Attachments]
(
	[Id] TEXT(36) NOT NULL,
	[Extension] TEXT(10) NOT NULL,
	[Data] BLOB,
	
	PRIMARY KEY ([Id])
);