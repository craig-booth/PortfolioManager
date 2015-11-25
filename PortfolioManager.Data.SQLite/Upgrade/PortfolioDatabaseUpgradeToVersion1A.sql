CREATE TABLE [DbVersion]
(
	[Version] INTEGER NOT NULL,
	[CreationTime] TEXT(19) NOT NULL,
	[UpgradeTime] TEXT(19) NOT NULL
);
INSERT INTO [DbVersion] ([Version], [CreationTime], [UpgradeTime]) VALUES (0, "0001-01-01T00:00:00", "0001-01-01T00:00:00");


ALTER TABLE [Transactions] RENAME TO [TransactionsBackup];

CREATE TABLE [Transactions]
(
	[Id] TEXT(36) NOT NULL,
	[TransactionDate] TEXT(10) NOT NULL,
	[Sequence] INTEGER NOT NULL,
	[Type] INTEGER NOT NULL,
	[ASXCode] TEXT(6) NOT NULL,
	[Description] TEXT(200)  NOT NULL,
	
	PRIMARY KEY ([Id])
);