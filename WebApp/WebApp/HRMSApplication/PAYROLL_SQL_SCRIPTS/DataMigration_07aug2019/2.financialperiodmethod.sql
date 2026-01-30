create function dbo.getFYPeriod(@pmonth varchar(20))
RETURNS varchar(10) 
AS   
BEGIN  
    DECLARE @fyStart varchar(10);
	DECLARE @fyGiven varchar(10);
	declare @fy varchar(10);
	declare @fyLast varchar(10);
    set @fyStart = left(@pmonth,4)+'-03-01';
	set @fyGiven = left(@pmonth,4)+'-'+right(@pmonth,2)+'01';
	if(convert(varchar, @fyGiven, 23)>=convert(varchar, @fyStart, 23))
	begin
	set @fyLast = cast(left(@pmonth,4)+1 as varchar);
	set @fy = cast(left(@pmonth,4) as varchar)+'-'+cast(right(@fyLast,2) as varchar);
	end
	else
	begin
	set @fyLast=cast(left(@pmonth,4) as int);
	set @fy = cast(left(@pmonth,4)-1 as varchar)+'-'+cast(right(@fyLast,2) as varchar);
	end
    RETURN @fy;  
END; 

--select dbo.getFYPeriod ('201904');//function calling
