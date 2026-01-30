

create procedure sp_dm_promotions @empcode int, @yearmon int as 

create table #pro(
Type	varchar(30)	,
EmpId	int	,
OldDesignation	int	,
NewDesignation	int	,
OldBranch	int	,
NewBranch	int	,
OldDepartment	int	,
NewDepartment	int	,
EffectiveFrom	datetime	,
EffectiveTo	datetime	,
Transfer_Type	varchar(30)	,
UpdatedBy	int	,
UpdatedDate	datetime	,
fy	int	,
fm	date	,
category	varchar(100)	,
new_basic	float	,
old_basic	float	,
incre_due_date	date	,
senoirity_order	int	,
authorisation	int	,
active	int);

declare @res date;
set @res=(select CAST((convert(date,convert(date,left(@yearmon,4)+'-'+right(@yearmon,2)+'-01'),102)) as varchar));

declare @newcatid int;
declare @newcat varchar(50);
select @newcatid=newcat from [TEST2].[dbo].pempprom where eid =(select eid from [TEST2].[dbo].PEMPMAST where empid=@empcode) and year(newincddate)=year(@res) and month(newincddate)=month(@res);
select @newcat=WinValue from [TEST2].[dbo].PWinfile where WinId=@newcatid

Declare @branch int;
Declare @Dep int;
Declare @id int;

select @id =id,@branch=Branch,@Dep=Department from Employees where empid=@empcode;

insert into #pro 
select 'Promotion',@id as Empid,
case when currdesgn in (	284,140,324	) then 	1
when currdesgn in (	366	) then 	2
when currdesgn in (	149	) then 	3
when currdesgn in (	142,358	) then 	4
when currdesgn in (	303,359,128	) then 	5
when currdesgn in (	163,165	) then 	6
when currdesgn in (	312,137	) then 	7
when currdesgn in (	365,168,269,134,360	) then 	8
when currdesgn in (	362,133,135	) then 	9
when currdesgn in (	145	) then 	13
when currdesgn in (	175	) then 	18
when currdesgn in (	181	) then 	20
when currdesgn in (	164	) then 	28
when currdesgn in (	363,178	) then 	29
when currdesgn in (	301,361	) then 	30
when currdesgn in (	304,155,270	) then 	31 else 0
end as OldDesignation,
case 
when newdesgn in (	284,140,324	) then 	1
when newdesgn in (	366	) then 	2
when newdesgn in (	149	) then 	3
when newdesgn in (	142,358	) then 	4
when newdesgn in (	303,359,128	) then 	5
when newdesgn in (	163,165	) then 	6
when newdesgn in (	312,137	) then 	7
when newdesgn in (	365,168,269,134,360	) then 	8
when newdesgn in (	362,133,135	) then 	9
when newdesgn in (	145	) then 	13
when newdesgn in (	175	) then 	18
when newdesgn in (	181	) then 	20
when newdesgn in (	164	) then 	28
when newdesgn in (	363,178	) then 	29
when newdesgn in (	301,361	) then 	30
when newdesgn in (	304,155,270	) then 	31 else 0
end as NewDesignation,
@branch as oldbranch,
@branch as newbranch,
@Dep	as OldDepartment,
@Dep	as NewDepartment,
newpdate	as EffectiveFrom,
'' as EffectiveTo,
'Promotion'	as Transfer_Type,
123456	as UpdatedBy,
getdate()	as UpdatedDate,
year(newincddate)	as fy,
cast (newincddate as date)	as fm,
@newcat	as category,
newbpay	as new_basic,
currbpay	as old_basic,
newincddate	as incre_due_date,
1	as senoirity_order,
101	as authorisation,
1	as active
from  [TEST2].[dbo].[pempprom] pro 
join [TEST2].[dbo].[PEMPMAST] mast on pro.eid=mast.Eid 
where mast.Empid=@empcode and year(newincddate)=year(@res) and month(newincddate)=month(@res);

insert into Employee_Transfer select 
Type	,
EmpId	,
OldDesignation	,
NewDesignation	,
OldBranch	,
NewBranch	,
OldDepartment	,
NewDepartment	,
EffectiveFrom	,
EffectiveTo	,
Transfer_Type	,
UpdatedBy	,
UpdatedDate	,
fy	,
fm	,
category	,
new_basic	,
old_basic	,
incre_due_date	,
senoirity_order	,
authorisation	,
active	from #pro;

drop table #pro;

--exec sp_dm_promotions 265,201909


