CREATE TABLE [dbo].[ClientRateLimitPolicyRule]
(
	[Id] INT NOT NULL IDENTITY(1,1),
    [CustomerAuthorizationId] UNIQUEIDENTIFIER NOT NULL, 
    [RateLimitPolicyId] INT NOT NULL,
    [CreatedDateUTC] DATETIME2 NOT NULL, 
    [LastUpdatedDateUTC] DATETIME2 NOT NULL
    CONSTRAINT [PK_ClientRateLimitPolicyRule] PRIMARY KEY ([Id])
)

GO

CREATE INDEX [IX_ClientRateLimitPolicyRule_CustomerAuthorizationId] ON [dbo].[ClientRateLimitPolicyRule] ([CustomerAuthorizationId])

GO

CREATE INDEX [IX_ClientRateLimitPolicyRule_RateLimitPolicyId] ON [dbo].[ClientRateLimitPolicyRule] ([RateLimitPolicyId])

GO

CREATE UNIQUE INDEX [IX_ClientRateLimitPolicyRule_UniqueKey] ON [dbo].[ClientRateLimitPolicyRule] ([CustomerAuthorizationId], [RateLimitPolicyId])

