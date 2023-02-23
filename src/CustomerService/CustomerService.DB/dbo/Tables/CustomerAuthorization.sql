CREATE TABLE [dbo].[CustomerAuthorization]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[ClientId] VARCHAR(512) NOT NULL,
	[UserId] VARCHAR(512) NOT NULL,
	[IsActive] BIT NOT NULL, 
    [CreatedDateUTC] DATETIME2 NOT NULL, 
    [LastUpdatedDateUTC] DATETIME2 NOT NULL
    CONSTRAINT [PK_CustomerAuthorization] PRIMARY KEY ([Id])
)
GO

CREATE UNIQUE INDEX [IX_CustomerAuthorization_ClientId] ON [dbo].[CustomerAuthorization] ([ClientId])
GO
