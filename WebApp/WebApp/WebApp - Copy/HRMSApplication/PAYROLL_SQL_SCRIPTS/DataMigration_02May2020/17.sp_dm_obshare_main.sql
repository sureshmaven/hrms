create procedure [dbo].[sp_dm_obshare_main](@empcode int ,  @monthyear varchar(20))
as
begin
 --select * from pr_ob_share;
 declare @year int;
set @year = cast(left(@monthyear,4) as int);

declare @StartValue int;
set @StartValue=(select last_num from new_num where table_name = 'pr_ob_share');
declare @initValue int;
set @initValue = @StartValue +1;

declare @fy NVARCHAR(100);
declare @fyQry NVARCHAR(100);
set @fyQry = 'select @fy = dbo.getFY('+@monthyear+');';
execute sp_executesql 
    @fyQry, 
    N'@fy NVARCHAR(100) OUTPUT', 
    @fy = @fy output;


declare @fm NVARCHAR(MAX);
set @fm = cast(left(@monthyear,4) as varchar(20))+'-'+ right(@monthyear,2)+'-'+'01';

--select top 1 * from pr_ob_share @monthyear;

select @initValue as id, (select id from Employees where EmpId=@empcode) as emp_id, @empcode as emp_code,
@fy as fy, @fm as fm, pslip.pf as own_share,case when pfo.PFprin  is null then 0 else pfo.PFprin end as own_share_open, 
(isnull(pslip.pf,0) + isnull(pfo.PFprin,0))  as own_share_total, case when pfo.PFint  is null then 0 else pfo.PFint end  as  own_share_intrst_open,
PFcont AS bank_share, case when pfo.Bankprin  is null then 0 else pfo.Bankprin end  as bank_share_open,CASE WHEN ((pslip.Grpay * 8.50 )/100)>1250 THEN (pslip.pf-1250) 
ELSE (pslip.pf-((pslip.Grpay * 8.50 )/100)) end  + pfo.Bankprin as bank_share_total,
case when pfo.Bankint  is null then 0 else pfo.Bankint end as bank_share_intrst_open, 
pslip.VPF as vpf,case when pfo.VPFprin  is null then 0 else pfo.VPFprin end  as vpf_open, (isnull(pslip.VPF,0) + isnull(pfo.VPFprin,0)) as vpf_total,
case when pfo.VPFint  is null then 0 else pfo.VPFint end  as  vpf_intrst_open,pslip.Maxpensin AS 
pension_open,0 as active,1000 as trans_id into #ob_share_dummy
from [TEST2].[dbo].POldslip pslip 
join [TEST2].[dbo].PFopen pfo on pslip.eid=pfo.eid 
join [TEST2].[dbo].PEMPMAST mas on mas.eid=pslip.eid where pslip.pmonth=@monthyear and pslip.Eid=(select eid from [TEST2].[dbo].PEMPMAST where empid=@empcode) and pfo.year=@year;

insert into pr_ob_share (id,emp_id,emp_code,fy,fm,own_share,own_share_open,own_share_total,own_share_intrst_open,bank_share,bank_share_open,bank_share_total,
bank_share_intrst_open,vpf,vpf_open,vpf_total,vpf_intrst_open,pension_open,active,trans_id)  select * from #ob_share_dummy;
drop table #ob_share_dummy;

--select * from pr_ob_share;
declare @yearend as int;
set @yearend = (select top 1 PMonth from [TEST2].dbo.pset order by PMonth desc);

declare @yearstart as int;
set @yearstart = CAST(@monthyear as int);
while (@yearstart <= @yearend)
begin
set @yearstart = @yearstart + 1;
set @initValue = @initValue+1;
print(@yearstart);
--old slip @yearstart
IF EXISTS (select * from [TEST2].dbo.pset where MFlag='Y' AND PMonth=@yearstart)
begin
declare @own_share_open as int;
declare @bank_share_open as int;
declare @vpf_open as int;


declare @own_share as int;
declare @bank_share as int;
declare @vpf as int;
--declare @yearcheck as int;
--set @yearcheck = @yearstart-1;

declare @a date;
declare @yearcheck as varchar(6);
set @a=DATEADD(month, -1,(concat(left(@yearstart,4),'-',right(@yearstart,2),'-',01)));
--set @yearcheck= (select concat(year(@a),(RIGHT('0'+ convert(varchar, month(@a)), 3))))
set @yearcheck=(select SUBSTRING(convert(varchar,@a, 112), 1, 6));
print @yearcheck

--SELECT convert(varchar(7), getdate(), 126) 

declare @yearone as varchar(20);
set @yearone = cast(left(@yearstart,4) as varchar(20))+'-'+ right(@yearstart,2)+'-'+'01';

declare @yeatwo as varchar(20);
set @yeatwo = cast(left(@yearcheck,4) as varchar(20))+'-'+ right(@yearcheck,2)+'-'+'01';

set @own_share_open =(select own_share_total from pr_ob_share where fm=@yeatwo and emp_code=@empcode);
set @bank_share_open =(select bank_share_total from pr_ob_share where fm=@yeatwo and emp_code=@empcode);
set @vpf_open =(select vpf_total from pr_ob_share where fm=@yeatwo and emp_code=@empcode);

set @own_share =(select pf from test2.dbo.POldslip pslip join [TEST2].[dbo].PEMPMAST mas on mas.eid=pslip.eid where pslip.pmonth=@yearstart and mas.Empid=@empcode);
set @bank_share =(select CASE WHEN ((pslip.Grpay * 8.50 )/100)>1250 THEN (pslip.pf-1250) ELSE (pslip.pf-((pslip.Grpay * 8.50 )/100)) END AS bank_share  from test2.dbo.POldslip pslip join [TEST2].[dbo].PEMPMAST mas on mas.eid=pslip.eid where pslip.pmonth=@yearstart and mas.Empid=@empcode);
set @vpf =(select pslip.VPF as vpf from test2.dbo.POldslip pslip join [TEST2].[dbo].PEMPMAST mas on mas.eid=pslip.eid where pslip.pmonth=@yearstart and mas.Empid=@empcode);

select @initValue as id, (select id from Employees where EmpId=@empcode) as emp_id, @empcode as emp_code,
@fy as fy, @yearone as fm, pslip.pf as own_share,@own_share_open as own_share_open, (isnull(@own_share,0) + isnull(@own_share_open,0)) as own_share_total, 0 as  own_share_intrst_open,
PFcont AS bank_share,@bank_share_open as bank_share_open,isnull(@bank_share,0) + isnull(@bank_share_open,0) as bank_share_total,
0 as bank_share_intrst_open, 
pslip.VPF as vpf,@vpf_open as vpf_open, (isnull(@vpf,0) + isnull(@vpf_open,0)) as vpf_total, 0 as  vpf_intrst_open,pslip.Maxpensin AS 
pension_open,0 as active,1000 as trans_id into #ob_share_dummy2
from [TEST2].[dbo].POldslip pslip 
join [TEST2].[dbo].PEMPMAST mas on mas.eid=pslip.eid where pslip.pmonth=@yearstart and pslip.Eid=(select eid from [TEST2].[dbo].PEMPMAST where empid=@empcode);

insert into pr_ob_share (id,emp_id,emp_code,fy,fm,own_share,own_share_open,own_share_total,own_share_intrst_open,bank_share,bank_share_open,bank_share_total,
bank_share_intrst_open,vpf,vpf_open,vpf_total,vpf_intrst_open,pension_open,active,trans_id)  select * from #ob_share_dummy2;
drop table #ob_share_dummy2;
end
--new slip
IF EXISTS (select * from [TEST2].dbo.pset where MFlag='N' AND PMonth=@yearstart)
begin
set @initValue = @initValue+1;
declare @own_share_open1 as int;
declare @bank_share_open1 as int;
declare @vpf_open1 as int;

declare @own_share_inst_total as int;
declare @bank_share_inst_total as int;
declare @vpf_inst_total as int;

declare @yearcheck1 as int;
set @yearcheck1 = @yearstart-1;

declare @yearone1 as varchar(20);
set @yearone1 = cast(left(@yearstart,4) as varchar(20))+'-'+ right(@yearstart,2)+'-'+'01';

declare @yeatwoo as varchar(20);
set @yeatwoo = cast(left(@yearcheck1,4) as varchar(20))+'-'+ right(@yearcheck1,2)+'-'+'01';

set @own_share_open1 =(select own_share_total from pr_ob_share where fm=@yeatwoo and emp_code=@empcode);
set @bank_share_open1 =(select bank_share_total from pr_ob_share where fm=@yeatwoo and emp_code=@empcode);
set @vpf_open1 =(select vpf_total from pr_ob_share where fm=@yeatwoo and emp_code=@empcode);


set @own_share_inst_total =(select PFint from TEST2.dbo.PFopen p join TEST2.dbo.PEMPMAST m on p.Eid=m.Eid where Year=left(@yearcheck1,4) and m.Empid=@empcode);
set @bank_share_inst_total =(select Bankint from TEST2.dbo.PFopen p join TEST2.dbo.PEMPMAST m on p.Eid=m.Eid where Year=left(@yearcheck1,4) and m.Empid=@empcode);
set @vpf_inst_total =(select VPFint from TEST2.dbo.PFopen p join TEST2.dbo.PEMPMAST m on p.Eid=m.Eid where Year=left(@yearcheck1,4) and m.Empid=@empcode);

select @initValue as id, (select id from Employees where EmpId=@empcode) as emp_id, @empcode as emp_code,
@fy as fy, @yearone1 as fm, pslip.pf as own_share,@own_share_open1 as own_share_open, (isnull(pslip.pf,0) + isnull(@own_share_open1,0)) as own_share_total, 0 as  own_share_intrst_open,
@own_share_inst_total as own_share_intrst_total,PFcont AS bank_share,@bank_share_open1 as bank_share_open,CASE 
WHEN ((pslip.Grpay * 8.50 )/100)>1250 
THEN (pslip.pf-1250)ELSE (pslip.pf-((pslip.Grpay * 8.50 )/100)) end + @bank_share_open1 as bank_share_total,
0 as bank_share_intrst_open, @bank_share_inst_total as bank_share_intrst_total,
pslip.VPF as vpf,@vpf_open1 as vpf_open, (isnull(pslip.VPF,0) + isnull(@vpf_open1,0)) as vpf_total, 0 as  vpf_intrst_open,@vpf_inst_total as vpf_intrst_total,pslip.Maxpensin AS 
pension_open, 1 as active,1000 as trans_id into #ob_share_dummy1
from [TEST2].[dbo].PPayslip pslip 
join [TEST2].[dbo].PEMPMAST mas on mas.eid=pslip.eid where pslip.pmonth=@yearstart and pslip.Eid=(select eid from [TEST2].[dbo].PEMPMAST where empid=@empcode);

insert into pr_ob_share (id,emp_id,emp_code,fy,fm,own_share,own_share_open,own_share_total,own_share_intrst_open,own_share_intrst_total,bank_share,bank_share_open,bank_share_total,
bank_share_intrst_open,bank_share_intrst_total,vpf,vpf_open,vpf_total,vpf_intrst_open,vpf_intrst_total,pension_open,active,trans_id)  select * from #ob_share_dummy1;
drop table #ob_share_dummy1;

end
end
end
 
--exec sp_dm_obshare_main 6400,201904
--delete from pr_ob_share where emp_code=6400;
--select * from pr_ob_share where emp_code=6400;
--select emp_code,fm, own_share,own_share_open,own_share_total,own_share_intrst_open,bank_share,bank_share_open,bank_share_total,
--bank_share_intrst_open,vpf,vpf_open,vpf_total,vpf_intrst_total,active  from pr_ob_share where emp_code=6400;
--select p.* from test2.dbo.PFopen p join test2.dbo.PEMPMAST m on p.Eid=m.Eid where p.Year=2019 and m.Empid=6400;

--select PFint,Bankint,VPFint from TEST2.dbo.PFopen p join TEST2.dbo.PEMPMAST m on p.Eid=m.Eid where Year=left(201908,4) and m.Empid=6400;
--select PFint,Bankint,VPFint from TEST2.dbo.PFopen p join TEST2.dbo.PEMPMAST m on p.Eid=m.Eid where Year=2019 and m.Empid=6400;
--select PFint,Bankint,VPFint from TEST2.dbo.PFopen p join TEST2.dbo.PEMPMAST m on p.Eid=m.Eid where Year=2019 and m.Empid=6400;