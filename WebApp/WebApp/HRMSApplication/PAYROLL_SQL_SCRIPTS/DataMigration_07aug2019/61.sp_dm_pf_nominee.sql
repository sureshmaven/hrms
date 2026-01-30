create procedure sp_dm_pf_nominee(@pmonth varchar(20))
as 
begin
declare @fy NVARCHAR(100);
declare @fyQry NVARCHAR(100);
set @fyQry = 'select @fy = dbo.getFY('+@pmonth+');';
execute sp_executesql 
@fyQry, 
N'@fy NVARCHAR(100) OUTPUT', 
@fy = @fy output;

declare @fyPeriod NVARCHAR(100);
declare @fyPeriodQry NVARCHAR(100);
set @fyPeriodQry = 'select @fyPeriod = dbo.getFYPeriod('+@pmonth+');';
execute sp_executesql 
@fyPeriodQry, 
N'@fyPeriod NVARCHAR(100) OUTPUT', 
@fyPeriod = @fyPeriod output;
print(@fyPeriod);
declare @fm NVARCHAR(MAX);
set @fm = cast(left(@pmonth,4) as varchar(20))+'-'+ right(@pmonth,2)+'-'+'01';
--select top 1 * from [TEST2].dbo.PItDet where FinYear='2019-20';

declare @StartValue int;
set @StartValue=(select last_num from new_num where table_name = 'pr_pf_nominee');
declare @initValue int;
set @initValue = @StartValue;
declare @employeecode int;

select distinct(emp_code) as Empid into #general_codes 
from pr_emp_general where active=1;

CREATE TABLE #codes_master (
m_id nvarchar(100), 
member_name nvarchar(100), 
gender nvarchar(100), 
relation nvarchar(100),
dob nvarchar(100),
age nvarchar(100),
dates nvarchar(100),
percentage int

);

declare @m_id nvarchar(100);
declare @member_name nvarchar(100);
declare @gender nvarchar(100);
declare @relation nvarchar(100);
declare @dob nvarchar(100);
declare @age nvarchar(100);
declare @date nvarchar(100);
declare @percentage int;


while exists (select * from #general_codes)
begin
select @employeecode = (select top 1 Empid from #general_codes order by Empid asc);

insert into #codes_master select p.slno as m_id,p.MemberName as member_name,case when p.gender=0 then 'Female' else 'Male' end as gender,p.Relation as relation,p.DOB as dob,
DATEDIFF(year, p.DOB, getdate()) as age,p.NominationDate as dates,p.Percentage as  percentage
from test2.dbo.PempFamily p  join test2.dbo.PEMPMAST m on p.eid=m.eid where m.Empid=@employeecode;;

while exists (select * from #codes_master)
begin
--inserion 
set @initValue = @initValue+1;

select @m_id = (select top 1 m_id from #codes_master order by m_id asc);

select @member_name = member_name, @gender = gender,@dob= dob,@age=age,@relation=relation,@date=dates,@percentage=percentage from #codes_master where m_id = @m_id;

insert into pr_pf_nominee (id,emp_id,emp_code,fy,fm,member_name,gender,relation,dob,age,date,percentage,active,trans_id) 
values (@initValue,(select id from employees where empid = @employeecode),@employeecode,@fy,@fm,@member_name,@gender,@relation,@dob,@age,@date,@percentage,1,1000);

delete from #codes_master where m_id=@m_id and  member_name = @member_name and gender= @gender;

end
delete from #general_codes where Empid=@employeecode;
end

declare @lastnum int;
set @lastnum = (select top 1 id from pr_pf_nominee order by id desc)
update new_num set last_num=@lastnum where table_name='pr_pf_nominee';
drop table #general_codes;
drop table #codes_master;


end

--exec sp_dm_pf_nominee '201911';

--select * from pr_pf_nominee where emp_code=271;

--delete from pr_pf_nominee;