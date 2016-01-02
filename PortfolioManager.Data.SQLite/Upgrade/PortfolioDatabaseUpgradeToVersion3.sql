
ALTER TABLE [OpeningBalances] RENAME TO [OpeningBalancesBackup];

CREATE TABLE [OpeningBalances]
(
	[Id] TEXT(36) NOT NULL,
	[Units] INTEGER NOT NULL,
	[CostBase] INTEGER NOT NULL,
	[AquisitionDate] TEXT(10) NOT NULL,
	[Comment] TEXT(200) NOT NULL,

	PRIMARY KEY ([Id])
);

INSERT INTO [OpeningBalances] ([Id], [Units], [CostBase], [AquisitionDate], [Comment]) 
   SELECT [OpeningBalancesBackup].[Id], [Units], [CostBase], [TransactionDate], [Comment] FROM [OpeningBalancesBackup]
   LEFT OUTER JOIN [Transactions] ON [Transactions].[Id] = [OpeningBalancesBackup].[Id];

DROP TABLE [OpeningBalancesBackup];