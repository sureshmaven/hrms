CREATE  PROC [dbo].[get_new_num] (@table_name NVARCHAR(50))
AS
	DECLARE @last_num INT
	SELECT @last_num=last_num FROM new_num WHERE table_name=@table_name
	SET @last_num = @last_num+1
	UPDATE new_num SET last_num=@last_num WHERE table_name=@table_name
	PRINT @last_num	

GO