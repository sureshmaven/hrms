create procedure sp_dm_pf_enash_adhoc as 

insert into pr_ob_share_encashment(
id	,
emp_id,
emp_code,
fy,
fm,
own_share,
vpf,
bank_share,
active,
trans_id,is_interest_caculated,pension_open,pension_total,pension_intrest_amount,own_share_open,own_share_total,
vpf_open,vpf_total,
bank_share_open,bank_share_total)
select 1,e.id,p.empid,year((cast(left(pmonth,4) as varchar(20))+'-'+ right(pmonth,2)+'-'+'01'))+1,
(cast(left(pmonth,4) as varchar(20))+'-'+ right(pmonth,2)+'-'+'01'),
  pf,c.vpf,PFcont,0,1001,0,0,0,0,0,pf,0,c.vpf,0,PFcont from test2.dbo.pencash c 
join test2.dbo.pempmast p on c.eid=p.eid 
join employees e on e.empid=p.empid COLLATE SQL_Latin1_General_CP1_CI_AS
where  pmonth>=201904 


DECLARE @counter int;
SET @counter = 0;
UPDATE pr_ob_share_encashment SET @counter = id = @counter + 1;

-----------------------------------------------------------------------------
--declare @id1 int;
--declare @empid int;
--declare @empid1 int;
--declare @fm1 nvarchar(20);
--declare @paydate nvarchar(50);

--SELECT distinct p.empid into #Empid from test2.dbo.PAdhslip c 
--join test2.dbo.pempmast p on c.eid=p.eid 
--join employees e on e.empid=p.empid COLLATE SQL_Latin1_General_CP1_CI_AS where c.pmonth>=201904
----select * from #Empid where empid=890
--while exists (select * from #Empid)
--begin

--set @empid=(select top 1 empid from #Empid)
--select * from #Empids1
--update new_num set last_num=case when (select max(id) from pr_ob_share_adhoc) is null then 1 else (select max(id) from pr_ob_share_adhoc) end where table_name='pr_ob_share_adhoc'
--SELECT p.empid,c.pmonth,c.paydate into #Empids1 from test2.dbo.PAdhslip c 
--join test2.dbo.pempmast p on c.eid=p.eid 
--join employees e on e.empid=p.empid COLLATE SQL_Latin1_General_CP1_CI_AS where p.empid=288 and c.pmonth is not null and c.pmonth>=201904
--print '#Empid';
--WHILE exists (SELECT * from #Empids1)
--begin

--set @id1=(select last_num+1 from new_num where table_name='pr_ob_share_adhoc')
----set @empid1= (select top 1 empid from #Empids1 order by paydate desc)
--set @fm1=(select top 1 pmonth from #Empids1 order by paydate desc)
--set @paydate=(select top 1 paydate from #Empids1 order by paydate desc)

--CREATE table #pr_ob_share_adhoc 

--(id	int,emp_id int, emp_code int, fy int, fm nvarchar(20), own_share decimal(18,2), vpf decimal(18,2), bank_share decimal(18,2), active int, trans_id int, is_interest_caculated decimal(18,2), pension_open decimal(18,2), pension_total decimal(18,2), pension_intrest_amount decimal(18,2),own_share_open decimal(18,2), own_share_total decimal(18,2), vpf_open decimal(18,2), vpf_total decimal(18,2), bank_share_open decimal(18,2), bank_share_total decimal(18,2))

--insert into #pr_ob_share_adhoc
--select @id1,e.id,p.empid,year((cast(left(@fm1,4) as varchar(20))+'-'+ right(@fm1,2)+'-'+'01'))+1,
--(cast(left(@fm1,4) as varchar(20))+'-'+ right(@fm1,2)+'-'+'01'),
--  pf,c.vpf,PFcont,0,1001,0,0,0,0,0,pf,0,c.vpf,0,PFcont from test2.dbo.PAdhslip c 
--join test2.dbo.pempmast p on c.eid=p.eid 
--join employees e on e.empid=p.empid COLLATE SQL_Latin1_General_CP1_CI_AS where p.empid=@empid and c.pmonth=@fm1 and c.paydate=@paydate

--while exists (select top 1 * from #pr_ob_share_adhoc)
--begin

--insert into pr_ob_share_adhoc(id, emp_id, emp_code, fy, fm, own_share, vpf, bank_share, active, trans_id, is_interest_caculated, pension_open, pension_total, pension_intrest_amount, own_share_open, own_share_total, vpf_open,vpf_total, bank_share_open, bank_share_total)

--select * from #pr_ob_share_adhoc;
--print '#pr_ob_share_adhoc';
--delete from #pr_ob_share_adhoc where emp_code=@empid;

--end
--drop table #pr_ob_share_adhoc;
--delete from #Empids1 where empid=@empid and pmonth=@fm1;
--update new_num set last_num=@id1 where table_name='pr_ob_share_adhoc'
--print '#Empids1';
--end
--drop table #Empids1;
--delete from #Empid where empid=@empid;
--end
--print '#Empid Completed';
--drop table #Empid;
insert into pr_ob_share_adhoc(
id	,
emp_id,
emp_code,
fy,
fm,
own_share,
vpf,
bank_share,
active,
trans_id,is_interest_caculated,pension_open,pension_total,pension_intrest_amount,own_share_open,own_share_total,
vpf_open,vpf_total,
bank_share_open,bank_share_total)
select 1,e.id,p.empid,year((cast(left(pmonth,4) as varchar(20))+'-'+ right(pmonth,2)+'-'+'01'))+1,
(cast(left(pmonth,4) as varchar(20))+'-'+ right(pmonth,2)+'-'+'01'),
  pf,c.vpf,PFcont,0,1001,0,0,0,0,0,pf,0,c.vpf,0,PFcont from test2.dbo.PAdhslip c 
join test2.dbo.pempmast p on c.eid=p.eid 
join employees e on e.empid=p.empid COLLATE SQL_Latin1_General_CP1_CI_AS
where  pmonth>=201904 

------------------------------------------------------------------------------------
declare @fmnew nvarchar(20);
declare @empidnew int;
select fm,count(fm) as countofmonths into #obshareadhoc from pr_ob_share_adhoc group by fm having count(fm)>1 order by count(fm) desc

while exists(select * from #obshareadhoc)
begin
set @fmnew=(select top 1 fm from #obshareadhoc)

select emp_code,count(emp_code) as countofempcodes into #obshareadhocnew from pr_ob_share_adhoc where fm=@fmnew and own_share=0 and vpf=0 and bank_share=0 and pension_open =0 and pension_total=0 and pension_intrest_amount =0 and own_share_open=0 and own_share_total=0 and vpf_open=0 and vpf_total=0 and bank_share_open =0 and bank_share_total=0 group by emp_code having count(emp_code)>1;

while exists (select * from #obshareadhocnew)
begin

set @empidnew=(select top 1 emp_code from #obshareadhocnew)

delete top (1) from pr_ob_share_adhoc where fm=@fmnew and emp_code=@empidnew;
delete from #obshareadhocnew where emp_code=@empidnew;
end
drop table #obshareadhocnew;
delete from #obshareadhoc where fm=@fmnew;
end
drop table #obshareadhoc;

DECLARE @counter1 int;
SET @counter1 = 0;
UPDATE pr_ob_share_adhoc SET @counter1 = id = @counter1 + 1;
--exec sp_dm_pf_enash_adhoc