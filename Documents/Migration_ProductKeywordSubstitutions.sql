USE [PurchaseHistory]
GO

CREATE TABLE [dbo].[ProductKeywordSubstitutions](
    [Id] [uniqueidentifier] NOT NULL,
    [OriginalTerm] [nvarchar](200) NOT NULL,
    [ReplacementTerm] [nvarchar](200) NOT NULL,
    [MatchType] [nvarchar](20) NOT NULL DEFAULT 'WholeWord',
    [IsActive] [bit] NOT NULL DEFAULT 1,
    [CreatedAt] [datetime2](7) NOT NULL,
    [UpdatedAt] [datetime2](7) NULL,
PRIMARY KEY CLUSTERED
(
    [Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ProductKeywordSubstitutions] ADD DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [dbo].[ProductKeywordSubstitutions] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
