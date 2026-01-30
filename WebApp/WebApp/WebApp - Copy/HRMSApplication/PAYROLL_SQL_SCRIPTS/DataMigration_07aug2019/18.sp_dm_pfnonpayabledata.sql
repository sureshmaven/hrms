


create procedure [dbo].[sp_dm_pfnonrepayable]  @empcode int  ,@month  varchar(10) as

CREATE TABLE #pfnonrepayable(id int,[EId] [int] NULL,Empcode int ,fy varchar(10),Pfmonth varchar(10),
PfAccountno varchar(10),Purposeofadvance varchar(50),[BasicDA] Float,
[LLeast] Float,[sactioned] Float,Fownshare Float,Fvpfshare Float,Fbankshare Float,FTotal Float,active bit,
authorisation int,process int, trans_id int,sanction float,SanctionDT Date,ProcessDT Date,
) 


insert into #pfnonrepayable select 1,non_refAdvances.eid,mas.Empid
Empcode,year(CAST((convert(date,convert(date,left(Pfmonth,4)+'-'+right(Pfmonth,2)+'-01'),102)) as varchar)),
CAST((convert(date,convert(date,left(Pfmonth,4)+'-'+right(Pfmonth,2)+'-01'),102)) as varchar)
,PfAccountno,(select id from pr_purpose_of_advance_master  where  purpose_code =non_refAdvances.Purposeofadvance COLLATE SQL_Latin1_General_CP1253_CI_AI  and active=1 and ptype='NONREPAY') as Purposeofadvance,BasicDA,LLeast,sactioned,Fownshare,Fvpfshare,Fbankshare,FTotal,1,1,1,101,
sactioned,
SanctionDT,ProcessDT from [TEST2].[dbo].non_refAdvances non_refAdvances join [TEST2].[dbo].PEMPMAST mas on 
non_refAdvances.eid=mas.Eid where loantype='nonrefund' --and Pfmonth=@month 
 and mas.empid=@empcode



 declare @t int;
 set @t =(select top 1 id from pr_emp_pf_nonrepayable_loan  order by id desc)
  if @t is null
 begin set @t=0 end;

 insert into pr_emp_pf_nonrepayable_loan  
 select @t+1,pf1.EId,Empcode,year(CAST((convert(date,convert(date,left(pf1.Pfmonth,4)+'-'+right(pf1.Pfmonth,2)+'-01'),102)) 
 as varchar)),CAST((convert(date,convert(date,left(pf1.Pfmonth,4)+'-'+right(pf1.Pfmonth,2)+'-01'),102)) 
 as varchar),pf1.PfAccountno,pf1.Purposeofadvance,pf1.BasicDA,pf1.LLeast,pf1.sactioned,pf1.Fownshare,pf1.Fvpfshare,
 pf1.Fbankshare,pf1.FTotal,0,1,1,trans_id,pf1.sactioned,pf1.SanctionDT,pf1.ProcessDT,0,0,0,0
 from #pfnonrepayable pf1 ;
declare @id int;

set @id=(select top 1 id from pr_emp_pf_nonrepayable_loan order by id desc);

if @id is Null
begin set @id=0
end

update new_num set last_num=@id where table_name='pr_emp_pf_nonrepayable_loan';

drop table #pfnonrepayable;

--delete  from pr_emp_pf_nonrepayable_loan where emp_code=793;
--exec sp_dm_pfnonrepayable 793,201908
--select * from pr_emp_pf_nonrepayable_loan where emp_code=793;
----select id from pr_purpose_of_advance_master  where  purpose_name=[TEST2].[dbo].non_refAdvances.Purposeofadvance COLLATE SQL_Latin1_General_CP1253_CI_AI  and active=1 and ptype='NONREPAY')


GO


