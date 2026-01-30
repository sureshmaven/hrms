create function dbo.getLeaveBalance(@empCode varchar(20))
RETURNS varchar(200) 
AS   
BEGIN  
    
	declare @balance varchar(200);
    declare @CL varchar(10);
	declare @ML varchar(10);
	declare @PL varchar(10);
	declare @MTL varchar(10);
	declare @PTL varchar(10);
	declare @EOL varchar(10);
	declare @SCL varchar(10);
	declare @Coff varchar(10);
	declare @LOP varchar(10);

	select @CL = CasualLeave, @ML = MedicalSickLeave,
	@PL= PrivilegeLeave,
	@MTL=MaternityLeave,
	@PTL =PaternityLeave,
	@EOL =ExtraordinaryLeave,
	@SCL = SpecialCasualLeave,
	@Coff = CompensatoryOff,
	@LOP = LOP FROM V_EmpLeaveBalance WHERE empid=(select id from Employees where EmpId=@empCode);
	

	set @balance='CL#'+@CL+',ML#'+@ML+',PL#'+@PL+',MTL#'+@MTL+',PTL#'+@PTL+',EOL#'+@EOL+',SCL#'+@SCL+',C-OFF#'+@Coff+',LOP#'+@LOP;
    RETURN @balance;  
END; 

--select dbo.leaveBalance(175);