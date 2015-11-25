DROP TABLE [TransactionsBackup];

CREATE INDEX [Index_Transactions_ByDate] ON [Transactions]
(
		[TransactionDate] ASC,
		[Sequence] ASC
);

CREATE UNIQUE INDEX [Index_Transactions_ByASXCode] ON [Transactions]
(
        [ASXCode]  ASC,
		[TransactionDate] ASC,
		[Sequence] ASC
);