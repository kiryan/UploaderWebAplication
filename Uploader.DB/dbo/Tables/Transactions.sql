CREATE TABLE [dbo].[Transactions] (
    [Id]           INT             IDENTITY (1, 1) NOT NULL,
    [Tid]          VARCHAR (50)    NOT NULL,
    [Amount]       DECIMAL (18, 2) NOT NULL,
    [CurrencyCode] TINYINT         NOT NULL,
    [TDate]        DATETIME2 (0)   NOT NULL,
    [StatusId]     TINYINT         NOT NULL,
    CONSTRAINT [PK_Transactions] PRIMARY KEY CLUSTERED ([Id] ASC)
);

