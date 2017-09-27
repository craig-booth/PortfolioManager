

CREATE TABLE [Parcels]
(
	[Id] TEXT(36) NOT NULL,
	[FromDate] TEXT(10) NOT NULL,
	[ToDate] TEXT(10) NOT NULL,
	[Stock] TEXT(36) NOT NULL,
    [AquisitionDate] TEXT(10) NOT NULL,
    [Units] INTEGER NOT NULL,
    [UnitPrice] INTEGER NOT NULL,
    [Amount] INTEGER NOT NULL,
    [CostBase] INTEGER NOT NULL,
    [PurchaseId] TEXT(36) NOT NULL,
	
	PRIMARY KEY ([Id] ASC, [FromDate] ASC)
);

CREATE TABLE [CGTEvents]
(
	[Id] TEXT(36) NOT NULL,	
	[Stock] TEXT(36) NOT NULL,
    [Units] INTEGER NOT NULL,
    [EventDate] TEXT(10) NOT NULL,
    [CostBase] INTEGER NOT NULL,
    [AmountReceived] INTEGER NOT NULL,
    [CapitalGain] INTEGER NOT NULL,
    [CGTMethod] INTEGER NOT NULL,

	PRIMARY KEY ([Id])
);


CREATE TABLE [CashAccountTransactions]
(
	[Id] TEXT(36) NOT NULL,
	[Type] INTEGER NOT NULL,
    [Date] TEXT(10) NOT NULL,
    [Description] TEXT(200)  NOT NULL,
    [Amount] INTEGER NOT NULL,
	
	PRIMARY KEY ([Id])
);

CREATE TABLE [ParcelAudit]
(
	[Id] TEXT(36) NOT NULL,
	[Parcel] TEXT(36) NOT NULL,
    [Date] TEXT(10) NOT NULL,
    [Transaction] TEXT(36) NOT NULL,
    [UnitCount] INTEGER NOT NULL,
    [CostBaseChange] INTEGER NOT NULL,
    [AmountChange] INTEGER NOT NULL,
	
	PRIMARY KEY ([Id])
);

CREATE TABLE [StockSettings]
(
	[Id] TEXT(36) NOT NULL,
    [FromDate] TEXT(10) NOT NULL,
	[ToDate] TEXT(10) NOT NULL,
	[ParticipateinDRP] TEXT(1) NOT NULL DEFAULT "N",
	
	PRIMARY KEY ([Id] ASC, [FromDate] ASC)
);

CREATE TABLE [DRPCashBalances]
(
	[Id] TEXT(36) NOT NULL,
	[FromDate] TEXT(10) NOT NULL,
	[ToDate] TEXT(10) NOT NULL,
	[Balance] INTEGER NOT NULL,
	
	PRIMARY KEY ([Id] ASC, [FromDate] ASC)
);