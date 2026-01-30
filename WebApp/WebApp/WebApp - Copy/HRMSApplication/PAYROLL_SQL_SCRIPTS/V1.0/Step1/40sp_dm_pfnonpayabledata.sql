CREATE procedure [dbo].[sp_dm_pfnonrepayable]  @month  varchar(10) as

CREATE TABLE #pfnonrepayable(id int,[EId] [int] NULL,Empcode int ,fy varchar(10),Pfmonth varchar(10),
PfAccountno varchar(10),Purposeofadvance varchar(50),[BasicDA] Float,
[LLeast] Float,[sactioned] Float,Fownshare Float,Fvpfshare Float,Fbankshare Float,FTotal Float,active bit,
authorisation int,process int, trans_id int,sanction float,SanctionDT Date,ProcessDT Date,
) 


insert into #pfnonrepayable select 1,non_refAdvances.eid,mas.Empid
Empcode,left(Pfmonth,4)+1 as fy,
CAST((convert(date,convert(date,left(Pfmonth,4)+'-'+right(Pfmonth,2)+'-01'),102)) as varchar) as fm, PfAccountno,(select id from pr_purpose_of_advance_master  where  purpose_code =non_refAdvances.Purposeofadvance COLLATE SQL_Latin1_General_CP1253_CI_AI  and active=1 and ptype='NONREPAY') as Purposeofadvance,BasicDA,LLeast,sactioned,Fownshare,Fvpfshare,Fbankshare,FTotal,1,1,1,101,
sactioned,
SanctionDT,ProcessDT from [TEST2].[dbo].non_refAdvances non_refAdvances join [TEST2].[dbo].PEMPMAST mas on 
non_refAdvances.eid=mas.Eid where loantype='nonrefund' and Pfmonth between '201904' and @month 

update new_num set last_num=(case when (select max(id)+1 from pr_emp_pf_nonrepayable_loan) is null then 1 else (select max(id)+1 from pr_emp_pf_nonrepayable_loan) end) where table_name = 'pr_emp_pf_nonrepayable_loan'
--select * from #pfnonrepayable
 declare @t int;
 declare @empids int;
 declare @pfaccno bigint;
 declare @basic decimal(18,2);
 declare @eligamount decimal(18,2);
 
 set @t=(select last_num+1 from new_num where table_name='pr_emp_pf_nonrepayable_loan')
 while exists (select * from #pfnonrepayable)
 begin
 
 set @empids= (select top 1 Empcode from #pfnonrepayable)
 set @pfaccno= (select top 1 PfAccountno from #pfnonrepayable)
  set @basic= (select top 1 BasicDA from #pfnonrepayable)
  set @eligamount= (select top 1 LLeast from #pfnonrepayable)
 
 --select * from pr_emp_pf_nonrepayable_loan
 select @t as id,Eid as emp_id,Empcode as emp_code,fy,Pfmonth as fm,PfAccountno as pf_account_no,Purposeofadvance as purpose_of_advance,BasicDA as rate_of_basic_da,LLeast as eligibility_amount,sactioned as amount_applied,Fownshare as own_share,Fvpfshare as vpf, 
 Fbankshare as bank_share ,FTotal as total,
 0 as active,1 as authorisation ,1 as process,trans_id,sactioned as sanctioned_amount,SanctionDT as sanction_date,ProcessDT as process_date,0 as bankshare_interest ,0 as ownshare_interest ,0 as vpf_interest ,0 as is_interest_calculated into #pfnonrepayable1 from #pfnonrepayable where Empcode=@empids and PfAccountno=@pfaccno and BasicDA=@basic and LLeast=@eligamount;

 Insert into pr_emp_pf_nonrepayable_loan select * from #pfnonrepayable1;
drop table #pfnonrepayable1;

 --insert into pr_emp_pf_nonrepayable_loan  
 --select @t,pf1.EId,Empcode,year(CAST((convert(date,convert(date,left(pf1.Pfmonth,4)+'-'+right(pf1.Pfmonth,2)+'-01'),102)) 
 --as varchar)),CAST((convert(date,convert(date,left(pf1.Pfmonth,4)+'-'+right(pf1.Pfmonth,2)+'-01'),102)) 
 --as varchar),pf1.PfAccountno,pf1.Purposeofadvance,pf1.BasicDA,pf1.LLeast,pf1.sactioned,pf1.Fownshare,pf1.Fvpfshare,
 --pf1.Fbankshare,pf1.FTotal,0,1,1,trans_id,pf1.sactioned,pf1.SanctionDT,pf1.ProcessDT,0,0,0,0
 --from #pfnonrepayable pf1 where pf1.Empcode=@empids and pf1.PfAccountno=@pfaccno;

 set @t=@t+1;
 update new_num set last_num=@t where table_name='pr_emp_pf_nonrepayable_loan';
 delete from #pfnonrepayable where Empcode=@empids and PfAccountno=@pfaccno and BasicDA=@basic  and LLeast=@eligamount;
 end

--declare @id int;

--set @id=(select top 1 id from pr_emp_pf_nonrepayable_loan order by id desc);

--if @id is Null
--begin set @id=0
--end

update new_num set last_num=@t where table_name='pr_emp_pf_nonrepayable_loan';

drop table #pfnonrepayable;

--delete  from pr_emp_pf_nonrepayable_loan where emp_code=793;
--exec sp_dm_pfnonrepayable 202007
--select * from pr_emp_pf_nonrepayable_loan where emp_code=793;
----select id from pr_purpose_of_advance_master  where  purpose_name=[TEST2].[dbo].non_refAdvances.Purposeofadvance COLLATE SQL_Latin1_General_CP1253_CI_AI  and active=1 and ptype='NONREPAY')


GO


