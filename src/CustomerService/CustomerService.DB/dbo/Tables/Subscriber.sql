CREATE TABLE [dbo].[Subscriber]
(
	[Id] INT NOT NULL IDENTITY(1,1),
	[SubscriptionId] UNIQUEIDENTIFIER NOT NULL,
	[CustomerCode] NVARCHAR(12) NOT NULL,
	[UrlAddress] VARCHAR(512) NOT NULL, 
	[IsActive] BIT NOT NULL, 
    [CreatedDateUTC] DATETIME2 NOT NULL, 
    [LastUpdatedDateUTC] DATETIME2 NOT NULL
    CONSTRAINT [PK_Subscriber] PRIMARY KEY ([Id])
)

GO

CREATE UNIQUE INDEX [IX_Subscriber_SubscriptionId] ON [dbo].[Subscriber] ([SubscriptionId])

GO

CREATE INDEX [IX_Subscriber_CustomerCode] ON [dbo].[Subscriber] ([CustomerCode])
