-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetByStatus]
	@Status int
AS
BEGIN
	Select [Tid]
      ,[Amount]
      ,c.CurrencyCode
      ,s.Val
       FROM [dbo].[Transactions] t
       join Currencies c on c.Id = t.CurrencyCode
       join Statuses s on s.Id = t.StatusId
       where t.StatusId=@Status
END
