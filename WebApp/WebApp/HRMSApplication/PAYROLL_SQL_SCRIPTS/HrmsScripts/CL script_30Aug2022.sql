--apply Casual leaves to S Shanker, Sub Watchmen, emp code 422   
--Date : 24.07.2022
--      
--Reason : Personal work

declare @Empid int;
declare @Leavetypeid int;
declare @StartDate Date;
declare @EndDate Date;
declare @UpdatedBy_EmployeeId int;
declare @Status varchar(10);
declare @LeaveDays int;
declare @TotalDays int;
declare @MaternityType varchar(10);
declare @pre_leave_balance int;


set @Empid=422;
set @Leavetypeid=1;
set @StartDate='2022-07-24';
set @EndDate='2022-07-24';
set @UpdatedBy_EmployeeId=123456;
set @Status='Pending';
set @LeaveDays=1;
set @TotalDays=1;
set @MaternityType='';
set @pre_leave_balance=(select LeaveBalance from EmpLeaveBalance where LeaveTypeId=@Leavetypeid and empid=
(select id from employees where empid=@Empid) and year=2022);


INSERT INTO [dbo].[Leaves]([EmpId],[ControllingAuthority],[SanctioningAuthority],[LeaveType],[StartDate],[EndDate],[Subject],[Reason],
[UpdatedBy],[UpdatedDate],[Status],[LeaveDays],[TotalDays],[LeaveTimeStamp],[LeavesYear],[Stage],[CancelReason],[MaternityType],
[BranchId],[DepartmentId],[DesignationId],[leave_balance]) 
VALUES((select id from employees where empid=@empid),
(select ControllingAuthority from employees where empid=@empid),
(select SanctioningAuthority from employees where empid=@empid),
@Leavetypeid,@StartDate,@EndDate,'Leave Applied','Personal work',
@UpdatedBy_EmployeeId,GETDATE(),@Status,@LeaveDays,@TotalDays,GETDATE(),2022,'','',
@MaternityType,(select Branch from employees where empid=@empid),
(select Department from employees where empid=@empid),
(select CurrentDesignation from employees where empid=@empid),@pre_leave_balance);


------- To update Leavebalances

declare @Empid int;
declare @LeaveTypeId int;
declare @LeaveBalance int;

set @Empid=422;
set @LeaveTypeId=1;
set @Leavebalance=(select LeaveBalance from EmpLeaveBalance where LeaveTypeId=@Leavetypeid and empid=
(select id from employees where empid=@Empid) and year=2022)-1;

update EmpLeaveBalance set LeaveBalance=@Leavebalance where LeaveTypeId=@LeaveTypeId and empid=
(select id from employees where empid=@Empid) and year=2022;

------------------------------------------------------------------------------------------------------------


--apply Casual leaves to S Shanker, Sub Watchmen, emp code 422   
--Date : 07.08.2022
--      
--Reason : Personal work

declare @Empid int;
declare @Leavetypeid int;
declare @StartDate Date;
declare @EndDate Date;
declare @UpdatedBy_EmployeeId int;
declare @Status varchar(10);
declare @LeaveDays int;
declare @TotalDays int;
declare @MaternityType varchar(10);
declare @pre_leave_balance int;


set @Empid=422;
set @Leavetypeid=1;
set @StartDate='2022-08-07';
set @EndDate='2022-08-07';
set @UpdatedBy_EmployeeId=123456;
set @Status='Pending';
set @LeaveDays=1;
set @TotalDays=1;
set @MaternityType='';
set @pre_leave_balance=(select LeaveBalance from EmpLeaveBalance where LeaveTypeId=@Leavetypeid and empid=
(select id from employees where empid=@Empid) and year=2022);


INSERT INTO [dbo].[Leaves]([EmpId],[ControllingAuthority],[SanctioningAuthority],[LeaveType],[StartDate],[EndDate],[Subject],[Reason],
[UpdatedBy],[UpdatedDate],[Status],[LeaveDays],[TotalDays],[LeaveTimeStamp],[LeavesYear],[Stage],[CancelReason],[MaternityType],
[BranchId],[DepartmentId],[DesignationId],[leave_balance]) 
VALUES((select id from employees where empid=@empid),
(select ControllingAuthority from employees where empid=@empid),
(select SanctioningAuthority from employees where empid=@empid),
@Leavetypeid,@StartDate,@EndDate,'Leave Applied','Personal work',
@UpdatedBy_EmployeeId,GETDATE(),@Status,@LeaveDays,@TotalDays,GETDATE(),2022,'','',
@MaternityType,(select Branch from employees where empid=@empid),
(select Department from employees where empid=@empid),
(select CurrentDesignation from employees where empid=@empid),@pre_leave_balance);


------- To update Leavebalances

declare @Empid int;
declare @LeaveTypeId int;
declare @LeaveBalance int;

set @Empid=422;
set @LeaveTypeId=1;
set @Leavebalance=(select LeaveBalance from EmpLeaveBalance where LeaveTypeId=@Leavetypeid and empid=
(select id from employees where empid=@Empid) and year=2022)-1;

update EmpLeaveBalance set LeaveBalance=@Leavebalance where LeaveTypeId=@LeaveTypeId and empid=
(select id from employees where empid=@Empid) and year=2022;

----------------------------------------------------------------------------------------------------------------------

--apply Casual leaves to S Shanker, Sub Watchmen, emp code 422   
--Date : 21.08.2022
--      
--Reason : Personal work

declare @Empid int;
declare @Leavetypeid int;
declare @StartDate Date;
declare @EndDate Date;
declare @UpdatedBy_EmployeeId int;
declare @Status varchar(10);
declare @LeaveDays int;
declare @TotalDays int;
declare @MaternityType varchar(10);
declare @pre_leave_balance int;


set @Empid=422;
set @Leavetypeid=1;
set @StartDate='2022-08-21';
set @EndDate='2022-08-21';
set @UpdatedBy_EmployeeId=123456;
set @Status='Pending';
set @LeaveDays=1;
set @TotalDays=1;
set @MaternityType='';
set @pre_leave_balance=(select LeaveBalance from EmpLeaveBalance where LeaveTypeId=@Leavetypeid and empid=
(select id from employees where empid=@Empid) and year=2022);


INSERT INTO [dbo].[Leaves]([EmpId],[ControllingAuthority],[SanctioningAuthority],[LeaveType],[StartDate],[EndDate],[Subject],[Reason],
[UpdatedBy],[UpdatedDate],[Status],[LeaveDays],[TotalDays],[LeaveTimeStamp],[LeavesYear],[Stage],[CancelReason],[MaternityType],
[BranchId],[DepartmentId],[DesignationId],[leave_balance]) 
VALUES((select id from employees where empid=@empid),
(select ControllingAuthority from employees where empid=@empid),
(select SanctioningAuthority from employees where empid=@empid),
@Leavetypeid,@StartDate,@EndDate,'Leave Applied','Personal work',
@UpdatedBy_EmployeeId,GETDATE(),@Status,@LeaveDays,@TotalDays,GETDATE(),2022,'','',
@MaternityType,(select Branch from employees where empid=@empid),
(select Department from employees where empid=@empid),
(select CurrentDesignation from employees where empid=@empid),@pre_leave_balance);


------- To update Leavebalances

declare @Empid int;
declare @LeaveTypeId int;
declare @LeaveBalance int;

set @Empid=422;
set @LeaveTypeId=1;
set @Leavebalance=(select LeaveBalance from EmpLeaveBalance where LeaveTypeId=@Leavetypeid and empid=
(select id from employees where empid=@Empid) and year=2022)-1;

update EmpLeaveBalance set LeaveBalance=@Leavebalance where LeaveTypeId=@LeaveTypeId and empid=
(select id from employees where empid=@Empid) and year=2022;
