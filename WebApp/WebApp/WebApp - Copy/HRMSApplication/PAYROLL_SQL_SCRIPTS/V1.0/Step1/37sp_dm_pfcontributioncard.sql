-- Procedure for PF opening amounts for 2019 year
CREATE procedure [dbo].[sp_dm_pfcontributioncard] @year int as 

CREATE TABLE #pf(id int identity(1,1), [EId] [int] NULL,Empcode int, [year] [int] NULL,[pfprint] Float ,[vpfprint] Float,[BankPrint] Float,[PFint] Float,
[VPFint] Float,[Bankint] Float,PFreturn Float,VPFreturn Float,Bankreturn Float,pfintcurr int,vpfintcurr int,bankintcurr int,intDate Date, pfintrate float, 
active bit, trans_id int) 

insert into #pf select
pf1.Eid,
mas.Empid as empcode,Year,PFprin,VPFprin,Bankprin,PFint,VPFint,Bankint,PFreturn,VPFreturn,Bankreturn,pfintcurr,vpfintcurr,bankintcurr,
case when IntDate is null then '' else IntDate end as IntDate,pfintrate,'1' as active, '101' as trans_id
from [TEST2].[dbo].[PFopen] pf1 join  [TEST2].[dbo].[pempmast] mas on pf1.eid=mas.Eid where year=@year;

--declare @t int;

-- set @t =(select top 1 id from pr_pf_open_bal_year  order by id desc)

-- if @t is null
-- begin set @t=0 end;
select empcode as emp_code into #pfid from #pf
--select * from #pfid
 declare @id int;
 declare @emp_code int;
 set @id=(select last_num+1 from new_num where table_name='pr_pf_open_bal_year')
 if @id is null
 begin set @id=1 end;

 while exists(select top 1 emp_code from #pfid)
 begin
 set @emp_code=(select top 1 emp_code from #pfid)
insert into [pr_pf_open_bal_year](id	,
emp_id	,
emp_code	,
Fy	,
os_open	,
os_open_int	,
bs_open	,
bs_open_int	,
vpf_open	,
vpf_open_int	,
pf_return,
vpf_return,
bank_return,
pf_int_rate	,
trans_id,
os_cur_int,bs_cur_int,vpf_cur_int
) select @id,Eid,
empcode,Year,
pfprint,
PFint,
Bankprint,
Bankint,
VPFprint,
VPFint,
PFreturn,
VPFreturn,
Bankreturn,
pfintrate,
trans_id,
pfintcurr,bankintcurr,vpfintcurr from #pf where empcode=@emp_code;
delete from #pfid where emp_code=@emp_code;
--declare @id int;

--set @id=(select top 1 id from pr_pf_open_bal_year order by id desc);
set @id=@id+1;
--if @id is Null
--begin set @id=0
--end
end
drop table #pf;
drop table #pfid;
update new_num set last_num=@id-1 where table_name='pr_pf_open_bal_year';
--exec [sp_dm_pfcontributioncard] 2019

--select * from pr_pf_open_bal_year order by id