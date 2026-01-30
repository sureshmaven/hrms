--Apply C-Off  to Sri M Srinivas , Sub-staff , Code 308  
--Date : 15-10-2021 to 15-10-2021
--      
--Reason : Domestic work

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


set @Empid=308;
set @Leavetypeid=13;
set @StartDate='2021-10-15';
set @EndDate='2021-10-15';
set @UpdatedBy_EmployeeId=123456;
set @Status='Pending';
set @LeaveDays=1;
set @TotalDays=1;
set @MaternityType='';
set @pre_leave_balance=(select LeaveBalance from EmpLeaveBalance where LeaveTypeId=@Leavetypeid and empid=
(select id from employees where empid=@Empid) and year=2021);


INSERT INTO [dbo].[Leaves]([EmpId],[ControllingAuthority],[SanctioningAuthority],[LeaveType],[StartDate],[EndDate],[Subject],[Reason],
[UpdatedBy],[UpdatedDate],[Status],[LeaveDays],[TotalDays],[LeaveTimeStamp],[LeavesYear],[Stage],[CancelReason],[MaternityType],
[BranchId],[DepartmentId],[DesignationId],[leave_balance]) 
VALUES((select id from employees where empid=@empid),
(select ControllingAuthority from employees where empid=@empid),
(select SanctioningAuthority from employees where empid=@empid),
@Leavetypeid,@StartDate,@EndDate,'Leave Applied','Domestic work',
@UpdatedBy_EmployeeId,GETDATE(),@Status,@LeaveDays,@TotalDays,GETDATE(),2021,'','',
@MaternityType,(select Branch from employees where empid=@empid),
(select Department from employees where empid=@empid),
(select CurrentDesignation from employees where empid=@empid),@pre_leave_balance);


------- To update Leavebalances

declare @Empid int;
declare @LeaveTypeId int;
declare @LeaveBalance int;

set @Empid=308;
set @LeaveTypeId=13;
set @Leavebalance=(select LeaveBalance from EmpLeaveBalance where LeaveTypeId=@Leavetypeid and empid=
(select id from employees where empid=@Empid) and year=2021)-1;

update EmpLeaveBalance set LeaveBalance=@Leavebalance where LeaveTypeId=@LeaveTypeId and empid=
(select id from employees where empid=@Empid) and year=2021;

---------------------------------------------------------------------------------------------------------------------------------------------


--Apply C-Off  to Sri M Srinivas , Sub-staff , Code 308  
--Date : 19-10-2021 to 19-10-2021
--      
--Reason : Domestic work

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


set @Empid=308;
set @Leavetypeid=13;
set @StartDate='2021-10-19';
set @EndDate='2021-10-19';
set @UpdatedBy_EmployeeId=123456;
set @Status='Pending';
set @LeaveDays=1;
set @TotalDays=1;
set @MaternityType='';
set @pre_leave_balance=(select LeaveBalance from EmpLeaveBalance where LeaveTypeId=@Leavetypeid and empid=
(select id from employees where empid=@Empid) and year=2021);


INSERT INTO [dbo].[Leaves]([EmpId],[ControllingAuthority],[SanctioningAuthority],[LeaveType],[StartDate],[EndDate],[Subject],[Reason],
[UpdatedBy],[UpdatedDate],[Status],[LeaveDays],[TotalDays],[LeaveTimeStamp],[LeavesYear],[Stage],[CancelReason],[MaternityType],
[BranchId],[DepartmentId],[DesignationId],[leave_balance]) 
VALUES((select id from employees where empid=@empid),
(select ControllingAuthority from employees where empid=@empid),
(select SanctioningAuthority from employees where empid=@empid),
@Leavetypeid,@StartDate,@EndDate,'Leave Applied','Domestic work',
@UpdatedBy_EmployeeId,GETDATE(),@Status,@LeaveDays,@TotalDays,GETDATE(),2021,'','',
@MaternityType,(select Branch from employees where empid=@empid),
(select Department from employees where empid=@empid),
(select CurrentDesignation from employees where empid=@empid),@pre_leave_balance);


------- To update Leavebalances

declare @Empid int;
declare @LeaveTypeId int;
declare @LeaveBalance int;

set @Empid=308;
set @LeaveTypeId=13;
set @Leavebalance=(select LeaveBalance from EmpLeaveBalance where LeaveTypeId=@Leavetypeid and empid=
(select id from employees where empid=@Empid) and year=2021)-1;

update EmpLeaveBalance set LeaveBalance=@Leavebalance where LeaveTypeId=@LeaveTypeId and empid=
(select id from employees where empid=@Empid) and year=2021;

