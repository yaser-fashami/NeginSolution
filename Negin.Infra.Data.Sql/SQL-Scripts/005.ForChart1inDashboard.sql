CREATE OR ALTER FUNCTION [dbo].[PersianToMiladi]
(
    @PersianDate VARCHAR(10)
)
RETURNS DATE
AS
BEGIN
    SET @PersianDate = RIGHT (@PersianDate, 9)
    DECLARE @Year INT = SUBSTRING(@PersianDate, 1, 3)
    DECLARE @Month INT = SUBSTRING(@PersianDate, 5, 2)
    DECLARE @Day INT = SUBSTRING(@PersianDate, 8, 2)
    DECLARE @DiffYear INT = @Year - 350

    DECLARE @Days INT = @DiffYear * 365.24 +
    CASE WHEN @Month < 7 THEN (@Month - 1) * 31
         ELSE 186 + (@Month - 7) * 30 END + @Day

    DECLARE @StartDate DATETIME = '03/21/1971'
    DECLARE @ResultDate DATE = @StartDate + @Days

    RETURN CONVERT(DATE, @ResultDate)  

END
GO

CREATE OR ALTER PROCEDURE  dbo.GetSumPriceInvoices @start DATE, @end DATE
AS
BEGIN
	SELECT SUM(SumPriceR) as SumPriceRial, SUM(SumPriceD) as SumPriceDollar from Billing.Invoices WHERE InvoiceDate >= @start AND InvoiceDate <= @end
END
GO

CREATE OR ALTER PROC dbo.GetDataForDashboardChart1 @year char(4)
AS
BEGIN
	DECLARE @result TABLE 
	(
		SumPriceR bigint,
		SumPriceD bigint
	)

	declare @start DATE = dbo.PersianToMiladi(@year+'/01/01')
	declare @end DATE = dbo.PersianToMiladi(@year+'/01/31')
	INSERT INTO @result 
	EXEC dbo.GetSumPriceInvoices @start, @end

	set @start = dbo.PersianToMiladi(@year+'/02/01')
	set @end =  dbo.PersianToMiladi(@year+'/02/31')
	INSERT INTO @result 
	EXEC dbo.GetSumPriceInvoices @start, @end

	set @start = dbo.PersianToMiladi(@year+'/03/01')
	set @end =  dbo.PersianToMiladi(@year+'/03/31')
	INSERT INTO @result 
	EXEC dbo.GetSumPriceInvoices @start, @end

	set @start = dbo.PersianToMiladi(@year+'/04/01')
	set @end =  dbo.PersianToMiladi(@year+'/04/31')
	INSERT INTO @result 
	EXEC dbo.GetSumPriceInvoices @start, @end

	set @start = dbo.PersianToMiladi(@year+'/05/01')
	set @end =  dbo.PersianToMiladi(@year+'/05/31')
	INSERT INTO @result 
	EXEC dbo.GetSumPriceInvoices @start, @end

	set @start = dbo.PersianToMiladi(@year+'/06/01')
	set @end =  dbo.PersianToMiladi(@year+'/06/31')
	INSERT INTO @result 
	EXEC dbo.GetSumPriceInvoices @start, @end

	set @start = dbo.PersianToMiladi(@year+'/07/01')
	set @end =  dbo.PersianToMiladi(@year+'/07/30')
	INSERT INTO @result 
	EXEC dbo.GetSumPriceInvoices @start, @end

	set @start = dbo.PersianToMiladi(@year+'/08/01')
	set @end =  dbo.PersianToMiladi(@year+'/08/30')
	INSERT INTO @result 
	EXEC dbo.GetSumPriceInvoices @start, @end

	set @start = dbo.PersianToMiladi(@year+'/09/01')
	set @end =  dbo.PersianToMiladi(@year+'/09/30')
	INSERT INTO @result 
	EXEC dbo.GetSumPriceInvoices @start, @end

	set @start = dbo.PersianToMiladi(@year+'/10/01')
	set @end =  dbo.PersianToMiladi(@year+'/10/30')
	INSERT INTO @result 
	EXEC dbo.GetSumPriceInvoices @start, @end

	set @start = dbo.PersianToMiladi(@year+'/11/01')
	set @end =  dbo.PersianToMiladi(@year+'/11/30')
	INSERT INTO @result 
	EXEC dbo.GetSumPriceInvoices @start, @end

	set @start = dbo.PersianToMiladi(@year+'/12/01')
	set @end =  dbo.PersianToMiladi(@year+'/12/30')
	INSERT INTO @result 
	EXEC dbo.GetSumPriceInvoices @start, @end

	select * from @result
END