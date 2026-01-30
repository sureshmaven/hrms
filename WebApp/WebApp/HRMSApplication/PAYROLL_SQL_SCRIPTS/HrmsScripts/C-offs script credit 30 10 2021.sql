--Credit leaves
INSERT INTO [dbo].[Leaves_CreditDebit] values(	(select id from Employees where empid=6386),13,16,0,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where empid=(select id from employees where empid=6386) and LeaveTypeId=13 and year=2021),
'C offs credited for period April-July 21',---Reason
(select ShortName from Employees where empid=6386),
(select Department from Employees where empid=6386),
(select Branch from Employees where empid=6386),
(select CurrentDesignation from Employees where empid=6386),'Credit',
(select branch_value1 from employees where empid=6386),
(16	+(select LeaveBalance from EmpLeaveBalance where empid=(select id from employees where empid=6386) and LeaveTypeId=13 and year=2021)),2021);


--Update leave balance
update EmpLeaveBalance set LeaveBalance=
(16+(select LeaveBalance from EmpLeaveBalance where empid=(select id from employees where empid=6386 and LeaveTypeId=13) and year=2021)) 
where LeaveTypeId=13 and empid=(select id from employees where empid=6386) and year=2021;



--Credit leaves
INSERT INTO [dbo].[Leaves_CreditDebit] values(	(select id from Employees where empid=5782),13,5,0,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where empid=(select id from employees where empid=5782) and LeaveTypeId=13 and year=2021),
'C offs credited for period April-June 21',---Reason
(select ShortName from Employees where empid=5782),
(select Department from Employees where empid=5782),
(select Branch from Employees where empid=5782),
(select CurrentDesignation from Employees where empid=5782),'Credit',
(select branch_value1 from employees where empid=5782),
(5	+(select LeaveBalance from EmpLeaveBalance where empid=(select id from employees where empid=5782) and LeaveTypeId=13 and year=2021)),2021);


--Update leave balance
update EmpLeaveBalance set LeaveBalance=
(5+(select LeaveBalance from EmpLeaveBalance where empid=(select id from employees where empid=5782 and LeaveTypeId=13) and year=2021)) 
where LeaveTypeId=13 and empid=(select id from employees where empid=5782) and year=2021;