-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetCurrencies] 
	
AS
BEGIN
	Select Id, CurrencyCode from Currencies
END
