

ALTER TABLE [OpeningBalances]
	ADD COLUMN [AquisitionDate] TEXT(10) NOT NULL DEFAULT "";

UPDATE [OpeningBalances]
	SET [AquisitionDate] = (SELECT [TransactionDate] FROM [Transactions] WHERE [Transactions].[Id] = [OpeningBalances].[Id]);