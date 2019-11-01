CREATE TABLE [dbo].[Statuses] (
    [Id]      TINYINT      IDENTITY (1, 1) NOT NULL,
    [Val]     CHAR (1)     NOT NULL,
    [CsvType] VARCHAR (10) NOT NULL,
    [XmlType] VARCHAR (10) NOT NULL,
    CONSTRAINT [PK_Statuses] PRIMARY KEY CLUSTERED ([Id] ASC)
);

