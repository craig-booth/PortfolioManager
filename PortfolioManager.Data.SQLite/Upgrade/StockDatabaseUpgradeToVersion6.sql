

ALTER TABLE [StockPrices]
	ADD COLUMN [Current] INTEGER NOT NULL DEFAULT 0;


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