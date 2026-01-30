-------------------cadre---------------------------

select CAST(ROW_NUMBER() over(order by department) AS INT) Id,
v.EmpId,v.Name,v.Name as Name ,case when b.id=43 then dp.name else b.name end as BranchDepartmet,CONVERT(VARCHAR, e.DOB,103) as DOJ,
CONVERT(VARCHAR, e.RetirementDate,103) as RetirementDate
from view_employee_senioritylist v join employees e on v.empid=e.empid
join designations d on e.CurrentDesignation=d.id join branches b on e.Branch=b.id join departments dp on e.department=dp.id
where e.RetirementDate>=getdate() order by v.designations


---------------Category-------------


select CAST(ROW_NUMBER() over(order by branch) AS INT) Id,
v.EmpId,v.EmpName,d.code as Designations,e.Category,e.Gender
from view_employee_category v join employees e on v.empid=e.empid
join designations d on e.CurrentDesignation=d.id join branches b on e.Branch=b.id join departments dp on e.department=dp.id
where e.RetirementDate>=getdate() order by v.category,v.gender,v.designations


---------------------DOB--------------------------------

select CAST(ROW_NUMBER() over(order by department) AS INT) Id,
v.EmpId,v.EmpName,v.code as Designations,CONVERT(VARCHAR, DOB,103) as DOB,YEAR(E.DOB) AS Year,year(getdate()) as PresentYear,
(year(getdate())-year(dob)) as Age
from view_employee_DOB_RetirementDateMonthWise v join employees e on v.empid=e.empid
join designations d on e.CurrentDesignation=d.id join branches b on e.Branch=b.id join departments dp on e.department=dp.id
where e.RetirementDate>=getdate() order by year(e.dob) ,v.designations
 
 
---------------Employees----------------------------------

select CAST(ROW_NUMBER() over(order by Empid) AS INT) Id,
e.EmpId,e.ShortName as Name,d.name as Designations,case when b.id=43 then dp.name else b.name end as BranchDepartmet
,concat(e.firstname,'',e.lastname) as EmpFullName
from  employees e join  designations d on e.CurrentDesignation=d.id join branches b on e.Branch=b.id join departments dp 
on e.department=dp.id where e.RetirementDate>=getdate()
 
------------Future Applied Leaves-----------------

select CAST(ROW_NUMBER() over(order by Empid) AS INT) Id,
e.EmpId,e.ShortName as Name,d.code as Designation,case when b.id=43 then dp.name else b.name end as BranchDepartmet
,l.UpdatedDate as AppliedDate,l.StartDate,l.EndDate,lt.code as Type,l.Subject,l.Reason,l.Status
from  employees e join  designations d on e.CurrentDesignation=d.id join branches b on e.Branch=b.id join departments dp 
on e.department=dp.id join leaves l on e.id=l.empid join LeaveTypes lt on lt.id=l.leavetype where l.StartDate > l.UpdatedDate 
 
 
------------------HeadOffice--------------------------

select CAST(ROW_NUMBER() over(order by department) AS INT) Id,
v.EmpId,e.ShortName as Name,d.code as Designation,case when b.id=43 then dp.name else b.name end as BranchDepartmet
from  view_employee_dept v join employees e on v.empid=e.empid join  designations d on e.CurrentDesignation=d.id
join branches b on e.Branch=b.id join departments dp 
on e.department=dp.id where dp.name!='OtherDepartment' and e.RetirementDate>=getdate() 

-----------------HeadOffice Attender--------------------------

select CAST(ROW_NUMBER() over(order by department) AS INT) Id,
v.EmpId,e.ShortName as Name,d.code as Designation,v.deptname as Department
from  view_employee_dept v join employees e on v.empid=e.empid join  designations d on e.CurrentDesignation=d.id
join departments dp  on e.department=dp.id where dp.name!='OtherDepartment' and  e.RetirementDate>=getdate() and 
d.code='Attender' or d.code='Substaff' or d.code='Driver'  

-----------------Key Officials-------------------------

select CAST(ROW_NUMBER() over(order by Empid) AS INT) Id,
e.EmpId,e.ShortName as Name,d.code as Designation,case when b.id=43 then dp.name else b.name end as BranchDepartmet,
e.Graduation,E.PostGraduation,E.ProfessionalQualifications,e.TotalExperience  from   employees e  join  
designations d on e.CurrentDesignation=d.id
join departments dp  on e.department=dp.id join branches b on e.branch=b.id where   e.RetirementDate>=getdate() and 
d.code='MD' or d.code='CGM' or d.code='DGM' or d.code='GM' 


-----------------------LTC---------------------------------------

select CAST(ROW_NUMBER() over(order by Empid) AS INT) Id,
e.EmpId,e.ShortName as Name,d.code as Designation,case when b.id=43 then dp.name else b.name end as BranchDepartmet,
lt.LTCType,CONVERT(VARCHAR, lt.UpdatedDate,103) as AppliedDate,CONVERT(VARCHAR, lt.StartDate,103) as StartDate,
CONVERT(VARCHAR, lt.EndDate,103) as EndDate,lt.PlaceOfVisits,lt.ModeOfTransport,lt.TravelAdvance,
bp.Block_Period,lt.Status  from   employees e  join  
designations d on e.CurrentDesignation=d.id
join departments dp  on e.department=dp.id join branches b on e.branch=b.id join
Leaves_LTC lt on lt.empid=e.id join BlockPeriod bp on lt.Block_Period=bp.id where e.RetirementDate>=getdate()


---------------------------Leaves------------------------
---------------------All leaves---------------
select CAST(ROW_NUMBER() over(order by Empid) AS INT) Id,
e.EmpId,e.ShortName as Name,d.code as Designation,CONVERT(varchar, lt.updateddate,103) AS AppliedDate,
CONVERT(varchar(5), lt.updateddate,108) AS AppliedTime,case when b.id=43 then dp.name else b.name end as BranchDepartmet,
 CONVERT(VARCHAR, lt.StartDate,103)as StartDate,CONVERT(VARCHAR, lt.EndDate,103) as EndDate,lp.code as LeaveType,
 lt.LeaveDays,lt.Status,concat((convert(varchar, lt.UpdatedDate, 106)),' - ',CONVERT(varchar(5), lt.updateddate,108) )as DateTime,
 LT.Subject,LT.Reason  from   employees e  join   designations d on e.CurrentDesignation=d.id
  join departments dp  on e.department=dp.id join branches b on e.branch=b.id join
  Leaves lt on lt.empid=e.id join leavetypes lp on lp.id=lt.LeaveType
-----------------------today leaves-------------------------
 
 select CAST(ROW_NUMBER() over(order by Empid) AS INT) Id,
e.EmpId,e.ShortName as Name,d.code as Designation,CONVERT(varchar, lt.updateddate,103) AS AppliedDate,
CONVERT(varchar(5), lt.updateddate,108) AS AppliedTime,case when b.id=43 then dp.name else b.name end as BranchDepartmet,
 CONVERT(VARCHAR, lt.StartDate,103)as StartDate,CONVERT(VARCHAR, lt.EndDate,103) as EndDate,lp.code as LeaveType,
 lt.LeaveDays,lt.Status,concat((convert(varchar, lt.UpdatedDate, 106)),' - ',CONVERT(varchar(5), lt.updateddate,108) )as DateTime,
 LT.Subject,LT.Reason  from   employees e  join   designations d on e.CurrentDesignation=d.id
  join departments dp  on e.department=dp.id join branches b on e.branch=b.id join
  Leaves lt on lt.empid=e.id join leavetypes lp on lp.id=lt.LeaveType where   lt.LeaveDays!=0 and 
   lt.status='Cancelled' and lt.status='Debited' and lt.status='Denied' 
   and (getdate()>=lt.StartDate and getdate()<=lt.EndDate) or (getdate()>=lt.StartDate and getdate()<=lt.EndDate)
  
-----------------Long leaves-------------------
----------above 20 --------------

select CAST(ROW_NUMBER() over(order by Empid) AS INT) Id,
e.EmpId,e.ShortName as Name,d.code as Designation,
case when b.id=43 then dp.name else b.name end as BranchDepartmet,
 CONVERT(VARCHAR, lt.StartDate,103)as StartDate,CONVERT(VARCHAR, lt.EndDate,103) as EndDate,lp.code as LeaveType,
 lt.TotalDays,lt.Status  from   employees e  join   designations d on e.CurrentDesignation=d.id
 join departments dp  on e.department=dp.id join branches b on e.branch=b.id join
 Leaves lt on lt.empid=e.id join leavetypes lp on lp.id=lt.LeaveType where   e.RetirementDate>=getdate() and
 lt.totaldays>20 and lt.status!='Cancelled' and lt.status!='Denied'

----------leaves 10-20-----------------

select CAST(ROW_NUMBER() over(order by Empid) AS INT) Id,
e.EmpId,e.ShortName as Name,d.code as Designation,
case when b.id=43 then dp.name else b.name end as BranchDepartmet,
 CONVERT(VARCHAR, lt.StartDate,103)as StartDate,CONVERT(VARCHAR, lt.EndDate,103) as EndDate,lp.code as LeaveType,
 lt.TotalDays,lt.Status  from   employees e  join   designations d on e.CurrentDesignation=d.id
  join departments dp  on e.department=dp.id join branches b on e.branch=b.id join
  Leaves lt on lt.empid=e.id join leavetypes lp on lp.id=lt.LeaveType where   e.RetirementDate>=getdate() and
  lt.totaldays>=10 and lt.totaldays<=20  and lt.status!='Cancelled' and lt.status!='Denied'
  
  
--------------------Leaves forward/approved--------------------
select CAST(ROW_NUMBER() over(order by Empid) AS INT) Id,
e.EmpId,e.ShortName as Name,d.code as Designation,
case when b.id=43 then dp.name else b.name end as BranchDepartmet,lt.UpdatedBy as ApprovedBy,
ApprovedBy=(select top 1 shortname from employees where EmpId=lt.UpdatedBy or id=lt.UpdatedBy) ,
concat((convert(varchar, lt.UpdatedDate, 106)),' - ',CONVERT(varchar(5), lt.updateddate,108) )as DateTime,
lt.Status  from   employees e  join   designations d on e.CurrentDesignation=d.id
join departments dp  on e.department=dp.id join branches b on e.branch=b.id join
Leaves lt on lt.empid=e.id join leavetypes lp on lp.id=lt.LeaveType where  
 lt.status='Forwarded' or lt.Status='Approved'

 
---------------------late  applied leaves------------------------------
select CAST(ROW_NUMBER() over(order by lt.updateddate) AS INT) Id,
e.EmpId,e.ShortName as Name,d.code as Designation,
case when b.id=43 then dp.name else b.name end as BranchDepartmet,convert(varchar,lt.UpdatedDate, 103) as AppliedDate,
convert(varchar,lt.StartDate, 103) as StartDate,
convert(varchar,lt.EndDate, 103) as EndDate,lp.code as LeaveType,lt.Subject,lt.Reason,lt.Status
 from   employees e  join   designations d on e.CurrentDesignation=d.id
join departments dp  on e.department=dp.id join branches b on e.branch=b.id join
Leaves lt on lt.empid=e.id join leavetypes lp on lp.id=lt.LeaveType where  
 lt.StartDate<lt.UpdatedDate

----------------------Leaves Cancelled--------------------------------

select CAST(ROW_NUMBER() over(order by Empid) AS INT) Id,
e.EmpId,e.ShortName as Name,
CONVERT(VARCHAR, lt.StartDate,103)as StartDate,CONVERT(VARCHAR, lt.EndDate,103)as EndDate,
concat((lt.updatedby),' - ',(select top 1 shortname from employees where empId=lt.UpdatedBy or id=lt.UpdatedBy)) as CancelledBy, 
concat((convert(varchar, lt.UpdatedDate, 103)),' - ',CONVERT(varchar(5), lt.updateddate,108) ) as CancelledDateTime ,lt.Reason as ReasonForCancelled,
 lt.Stage  from   employees e  join   designations d on e.CurrentDesignation=d.id
  join departments dp  on e.department=dp.id join branches b on e.branch=b.id join
  Leaves lt on lt.empid=e.id join leavetypes lp on lp.id=lt.LeaveType where   lt.status='Cancelled'
  
--------------------MonthWISE CL/PL/ML-----------------  
select CAST(ROW_NUMBER() over(order by Empid) AS INT) Id,
e.EmpId,e.ShortName as Name,d.code as Designation,
case when b.id=43 then dp.name else b.name end as BranchDepartmet,
CONVERT(VARCHAR, lt.UpdatedDate,103)as AppliedDate,CONVERT(VARCHAR, lt.StartDate,103)as StartDate,
 DATEDIFF(day, lt.StartDate, lt.UpdatedDate) AS DiffFromAppliedDate,CONVERT(VARCHAR, lt.EndDate,103)as EndDate,
 DATEDIFF(day, lt.EndDate, lt.UpdatedDate) AS DiffEndAppliedDate
  from   employees e  join   designations d on e.CurrentDesignation=d.id
  join departments dp  on e.department=dp.id join branches b on e.branch=b.id join
  Leaves lt on lt.empid=e.id join leavetypes lp on lp.id=lt.LeaveType where   
   lp.code='CL' or lp.code='PL' or lp.code='ML'
  
----------------Monthwise Leaves-------------------------------
select CAST(ROW_NUMBER() over(order by Empid) AS INT) Id,
e.EmpId,e.ShortName as Name,d.code as Designation,
case when b.id=43 then dp.name else b.name end as BranchDepartmet,
CONVERT(VARCHAR, lt.UpdatedDate,103)as AppliedDate,CONVERT(varchar(5), lt.updateddate,108)as AppliedTime,
 CONVERT(VARCHAR, lt.StartDate,103)as StartDate, CONVERT(VARCHAR, lt.EndDate,103)as EndDate,lt.LeaveDays,
 lt.Subject,lt.Reason,lp.code as LeaveType,lt.Status
  from   employees e  join   designations d on e.CurrentDesignation=d.id
  join departments dp  on e.department=dp.id join branches b on e.branch=b.id join
  Leaves lt on lt.empid=e.id join leavetypes lp on lp.id=lt.LeaveType

-----------------Month wise OD----------------

select CAST(ROW_NUMBER() over(order by Empid) AS INT) Id,
e.EmpId,e.ShortName as Name,d.code as Designation,
case when b.id=43 then dp.name else b.name end as BranchDepartmet,
 CONVERT(VARCHAR, lt.StartDate,103)as StartDate, CONVERT(VARCHAR, lt.EndDate,103)as EndDate,lt.VistorFrom,
 lt.VistorTo,lt.Description,lt.Status
  from   employees e  join   designations d on e.CurrentDesignation=d.id
  join departments dp  on e.department=dp.id join branches b on e.branch=b.id join
  OD_OtherDuty lt on lt.empid=e.id 

--------------  Month wise TEMPORARY Transfer--------------------------

select CAST(ROW_NUMBER() over(order by Empid) AS INT) Id,
e.EmpId,e.ShortName as Name,LT.Type,
case when lt.oldbranch=43 then (select name from Departments where id=lt.OldDepartment) else 
(select name from branches where id=lt.OldBranch) end as OldDepartmentBranch,
case when lt.NewBranch=43 then (select name from Departments where id=lt.NewDepartment) else 
(select name from branches where id=lt.NewBranch) end as NewDepartmentBranch,
d.code as OldDesignation,d.code as NewDesignation,
lt.EffectiveFrom,lt.EffectiveTo   from   employees e  join   designations d on e.CurrentDesignation=d.id
join departments dp  on e.department=dp.id join branches b on e.branch=b.id join
Employee_Transfer lt on lt.empid=e.id  where  lt.Type='TemporaryTransfer' and e.RetirementDate>=getdate() 


-------------------------OD--------------------------------------
select CAST(ROW_NUMBER() over(order by Empid) AS INT) Id,
e.EmpId,e.ShortName as Name,d.code as Designation,
CONVERT(VARCHAR, lt.UpdatedDate,103)as AppliedDate,case when b.id=43 then dp.name else b.name end as BranchDepartmet,
case when lt.VistorFrom=42 then (select name from Departments where id=dp.id) else 
(select name from branches where id=b.id) end as aa,lt.VistorTo,
concat((CONVERT(VARCHAR, lt.startdate,103)),' ',right(convert(varchar(32),lt.startdate,100),8) )as FromDate,
concat((CONVERT(VARCHAR, lt.EndDate,103)),' ',right(convert(varchar(32),lt.EndDate,100),8) ) as ToDate,
concat(datediff(day,lt.startdate,lt.enddate),' - ',CAST((lt.EndDate-lt.StartDate) as time(0)) ) as Duration, m.odtype as Purpose,lt.Status,lt.Description
 from   employees e  join   designations d on e.CurrentDesignation=d.id
join departments dp  on e.department=dp.id join branches b on e.branch=b.id join
OD_OtherDuty lt on lt.empid=e.id join OD_Master m on lt.Purpose=m.id where   e.RetirementDate>=getdate() 

---------------------------Permanent Transfer---------------------------

select CAST(ROW_NUMBER() over(order by Empid) AS INT) Id,
e.EmpId,e.ShortName as Name,LT.Type,
case when lt.oldbranch=43 then (select name from Departments where id=lt.OldDepartment) else 
(select name from branches where id=lt.OldBranch) end as OldDepartmentBranch,
case when lt.NewBranch=43 then (select name from Departments where id=lt.NewDepartment) else 
(select name from branches where id=lt.NewBranch) end as NewDepartmentBranch,
d.code as OldDesignation,d.code as NewDesignation,
convert(varchar,lt.EffectiveFrom,103) as EffectiveFrom from   employees e  join   designations d on e.CurrentDesignation=d.id
join departments dp  on e.department=dp.id join branches b on e.branch=b.id join
Employee_Transfer lt on lt.empid=e.id  where  lt.Type='PermanentTransfer' and e.RetirementDate>=getdate()

------------------------Promotion---------------------------------------------

select CAST(ROW_NUMBER() over(order by Empid) AS INT) Id,
e.EmpId,e.ShortName as Name,LT.Type,
case when lt.oldbranch=43 then (select name from Departments where id=lt.OldDepartment) else 
(select name from branches where id=lt.OldBranch) end as OldDepartmentBranch,
case when lt.NewBranch=43 then (select name from Departments where id=lt.NewDepartment) else 
(select name from branches where id=lt.NewBranch) end as NewDepartmentBranch,
d.code as OldDesignation,d.code as NewDesignation,
convert(varchar,lt.EffectiveFrom,103) as EffectiveFrom,convert(varchar,lt.EffectiveTo,103) as EffectiveTo from   employees e  join   designations d on e.CurrentDesignation=d.id
join departments dp  on e.department=dp.id join branches b on e.branch=b.id join
Employee_Transfer lt on lt.empid=e.id  where  lt.Type in ('Promotion','PromotionTransfer') and e.RetirementDate>=getdate()

--------------------------------PL-------------------------------------------------

select CAST(ROW_NUMBER() over(order by Empid) AS INT) Id,
e.EmpId,e.ShortName as Name,d.code as Designation,lt.TotalExperience,lt.TotalPL,lt.PLEncash,lt.Subject,lt.Status
 from   employees e  join   designations d on e.CurrentDesignation=d.id
join departments dp  on e.department=dp.id join branches b on e.branch=b.id join
PLE_Type lt on lt.empid=e.id  where   e.RetirementDate>=getdate()

------------------------Retirements-----------------------------------------------

select CAST(ROW_NUMBER() over(order by Empid) AS INT) Id,
e.EmpId,e.ShortName as Name,d.code as Designation,case when b.id=43 then dp.name else b.name end as BranchDepartmet,
convert(varchar,e.dob,103) as DOB,convert(varchar,e.DOJ,103) as DOJ,convert(varchar,e.RetirementDate,103) as RetirementDate,
(year(getdate())-year(dob)) as Age
 from   employees e  join   designations d on e.CurrentDesignation=d.id
join departments dp  on e.department=dp.id join branches b on e.branch=b.id 
  where   e.RetirementDate>=getdate() 
  
---------------Seniority-----------------

select CAST(ROW_NUMBER() over(order by Empid) AS INT) Id,
e.EmpId,e.ShortName as Name,d.name as Designation,case when b.id=43 then dp.name else b.name end as BranchDepartmet,
convert(varchar,e.DOB,103) as DOB,convert(varchar,e.DOJ,103) AS DOJ,convert(varchar,e.RetirementDate,103) AS RetirementDate
 from   employees e  join   designations d on e.CurrentDesignation=d.id
join departments dp  on e.department=dp.id join branches b on e.branch=b.id 
 where e.RetirementDate>=getdate()
 
---------------Staff Master-------------------


select CAST(ROW_NUMBER() over(order by Empid) AS INT) Id,
e.EmpId,e.ShortName as Name,d.code as Designation,
convert(varchar,e.dob,103) as DOB,convert(varchar,e.DOJ,103) as DOJ,convert(varchar,e.RetirementDate,103) as RetirementDate,
E.FatherName,e.MotherName,e.PresentAddress as Address,e.MobileNumber,
e.Category,e.Graduation,e.PostGraduation,e.ProfessionalQualifications,
case when b.id=43 then dp.name else b.name end as BranchDepartmet,
case when t.oldbranch=43 then (select name from Departments where id=t.OldDepartment) else 
(select name from branches where id=t.OldBranch) end as OldDepartmentBranch,
case when t.NewBranch=43 then (select name from Departments where id=t.NewDepartment) else 
(select name from branches where id=t.NewBranch) end as NewDepartmentBranch,convert(varchar,t.UpdatedDate,103) as AppliedDate
,(select code from Designations where id=t.oldDesignation) as FromCadre,ToCadre=(select code from Designations where id=t.NewDesignation) 
 from   employees e  join   designations d on e.CurrentDesignation=d.id
join departments dp  on e.department=dp.id join branches b on e.branch=b.id  join Employee_Transfer t on t.empid=e.id

 
-------------------temporary transfer------------------ 

  select CAST(ROW_NUMBER() over(order by Empid) AS INT) Id,
e.EmpId,e.ShortName as Name,LT.Type,
case when lt.oldbranch=43 then (select name from Departments where id=lt.OldDepartment) else 
(select name from branches where id=lt.OldBranch) end as OldDepartmentBranch,
case when lt.NewBranch=43 then (select name from Departments where id=lt.NewDepartment) else 
(select name from branches where id=lt.NewBranch) end as NewDepartmentBranch,
d.code as OldDesignation,d.code as NewDesignation,
convert(varchar,lt.EffectiveFrom,103) as EffectiveFrom,convert(varchar,lt.EffectiveTo,103) as EffectiveTo from   employees e  join   designations d on e.CurrentDesignation=d.id
join departments dp  on e.department=dp.id join branches b on e.branch=b.id join
Employee_Transfer lt on lt.empid=e.id  where  lt.Type='TemporaryTransfer' and
e.RetirementDate>=getdate()


--------------------Tx_History----------------------------------

select Id,Tx_type,Tx_subtype,Tx_By,Tx_On,Tx_date from V_Tx_History

-------------------Top Management--------------------
select CAST(ROW_NUMBER() over(order by Empid) AS INT) Id,
e.EmpId,e.ShortName as Name,d.name as Designation,case when b.id=43 then dp.name else b.name end as BranchDepartmet,e.MobileNumber,
e.EmergencyContactNo as Extension 
 from   employees e  join   designations d on e.CurrentDesignation=d.id
join departments dp  on e.department=dp.id join branches b on e.branch=b.id 
 where  d.name in ('President','Chief General Manager','General Manager','Deputy General Manager','Assistant General Manager')
  and  e.RetirementDate>=getdate()

  
---------------------Yearwise Leave Balance------------------
  
 select E.EMPID as Empid,e.shortname as EmpName,ecf.DesignationName,case when ecf.BranchName='OtherBranch' then ecf.DeptName 
else ecf.BranchName end as BrDept,ecf.Year,
((select COALESCE(sum(leavebalance),0) from EmpLeaveBalance where empid=ecf.empid and LeaveTypeId=1)+
(select COALESCE(Sum(LeaveDays),0) from Leaves where LeaveType =1 and empid = ecf.empid and Status = 'Approved'))
 as TotalCasualLeaves, 
  (select COALESCE(Sum(LeaveDays),0) from Leaves where LeaveType =1 and empid = ecf.empid and Status = 'Approved') as ConsumedCL, 
 (select top 1 COALESCE(sum(LeaveBalance),0) from EmpLeaveBalance where empid = ecf.empid and leavetypeid=1)  
 as RemainingCL,ecf.CarryForward as CarryForwardCL,
 
((select COALESCE(sum(leavebalance),0) from EmpLeaveBalance where empid=ecf.empid and LeaveTypeId=2)+
(select COALESCE(Sum(LeaveDays),0) from Leaves where LeaveType =2 and empid = ecf.empid and Status = 'Approved')) as TotalMedicalSickLeave,
  (select COALESCE(Sum(LeaveDays),0) from Leaves where empid=ecf.empid AND LeaveType =2 and empid = ecf.empid and 
  Status = 'Approved') as ConsumedML, 
 (select top 1 COALESCE(sum(LeaveBalance),0) from EmpLeaveBalance where empid = ecf.empid and leavetypeid=2)
  as RemainingML,ecf.CarryForward as CarryForwardML, 
 
 ((select COALESCE(sum(leavebalance),0) from EmpLeaveBalance where empid=ecf.empid and LeaveTypeId=3)+
(select COALESCE(Sum(LeaveDays),0) from Leaves where LeaveType =3 and empid = ecf.empid and Status = 'Approved'))  as TotalPrivilegeLeave,
  (select COALESCE(Sum(LeaveDays),0) from Leaves where empid=ecf.empid AND LeaveType =3 and empid = ecf.empid and Status = 'Approved')
   as ConsumedPL,(select top 1 COALESCE(sum(LeaveBalance),0) from EmpLeaveBalance where empid = ecf.empid and leavetypeid=3) as RemainingPL,ecf.CarryForward as CarryForwardPL, 
 
  ((select COALESCE(sum(leavebalance),0) from EmpLeaveBalance where empid=ecf.empid and LeaveTypeId=4)+
(select COALESCE(Sum(LeaveDays),0) from Leaves where LeaveType =4 and empid = ecf.empid and Status = 'Approved'))  as TotalMaternityLeave,
  (select COALESCE(Sum(LeaveDays),0) from Leaves where empid=ecf.empid AND LeaveType =4 and empid = ecf.empid and Status = 'Approved')
   as ConsumedMTL, (select top 1 COALESCE(sum(LeaveBalance),0) from EmpLeaveBalance where empid = ecf.empid and leavetypeid=4) as RemainingMTL,ecf.CarryForward as CarryForwardMTL, 

   ((select COALESCE(sum(leavebalance),0) from EmpLeaveBalance where empid=ecf.empid and LeaveTypeId=5)+
(select COALESCE(Sum(LeaveDays),0) from Leaves where LeaveType =5 and empid = ecf.empid and Status = 'Approved'))  as TotalPaternityLeave,
  (select COALESCE(Sum(LeaveDays),0) from Leaves where empid=ecf.empid AND LeaveType =5 and empid = ecf.empid and Status = 'Approved')
   as ConsumedPTL, (select top 1 COALESCE(sum(LeaveBalance),0) from EmpLeaveBalance where empid = ecf.empid and leavetypeid=5) as RemainingPTL,ecf.CarryForward as CarryForwardPTL,    

   ((select COALESCE(sum(leavebalance),0) from EmpLeaveBalance where empid=ecf.empid and LeaveTypeId=6)+
(select COALESCE(Sum(LeaveDays),0) from Leaves where LeaveType =6 and empid = ecf.empid and Status = 'Approved'))  as TotalExtraordinaryLeave,
  (select COALESCE(Sum(LeaveDays),0) from Leaves where empid=ecf.empid AND LeaveType =6 and empid = ecf.empid and Status = 'Approved')
   as ConsumedEOL, (select top 1 COALESCE(sum(LeaveBalance),0) from EmpLeaveBalance where empid = ecf.empid and leavetypeid=6) as RemainingEOL,ecf.CarryForward as CarryForwardEOL,     
  
  ((select COALESCE(sum(leavebalance),0) from EmpLeaveBalance where empid=ecf.empid and LeaveTypeId=7)+
(select COALESCE(Sum(LeaveDays),0) from Leaves where LeaveType =7 and empid = ecf.empid and Status = 'Approved'))  as TotalSpecialCasualLeave,
  (select COALESCE(Sum(LeaveDays),0) from Leaves where empid=ecf.empid AND LeaveType =7 and empid = ecf.empid and Status = 'Approved')
   as ConsumedSCL,(select top 1 COALESCE(sum(LeaveBalance),0) from EmpLeaveBalance where empid = ecf.empid and leavetypeid=7) as RemainingSCL,
   ecf.CarryForward as CarryForwardSCL
   from V_EmpLeavesCarryForward ecf join  V_EmpLeaveBalance eb on ecf.empid=eb.empid join employees e on ecf.empid=e.id  
  --where ecf.empid=479
 
 