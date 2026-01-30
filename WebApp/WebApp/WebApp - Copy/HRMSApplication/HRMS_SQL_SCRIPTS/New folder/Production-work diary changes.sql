alter table FamilyRelations add LTCId int null;

alter table Leaves add DesignationId  int ;

alter table WorkDiary add CurDesig  int ;
alter table WorkDiary add CurBr  int ;
alter table WorkDiary add CurDept  int ;

UPDATE l SET l.curbr=e.branch,l.curdept=e.department  FROM employees AS e JOIN WorkDiary AS l ON e.empid = l.empid;
UPDATE l SET l.DesignationId=e.CurrentDesignation FROM employees AS e JOIN leaves AS l ON e.id = l.empid;
UPDATE l SET l.CurDesig=e.CurrentDesignation FROM employees AS e JOIN WorkDiary AS l ON e.empid = l.empid;


ALTER TABLE Leaves_CreditDebit ADD TotalBalance int;
UPDATE l SET l.totalbalance=e.leavebalance FROM EmpLeaveBalance AS e JOIN Leaves_CreditDebit AS l ON e.empid = l.empid;


ALTER TRIGGER [dbo].[WDTRIGGERupdate]
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
DECLARE @CurBr int;
DECLARE @CurDept int;
DECLARE @CurDesig int;

 
  SELECT @CA =(select id from employees where id=inserted.ControllingAuthority), 
    @SA=(select id from employees where id=inserted.SanctioningAuthority),
    @EmpId=(select empid from employees where id=inserted.empid),@Status=inserted.[Status],@RefId = inserted.id,
    @StartDate=CAST(inserted.StartDate as date),
	@Desc=(select code from leavetypes where id=inserted.LeaveType),
    @EndDate=CAST(inserted.EndDate as date),
	@UpdatedDate =inserted.UpdatedDate ,@CurBr=inserted.BranchId,
	@CurDept=inserted.DepartmentId,@CurDesig=inserted.DesignationId
    FROM INSERTED;
 
    SELECT @OLD_Status = [Status] FROM DELETED;
    IF @Status = 'Approved'
    BEGIN
        WHILE @EndDate >= @StartDate
        
        BEGIN            
            -- inserting to workdiary
            INSERT INTO WorkDiary([EmpId],[Status],[CA],[SA],[UpdatedDate],[WDDate],[UpdatedBy],[RefId],[CurBr],[CurDept],[CurDesig])
			VALUES(@EmpId,@Status,@CA,@SA,@UpdatedDate,@StartDate,@UpdatedBy, @RefId,@CurBr,@CurDept,@CurDesig);       
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





ALTER TRIGGER [dbo].[WDTLTCRIGGERupdate]
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
DECLARE @CurBr int;
DECLARE @CurDept int;
DECLARE @CurDesig int;

 
  SELECT @CA =(select id from employees where id=inserted.ControllingAuthority), 
    @SA=(select id from employees where id=inserted.SanctioningAuthority),
    @EmpId=(select empid from employees where id=inserted.empid),@Status=inserted.[Status],@RefId = inserted.id,
    @StartDate=CAST(inserted.StartDate as date),
	@Desc=(select code from leavetypes where id=inserted.LeaveType),
    @EndDate=CAST(inserted.EndDate as date)  ,@UpdatedDate =inserted.UpdatedDate,@LTCType=inserted.[LTCType] ,
	@curbr=(select branch from employees where id=inserted.empid),
	@CurDept=(select Department from employees where id=inserted.empid),
	@CurDesig=(select CurrentDesignation from employees where id=inserted.empid)
	       
    FROM INSERTED;
 
    SELECT @OLD_Status = [Status] FROM DELETED;

    IF @Status ='Approved' AND @LTCType = 'Availment'	
    BEGIN
        WHILE @EndDate >= @StartDate        
        BEGIN            
            -- inserting to workdiary
            INSERT INTO WorkDiary([EmpId],[Status],[CA],[SA],[UpdatedDate],[WDDate],[UpdatedBy],[RefId],[curbr],[curdept],[CurDesig])
			VALUES(@EmpId,@Status,@CA,@SA,@UpdatedDate,@StartDate,@UpdatedBy, @RefId,@curbr,@curdept,@CurDesig);       
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







ALTER TRIGGER [dbo].[WDODTRIGGERupdate]
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
DECLARE @CurBr int;
DECLARE @CurDept int;
DECLARE @CurDesig int;
 
  SELECT @CA =(select id from employees where id=inserted.ControllingAuthority), 
    @SA=(select id from employees where id=inserted.SanctioningAuthority),
    @EmpId=(select empid from employees where id=inserted.empid),@Status=inserted.[Status],@RefId = inserted.id,
    @StartDate=CAST(inserted.StartDate as date),
	@Desc=(select ODType from OD_Master where id=inserted.purpose),
    @EndDate=CAST(inserted.EndDate as date),@UpdatedDate =inserted.UpdatedDate,
	@curbr=(select branch from employees where id=inserted.empid),
	@CurDept=(select Department from employees where id=inserted.empid),
	@CurDesig=(select CurrentDesignation from employees where id=inserted.empid)
    FROM INSERTED;
 
    SELECT @OLD_Status = [Status] FROM DELETED;
    IF @Status = 'Approved'
    BEGIN
        WHILE @EndDate >= @StartDate
        
        BEGIN            
            -- inserting to workdiary
            INSERT INTO WorkDiary([EmpId],[Status],[CA],[SA],[UpdatedDate],[WDDate],[UpdatedBy],[RefId],[CurBr],[CurDept],[CurDesig])
			VALUES(@EmpId,@Status,@CA,@SA,@UpdatedDate,@StartDate,@UpdatedBy, @RefId,@CurBr,@CurDept,@CurDesig);  
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


/*ALTER view [dbo].[V_LeaveHistory]
as
select CAST(ROW_NUMBER() over(order by e.Id desc) AS INT) Id,e.EmpId as EmployeeCode, CONCAT(e.FirstName, ' ',e.LastName) AS EmployeeName,
l.EmpId,l.LeaveType,e.FirstName,e.Branch,e.Department,
e.LastName,e.CurrentDesignation, l.StartDate,l.EndDate,l.TotalDays,l.LeaveDays,l.Subject
,l.Reason,l.Status from leaves l inner join Employees e on e.Id= l.EmpId 
where l.EmpId=e.Id*/

