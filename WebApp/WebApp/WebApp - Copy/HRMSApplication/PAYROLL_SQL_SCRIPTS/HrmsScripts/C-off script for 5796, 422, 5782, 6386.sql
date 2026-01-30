--Credit leaves
INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
 values(	(select id from Employees where empid=5796),13,14,0,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where empid=(select id from employees where empid=5796) and LeaveTypeId=13 and year=2022),
' C Offs credited',---Reason
(select ShortName from Employees where empid=5796),
(select Department from Employees where empid=5796),
(select Branch from Employees where empid=5796),
(select CurrentDesignation from Employees where empid=5796),'Credit',
(select branch_value1 from employees where empid=5796),
(14	+(select LeaveBalance from EmpLeaveBalance where empid=(select id from employees where empid=5796) and LeaveTypeId=13 and year=2022)),2022);


--Update leave balance
update EmpLeaveBalance set LeaveBalance=
(14+(select LeaveBalance from EmpLeaveBalance where empid=(select id from employees where empid=5796 and LeaveTypeId=13) and year=2022)) 
where LeaveTypeId=13 and empid=(select id from employees where empid=5796) and year=2022;




--Credit leaves
INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
 values(	(select id from Employees where empid=422),13,12,0,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where empid=(select id from employees where empid=422) and LeaveTypeId=13 and year=2022),
'C offs credited',---Reason
(select ShortName from Employees where empid=422),
(select Department from Employees where empid=422),
(select Branch from Employees where empid=422),
(select CurrentDesignation from Employees where empid=422),'Credit',
(select branch_value1 from employees where empid=422),
(12	+(select LeaveBalance from EmpLeaveBalance where empid=(select id from employees where empid=422) and LeaveTypeId=13 and year=2022)),2022);


--Update leave balance
update EmpLeaveBalance set LeaveBalance=
(12+(select LeaveBalance from EmpLeaveBalance where empid=(select id from employees where empid=422 and LeaveTypeId=13) and year=2022)) 
where LeaveTypeId=13 and empid=(select id from employees where empid=422) and year=2022;






--Credit leaves
INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
 values(	(select id from Employees where empid=5782),13,16,0,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where empid=(select id from employees where empid=5782) and LeaveTypeId=13 and year=2022),
'C offs credited',---Reason
(select ShortName from Employees where empid=5782),
(select Department from Employees where empid=5782),
(select Branch from Employees where empid=5782),
(select CurrentDesignation from Employees where empid=5782),'Credit',
(select branch_value1 from employees where empid=5782),
(16	+(select LeaveBalance from EmpLeaveBalance where empid=(select id from employees where empid=5782) and LeaveTypeId=13 and year=2022)),2022);


--Update leave balance
update EmpLeaveBalance set LeaveBalance=
(16+(select LeaveBalance from EmpLeaveBalance where empid=(select id from employees where empid=5782 and LeaveTypeId=13) and year=2022)) 
where LeaveTypeId=13 and empid=(select id from employees where empid=5782) and year=2022;




--Credit leaves
INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
 values(	(select id from Employees where empid=6386),13,8,0,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where empid=(select id from employees where empid=6386) and LeaveTypeId=13 and year=2022),
'C offs credited',---Reason
(select ShortName from Employees where empid=6386),
(select Department from Employees where empid=6386),
(select Branch from Employees where empid=6386),
(select CurrentDesignation from Employees where empid=6386),'Credit',
(select branch_value1 from employees where empid=6386),
(8	+(select LeaveBalance from EmpLeaveBalance where empid=(select id from employees where empid=6386) and LeaveTypeId=13 and year=2022)),2022);


--Update leave balance
update EmpLeaveBalance set LeaveBalance=
(8+(select LeaveBalance from EmpLeaveBalance where empid=(select id from employees where empid=6386 and LeaveTypeId=13) and year=2022)) 
where LeaveTypeId=13 and empid=(select id from employees where empid=6386) and year=2022;
