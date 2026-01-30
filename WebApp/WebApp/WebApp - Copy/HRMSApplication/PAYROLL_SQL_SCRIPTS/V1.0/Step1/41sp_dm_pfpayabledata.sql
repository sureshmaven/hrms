CREATE procedure [dbo].[pr_emp_repayable]  @month  varchar(10) as

declare @ids int;
declare @fm NVARCHAR(20);
declare @empid int;
declare @amount decimal(18,2);
declare @amount1 decimal(18,2);

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
(Select left(cast(Pfmonth as varchar), 4)+1),	
(Select concat(left(cast(Pfmonth as varchar), 4),'-',right(cast(Pfmonth  as varchar),2),'-01')),	
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

update new_num set last_num=(case when (select max(id)+1 from pr_emp_pf_repayable_loan) is null then 1 else (select max(id)+1 from pr_emp_pf_repayable_loan) end) where table_name = 'pr_emp_pf_repayable_loan'

while exists (select * from #pfrepayable)
begin
set @ids= (select last_num from new_num where table_name='pr_emp_pf_repayable_loan')
set @empid =(select top 1 emp_code from #pfrepayable)
set @fm=(select top 1 fm from #pfrepayable)
set @amount=(select top 1 basicDA from #pfrepayable)
set @amount1=(select top 1 amount_applied_for from #pfrepayable)

select @ids as id,emp_id, emp_code, fy, fm, pf_account_no, purpose_of_advance, pf_loans_id, amount_applied_for, basicDA, rate_of_interest, amount_applied_for_2, calculating_months, netownshare25netbankshare_3, least_of_3, gross_salary, net_salary, net_minus_pf, rd_of_gross_salary, total_outstanding_loan, amount_recommended_for_sanction, active, authorisation, process, trans_id, sanction_date, process_date into #pfrepayable1 from #pfrepayable where emp_code=@empid and fm=@fm and basicDA=@amount and amount_applied_for=@amount1

Insert into pr_emp_pf_repayable_loan select * from #pfrepayable1;
drop table #pfrepayable1;

set @ids=@ids+1;
update new_num set last_num=@ids where table_name='pr_emp_pf_repayable_loan';
delete from #pfrepayable where emp_code=@empid and fm=@fm and basicDA=@amount and amount_applied_for=@amount1
end
drop table #pfrepayable;

-- declare @t int;
-- set @t =(select top 1 id from pr_emp_pf_repayable_loan  order by id desc)
--  if @t is null
-- begin set @t=0 end;

-- insert into pr_emp_pf_repayable_loan  
-- select *
-- from #pfrepayable pf1 ;
--declare @id int;

--set @id=(select top 1 id from pr_emp_pf_repayable_loan order by id desc);

--if @id is Null
--begin set @id=0
--end

update new_num set last_num=@ids where table_name='pr_emp_pf_repayable_loan';



--delete  from pr_emp_pf_repayable_loan where emp_code=793;
--exec pr_emp_repayable 793,201908
--select * from pr_emp_pf_repayable_loan where emp_code=793;
----select id from pr_purpose_of_advance_master  where  purpose_name=[TEST2].[dbo].non_refAdvances.Purposeofadvance COLLATE SQL_Latin1_General_CP1253_CI_AI  and active=1 and ptype='NONREPAY')


GO


--exec pr_emp_repayable 202007