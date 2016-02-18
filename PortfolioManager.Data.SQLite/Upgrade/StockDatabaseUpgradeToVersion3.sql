CREATE TABLE [CompositeActions]
(
	[Id] TEXT(36) NOT NULL,
	[Sequence] INTEGER NOT NULL,
	[ChildAction] TEXT(36) NOT NULL,

	PRIMARY KEY ([Id], [Sequence])
);