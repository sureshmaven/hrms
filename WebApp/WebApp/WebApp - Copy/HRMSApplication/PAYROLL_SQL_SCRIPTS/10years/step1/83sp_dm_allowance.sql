create procedure sp_dm_allowance(@pmonth varchar(20))
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
--print(@fyPeriod);
declare @fm NVARCHAR(MAX);
set @fm = cast(left(@pmonth,4) as varchar(20))+'-'+ right(@pmonth,2)+'-'+'01';
--select top 1 * from [TEST2].dbo.PItDet where FinYear='2019-20';

declare @StartValue int;
set @StartValue=(select last_num from new_num where table_name = 'pr_emp_branch_allowances');
declare @initValue int;
set @initValue = @StartValue;

CREATE TABLE #codes_master (
emp_code nvarchar(100), 
m_id nvarchar(100), 
allowance nvarchar(100),
from_date nvarchar(100),
to_date nvarchar(100),
);

declare @m_id nvarchar(100);
declare @emp_code nvarchar(100);
declare @allowance nvarchar(100);
declare @from_date nvarchar(100);
declare @to_date nvarchar(100);


insert into #codes_master select m.Empid as emp_code,p.slno as m_id,
p.Allowance as allowance,p.Fromdate as from_date,p.Todate  as to_date
from test2.dbo.PAllowancemast p join test2.dbo.PEMPMAST m on p.eid=m.eid --where m.dor>'2019-04-01' and p.Pmonth>201903;

while exists (select * from #codes_master)
begin
--inserion 
set @initValue = @initValue+1;

select @m_id = (select top 1 m_id from #codes_master order by m_id asc);

select @emp_code = emp_code, @allowance = allowance,@from_date= from_date,@to_date=to_date from #codes_master where m_id = @m_id;

--print(@to_date);


insert into pr_emp_branch_allowances (id,emp_id,emp_code,fy,fm,allowance_mid,from_date,to_date,active,trans_id)
values(@initValue,(select id from employees where empid = @emp_code),@emp_code,@fy,@fm,
(select id from pr_branch_allowance_master where name=@allowance),@from_date,case  when convert(date,@to_date)  = '1900-01-01' then null else @to_date end,
case when convert(date,@to_date)  = '1900-01-01' or convert(date,@to_date)>=convert(date,@fm) then 1 else 0 end,1000);

--case when convert(date,@to_date)>=convert(date,@fm) and @to_date is not null then 1 else 0 end

delete from #codes_master where m_id=@m_id and emp_code=@emp_code and allowance = @allowance and from_date= @from_date and to_date=@to_date;

end

declare @lastnum int;
set @lastnum = (select top 1 id from pr_emp_branch_allowances order by id desc)
update new_num set last_num=@lastnum where table_name='pr_emp_branch_allowances';

drop table #codes_master;

end

--exec sp_dm_allowance '201909';

--select * from pr_emp_branch_allowances where emp_code=555 order by m_id asc;

--delete from pr_emp_branch_allowances