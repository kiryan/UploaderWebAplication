-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetStatus]
	@StatusText varchar(10),
    @FileType char(3)
AS
BEGIN
    if(@FileType = 'csv')
	    Select Id from Statuses 
        where upper(CsvType) = @StatusText
    if(@FileType = 'xml')
	    Select Id from Statuses 
        where upper(XmlType) = @StatusText
END
