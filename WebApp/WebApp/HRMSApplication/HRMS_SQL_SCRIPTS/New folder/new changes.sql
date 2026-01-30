alter table WorkDiary add CurBr  int ;
alter table WorkDiary add CurDept  int ;

UPDATE l SET l.curbr=e.branch,l.curdept=e.department  FROM employees AS e JOIN WorkDiary AS l ON e.empid = l.empid;

alter table FamilyRelations add LTCId int null;

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

 
  SELECT @CA =(select id from employees where id=inserted.ControllingAuthority), 
    @SA=(select id from employees where id=inserted.SanctioningAuthority),
    @EmpId=(select empid from employees where id=inserted.empid),@Status=inserted.[Status],@RefId = inserted.id,
    @StartDate=CAST(inserted.StartDate as date),
	@Desc=(select code from leavetypes where id=inserted.LeaveType),
    @EndDate=CAST(inserted.EndDate as date),
	@UpdatedDate =inserted.UpdatedDate ,@CurBr=inserted.BranchId,
	@CurDept=inserted.DepartmentId
    FROM INSERTED;
 
    SELECT @OLD_Status = [Status] FROM DELETED;
    IF @Status = 'Approved'
    BEGIN
        WHILE @EndDate >= @StartDate
        
        BEGIN            
            -- inserting to workdiary
            INSERT INTO WorkDiary([EmpId],[Status],[CA],[SA],[UpdatedDate],[WDDate],[UpdatedBy],[RefId],[CurBr],[CurDept])
			VALUES(@EmpId,@Status,@CA,@SA,@UpdatedDate,@StartDate,@UpdatedBy, @RefId,@CurBr,@CurDept);       
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

 
  SELECT @CA =(select id from employees where id=inserted.ControllingAuthority), 
    @SA=(select id from employees where id=inserted.SanctioningAuthority),
    @EmpId=(select empid from employees where id=inserted.empid),@Status=inserted.[Status],@RefId = inserted.id,
    @StartDate=CAST(inserted.StartDate as date),
	@Desc=(select code from leavetypes where id=inserted.LeaveType),
    @EndDate=CAST(inserted.EndDate as date)  ,@UpdatedDate =inserted.UpdatedDate,@LTCType=inserted.[LTCType] ,
	@curbr=(select branch from employees where id=inserted.empid),
	@CurDept=(select Department from employees where id=inserted.empid)
	       
    FROM INSERTED;
 
    SELECT @OLD_Status = [Status] FROM DELETED;

    IF @Status ='Approved' AND @LTCType = 'Availment'	
    BEGIN
        WHILE @EndDate >= @StartDate        
        BEGIN            
            -- inserting to workdiary
            INSERT INTO WorkDiary([EmpId],[Status],[CA],[SA],[UpdatedDate],[WDDate],[UpdatedBy],[RefId],[curbr],[curdept])
			VALUES(@EmpId,@Status,@CA,@SA,@UpdatedDate,@StartDate,@UpdatedBy, @RefId,@curbr,@curdept);       
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
 
  SELECT @CA =(select id from employees where id=inserted.ControllingAuthority), 
    @SA=(select id from employees where id=inserted.SanctioningAuthority),
    @EmpId=(select empid from employees where id=inserted.empid),@Status=inserted.[Status],@RefId = inserted.id,
    @StartDate=CAST(inserted.StartDate as date),
	@Desc=(select ODType from OD_Master where id=inserted.purpose),
    @EndDate=CAST(inserted.EndDate as date),@UpdatedDate =inserted.UpdatedDate ,
	@curbr=(select branch from employees where id=inserted.empid),
	@CurDept=(select Department from employees where id=inserted.empid)
    FROM INSERTED;
 
    SELECT @OLD_Status = [Status] FROM DELETED;
    IF @Status = 'Approved'
    BEGIN
        WHILE @EndDate >= @StartDate
        
        BEGIN            
            -- inserting to workdiary
            INSERT INTO WorkDiary([EmpId],[Status],[CA],[SA],[UpdatedDate],[WDDate],[UpdatedBy],[RefId],[curbr],[curdept])
			VALUES(@EmpId,@Status,@CA,@SA,@UpdatedDate,@StartDate,@UpdatedBy, @RefId,@curbr,@curdept);  
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


 /*SELECT wd.Id,wd.EmpId, workdet.Name,workdet.[Desc], wd.Status,wd.UpdatedDate,wd.WDDate,wd.RefId,ep.ShortName,ds.Code as Designation, 
 case when br.Name='OtherBranch' then dp.Name else br.Name end as BrDepot  
 FROM WorkDiary wd  join Employees ep on wd.EmpId = ep.EmpId
 join Designations ds on ds.Id = ep.CurrentDesignation  
 join Departments dp on dp.Id = wd.CurDept  
 join Branches br on br.Id = wd.CurBr  
 join workdiary_det workdet on wd.Id = workdet.WDId 
 where ep.EmpId  = 381 order by wd.WDDate desc*/