
CREATE TABLE [dbo].[WorkDiary](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmpId] [int] NULL,
	[Status] [varchar](250) NULL,
	[CA] [int] NULL,
	[SA] [int] NULL,
	[UpdatedDate] [datetime] NULL,
	[WDDate] [datetime] NULL,
	[UpdatedBy] [int] NULL,
	[RefId] [int] NULL,
 CONSTRAINT [PK_WorkDiary_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

alter table WorkDiary add Br int default(0);
alter table WorkDiary add Org int default(0);


CREATE TABLE [dbo].[WorkDiary_Det](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NULL,
	[Desc] [varchar](300) NULL,
	[WDId] [int] NULL,
 CONSTRAINT [PK_WorkDiary_Det_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TRIGGER [dbo].[WDTRIGGERupdate]
ON [dbo].[Leaves]
for UPDATE
AS
BEGIN
 
IF UPDATE(Status)
DECLARE @EmpId int;
DECLARE @Status varchar(50);
DECLARE @OLD_Status varchar(50);
DECLARE @CA varchar(200);
DECLARE @SA varchar(200);
DECLARE @UpdatedDate datetime ;
DECLARE @WDDate datetime;
DECLARE @UpdatedBy int;
DECLARE @RefId int; 
DECLARE @parent_id int;
DECLARE @StartDate datetime;
DECLARE @EndDate datetime;
DECLARE @Desc varchar(200);
 
  SELECT @CA =(select id from employees where id=inserted.ControllingAuthority), 
    @SA=(select id from employees where id=inserted.SanctioningAuthority),
    @EmpId=(select empid from employees where id=inserted.empid),@Status=inserted.[Status],@RefId = inserted.id,
    @StartDate=CAST(inserted.StartDate as date),
	@Desc=(select code from leavetypes where id=inserted.LeaveType),
    @EndDate=CAST(inserted.EndDate as date),
	@UpdatedDate =inserted.UpdatedDate 
    FROM INSERTED;
 
    SELECT @OLD_Status = [Status] FROM DELETED;
    IF @Status = 'Approved'
    BEGIN
        WHILE @EndDate >= @StartDate
        
        BEGIN            
            -- inserting to workdiary
            INSERT INTO WorkDiary([EmpId],[Status],[CA],[SA],[UpdatedDate],[WDDate],[UpdatedBy],[RefId])
			VALUES(@EmpId,@Status,@CA,@SA,@UpdatedDate,@StartDate,@UpdatedBy, @RefId);       
            SELECT @parent_id=SCOPE_IDENTITY();
            --inserting to workdiary_det with the latest workdiary primary key
            INSERT INTO WorkDiary_Det([Name],[Desc],WDId) VALUES('Leave', @Desc, @parent_id);       
            SET @StartDate = DATEADD(DAY,1, @StartDate);
        END
    END
    
    IF @OLD_Status = 'Approved' AND  @Status = 'Cancelled'        
    BEGIN
        DELETE FROM WorkDiary_Det WHERE WDId in (SELECT Id FROM WorkDiary WHERE RefId = @RefId);
        DELETE FROM WorkDiary WHERE RefId = @RefId;  
    END
END
 
GO


USE [hrms7sep]
GO

/****** Object:  Trigger [dbo].[WDTLTCRIGGERupdate]    Script Date: 08-09-2018 14:57:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


create TRIGGER [dbo].[WDTLTCRIGGERupdate]
ON [dbo].[Leaves_LTC]
for UPDATE
AS
BEGIN
 
IF UPDATE(Status)
DECLARE @EmpId int;
DECLARE @Status varchar(50);
DECLARE @OLD_Status varchar(50);
DECLARE @CA varchar(200);
DECLARE @SA varchar(200);
DECLARE @UpdatedDate datetime ;
DECLARE @WDDate datetime;
DECLARE @UpdatedBy int;
DECLARE @RefId int; 
DECLARE @parent_id int;
DECLARE @StartDate datetime;
DECLARE @EndDate datetime;
DECLARE @Desc varchar(200);
DECLARE @LTCType varchar(50);
 
  SELECT @CA =(select id from employees where id=inserted.ControllingAuthority), 
    @SA=(select id from employees where id=inserted.SanctioningAuthority),
    @EmpId=(select empid from employees where id=inserted.empid),@Status=inserted.[Status],@RefId = inserted.id,
    @StartDate=CAST(inserted.StartDate as date),
	@Desc=(select code from leavetypes where id=inserted.LeaveType),
    @EndDate=CAST(inserted.EndDate as date)  ,@UpdatedDate =inserted.UpdatedDate,@LTCType=inserted.[LTCType]        
    FROM INSERTED;
 
    SELECT @OLD_Status = [Status] FROM DELETED;

    IF @Status ='Approved' AND @LTCType = 'Availment'	
    BEGIN
        WHILE @EndDate >= @StartDate        
        BEGIN            
            -- inserting to workdiary
            INSERT INTO WorkDiary([EmpId],[Status],[CA],[SA],[UpdatedDate],[WDDate],[UpdatedBy],[RefId])
			VALUES(@EmpId,@Status,@CA,@SA,@UpdatedDate,@StartDate,@UpdatedBy, @RefId);       
            SELECT @parent_id=SCOPE_IDENTITY();
            --inserting to workdiary_det with the latest workdiary primary key
            INSERT INTO WorkDiary_Det([Name],[Desc],WDId) VALUES('LTC', @Desc, @parent_id);       
            SET @StartDate = DATEADD(DAY,1, @StartDate);
        END
    END
   
    IF @OLD_Status = 'Approved' AND  @Status = 'Cancelled'        
    BEGIN
        DELETE FROM WorkDiary_Det WHERE WDId in (SELECT Id FROM WorkDiary WHERE RefId = @RefId);
        DELETE FROM WorkDiary WHERE RefId = @RefId;  
		 --select * from Leaves_LTC where LTCType not in ('Encashment')
    END
END
 
GO




CREATE TRIGGER [dbo].[WDODTRIGGERupdate]
ON [dbo].[OD_OtherDuty]
for UPDATE
AS
BEGIN
 
IF UPDATE(Status)
DECLARE @EmpId int;
DECLARE @Status varchar(50);
DECLARE @OLD_Status varchar(50);
DECLARE @CA varchar(200);
DECLARE @SA varchar(200);
DECLARE @UpdatedDate datetime ;
DECLARE @WDDate datetime;
DECLARE @UpdatedBy int;
DECLARE @RefId int; 
DECLARE @parent_id int;
DECLARE @StartDate datetime;
DECLARE @EndDate datetime;
DECLARE @Desc varchar(200);
 
  SELECT @CA =(select id from employees where id=inserted.ControllingAuthority), 
    @SA=(select id from employees where id=inserted.SanctioningAuthority),
    @EmpId=(select empid from employees where id=inserted.empid),@Status=inserted.[Status],@RefId = inserted.id,
    @StartDate=CAST(inserted.StartDate as date),
	@Desc=(select ODType from OD_Master where id=inserted.purpose),
    @EndDate=CAST(inserted.EndDate as date),@UpdatedDate =inserted.UpdatedDate 
    FROM INSERTED;
 
    SELECT @OLD_Status = [Status] FROM DELETED;
    IF @Status = 'Approved'
    BEGIN
        WHILE @EndDate >= @StartDate
        
        BEGIN            
            -- inserting to workdiary
            INSERT INTO WorkDiary([EmpId],[Status],[CA],[SA],[UpdatedDate],[WDDate],[UpdatedBy],[RefId])
			VALUES(@EmpId,@Status,@CA,@SA,@UpdatedDate,@StartDate,@UpdatedBy, @RefId);       
            SELECT @parent_id=SCOPE_IDENTITY();
            --inserting to workdiary_det with the latest workdiary primary key
            INSERT INTO WorkDiary_Det([Name],[Desc],WDId) VALUES('OD', @Desc, @parent_id);       
            SET @StartDate = DATEADD(DAY,1, @StartDate);
        END
    END
    
    IF @OLD_Status = 'Approved' AND  @Status = 'Cancelled'        
    BEGIN
        DELETE FROM WorkDiary_Det WHERE WDId in (SELECT Id FROM WorkDiary WHERE RefId = @RefId);
        DELETE FROM WorkDiary WHERE RefId = @RefId;  
    END
END
 
GO


alter table leaves add BranchId int null;
alter table leaves add DepartmentId int null;

UPDATE l SET l.BranchId=e.branch,l.DepartmentId=e.department  FROM employees AS e JOIN leaves AS l ON e.id = l.empid;


Create TRIGGER [dbo].[WorkDiaryTRIGGERinserts]
ON [dbo].WorkDiary
for insert
AS
BEGIN
declare @Tx_id int;
declare @Tx_type varchar(200);
declare @Tx_subtype varchar(200);
declare @Tx_by varchar(200);
declare @Tx_on varchar(200);
declare @Tx_date varchar(200);
declare @Notes varchar(200);
declare @Comments varchar(250);

    SELECT @Tx_by = INSERTED.[UpdatedBy], @Tx_on=inserted.UpdatedBy, @Tx_date=inserted.UpdatedDate,@Tx_id=inserted.Id
    FROM INSERTED
INSERT INTO Tx_History
    VALUES(@Tx_id,'WD','Pending',@Tx_by,@Tx_on,GETDATE(),'','');
End
GO




create TRIGGER [dbo].[WorkDiaryTRIGGERupdate]
ON [dbo].[WorkDiary]
for update
AS
BEGIN

declare @Tx_id int;
declare @Tx_type varchar(200);
declare @Tx_subtype varchar(200);
declare @Tx_by varchar(200);
declare @Tx_on varchar(200);
declare @Tx_date datetime;
declare @Notes varchar(200);
declare @Comments varchar(250);

    SELECT  @Tx_on=INSERTED.EMPID, 
	@Tx_date=inserted.UpdatedDate,@Tx_id=inserted.Id,@Tx_subtype=inserted.[Status]
    FROM INSERTED

	IF (@Tx_subtype) = 'Pending'
    BEGIN
    select @Tx_by=inserted.empid from inserted;
	END
	ELSE
	
	select @Tx_by=(select empid from employees where id=inserted.ca) from inserted;
    END

	INSERT INTO Tx_History
    VALUES(@Tx_id,'WD',@Tx_subtype,@Tx_by,@Tx_on,GETDATE(),'','');
GO






alter table employees add PerBranch int null;
alter table employees add PerDepartment int null; 
update employees set PerBranch=branch,PerDepartment=Department;


update roles set name='SuperAdmin' where id=1;
SET IDENTITY_INSERT [Roles] on
INSERT Roles ([Id], [Name], [Description], [UpdatedBy], [UpdatedDate]) VALUES (5, 'AdminHRDPayments', 'AdminHRDPayment', NULL, getdate());
INSERT Roles ([Id], [Name], [Description], [UpdatedBy], [UpdatedDate]) VALUES (6, 'AdminHRDPolicy', 'AdminHRDPolicy', NULL, getdate());
SET IDENTITY_INSERT [Roles] OFF

update roles set description='All AGM,GM,CGM,MD' where id=2;
update roles set description='DGM' where id=1;

update employees set loginmode='SuperAdmin',role=1 where empid=381;
update employees set loginmode='Executive',role=2 where empid=371;
update employees set loginmode='Manager',role=3 where empid=555;
update employees set loginmode='Employee',role=4 where empid=559;
update employees set loginmode='AdminHRDPayments',role=5 where empid=793;
update employees set loginmode='AdminHRDPolicy',role=6 where empid=271;

update roles set description='HRD-Administration' where id=6;
update roles set description='HRD–Payments,HRD–Gratuity and PF' where id=5;

alter table WorkDiary_Det add WorkType int null;

create table All_Masters(Id int identity(1,1) primary key not null,Code varchar(50) null,Name varchar(50) null,
Description varchar(100) null,Type varchar(20) null,
Br int null default 0,Org int default '1',Active int null);


INSERT into [All_Masters] ([Code], [Name], [Description], [Type], [Br], [Org], [Active]) VALUES ('Code1','Deposits~Customer ID~IDs created','Description1','WD', 0, 0, 1);
INSERT into [All_Masters] ([Code], [Name], [Description], [Type], [Br], [Org], [Active]) VALUES ('Code2','Deposits~Term Deposits~Opened','Description2','WD', 0, 0, 1);
INSERT into [All_Masters] ([Code], [Name], [Description], [Type], [Br], [Org], [Active]) VALUES ('Code3','Deposits~Term Deposits~Closed','Description3','WD', 0, 0, 1);
INSERT into [All_Masters] ([Code], [Name], [Description], [Type], [Br], [Org], [Active]) VALUES ('Code4','Deposits~Term Deposits~Renewed','Description4','WD', 0, 0, 1);
INSERT into [All_Masters] ([Code], [Name], [Description], [Type], [Br], [Org], [Active]) VALUES ('Code5','Deposits~Term Deposits~Deposit loans','Description5','WD', 0, 1, 1);
INSERT into [All_Masters] ([Code], [Name], [Description], [Type], [Br], [Org], [Active]) VALUES ('Code6','Deposits~CASA~SB Accounts Opened','Description6','WD', 0, 1, 1);
INSERT into [All_Masters] ([Code], [Name], [Description], [Type], [Br], [Org], [Active]) VALUES ('Code7','Deposits~CASA~Current Accounts Opened','Description7','WD', 0, 0, 1);
INSERT into [All_Masters] ([Code], [Name], [Description], [Type], [Br], [Org], [Active]) VALUES ('Code8','Deposits~CASA~Signature and Photo Scanned','Description8','WD', 0, 0, 1);
INSERT into [All_Masters] ([Code], [Name], [Description], [Type], [Br], [Org], [Active]) VALUES ('Code9','Deposits~CASA~Cash & Transfer entries','Description9','WD', 0, 0, 1);
INSERT into [All_Masters] ([Code], [Name], [Description], [Type], [Br], [Org], [Active]) VALUES ('Code10','Deposits~CASA~Cheque books issued','Description10','WD', 0, 0, 1);
INSERT into [All_Masters] ([Code], [Name], [Description], [Type], [Br], [Org], [Active]) VALUES ('Code11','Loans~Gold Loan Accounts Opened','Description11','WD', 0, 0, 1);
INSERT into [All_Masters] ([Code], [Name], [Description], [Type], [Br], [Org], [Active]) VALUES ('Code12','Loans~Gold Loan Accounts Closed','Description12','WD', 0, 0, 1);
INSERT into [All_Masters] ([Code], [Name], [Description], [Type], [Br], [Org], [Active]) VALUES ('Code13','Loans~Other Loan Accounts Opened','Description13','WD', 0, 0, 1);
INSERT into [All_Masters] ([Code], [Name], [Description], [Type], [Br], [Org], [Active]) VALUES ('Code14','Loans~Other Loan Accounts Closed','Description14','WD', 0, 0, 1);
INSERT into [All_Masters] ([Code], [Name], [Description], [Type], [Br], [Org], [Active]) VALUES ('Code15','Cashier~Receipts','Description15','WD', 0, 0, 1);
INSERT into [All_Masters] ([Code], [Name], [Description], [Type], [Br], [Org], [Active]) VALUES ('Code16','Cashier~Payments','Description16','WD', 0, 0, 1);
INSERT into [All_Masters] ([Code], [Name], [Description], [Type], [Br], [Org], [Active]) VALUES ('Code17','Others','Description17','WD', 0, 0, 1);
INSERT into [All_Masters] ([Code], [Name], [Description], [Type], [Br], [Org],
  [Active]) VALUES ('Code18','Leave','Description18','WD', 0, 0, 1);
   INSERT into [All_Masters] ([Code], [Name], [Description], [Type], [Br], [Org],
  [Active]) VALUES ('Code19','LTC','Description19','WD', 0, 0, 1);
   INSERT into [All_Masters] ([Code], [Name], [Description], [Type], [Br], [Org],
  [Active]) VALUES ('Code20','OD','Description20','WD', 0, 0, 1);