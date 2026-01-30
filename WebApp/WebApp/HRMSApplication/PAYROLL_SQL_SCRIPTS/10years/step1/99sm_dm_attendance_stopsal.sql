create procedure sm_dm_attendance_stopsal	
as
select 1 as id1,(left(Pmonth,4)+1) as fy,
CAST((convert(date,convert(date,left(Pmonth,4)+'-'+right(Pmonth,2)+'-01'),102)) as varchar) as fm,e.id,m.empid,
'StopSalary' as status,'' as status_date,'' as lev_bal,lopdays,absentdays,workdays,0 as active,
1000 as trans_id,'' as sus_per into tblstopsal from
test2.dbo.poldslip p 
join test2.dbo.pempmast m on p.eid=m.eid join employees e on e.empid=m.empid   
COLLATE SQL_Latin1_General_CP1_CI_AS
where status='S' and pmonth>=201904


insert into pr_month_attendance select * from tblstopsal

drop table tblstopsal

--select * from pr_month_attendance where status='StopSalary'

declare @employeecode AS NVARCHAR(100);


declare @date varchar(10);
declare @date1 date;

set @date=(select distinct top 1 pmonth
from test2.dbo.poldslip p 
join test2.dbo.pempmast m on p.eid=m.eid join employees e on e.empid=m.empid
COLLATE SQL_Latin1_General_CP1_CI_AS
 where status='S'
order by pmonth desc)


set @date1=CAST((convert(date,convert(date,left(@date,4)+'-'+right(@date,2)+'-01'),102)) as varchar)


select distinct(m.empid) into #emp_code
 from test2.dbo.poldslip p 
join test2.dbo.pempmast m on p.eid=m.eid join employees e on e.empid=m.empid   
COLLATE SQL_Latin1_General_CP1_CI_AS
where status='S' and pmonth=@date

while exists(select * from #emp_code)
begin
select @employeecode = (select top 1 empid from #emp_code order by empid asc);
declare @bal NVARCHAR(100);
declare @balQry NVARCHAR(100);

set @balQry = 'select @bal = dbo.getLeaveBalance('+@employeecode+');';
execute sp_executesql @balQry, N'@bal NVARCHAR(100) OUTPUT', 
    @bal = @bal output;
	--print(@bal);
	
	update pr_month_attendance set leaves_available=@bal, active=1 where emp_code=@employeecode and 
	fm=@date1 and status='StopSalary'

delete from #emp_code where empid=@employeecode;
end

DECLARE @counter int;
SET @counter = 0;
UPDATE pr_month_attendance SET @counter = id = @counter + 1;
drop table #emp_code;

--select * from pr_month_attendance where fm='2020-09-01' and leaves_available!=''

--update pr_month_attendance set leaves_available='' where  fm='2020-09-01' and status='StopSalary'

--select * from pr_month_attendance where status='StopSalary' order by fm
--exec sm_dm_attendance_stopsal

