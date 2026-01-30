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



DECLARE @counter int;
SET @counter = 0;
UPDATE pr_ob_share_adhoc SET @counter = id = @counter + 1;








