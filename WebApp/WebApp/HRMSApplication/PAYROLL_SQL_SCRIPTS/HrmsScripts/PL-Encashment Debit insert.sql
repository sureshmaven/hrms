INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=763),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=763) 
and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=763),
(select Department from Employees where empid=763),
(select Branch from Employees where empid=763),
(select CurrentDesignation from Employees where empid=763),'Debit',
(select branch_value1 from employees where empid=763),((select (leavebalance-5) from 
EmpLeaveBalance where EmpId=(select id from employees where empid=763) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=942),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=942) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=942),
(select Department from Employees where empid=942),
(select Branch from Employees where empid=942),
(select CurrentDesignation from Employees where empid=942),'Debit',
(select branch_value1 from employees where empid=942),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=942) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=590),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=590) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=590),
(select Department from Employees where empid=590),
(select Branch from Employees where empid=590),
(select CurrentDesignation from Employees where empid=590),'Debit',
(select branch_value1 from employees where empid=590),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=590) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=586),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=586) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=586),
(select Department from Employees where empid=586),
(select Branch from Employees where empid=586),
(select CurrentDesignation from Employees where empid=586),'Debit',
(select branch_value1 from employees where empid=586),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=586) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=804),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=804) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=804),
(select Department from Employees where empid=804),
(select Branch from Employees where empid=804),
(select CurrentDesignation from Employees where empid=804),'Debit',
(select branch_value1 from employees where empid=804),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=804) 
and LeaveTypeId=3 and year=2021)),2021);



INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=598),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=598) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=598),
(select Department from Employees where empid=598),
(select Branch from Employees where empid=598),
(select CurrentDesignation from Employees where empid=598),'Debit',
(select branch_value1 from employees where empid=598),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=598) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=941),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=941) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=941),
(select Department from Employees where empid=941),
(select Branch from Employees where empid=941),
(select CurrentDesignation from Employees where empid=941),'Debit',
(select branch_value1 from employees where empid=941),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=941) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=905),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=905) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=905),
(select Department from Employees where empid=905),
(select Branch from Employees where empid=905),
(select CurrentDesignation from Employees where empid=905),'Debit',
(select branch_value1 from employees where empid=905),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=905) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=935),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=935) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=935),
(select Department from Employees where empid=935),
(select Branch from Employees where empid=935),
(select CurrentDesignation from Employees where empid=935),'Debit',
(select branch_value1 from employees where empid=935),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=935) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=940),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=940) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=940),
(select Department from Employees where empid=940),
(select Branch from Employees where empid=940),
(select CurrentDesignation from Employees where empid=940),'Debit',
(select branch_value1 from employees where empid=940),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=940) 
and LeaveTypeId=3 and year=2021)),2021);



INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=5862),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=5862) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=5862),
(select Department from Employees where empid=5862),
(select Branch from Employees where empid=5862),
(select CurrentDesignation from Employees where empid=5862),'Debit',
(select branch_value1 from employees where empid=5862),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=5862) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=564),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=564) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=564),
(select Department from Employees where empid=564),
(select Branch from Employees where empid=564),
(select CurrentDesignation from Employees where empid=564),'Debit',
(select branch_value1 from employees where empid=564),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=564) 
and LeaveTypeId=3 and year=2021)),2021);



INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=359),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=359) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=359),
(select Department from Employees where empid=359),
(select Branch from Employees where empid=359),
(select CurrentDesignation from Employees where empid=359),'Debit',
(select branch_value1 from employees where empid=359),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=359) 
and LeaveTypeId=3 and year=2021)),2021);



INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=358),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=358) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=358),
(select Department from Employees where empid=358),
(select Branch from Employees where empid=358),
(select CurrentDesignation from Employees where empid=358),'Debit',
(select branch_value1 from employees where empid=358),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=358) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=365),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=365) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=365),
(select Department from Employees where empid=365),
(select Branch from Employees where empid=365),
(select CurrentDesignation from Employees where empid=365),'Debit',
(select branch_value1 from employees where empid=365),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=365) 
and LeaveTypeId=3 and year=2021)),2021);



INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=777),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=777) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=777),
(select Department from Employees where empid=777),
(select Branch from Employees where empid=777),
(select CurrentDesignation from Employees where empid=777),'Debit',
(select branch_value1 from employees where empid=777),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=777) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=563),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=563) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=563),
(select Department from Employees where empid=563),
(select Branch from Employees where empid=563),
(select CurrentDesignation from Employees where empid=563),'Debit',
(select branch_value1 from employees where empid=563),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=563) 
and LeaveTypeId=3 and year=2021)),2021);



INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=566),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=566) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=566),
(select Department from Employees where empid=566),
(select Branch from Employees where empid=566),
(select CurrentDesignation from Employees where empid=566),'Debit',
(select branch_value1 from employees where empid=566),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=566) 
and LeaveTypeId=3 and year=2021)),2021);



INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=557),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=557) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=557),
(select Department from Employees where empid=557),
(select Branch from Employees where empid=557),
(select CurrentDesignation from Employees where empid=557),'Debit',
(select branch_value1 from employees where empid=557),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=557) 
and LeaveTypeId=3 and year=2021)),2021);



INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=744),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=744) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=744),
(select Department from Employees where empid=744),
(select Branch from Employees where empid=744),
(select CurrentDesignation from Employees where empid=744),'Debit',
(select branch_value1 from employees where empid=744),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=744) 
and LeaveTypeId=3 and year=2021)),2021);



INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=574),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=574) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=574),
(select Department from Employees where empid=574),
(select Branch from Employees where empid=574),
(select CurrentDesignation from Employees where empid=574),'Debit',
(select branch_value1 from employees where empid=574),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=574) 
and LeaveTypeId=3 and year=2021)),2021);



INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=785),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=785) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=785),
(select Department from Employees where empid=785),
(select Branch from Employees where empid=785),
(select CurrentDesignation from Employees where empid=785),'Debit',
(select branch_value1 from employees where empid=785),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=785) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=565),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=565) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=565),
(select Department from Employees where empid=565),
(select Branch from Employees where empid=565),
(select CurrentDesignation from Employees where empid=565),'Debit',
(select branch_value1 from employees where empid=565),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=565) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=981),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=981) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=981),
(select Department from Employees where empid=981),
(select Branch from Employees where empid=981),
(select CurrentDesignation from Employees where empid=981),'Debit',
(select branch_value1 from employees where empid=981),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=981) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=965),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=965) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=965),
(select Department from Employees where empid=965),
(select Branch from Employees where empid=965),
(select CurrentDesignation from Employees where empid=965),'Debit',
(select branch_value1 from employees where empid=965),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=965) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=775),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=775) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=775),
(select Department from Employees where empid=775),
(select Branch from Employees where empid=775),
(select CurrentDesignation from Employees where empid=775),'Debit',
(select branch_value1 from employees where empid=775),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=775) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=966),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=966) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=966),
(select Department from Employees where empid=966),
(select Branch from Employees where empid=966),
(select CurrentDesignation from Employees where empid=966),'Debit',
(select branch_value1 from employees where empid=966),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=966) 
and LeaveTypeId=3 and year=2021)),2021);



INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=420),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=420) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=420),
(select Department from Employees where empid=420),
(select Branch from Employees where empid=420),
(select CurrentDesignation from Employees where empid=420),'Debit',
(select branch_value1 from employees where empid=420),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=420) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=6310),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=6310) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=6310),
(select Department from Employees where empid=6310),
(select Branch from Employees where empid=6310),
(select CurrentDesignation from Employees where empid=6310),'Debit',
(select branch_value1 from employees where empid=6310),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=6310) 
and LeaveTypeId=3 and year=2021)),2021);



INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=5757),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=5757) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=5757),
(select Department from Employees where empid=5757),
(select Branch from Employees where empid=5757),
(select CurrentDesignation from Employees where empid=5757),'Debit',
(select branch_value1 from employees where empid=5757),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=5757) 
and LeaveTypeId=3 and year=2021)),2021);



INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=912),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=912) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=912),
(select Department from Employees where empid=912),
(select Branch from Employees where empid=912),
(select CurrentDesignation from Employees where empid=912),'Debit',
(select branch_value1 from employees where empid=912),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=912) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=991),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=991) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=991),
(select Department from Employees where empid=991),
(select Branch from Employees where empid=991),
(select CurrentDesignation from Employees where empid=991),'Debit',
(select branch_value1 from employees where empid=991),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=991) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=937),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=937) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=937),
(select Department from Employees where empid=937),
(select Branch from Employees where empid=937),
(select CurrentDesignation from Employees where empid=937),'Debit',
(select branch_value1 from employees where empid=937),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=937) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=956),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=956) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=956),
(select Department from Employees where empid=956),
(select Branch from Employees where empid=956),
(select CurrentDesignation from Employees where empid=956),'Debit',
(select branch_value1 from employees where empid=956),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=956) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=859),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=859) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=859),
(select Department from Employees where empid=859),
(select Branch from Employees where empid=859),
(select CurrentDesignation from Employees where empid=859),'Debit',
(select branch_value1 from employees where empid=859),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=859) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=921),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=921) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=921),
(select Department from Employees where empid=921),
(select Branch from Employees where empid=921),
(select CurrentDesignation from Employees where empid=921),'Debit',
(select branch_value1 from employees where empid=921),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=921) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=896),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=896) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=896),
(select Department from Employees where empid=896),
(select Branch from Employees where empid=896),
(select CurrentDesignation from Employees where empid=896),'Debit',
(select branch_value1 from employees where empid=896),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=896) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=924),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=924) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=924),
(select Department from Employees where empid=924),
(select Branch from Employees where empid=924),
(select CurrentDesignation from Employees where empid=924),'Debit',
(select branch_value1 from employees where empid=924),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=924) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=6454),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=6454) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=6454),
(select Department from Employees where empid=6454),
(select Branch from Employees where empid=6454),
(select CurrentDesignation from Employees where empid=6454),'Debit',
(select branch_value1 from employees where empid=6454),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=6454) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=545),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=545) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=545),
(select Department from Employees where empid=545),
(select Branch from Employees where empid=545),
(select CurrentDesignation from Employees where empid=545),'Debit',
(select branch_value1 from employees where empid=545),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=545) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=833),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=833) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=833),
(select Department from Employees where empid=833),
(select Branch from Employees where empid=833),
(select CurrentDesignation from Employees where empid=833),'Debit',
(select branch_value1 from employees where empid=833),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=833) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=6344),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=6344) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=6344),
(select Department from Employees where empid=6344),
(select Branch from Employees where empid=6344),
(select CurrentDesignation from Employees where empid=6344),'Debit',
(select branch_value1 from employees where empid=6344),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=6344) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=521),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=521) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=521),
(select Department from Employees where empid=521),
(select Branch from Employees where empid=521),
(select CurrentDesignation from Employees where empid=521),'Debit',
(select branch_value1 from employees where empid=521),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=521) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=6366),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=6366) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=6366),
(select Department from Employees where empid=6366),
(select Branch from Employees where empid=6366),
(select CurrentDesignation from Employees where empid=6366),'Debit',
(select branch_value1 from employees where empid=6366),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=6366) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=996),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=996) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=996),
(select Department from Employees where empid=996),
(select Branch from Employees where empid=996),
(select CurrentDesignation from Employees where empid=996),'Debit',
(select branch_value1 from employees where empid=996),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=996) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=513),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=513) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=513),
(select Department from Employees where empid=513),
(select Branch from Employees where empid=513),
(select CurrentDesignation from Employees where empid=513),'Debit',
(select branch_value1 from employees where empid=513),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=513) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=968),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=968) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=968),
(select Department from Employees where empid=968),
(select Branch from Employees where empid=968),
(select CurrentDesignation from Employees where empid=968),'Debit',
(select branch_value1 from employees where empid=968),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=968) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=6340),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=6340) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=6340),
(select Department from Employees where empid=6340),
(select Branch from Employees where empid=6340),
(select CurrentDesignation from Employees where empid=6340),'Debit',
(select branch_value1 from employees where empid=6340),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=6340) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=6389),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=6389) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=6389),
(select Department from Employees where empid=6389),
(select Branch from Employees where empid=6389),
(select CurrentDesignation from Employees where empid=6389),'Debit',
(select branch_value1 from employees where empid=6389),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=6389) 
and LeaveTypeId=3 and year=2021)),2021);



INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=950),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=950) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=950),
(select Department from Employees where empid=950),
(select Branch from Employees where empid=950),
(select CurrentDesignation from Employees where empid=950),'Debit',
(select branch_value1 from employees where empid=950),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=950) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=303),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=303) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=303),
(select Department from Employees where empid=303),
(select Branch from Employees where empid=303),
(select CurrentDesignation from Employees where empid=303),'Debit',
(select branch_value1 from employees where empid=303),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=303) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=444),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=444) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=444),
(select Department from Employees where empid=444),
(select Branch from Employees where empid=444),
(select CurrentDesignation from Employees where empid=444),'Debit',
(select branch_value1 from employees where empid=444),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=444) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=6390),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=6390) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=6390),
(select Department from Employees where empid=6390),
(select Branch from Employees where empid=6390),
(select CurrentDesignation from Employees where empid=6390),'Debit',
(select branch_value1 from employees where empid=6390),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=6390) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=509),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=509) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=509),
(select Department from Employees where empid=509),
(select Branch from Employees where empid=509),
(select CurrentDesignation from Employees where empid=509),'Debit',
(select branch_value1 from employees where empid=509),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=509) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=6367),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=6367) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=6367),
(select Department from Employees where empid=6367),
(select Branch from Employees where empid=6367),
(select CurrentDesignation from Employees where empid=6367),'Debit',
(select branch_value1 from employees where empid=6367),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=6367) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=948),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=948) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=948),
(select Department from Employees where empid=948),
(select Branch from Employees where empid=948),
(select CurrentDesignation from Employees where empid=948),'Debit',
(select branch_value1 from employees where empid=948),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=948) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=731),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=731) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=731),
(select Department from Employees where empid=731),
(select Branch from Employees where empid=731),
(select CurrentDesignation from Employees where empid=731),'Debit',
(select branch_value1 from employees where empid=731),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=731) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=928),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=928) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=928),
(select Department from Employees where empid=928),
(select Branch from Employees where empid=928),
(select CurrentDesignation from Employees where empid=928),'Debit',
(select branch_value1 from employees where empid=928),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=928) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=308),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=308) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=308),
(select Department from Employees where empid=308),
(select Branch from Employees where empid=308),
(select CurrentDesignation from Employees where empid=308),'Debit',
(select branch_value1 from employees where empid=308),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=308) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=6421),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=6421) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=6421),
(select Department from Employees where empid=6421),
(select Branch from Employees where empid=6421),
(select CurrentDesignation from Employees where empid=6421),'Debit',
(select branch_value1 from employees where empid=6421),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=6421) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=478),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=478) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=478),
(select Department from Employees where empid=478),
(select Branch from Employees where empid=478),
(select CurrentDesignation from Employees where empid=478),'Debit',
(select branch_value1 from employees where empid=478),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=478) 
and LeaveTypeId=3 and year=2021)),2021);



INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=6335),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=6335) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=6335),
(select Department from Employees where empid=6335),
(select Branch from Employees where empid=6335),
(select CurrentDesignation from Employees where empid=6335),'Debit',
(select branch_value1 from employees where empid=6335),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=6335) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=916),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=916) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=916),
(select Department from Employees where empid=916),
(select Branch from Employees where empid=916),
(select CurrentDesignation from Employees where empid=916),'Debit',
(select branch_value1 from employees where empid=916),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=916) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=831),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=831) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=831),
(select Department from Employees where empid=831),
(select Branch from Employees where empid=831),
(select CurrentDesignation from Employees where empid=831),'Debit',
(select branch_value1 from employees where empid=831),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=831) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=6302),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=6302) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=6302),
(select Department from Employees where empid=6302),
(select Branch from Employees where empid=6302),
(select CurrentDesignation from Employees where empid=6302),'Debit',
(select branch_value1 from employees where empid=6302),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=6302) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=878),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=878) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=878),
(select Department from Employees where empid=878),
(select Branch from Employees where empid=878),
(select CurrentDesignation from Employees where empid=878),'Debit',
(select branch_value1 from employees where empid=878),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=878) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=560),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=560) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=560),
(select Department from Employees where empid=560),
(select Branch from Employees where empid=560),
(select CurrentDesignation from Employees where empid=560),'Debit',
(select branch_value1 from employees where empid=560),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=560) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=6332),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=6332) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=6332),
(select Department from Employees where empid=6332),
(select Branch from Employees where empid=6332),
(select CurrentDesignation from Employees where empid=6332),'Debit',
(select branch_value1 from employees where empid=6332),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=6332) 
and LeaveTypeId=3 and year=2021)),2021);



INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=913),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=913) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=913),
(select Department from Employees where empid=913),
(select Branch from Employees where empid=913),
(select CurrentDesignation from Employees where empid=913),'Debit',
(select branch_value1 from employees where empid=913),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=913) 
and LeaveTypeId=3 and year=2021)),2021);



INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=989),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=989) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=989),
(select Department from Employees where empid=989),
(select Branch from Employees where empid=989),
(select CurrentDesignation from Employees where empid=989),'Debit',
(select branch_value1 from employees where empid=989),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=989) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=6336),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=6336) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=6336),
(select Department from Employees where empid=6336),
(select Branch from Employees where empid=6336),
(select CurrentDesignation from Employees where empid=6336),'Debit',
(select branch_value1 from employees where empid=6336),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=6336) 
and LeaveTypeId=3 and year=2021)),2021);



INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=550),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=550) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=550),
(select Department from Employees where empid=550),
(select Branch from Employees where empid=550),
(select CurrentDesignation from Employees where empid=550),'Debit',
(select branch_value1 from employees where empid=550),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=550) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=6337),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=6337) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=6337),
(select Department from Employees where empid=6337),
(select Branch from Employees where empid=6337),
(select CurrentDesignation from Employees where empid=6337),'Debit',
(select branch_value1 from employees where empid=6337),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=6337) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=6426),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=6426) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=6426),
(select Department from Employees where empid=6426),
(select Branch from Employees where empid=6426),
(select CurrentDesignation from Employees where empid=6426),'Debit',
(select branch_value1 from employees where empid=6426),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=6426) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=862),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=862) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=862),
(select Department from Employees where empid=862),
(select Branch from Employees where empid=862),
(select CurrentDesignation from Employees where empid=862),'Debit',
(select branch_value1 from employees where empid=862),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=862) 
and LeaveTypeId=3 and year=2021)),2021);


INSERT INTO [dbo].[Leaves_CreditDebit]([EmpId],[LeaveTypeId],[CreditLeave],
[DebitLeave],[UpdatedBy],[UpdatedDate],
[LeaveBalance],[Comments],[EmpName],[Department],[Branch],
[CurrentDesignation],[Type],[Head_Branch_Value],[TotalBalance],[Year]) 
VALUES(
(select id from Employees where empid=801),3,	0	,	5	,123456,GETDATE(),
(select LeaveBalance from EmpLeaveBalance where EmpId=(select id from employees where empid=801) and LeaveTypeId=3 and year=2021),'PL Encashment for festival-2021',
(select ShortName from Employees where empid=801),
(select Department from Employees where empid=801),
(select Branch from Employees where empid=801),
(select CurrentDesignation from Employees where empid=801),'Debit',
(select branch_value1 from employees where empid=801),((select (leavebalance-5) from EmpLeaveBalance where EmpId=(select id from employees where empid=801) 
and LeaveTypeId=3 and year=2021)),2021);




