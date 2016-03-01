

ALTER TABLE [Transformations]
	ADD COLUMN [RolloverRelief] TEXT(1) NOT NULL DEFAULT "Y";

ALTER TABLE [TransformationResultingStocks]
	ADD COLUMN [AquisitionDate] CHAR(10) NOT NULL DEFAULT "0001-01-01";

