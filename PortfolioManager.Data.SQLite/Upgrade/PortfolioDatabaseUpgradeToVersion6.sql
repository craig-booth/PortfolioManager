
ALTER TABLE [Aquisitions]
	ADD COLUMN [CreateCashTransaction] TEXT(1) NOT NULL DEFAULT "N";
	

ALTER TABLE [Disposals]
	ADD COLUMN [CreateCashTransaction] TEXT(1) NOT NULL DEFAULT "N";


ALTER TABLE [IncomeReceived]
	ADD COLUMN [CreateCashTransaction] TEXT(1) NOT NULL DEFAULT "N";


ALTER TABLE [ReturnsOfCapital]
	ADD COLUMN [CreateCashTransaction] TEXT(1) NOT NULL DEFAULT "N";