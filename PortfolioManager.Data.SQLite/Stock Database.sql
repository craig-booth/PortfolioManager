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
	[DividendRounding] INTEGER NOT NULL,
	[DRPMethod] INTEGER NOT NULL,
	[Category] INTEGER NOT NULL;

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
	[Id] TEXT(36) NOT NULL,
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


CREATE TABLE [CompositeActions]
(
	[Id] TEXT(36) NOT NULL,
	[Sequence] INTEGER NOT NULL,
	[ChildAction] TEXT(36) NOT NULL,
	[ChildType] INTEGER NOT NULL,

	PRIMARY KEY ([Id], [Sequence])
);

CREATE TABLE [Transformations]
(
	[Id] TEXT(36) NOT NULL,
	[ImplementationDate] TEXT(10) NOT NULL,
	[CashComponent] INTEGER NOT NULL,
    [RolloverRelief] TEXT(1) NOT NULL DEFAULT "Y",

	PRIMARY KEY ([Id])
);

CREATE TABLE [TransformationResultingStocks]
(
	[Id] TEXT(36) NOT NULL,
	[Stock]  TEXT(36) NOT NULL,
	[OriginalUnits] INTEGER NOT NULL,
	[NewUnits] INTEGER NOT NULL,
	[CostBasePercentage] INTEGER NOT NULL,
	[AquisitionDate] CHAR(10) NOT NULL DEFAULT "0001-01-01",

	PRIMARY KEY ([Id], [Stock])
);

CREATE TABLE [StockPrices]
(
	[Stock]  TEXT(36) NOT NULL,
	[Date] TEXT(10) NOT NULL,
	[Price] INTEGER NOT NULL,
	[Current] TEXT(1) NOT NULL DEFAULT "N",

	PRIMARY KEY ([Stock], [Date])
);

CREATE INDEX [StockPrices_Current] ON [StockPrices]
(
	[Current] ASC,
	[Stock]  ASC
);

CREATE TABLE [NonTradingDays]
(
	[Date] TEXT(10) NOT NULL,

	PRIMARY KEY ([Date])
);