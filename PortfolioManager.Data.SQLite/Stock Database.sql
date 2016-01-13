CREATE TABLE [DbVersion]
(
	[Version] INTEGER NOT NULL,
	[CreationTime] TEXT(19) NOT NULL,
	[UpgradeTime] TEXT(19) NOT NULL
);

CREATE TABLE [Stocks]
(
	[Id] TEXT(36) NOT NULL,
	[FromDate] TEXT(10) NOT NULL,
	[ToDate] TEXT(10) NOT NULL,
	[ASXCode] TEXT(6) NOT NULL,
	[Name] TEXT(50)  NOT NULL,
	[Type] INTEGER NOT NULL,
	[Parent] TEXT(36) NOT NULL,
	[DividendRounding] INTEGER,
	
	PRIMARY KEY ([Id] ASC, [FromDate] ASC)
);
CREATE UNIQUE INDEX [Index_Stocks_ByCode] ON [Stocks]
(
	[ASXCode] ASC, 
	[FromDate] ASC
);
CREATE INDEX [Index_Stocks_ByParent] ON [Stocks]
(
	[Parent]  ASC
);


CREATE TABLE [RelativeNTAs]
(
	[Id] TEXT(36) NOT NULL,
	[Date] TEXT(10) NOT NULL,
	[Parent] TEXT(36) NOT NULL,
	[Child] TEXT(36) NOT NULL,
	[Percentage] INTEGER,

	PRIMARY KEY ([Id])
);
CREATE UNIQUE INDEX [Index_RelativeNTAs_ByChild] ON [RelativeNTAs]
(
	[Parent] ASC,
	[Child]  ASC,
	[Date] ASC
);


CREATE TABLE [CorporateActions]
(
	[Id] TEXT(36) NOT NULL,
	[Stock]  TEXT(36) NOT NULL,
	[ActionDate] TEXT(10) NOT NULL,
	[Description] TEXT(50) NOT NULL,
	[Type] INTEGER NOT NULL,

	PRIMARY KEY ([Id])
);
CREATE INDEX [Index_CorporateActions_ByStock] ON [CorporateActions]
(
	[Stock]  ASC,
	[ActionDate]  ASC
);

CREATE TABLE [Dividends]
(
	[Id] TEXT(36) NOT NULL ,
	[PaymentDate] TEXT(10) NOT NULL,
	[DividendAmount] INTEGER NOT NULL,
	[CompanyTaxRate] INTEGER NOT NULL,
	[PercentFranked] INTEGER NOT NULL,
	[DRPPrice] INTEGER NOT NULL,

	PRIMARY KEY ([Id])
);

CREATE TABLE [CapitalReturns]
(
	[Id] TEXT(36) NOT NULL,
	[PaymentDate] TEXT(10) NOT NULL,
	[Amount] INTEGER NOT NULL,

	PRIMARY KEY ([Id])
);

CREATE TABLE [SplitConsolidations]
(
	[Id] TEXT(36) NOT NULL,
	[OldUnits] INTEGER NOT NULL,
	[NewUnits] INTEGER NOT NULL,

	PRIMARY KEY ([Id])
);


CREATE TABLE [Transformations]
(
	[Id] TEXT(36) NOT NULL,
	[ImplementationDate] TEXT(10) NOT NULL,
	[CashComponent] INTEGER NOT NULL,

	PRIMARY KEY ([Id])
);

CREATE TABLE [TransformationResultingStocks]
(
	[Id] TEXT(36) NOT NULL,
	[Stock]  TEXT(36) NOT NULL,
	[OriginalUnits] INTEGER NOT NULL,
	[NewUnits] INTEGER NOT NULL,
	[CostBasePercentage] INTEGER NOT NULL,

	PRIMARY KEY ([Id], [Stock])
);

CREATE TABLE [StockPrices]
(
	[Stock]  TEXT(36) NOT NULL,
	[Date] TEXT(10) NOT NULL,
	[Price] INTEGER NOT NULL,

	PRIMARY KEY ([Stock], [Date])
);