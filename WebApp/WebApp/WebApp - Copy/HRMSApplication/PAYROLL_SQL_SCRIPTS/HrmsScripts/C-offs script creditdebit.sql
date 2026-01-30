--Please credit C-offs to below employees with reason " As per Note orders of DGM(HRD) dated 20.02.2021"
--Emp code              Emp Name               C-Offs
--6386                       S Bujji babu               4

--Credit leaves
INSERT INTO [dbo].[Leaves_CreditDebit] values((select id from Employees where empid=308),13,10,0,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where empid=(select id from employees where empid=308) and LeaveTypeId=13 and year=2021),
'As per Note orders of DGM(HRD) dated 29.09.2021',---Reason
(select ShortName from Employees where empid=308),
(select Department from Employees where empid=308),
(select Branch from Employees where empid=308),
(select CurrentDesignation from Employees where empid=308),'Credit',
(select branch_value1 from employees where empid=308),
(10+(select LeaveBalance from EmpLeaveBalance where empid=(select id from employees where empid=308) and LeaveTypeId=13 and year=2021)),2021);


--Update leave balance
update EmpLeaveBalance set LeaveBalance=
(10+(select LeaveBalance from EmpLeaveBalance where empid=(select id from employees where empid=308 and LeaveTypeId=13) and year=2021)) 
where LeaveTypeId=13 and empid=(select id from employees where empid=308) and year=2021;