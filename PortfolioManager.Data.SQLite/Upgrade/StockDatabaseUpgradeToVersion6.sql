

ALTER TABLE [StockPrices]
	ADD COLUMN [Current] TEXT(1) NOT NULL DEFAULT "N",


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