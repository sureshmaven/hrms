
CREATE procedure [dbo].[sp_dm_pfcontributioncard] @empcode int, @year int as 

CREATE TABLE #pf([EId] [int] NULL,Empcode int, [year] [int] NULL,[pfprint] Float ,[vpfprint] Float,[BankPrint] Float,[PFint] Float,
[VPFint] Float,[Bankint] Float,PFreturn int,VPFreturn int,Bankreturn int,pfintcurr int,vpfintcurr int,bankintcurr int,intDate Date, pfintrate int, 
active bit, trans_id int) 

insert into #pf select
pf1.Eid,
mas.Empid as empcode,Year,PFprin,VPFprin,Bankprin,PFint,VPFint,Bankint,PFreturn,VPFreturn,Bankreturn,pfintcurr,vpfintcurr,bankintcurr,
case when IntDate is null then '' else IntDate end as IntDate,pfintrate,'1' as active, '101' as trans_id
from [TEST2].[dbo].[PFopen] pf1 join [TEST2].[dbo].PEMPMAST mas on pf1.eid=mas.Eid where year=@year and mas.empid=@empcode;

declare @t int;

 set @t =(select top 1 id from pr_pfopeningbal  order by id desc)

 if @t is null
 begin set @t=0 end;

 insert into [pr_pfopeningbal] select @t+1,Eid,
empcode,Year,pfprint,VPFprint,Bankprint,PFint,VPFint,Bankint,PFreturn,VPFreturn,Bankreturn,pfintcurr,vpfintcurr,bankintcurr,
cast(IntDate as date) ,pfintrate,active,trans_id from #pf;

declare @id int;

set @id=(select top 1 id from pr_pfopeningbal order by id desc);

if @id is Null
begin set @id=0
end
update new_num set last_num=@id where table_name='pr_pfopeningbal';

drop table #pf;

--exec sp_dm_pfcontributioncard 544,2012

--exec [sp_dm_pfcontributioncard] 271,2019



