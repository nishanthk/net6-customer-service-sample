CREATE TABLE [dbo].[SubscriptionFilter]
(
	[Id] INT NOT NULL IDENTITY(1,1),
	[SubscriberId] INT NOT NULL,
	[Name] NVARCHAR(50) NULL,
	[Rules] VARCHAR(Max) NOT NULL, 
	[IsActive] BIT NOT NULL, 
    [CreatedDateUTC] DATETIME2 NOT NULL, 
    [LastUpdatedDateUTC] DATETIME2 NOT NULL
    CONSTRAINT [PK_SubscriptionFilter] PRIMARY KEY ([Id]),
	[FilterSource] INT NOT NULL DEFAULT 1, 
    CONSTRAINT [FK_SubscriptionFilter_SubscriptionId] FOREIGN KEY (SubscriberId) REFERENCES Subscriber(Id)
)
GO

CREATE INDEX [IX_SubscriberFilter_SubscriptionId] ON [dbo].[SubscriptionFilter] ([SubscriberId])
GO
