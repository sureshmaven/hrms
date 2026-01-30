


create procedure [dbo].[sp_dm_obshare] @empcode int ,  @monthyear int

as
create table #ob(
id	int,
eid	int,
emp_code	int,
fy	int,
pmonth	date,
pf	float,
osint	float,
vpf	float,
vpfintt	float,
CAL	float,
bsint	float,
DA	float,
basic	float,
active	bit,
trans_id	int,
Osfund	float,
ostotal	float,
vpffund	float,
vpftotal	float,
bsfund	float,
bstotal	float,
pension_open	float,
pension_total	float,
pension_intrest_amount	float,
bankint	float,
bankprin	float,
pfint	float,
pfprin	float,
vpfint	float,
vpfprin	float);




IF EXISTS (select * from [TEST2].dbo.pset where MFlag='N' AND PMonth=@monthyear)
begin
insert into #ob 
select 1,
pslip.eid, mas.Empid as emp_code,year(CAST((convert(date,convert(date,left(PMonth,4)+'-'+right(PMonth,2)+'-01'),102)) as varchar)) ,
CAST((convert(date,convert(date,left(PMonth,4)+'-'+right(PMonth,2)+'-01'),102)) as varchar),pslip.pf,0 as osint,pslip.VPF,0 as vpfint,CASE 
WHEN ((pslip.Grpay * 8.50 )/100)>1250 
THEN (pslip.pf-1250)ELSE (pslip.pf-((pslip.Grpay * 8.50 )/100)) END AS 
Bank_share,0 as bsint, pslip.DA,pslip.BASIC,1,101,0 as Osfund,pfo.PFprin as ostotal,
0 as vpffund,pfo.VPFprin as vpftotal,0 as bsfund,pfo.Bankprin as bstotal,
CASE WHEN ((pslip.Grpay * 8.50 )/100)>1250 THEN 1250 ELSE ((pslip.Grpay * 8.50 )/100) END AS 
 pension_open ,0 as pension_total
,0 as pension_intrest_amount,pfo.bankint,pfo.bankprin,pfo.pfint,pfo.pfprin,pfo.vpfint,pfo.vpfprin

from [TEST2].[dbo].PPayslip pslip 
join [TEST2].[dbo].PFopen pfo on pslip.eid=pfo.eid 
join [TEST2].[dbo].PEMPMAST mas on mas.eid=pslip.eid where pslip.pmonth=@monthyear and pslip.Eid=(select eid from [TEST2].[dbo].PEMPMAST where empid=@empcode) and pfo.year=2019
end 
else
begin 

insert into #ob 
select 1,
pslip.eid, mas.Empid as emp_code,year(CAST((convert(date,convert(date,left(PMonth,4)+'-'+right(PMonth,2)+'-01'),102)) as varchar)) ,
CAST((convert(date,convert(date,left(PMonth,4)+'-'+right(PMonth,2)+'-01'),102)) as varchar),pslip.pf,0 as osint,pslip.VPF,0 as vpfint,CASE 
WHEN ((pslip.Grpay * 8.50 )/100)>1250 
THEN (pslip.pf-1250)ELSE (pslip.pf-((pslip.Grpay * 8.50 )/100)) END AS 
Bank_share,0 as bsint, pslip.DA,pslip.BASIC,1,101,0 as Osfund,pfo.PFprin as ostotal,
0 as vpffund,pfo.VPFprin as vpftotal,0 as bsfund,pfo.Bankprin as bstotal,
CASE WHEN ((pslip.Grpay * 8.50 )/100)>1250 THEN 1250 ELSE ((pslip.Grpay * 8.50 )/100) END AS 
 pension_open ,0 as pension_total
,0 as pension_intrest_amount,pfo.bankint,pfo.bankprin,pfo.pfint,pfo.pfprin,pfo.vpfint,pfo.vpfprin

from [TEST2].[dbo].POldslip pslip 
join [TEST2].[dbo].PFopen pfo on pslip.eid=pfo.eid 
join [TEST2].[dbo].PEMPMAST mas on mas.eid=pslip.eid where pslip.pmonth=@monthyear and pslip.Eid=(select eid from [TEST2].[dbo].PEMPMAST where empid=@empcode) and pfo.year=2019
end


declare @t int;

 set @t =(select top 1 id from [pr_aug12_lalitha].[dbo].pr_ob_share order by id desc)

 if @t is null
 begin set @t=0 end;

insert into [pr_aug12_lalitha].[dbo].pr_ob_share 
select @t+1,
eid,
emp_code,
fy,
CAST((convert(date,convert(date,left(@monthyear,4)+'-'+right(@monthyear,2)+'-01'),102)) as varchar),
pf,
osint,
vpf,
vpfint,
CAL,
bsint,
DA,
basic,
active,
trans_id,
Osfund,
ostotal,
vpffund,
vpftotal,
bsfund,
bstotal,
pension_open,
pension_total,
pension_intrest_amount,
bankint,
bankprin,
pfint,
pfprin,
vpfint,
vpfprin from #ob;
 --------------------------------------------------------------------------------------------------------------------------
 truncate table #ob;

 --declare @id int;
--set @id=1;

 --while(@id<(select count(*) from pr_ob_share))
 --begin
 
 --if(getdate()>=(CAST((convert(date,convert(date,left(@monthyear,4)+'-'+right(@monthyear,2)+'-01'),102)) as varchar)))

 --begin

 declare @newdate date;

 select @newdate=CAST((convert(date,convert(date,left(@monthyear,4)+'-'+right(@monthyear,2)+'-01'),102)) as varchar);--201905

 declare @newdate1 date;
 select @newdate1=dateadd(month,1,@newdate);--201906

 declare @monthyear1 int;

set @monthyear1=(select cast(convert(varchar(6),@newdate1,112) as int));


 IF EXISTS (select * from [TEST2].dbo.pset where MFlag='N' AND PMonth=@monthyear1)
begin
insert into #ob 
select 1,
pslip.eid, mas.Empid as emp_code,year(CAST((convert(date,convert(date,left(PMonth,4)+'-'+right(PMonth,2)+'-01'),102)) as varchar)) ,
CAST((convert(date,convert(date,left(PMonth,4)+'-'+right(PMonth,2)+'-01'),102)) as varchar),pslip.pf,0 as osint,pslip.VPF,0 as vpfint,CASE 
WHEN ((pslip.Grpay * 8.50 )/100)>1250 
THEN (pslip.pf-1250)ELSE (pslip.pf-((pslip.Grpay * 8.50 )/100)) END AS 
Bank_share,0 as bsint, pslip.DA,pslip.BASIC,1,101,0 as Osfund,((pslip.pf)+(select top 1 own_share_total  from 
[pr_aug12_lalitha].[dbo].pr_ob_share where emp_code=@empcode and 
fm=CAST((convert(date,convert(date,left(@monthyear,4)+'-'+right(@monthyear,2)+'-01'),102)) as varchar) order by fm desc)) as ostotal,
0 as vpffund,pfo.VPFprin as vpftotal,0 as bsfund,pfo.Bankprin as bstotal,
CASE WHEN ((pslip.Grpay * 8.50 )/100)>1250 THEN 1250 ELSE ((pslip.Grpay * 8.50 )/100) END AS 
 pension_open ,0 as pension_total
,0 as pension_intrest_amount,((CASE 
WHEN ((pslip.Grpay * 8.50 )/100)>1250 
THEN (pslip.pf-1250)ELSE (pslip.pf-((pslip.Grpay * 8.50 )/100)) END 
)+( select top 1 bank_share_total  from [pr_aug12_lalitha].[dbo].pr_ob_share 
where emp_code=@empcode and fm=CAST((convert(date,convert(date,left(@monthyear,4)+'-'+right(@monthyear,2)+'-01'),102)) as varchar)
 order by fm desc )) as bankint,pfo.bankprin,pfo.pfint,pfo.pfprin,((pslip.VPF)+
 ( select top 1 vpf_total from [pr_aug12_lalitha].[dbo].pr_ob_share where emp_code=@empcode and 
 fm=CAST((convert(date,convert(date,left(@monthyear,4)+'-'+right(@monthyear,2)+'-01'),102)) as varchar))) 
 as vpfint,pfo.vpfprin

from [TEST2].[dbo].PPayslip pslip 
join [TEST2].[dbo].PFopen pfo on pslip.eid=pfo.eid 
join [TEST2].[dbo].PEMPMAST mas on mas.eid=pslip.eid where pslip.pmonth=@monthyear1 and 
pslip.Eid=(select eid from [TEST2].[dbo].PEMPMAST where empid=@empcode) and pfo.year=2019
end 
else
begin 
 
insert into #ob 
select 1,
pslip.eid, mas.Empid as emp_code,year(CAST((convert(date,convert(date,left(PMonth,4)+'-'+right(PMonth,2)+'-01'),102)) as varchar)) ,
CAST((convert(date,convert(date,left(PMonth,4)+'-'+right(PMonth,2)+'-01'),102)) as varchar),pslip.pf,0 as osint,pslip.VPF,0 as vpfint,CASE 
WHEN ((pslip.Grpay * 8.50 )/100)>1250 
THEN (pslip.pf-1250)ELSE (pslip.pf-((pslip.Grpay * 8.50 )/100)) END AS 
Bank_share,0 as bsint, pslip.DA,pslip.BASIC,1,101,0 as Osfund,((pslip.pf)+(select top 1 own_share_total  from [pr_aug12_lalitha].[dbo].pr_ob_share where emp_code=@empcode and fm=CAST((convert(date,convert(date,left(201904,4)+'-'+right(201904,2)+'-01'),102)) as varchar) order by fm desc)) as ostotal,
0 as vpffund,pfo.VPFprin as vpftotal,0 as bsfund,pfo.Bankprin as bstotal,
CASE WHEN ((pslip.Grpay * 8.50 )/100)>1250 THEN 1250 ELSE ((pslip.Grpay * 8.50 )/100) END AS 
 pension_open ,0 as pension_total
,0 as pension_intrest_amount,((CASE 
WHEN ((pslip.Grpay * 8.50 )/100)>1250 
THEN (pslip.pf-1250)ELSE (pslip.pf-((pslip.Grpay * 8.50 )/100)) END 
)+( select top 1 bank_share_total  from [pr_aug12_lalitha].[dbo].pr_ob_share 
where emp_code=@empcode and fm=CAST((convert(date,convert(date,left(@monthyear,4)+'-'+right(@monthyear,2)+'-01'),102)) as varchar)
 order by fm desc )) as bankint,pfo.bankprin,pfo.pfint,pfo.pfprin,((pslip.VPF)+
 ( select top 1 vpf_total from [pr_aug12_lalitha].[dbo].pr_ob_share where emp_code=@empcode and 
 fm=CAST((convert(date,convert(date,left(@monthyear,4)+'-'+right(@monthyear,2)+'-01'),102)) as varchar))) 
 as vpfint,pfo.vpfprin

from [TEST2].[dbo].poldslip pslip 
join [TEST2].[dbo].PFopen pfo on pslip.eid=pfo.eid 
join [TEST2].[dbo].PEMPMAST mas on mas.eid=pslip.eid where pslip.pmonth=@monthyear1 and 
pslip.Eid=(select eid from [TEST2].[dbo].PEMPMAST where empid=@empcode) and pfo.year=2019
end
declare @t1 int;

 set @t1 =(select top 1 id from [pr_aug12_lalitha].[dbo].pr_ob_share order by id desc)

 if @t1 is null
 begin set @t1=0 end;

insert into [pr_aug12_lalitha].[dbo].pr_ob_share 
select @t1+1,
eid,
emp_code,
fy,
CAST((convert(date,convert(date,left(@monthyear1,4)+'-'+right(@monthyear1,2)+'-01'),102)) as varchar),
pf,
osint,
vpf,
vpfint,
CAL,
bsint,
DA,
basic,
active,
trans_id,
Osfund,
ostotal,
vpffund,
vpftotal,
bsfund,
bstotal,
pension_open,
pension_total,
pension_intrest_amount,
bankint,
bankprin,
pfint,
pfprin,
vpfint,
vpfprin from #ob;


--declare @id int;
--set @id=(select top 1 id from [pr_aug12_lalitha].[dbo].pr_ob_share  order by id desc);
--print @id;
--update [pr_aug12_lalitha].[dbo].new_num set last_num=@id where table_name='pr_ob_share';

--set @id=@id+1;
--end;
--end;
drop table #ob;

---select  pf from test2.dbo.poldslip where eid=850 and pmonth=201905
--select *  from [pr_aug12_lalitha].[dbo].pr_ob_share
--delete from pr_ob_share
--exec sp_dm_obshare 562,201904

GO


