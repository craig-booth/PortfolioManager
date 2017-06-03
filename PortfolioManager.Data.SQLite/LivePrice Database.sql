CREATE TABLE [DbVersion]
(
	[Version] INTEGER NOT NULL,
	[CreationTime] TEXT(19) NOT NULL,
	[UpgradeTime] TEXT(19) NOT NULL
);

CREATE TABLE [LivePrices]
(
	[Stock] TEXT(36) NOT NULL,
	[Price] INTEGER NOT NULL,

	PRIMARY KEY ([Stock] ASC)
);
