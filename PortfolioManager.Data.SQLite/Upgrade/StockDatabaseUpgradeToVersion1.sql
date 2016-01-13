CREATE TABLE [DbVersion]
(
	[Version] INTEGER NOT NULL,
	[CreationTime] TEXT(19) NOT NULL,
	[UpgradeTime] TEXT(19) NOT NULL
);
INSERT INTO [DbVersion] ([Version], [CreationTime], [UpgradeTime]) VALUES (0, "0001-01-01T00:00:00", "0001-01-01T00:00:00");


CREATE TABLE [SplitConsolidations]
(
	[Id] TEXT(36) NOT NULL,
	[OldUnits] INTEGER NOT NULL,
	[NewUnits] INTEGER NOT NULL,

	PRIMARY KEY ([Id])
);