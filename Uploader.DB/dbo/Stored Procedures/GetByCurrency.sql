-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetByCurrency]
	@CurrencyId int
AS
BEGIN
	Select [Tid]
      ,[Amount]
      ,c.CurrencyCode
      ,s.Val
       FROM [dbo].[Transactions] t
       join Currencies c on c.Id = t.CurrencyCode
       join Statuses s on s.Id = t.StatusId
       where t.CurrencyCode=@CurrencyId
END
