create procedure [dbo].[pr_emp_repayable]  @month  varchar(10) as

CREATE TABLE #pfrepayable(id	int ,
emp_id	int	,
emp_code	int	,
fy	varchar(10)	,
fm	varchar(10)	,
pf_account_no	varchar(10)	,
purpose_of_advance	varchar(50)	,
pf_loans_id	varchar(50)	,
amount_applied_for	float	,
basicDA	float	,
rate_of_interest	float	,
amount_applied_for_2	float	,
calculating_months	int	,
netownshare25netbankshare_3	float	,
least_of_3	float	,
gross_salary	float	,
net_salary	float	,
net_minus_pf	float	,
rd_of_gross_salary	float	,
total_outstanding_loan	float	,
amount_recommended_for_sanction	float	,
active	bit	,
authorisation	bit	,
process	bit	,
trans_id	int	,
sanction_date	Date	,
process_date	Date	
) 


insert into #pfrepayable select 1,
mas.eid,	
empid,	
(Select left(cast(Pfmonth as varchar), 4)),	
(Select concat(left(cast(Pfmonth as varchar), 4),'-04-01')),	
PfAccountno	,
(select id from pr_purpose_of_advance_master  where  purpose_code =non_refAdvances.Purposeofadvance COLLATE SQL_Latin1_General_CP1253_CI_AI  and active=1 and ptype='REPAY'),
case when pfloantype='PF Loan ST 1' then 'PFL1' else 'PFL2' end as loan_type,	
LLeast	,
BasicDA	,
ROI	,
LLeast	,
NoOfMonths	,
ownbankshare	,
LLeast	,
grpay	,
netpay	,
netpaympf	,
otgrpay	,
FTotal	,
sactioned	,
1	,
1	,
1	,
101	,
SanctionDT	,
ProcessDT	
 from [TEST2].[dbo].non_refAdvances non_refAdvances join [TEST2].[dbo].PEMPMAST mas on 	
non_refAdvances.eid=mas.Eid where loantype='refund' and Pfmonth between '201904' and @month	



 declare @t int;
 set @t =(select top 1 id from pr_emp_pf_nonrepayable_loan  order by id desc)
  if @t is null
 begin set @t=0 end;

 insert into pr_emp_pf_repayable_loan  
 select *
 from #pfrepayable pf1 ;
declare @id int;

set @id=(select top 1 id from pr_emp_pf_nonrepayable_loan order by id desc);

if @id is Null
begin set @id=0
end

update new_num set last_num=@id where table_name='pr_emp_pf_repayable_loan';



--delete  from pr_emp_pf_repayable_loan where emp_code=793;
--exec pr_emp_repayable 793,201908
--select * from pr_emp_pf_repayable_loan where emp_code=793;
----select id from pr_purpose_of_advance_master  where  purpose_name=[TEST2].[dbo].non_refAdvances.Purposeofadvance COLLATE SQL_Latin1_General_CP1253_CI_AI  and active=1 and ptype='NONREPAY')


GO


--exec pr_emp_repayable 202005