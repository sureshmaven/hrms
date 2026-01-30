create procedure sp_dm_form12ba(@pmonth varchar(20))
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
set @StartValue=(select last_num from new_num where table_name = 'pr_emp_incometax_12ba');
declare @initValue int;
set @initValue = @StartValue;

CREATE TABLE #codes_master (
emp_code nvarchar(100), 
m_id nvarchar(100), 
perq nvarchar(100),
rec nvarchar(100),
tax nvarchar(100),
);

declare @m_id nvarchar(100);
declare @emp_code nvarchar(100);
declare @perq nvarchar(100);
declare @rec nvarchar(100);
declare @tax nvarchar(100);


insert into #codes_master select m.Empid,p.SLNO,p.PERQAMT,p.RECAMT,p.TAXAMT 
from test2.dbo.PIT12BA p join test2.dbo.PEMPMAST m on p.eid=m.eid where m.dor>'2019-04-01';

while exists (select * from #codes_master)
begin
--inserion 
set @initValue = @initValue+1;

select @m_id = (select top 1 m_id from #codes_master order by m_id asc);

select @emp_code = emp_code, @perq = perq,@rec= rec,@tax=tax from #codes_master where m_id = @m_id;

insert into pr_emp_incometax_12ba(id,fy,fm,emp_id,emp_code,m_id,perq_amt,rec_amt,tax_amt,active,trans_id) 
values(@initValue,@fy,@fm,(select id from employees where empid = @emp_code),@emp_code,@m_id,@perq,@rec,@tax,1,1000);

delete from #codes_master where m_id=@m_id and emp_code=@emp_code and perq=@perq and rec=@rec and tax=@tax;

end

declare @lastnum int;
set @lastnum = (select top 1 id from pr_emp_incometax_12ba order by id desc)
update new_num set last_num=@lastnum where table_name='pr_emp_incometax_12ba';

drop table #codes_master;

end

--exec sp_dm_form12ba '201909';

--select * from pr_emp_incometax_12ba where emp_code=271 order by m_id asc;