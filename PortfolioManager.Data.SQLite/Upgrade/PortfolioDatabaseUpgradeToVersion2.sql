
CREATE TABLE [UnitCountAdjustments]
(
	[Id] TEXT(36) NOT NULL,
	[OriginalUnits] INTEGER NOT NULL,
	[NewUnits] INTEGER NOT NULL,
	[Comment] TEXT(200)  NOT NULL,
	
	PRIMARY KEY ([Id])
);