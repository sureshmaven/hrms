create function dbo.getFY(@pmonth varchar(20))
RETURNS int 
AS   
BEGIN  
    DECLARE @fyStart varchar(10);
	DECLARE @fyGiven varchar(10);
	declare @fy int;
    set @fyStart = left(@pmonth,4)+'-03-01';
	set @fyGiven = left(@pmonth,4)+'-'+right(@pmonth,2)+'01';
	if(convert(varchar, @fyGiven, 23)>=convert(varchar, @fyStart, 23))
	set @fy=cast(left(@pmonth,4)+1 as int);
	else
	set @fy=cast(left(@pmonth,4) as int);
	
    RETURN @fy;  
END; 

--select dbo.getFY ('201903');//function calling

