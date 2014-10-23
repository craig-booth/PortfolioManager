CREATE TABLE [Transactions]
(
	[Id] TEXT(36) NOT NULL,
	[TransactionDate] TEXT(10) NOT NULL,
	[Type] INTEGER NOT NULL,
	[ASXCode] TEXT(6) NOT NULL,
	[Description] TEXT(200)  NOT NULL,
	
	PRIMARY KEY ([Id])
);
CREATE INDEX [Index_Transactions_ByASXCode] ON [Transactions]
(
        [ASXCode]  ASC,
		[TransactionDate] ASC
);

CREATE TABLE [Aquisitions]
(
	[Id] TEXT(36) NOT NULL,
	[Units] INTEGER NOT NULL,
	[AveragePrice] INTEGER NOT NULL,
	[TransactionCosts] INTEGER NOT NULL,
	[Comment] TEXT(200)  NOT NULL,
	
	PRIMARY KEY ([Id])
);

CREATE TABLE [CostBaseAdjustments]
(
	[Id] TEXT(36) NOT NULL,
	[Percentage] INTEGER NOT NULL,
	[Comment] TEXT(200)  NOT NULL,
	
	PRIMARY KEY ([Id])
);


CREATE TABLE [Disposals]
(
	[Id] TEXT(36) NOT NULL,
	[Units] INTEGER NOT NULL,
	[AveragePrice] INTEGER NOT NULL,
	[TransactionCosts] INTEGER NOT NULL,
	[CGTMethod] INTEGER NOT NULL,
	[Comment] TEXT(200)  NOT NULL,
	
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
	[Comment] TEXT(200)  NOT NULL,
	
	PRIMARY KEY ([Id])
);


CREATE TABLE [OpeningBalances]
(
	[Id] TEXT(36) NOT NULL,
	[Units] INTEGER NOT NULL,
	[CostBase] INTEGER NOT NULL,
	[Comment] TEXT(200)  NOT NULL,
	
	PRIMARY KEY ([Id])
);


CREATE TABLE [ReturnsOfCapital]
(
	[Id] TEXT(36) NOT NULL,
	[Amount] INTEGER NOT NULL,
	[Comment] TEXT(200)  NOT NULL,
	
	PRIMARY KEY ([Id])
);