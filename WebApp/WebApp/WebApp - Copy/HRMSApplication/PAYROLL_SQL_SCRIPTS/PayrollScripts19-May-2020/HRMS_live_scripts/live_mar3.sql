------- To update Leavebalances

declare @Empid int;
declare @LeaveTypeId int;
declare @LeaveBalance int;

set @Empid=945;
set @LeaveTypeId=4;
set @Leavebalance=184;

update EmpLeaveBalance set LeaveBalance=@Leavebalance where LeaveTypeId=@LeaveTypeId and empid=
(select id from employees where empid=@Empid) and year=2020;


---- For Leaves Credit/Debit

declare @Empid int;
declare @Leavetypeid int;
declare @Numberofleaves_to_Credit int;
declare @Numberofleaves_to_Debit int;
declare @UpdatedBy_EmployeeId int;
declare @Previous_LeaveBalance int;
declare @Comments varchar(20);
declare @Type varchar(20);
declare @TotalBalance int;

set @Empid=945;
set @Leavetypeid=4;
set @Numberofleaves_to_Credit=184;
set @Numberofleaves_to_Debit=0;
set @UpdatedBy_EmployeeId=123456;
set @Previous_LeaveBalance=0;
set @Comments='Leaves Credited';
set @Type='Credit'
set @TotalBalance=184;



INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=@Empid),
@Leavetypeid,@Numberofleaves_to_Credit,
@Numberofleaves_to_Debit,@UpdatedBy_EmployeeId,
GETDATE(),@Previous_LeaveBalance,@Comments,
(select ShortName from Employees where empid=@empid),
(select Department from Employees where empid=@empid),
(select Branch from Employees where empid=@empid),
(select CurrentDesignation from Employees where empid=@empid),@Type,
(select branch_value1 from employees where empid=@empid),@TotalBalance,2020)



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


set @Empid=945;
set @Leavetypeid=4;
set @StartDate='2020-03-24';
set @EndDate='2020-09-23';
set @UpdatedBy_EmployeeId=123456;
set @Status='Pending';
set @LeaveDays=184;
set @TotalDays=184;
set @MaternityType='';
set @pre_leave_balance=0;


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

set @Empid=945;
set @LeaveTypeId=4;
set @Leavebalance=0;

update EmpLeaveBalance set LeaveBalance=@Leavebalance where LeaveTypeId=@LeaveTypeId and empid=
(select id from employees where empid=@Empid) and year=2020;

