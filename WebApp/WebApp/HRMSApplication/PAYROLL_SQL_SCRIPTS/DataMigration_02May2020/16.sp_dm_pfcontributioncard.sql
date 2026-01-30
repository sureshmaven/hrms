-- Procedure for PF opening amounts for 2019 year
CREATE procedure [dbo].[sp_dm_pfcontributioncard] @year int as 

CREATE TABLE #pf(id int identity(1,1), [EId] [int] NULL,Empcode int, [year] [int] NULL,[pfprint] Float ,[vpfprint] Float,[BankPrint] Float,[PFint] Float,
[VPFint] Float,[Bankint] Float,PFreturn int,VPFreturn int,Bankreturn int,pfintcurr int,vpfintcurr int,bankintcurr int,intDate Date, pfintrate float, 
active bit, trans_id int) 

insert into #pf select
pf1.Eid,
mas.Empid as empcode,Year,PFprin,VPFprin,Bankprin,PFint,VPFint,Bankint,PFreturn,VPFreturn,Bankreturn,pfintcurr,vpfintcurr,bankintcurr,
case when IntDate is null then '' else IntDate end as IntDate,pfintrate,'1' as active, '101' as trans_id
from [TEST2].[dbo].[PFopen] pf1 join employees mas on pf1.eid=mas.Eid where year=@year;

declare @t int;

 set @t =(select top 1 id from pr_pf_open_bal_year  order by id desc)

 if @t is null
 begin set @t=0 end;

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
trans_id
) select id,Eid,
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
trans_id from #pf;

declare @id int;

set @id=(select top 1 id from pr_pf_open_bal_year order by id desc);

if @id is Null
begin set @id=0
end
update new_num set last_num=@id where table_name='pr_pf_open_bal_year';

drop table #pf;

--exec [sp_dm_pfcontributioncard] 2019

--select * from pr_pf_open_bal_year order by id