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
	[Sequence] INTEGER PRIMARY KEY AUTOINCREMENT,
	[Type] INTEGER NOT NULL,
	[ASXCode] TEXT(6) NOT NULL,
	[Description] TEXT(200)  NOT NULL
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

INSERT INTO [Transactions] ([Id], [TransactionDate], [Type], [ASXCode], [Description]) 
   SELECT [Id], [TransactionDate], [Type], [ASXCode], [Description] FROM [TransactionsBackup]; 

DROP TABLE [TransactionsBackup];