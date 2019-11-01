-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE dbo.GetByDate
	@From varchar(10),
    @Till varchar(10)
AS
BEGIN
	Select [Tid]
      ,[Amount]
      ,c.CurrencyCode
      ,s.Val
       FROM [dbo].[Transactions] t
       join Currencies c on c.Id = t.CurrencyCode
       join Statuses s on s.Id = t.StatusId
       where cast(t.TDate as date) between @From and @Till
END
