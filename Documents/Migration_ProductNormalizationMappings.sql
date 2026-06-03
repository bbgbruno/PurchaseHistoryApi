USE [PurchaseHistory]
GO

CREATE TABLE [dbo].[ProductNormalizationMappings](
    [Id] [uniqueidentifier] NOT NULL,
    [OriginalText] [nvarchar](500) NOT NULL,
    [ReplacementText] [nvarchar](500) NOT NULL,
    [MatchType] [nvarchar](20) NOT NULL DEFAULT 'Exact',
    [IsActive] [bit] NOT NULL DEFAULT 1,
    [CreatedAt] [datetime2](7) NOT NULL,
    [UpdatedAt] [datetime2](7) NULL,
PRIMARY KEY CLUSTERED
(
    [Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ProductNormalizationMappings] ADD DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[ProductNormalizationMappings] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
