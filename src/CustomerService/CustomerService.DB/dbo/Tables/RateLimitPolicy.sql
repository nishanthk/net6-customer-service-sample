CREATE TABLE [dbo].[RateLimitPolicy]
(
	[Id] INT NOT NULL IDENTITY(1,1),
    [Endpoint] NVARCHAR(100) NULL DEFAULT '*', 
    [Period] NVARCHAR(5) NULL DEFAULT '20s',
    [Limit] int NULL DEFAULT 100,
    [CreatedDateUTC] DATETIME2 NOT NULL, 
    [LastUpdatedDateUTC] DATETIME2 NOT NULL
    CONSTRAINT [PK_RateLimitPolicy] PRIMARY KEY ([Id])
)

GO

CREATE UNIQUE INDEX [IX_RateLimitPolicy_UniqueKey] ON [dbo].[RateLimitPolicy] ([Endpoint], [Period], [Limit])