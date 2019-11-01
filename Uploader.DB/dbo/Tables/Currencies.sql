CREATE TABLE [dbo].[Currencies] (
    [Id]           TINYINT  IDENTITY (1, 1) NOT NULL,
    [CurrencyCode] CHAR (3) NOT NULL,
    CONSTRAINT [PK_Currencies] PRIMARY KEY CLUSTERED ([Id] ASC)
);

