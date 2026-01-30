--Apply CL for 6421 on 26-02-2020

------------To Apply Leave


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


set @Empid=6421;
set @Leavetypeid=1;
set @StartDate='2020-02-26';
set @EndDate='2020-02-26';
set @UpdatedBy_EmployeeId=123456;
set @Status='Pending';
set @LeaveDays=1;
set @TotalDays=1;
set @MaternityType='';
set @pre_leave_balance=(select LeaveBalance from EmpLeaveBalance where LeaveTypeId=@Leavetypeid and empid=
(select id from employees where empid=@Empid) and year=2020);


INSERT INTO [dbo].[Leaves]([EmpId],[ControllingAuthority],[SanctioningAuthority],[LeaveType],[StartDate],[EndDate],[Subject],[Reason],
[UpdatedBy],[UpdatedDate],[Status],[LeaveDays],[TotalDays],[LeaveTimeStamp],[LeavesYear],[Stage],[CancelReason],[MaternityType],
[BranchId],[DepartmentId],[DesignationId],[leave_balance]) 
VALUES((select id from employees where empid=@empid),
(select ControllingAuthority from employees where empid=@empid),
(select SanctioningAuthority from employees where empid=@empid),
@Leavetypeid,@StartDate,@EndDate,'Leave Applied','Leave Applied Manually',
@UpdatedBy_EmployeeId,GETDATE(),@Status,@LeaveDays,@TotalDays,GETDATE(),2020,'','',
@MaternityType,(select Branch from employees where empid=@empid),
(select Department from employees where empid=@empid),
(select CurrentDesignation from employees where empid=@empid),@pre_leave_balance);


------- To update Leavebalances

declare @Empid int;
declare @LeaveTypeId int;
declare @LeaveBalance int;

set @Empid=6421;
set @LeaveTypeId=1;
set @Leavebalance=2;

update EmpLeaveBalance set LeaveBalance=@Leavebalance where LeaveTypeId=@LeaveTypeId and empid=
(select id from employees where empid=@Empid) and year=2020;

