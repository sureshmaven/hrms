CREATE PROCEDURE [dbo].[sp_dm_sub_loans_hl2bc] @emp_code integer,  @fmn integer
AS  
--all two sub loans codes
create table #two_sub(
loan_id varchar(20)
);

declare @l_id varchar(20);
--select * from pr_loan_master;
insert into #two_sub select distinct(LoanId) as loan_id from [TEST2].dbo.PLoan where LoanId='HL2BC';
while exists(select top 1 * from #two_sub)
begin

select top 1 @l_id = loan_id from #two_sub;

declare @loan_code_mid int;
set @loan_code_mid = (select id from pr_loan_master where loan_id=@l_id);

--business logic
-- Main Table Insertion ----
declare @loan_code varchar(20);
declare @total_amount int;
declare @total_installment int;
declare @remaining_installment int; 
declare @principal_installment int;
declare @interest_installment int;
declare @completed_installment int;
declare @sanction_date date;
declare @installment_start_date date;
declare @installment_end_date date;
declare @method nvarchar(50);
declare @interest_rate float;
declare @installment_amount float;
declare @interest_installment_amount float; 
declare @total_recovered_amount float;
declare @vender_name varchar(30);
declare @os_principal_amount int; 
declare @fm date;
declare @fy int;
declare @principal_recoverd int;
declare @remaing_amount int;
declare @date_disburse date;
declare @loanslno int;
declare @os_total_balace_amount int;

select 
@emp_code =CAST(m.Empid as varchar), 
@loan_code=CAST(l.LoanId as varchar),
@total_amount=l.Amount,
@total_installment=Period+IntPeriod,
@remaining_installment=Period+IntPeriod-l.InstNo,
@principal_installment=CAST(l.Period as varchar),
@interest_installment=IntPeriod,
@completed_installment=CAST(l.InstNo as varchar),
@sanction_date=cast(l.SanDate as Date),
@installment_start_date=cast(CASE WHEN LoanStart = '' or LoanStart is null then cast(l.SanDate as Date) 
 else convert(date,convert(date,left(LoanStart,4)+'-'+right(LoanStart,2)+'-01'),102) 
 end as varchar),
@installment_end_date = CAST(+cast(case when LoanStart is null or LoanStart='' then SanDate else 
DATEADD(month, Period,convert(date,convert(date,left(LoanStart,4)+'-'+right(LoanStart,2)+'-01'),102))  end as Date) as varchar), 
@method='Inrtrest to be recovered at end',
@interest_rate = CAST(l.IntRate as varchar),
@installment_amount=CAST(l.InstAmt as varchar),
@interest_installment_amount = CAST(l.IntAmount as varchar),
@total_recovered_amount=CAST(l.Recovery as varchar),
@principal_recoverd = CAST(l.CumLoanRepaid as int),
@remaing_amount = CAST(ceiling(cast(Amount as int))- (cast(l.Recovery as int)) as int),
@vender_name = CAST(l.SubLoanId as varchar),
@os_principal_amount=Amount-CumLoanRepaid,
@date_disburse=cast(CASE WHEN LoanStart = '' or LoanStart is null then cast(l.SanDate as Date) 
 else convert(date,convert(date,left(LoanStart,4)+'-'+right(LoanStart,2)+'-01'),102) 
 end as varchar),
@fm= CAST((convert(date,convert(date,left(PMonth,4)+'-'+right(PMonth,2)+'-01'),102)) as varchar),
@fy=CAST(year(convert(date,convert(date,left(PMonth,4)+'-'+right(PMonth,2)+'-01'),102)) as varchar),
@loanslno = l.LoanSlNo
 from   TEST2.dbo.PEmpMast m  join  TEST2.dbo.PLoan l  on m.Eid=l.Eid 
where l.LoanId=@l_id and  m.EmpId = @emp_code order by l.Eid


if not exists (select * from pr_emp_adv_loans p join pr_loan_master m on p.loan_type_mid=m.id where p.emp_code = @emp_code and m.loan_id=@l_id and loan_sl_no = @loanslno)
	begin
--select * from Employees;
  if exists (select * from test2.dbo.PLoan p join test2.dbo.PEMPMAST m on p.Eid=m.eid where p.LoanId=@l_id and m.Empid=@emp_code)
  begin

exec sp_insert_two_sub_Loans @emp_code ,@loan_code_mid,  @total_amount, @total_installment, @remaining_installment, 
@principal_installment, @interest_installment,@completed_installment, @sanction_date, @installment_start_date,
@installment_end_date, @method, @interest_rate, @installment_amount, @interest_installment_amount, 
@total_recovered_amount, @vender_name, @fm, @fy,@loanslno

--- Priority 1----

if exists (select * from test2.dbo.PLoandet where eid = (select eid from test2.dbo.PEMPMAST where Empid=@emp_code) and LoanId = @l_id and priority=1)
  begin
declare @loan_amount int;  
declare @interest_rate_one float;
declare @interest_accured float; 
declare @principal_amount_recovered_one int;
declare @interest_amount_recovered_one float; 
declare @total_amount_recovered_one float;
declare @principal_start_date date;
declare @principal_end_date date;
declare @interest_start_date_one date; 
declare @interest_end_date_one date;
declare @total_principal_installments int;
declare @total_interest_installments int; 
declare @os_interest_amount float;


select 
 @emp_code=CAST(m.Empid as varchar),
 @loan_code=d.LoanId,
 @date_disburse=cast(CASE WHEN LoanStart = '' or LoanStart is null then cast(l.SanDate as Date) 
 else convert(date,convert(date,left(LoanStart,4)+'-'+right(LoanStart,2)+'-01'),102) 
 end as varchar),
 @loan_amount=d.Amount,
 @interest_rate_one=d.IntRate,
 @interest_accured=d.IntAmount,
 @principal_amount_recovered_one=d.CumLoanRepaid,
 @interest_amount_recovered_one=d.CumIntRepaid,
 @total_amount_recovered_one=d.Recovery,
 @principal_start_date=cast(CASE WHEN LoanStart = '' or LoanStart is null then 
 cast(l.SanDate as Date) else convert(date,convert(date,left(LoanStart,4)+'-'+right(LoanStart,2)+'-01'),102) end as varchar),
 @principal_end_date=CAST(Dateadd(month,ceiling(cast(period as int)/4*(3)), cast(CASE WHEN LoanStart = '' 
 or LoanStart is null then cast(l.SanDate as Date) else convert(date,
 convert(date,left(LoanStart,4)+'-'+right(LoanStart,2)+'-01'),102) end as Date)) as varchar),
 @interest_start_date_one=CAST(Dateadd(month,ceiling(cast(period as int)+1), cast(CASE WHEN LoanStart = '' or LoanStart is null then cast(l.SanDate as Date) else convert(date,
 convert(date,left(LoanStart,4)+'-'+right(LoanStart,2)+'-01'),102) end as Date)) as varchar),
 @interest_end_date_one=CAST(Dateadd(month,ceiling(cast(period as int)+1+ceiling(cast(IntPeriod as int)/4*(3))), 
 cast(CASE WHEN LoanStart = '' or LoanStart is null then cast(l.SanDate as Date) else 
 convert(date,convert(date,left(LoanStart,4)+'-'+right(LoanStart,2)+'-01'),102) end as Date)) as varchar),
 @total_principal_installments=Period,
 @total_interest_installments=IntPeriod,
 @os_principal_amount=d.Amount-d.CumLoanRepaid,
 @os_interest_amount=d.IntAmount-d.CumIntRepaid,
 @os_total_balace_amount = d.Amount-d.CumLoanRepaid+d.IntAmount-d.CumIntRepaid
from [TEST2].dbo.PLoanDet d join [TEST2].dbo.PEMPMAST m on d.eid=m.eid join [TEST2].dbo.PLoan l on d.loanslno=l.LoanSlNo 
where 
d.Priority=1 and  m.EmpId = @emp_code  and  l.LoanId = @l_id

exec sp_insert_two_sub_Loans_priority1 @emp_code, @loan_code, @date_disburse, @loan_amount, @interest_rate_one,
@interest_accured, @principal_amount_recovered_one, @interest_amount_recovered_one, @total_amount_recovered_one,
@principal_start_date ,@principal_end_date,@interest_start_date_one,@interest_end_date_one,
@total_principal_installments,@total_interest_installments, @os_principal_amount, @os_interest_amount, @os_total_balace_amount, @loanslno 

---- Priority 2 ---------
end
if exists (select * from test2.dbo.PLoandet where eid = (select eid from test2.dbo.PEMPMAST where Empid=@emp_code) and LoanId = @l_id and priority=2)
begin
declare @date_disburse_two date; 
declare @loan_amount_two int;  
declare @interest_rate_two float;
declare @interest_accured_two float; 
declare @principal_amount_recovered_two int;
declare @interest_amount_recovered_two float; 
declare @total_amount_recovered_two float;
declare @principal_start_date_two date;
declare @principal_end_date_two date;
declare @interest_start_date_two date; 
declare @interest_end_date_two date;
declare @total_principal_installments_two int;
declare @total_interest_installments_two int; 
declare @os_principal_amount_two int; 
declare @os_interest_amount_two float;


select 
 @emp_code=CAST(m.Empid as varchar),
 @loan_code=d.LoanId,
 @date_disburse_two=cast(CASE WHEN LoanStart = '' or LoanStart is null then cast(l.SanDate as Date) 
 else convert(date,convert(date,left(LoanStart,4)+'-'+right(LoanStart,2)+'-01'),102) 
 end as varchar),
 @loan_amount_two=d.Amount,
 @interest_rate_two=d.IntRate,
 @interest_accured_two=d.IntAmount,
 @principal_amount_recovered_two=d.CumLoanRepaid,
 @interest_amount_recovered_two=d.CumIntRepaid,
 @total_amount_recovered_two=d.Recovery,
 @principal_start_date_two=cast(CASE WHEN LoanStart = '' or LoanStart is null then 
 cast(l.SanDate as Date) else convert(date,convert(date,left(LoanStart,4)+'-'+right(LoanStart,2)+'-01'),102) end as varchar),
 @principal_end_date_two=CAST(Dateadd(month,ceiling(cast(period as int)/4*(3)), cast(CASE WHEN LoanStart = '' 
 or LoanStart is null then cast(l.SanDate as Date) else convert(date,
 convert(date,left(LoanStart,4)+'-'+right(LoanStart,2)+'-01'),102) end as Date)) as varchar),
 @interest_start_date_two=CAST(Dateadd(month,ceiling(cast(period as int)+1), cast(CASE WHEN LoanStart = '' or LoanStart is null then cast(l.SanDate as Date) else convert(date,
 convert(date,left(LoanStart,4)+'-'+right(LoanStart,2)+'-01'),102) end as Date)) as varchar),
 @interest_end_date_two=CAST(Dateadd(month,ceiling(cast(period as int)+1+ceiling(cast(IntPeriod as int)/4*(3))), 
 cast(CASE WHEN LoanStart = '' or LoanStart is null then cast(l.SanDate as Date) else 
 convert(date,convert(date,left(LoanStart,4)+'-'+right(LoanStart,2)+'-01'),102) end as Date)) as varchar),
 @total_principal_installments_two=Period,
 @total_interest_installments_two=IntPeriod,
 @os_principal_amount_two=d.Amount-d.CumLoanRepaid,
 @os_interest_amount_two=d.IntAmount-d.CumIntRepaid,
 @os_total_balace_amount = d.Amount-d.CumLoanRepaid+d.IntAmount-d.CumIntRepaid
from [TEST2].dbo.PLoanDet d join [TEST2].dbo.PEMPMAST m on d.eid=m.eid join [TEST2].dbo.PLoan l on d.loanslno=l.LoanSlNo 
where 
d.Priority=2 and  m.EmpId = @emp_code  and  l.LoanId = @l_id

exec sp_insert_two_sub_Loans_priority2 @emp_code, @loan_code, @date_disburse_two, @loan_amount_two, @interest_rate_two,
@interest_accured_two, @principal_amount_recovered_two, @interest_amount_recovered_two, @total_amount_recovered_two,
@principal_start_date_two ,@principal_end_date_two, @interest_start_date_two,@interest_end_date_two,
@total_principal_installments_two,@total_interest_installments_two, @os_principal_amount_two, @os_interest_amount_two,@os_total_balace_amount,@loanslno 
end 

--- check without two subloans and insert in child table ---

if not exists (select * from test2.dbo.PLoanDET where eid = (select eid from test2.dbo.PEMPMAST where Empid=@emp_code) and loanslno = @loanslno)
  begin
select 
 @emp_code=CAST(m.Empid as varchar),
 @loan_code=LoanId,
 @date_disburse=cast(CASE WHEN LoanStart = '' or LoanStart is null then cast(l.SanDate as Date) 
 else convert(date,convert(date,left(LoanStart,4)+'-'+right(LoanStart,2)+'-01'),102) 
 end as varchar),
 @loan_amount=l.Amount,
 @interest_rate_one=l.IntRate,
 @interest_accured=l.IntAmount,
 @principal_amount_recovered_one=l.CumLoanRepaid,
 @interest_amount_recovered_one=l.CumIntRepaid,
 @total_amount_recovered_one=Recovery,
 @principal_start_date=cast(CASE WHEN LoanStart = '' or LoanStart is null then 
 cast(l.SanDate as Date) else convert(date,convert(date,left(LoanStart,4)+'-'+right(LoanStart,2)+'-01'),102) end as varchar),
 @principal_end_date=CAST(Dateadd(month,ceiling(cast(period as int)/4*(3)), cast(CASE WHEN LoanStart = '' 
 or LoanStart is null then cast(l.SanDate as Date) else convert(date,
 convert(date,left(LoanStart,4)+'-'+right(LoanStart,2)+'-01'),102) end as Date)) as varchar),
 @interest_start_date_one=CAST(Dateadd(month,ceiling(cast(period as int)+1), cast(CASE WHEN LoanStart = '' or LoanStart is null then cast(l.SanDate as Date) else convert(date,
 convert(date,left(LoanStart,4)+'-'+right(LoanStart,2)+'-01'),102) end as Date)) as varchar),
 @interest_end_date_one=CAST(Dateadd(month,ceiling(cast(period as int)+1+ceiling(cast(IntPeriod as int)/4*(3))), 
 cast(CASE WHEN LoanStart = '' or LoanStart is null then cast(l.SanDate as Date) else 
 convert(date,convert(date,left(LoanStart,4)+'-'+right(LoanStart,2)+'-01'),102) end as Date)) as varchar),
 @total_principal_installments=Period,
 @total_interest_installments=IntPeriod,
 @os_principal_amount=Amount-CumLoanRepaid,
 @os_interest_amount=IntAmount-CumIntRepaid,
 @os_total_balace_amount = Amount-CumLoanRepaid+IntAmount-CumIntRepaid
from [TEST2].dbo.PLoan l join [TEST2].dbo.PEMPMAST m on l.eid=m.eid
where 
  m.EmpId = @emp_code  and  l.LoanId = @l_id

exec sp_insert_two_sub_Loans_priority1 @emp_code, @loan_code, @date_disburse, @loan_amount, @interest_rate_one,
@interest_accured, @principal_amount_recovered_one, @interest_amount_recovered_one, @total_amount_recovered_one,
@principal_start_date ,@principal_end_date,@interest_start_date_one,@interest_end_date_one,
@total_principal_installments,@total_interest_installments, @os_principal_amount, @os_interest_amount,@os_total_balace_amount,@loanslno 
end 

end
end

---- Insertin in loan adjustments table ----
declare @total_interest_installments_adj int;
declare @interest_accured_adj int;
declare @interest_open_amount int;
declare @installments_paid_amount_adj int;
declare @interest_amount_recovered_adj int;
declare @os_interest_amount_adj int;
declare @principal_amount_recovered_one_adj int;
declare @remaing_amount_adj int;
declare @fest_recovery int;
declare @fm_prev date;
declare @os_priciapl_open_amount float;
declare @priority int;
--declare @total_recovered_amount float;
declare @remaing_amount_adj_festival int;
declare @principal_paid_amount int;

select 
@os_priciapl_open_amount=Amount-CumLoanRepaid,
@total_recovered_amount=CumLoanRepaid,
@remaing_amount_adj_festival = Amount-CumLoanRepaid-InstAmt
from   TEST2.dbo.PEmpMast m  join  TEST2.dbo.PLoan l  on m.Eid=l.Eid 
where l.LoanId=@l_id and  m.EmpId = @emp_code order by l.Eid

if exists (select * from test2.dbo.PLoanDet where eid = (select eid from test2.dbo.PEMPMAST where Empid=@emp_code) and LoanId = @l_id and  loanslno = @loanslno
and priority = 1 )
begin

select 
 --@os_priciapl_open_amount=d.Amount-d.CumLoanRepaid,
  @os_priciapl_open_amount=d.Amount-d.CumLoanRepaid+d.LoanRepaid,
 @remaing_amount_adj_festival = d.Amount-d.CumLoanRepaid,
 @interest_accured_adj=d.IntAccured,
 @interest_open_amount = d.IntAmount-d.CumIntRepaid+d.IntRepaid,
 @total_interest_installments_adj=CAST(ceiling(cast(IntPeriod as int)/4*(3)) as varchar),
 @interest_amount_recovered_adj=d.IntRepaid,
 @os_interest_amount_adj=d.IntAmount-d.CumIntRepaid,
 @principal_amount_recovered_one_adj=d.IntAmount-d.CumIntRepaid-d.IntRepaid,
 @fest_recovery = l.recovery,
 @remaing_amount_adj = CAST(ceiling(cast(l.Amount as int))- (cast(d.CumLoanRepaid as int)) as int),
 @priority = d.priority,
 @principal_paid_amount=d.LoanRepaid
from [TEST2].dbo.PLoanDet d join [TEST2].dbo.PEMPMAST m on d.eid=m.eid join [TEST2].dbo.PLoan l on d.loanslno=l.LoanSlNo 
where 
d.Priority=1 and  m.EmpId = @emp_code  and  l.LoanId = @l_id and d.loanslno = @loanslno

exec sp_insert_loans_adjustments @emp_code, @loan_code, @os_priciapl_open_amount, @principal_paid_amount,   @interest_accured_adj,  @interest_open_amount,
 @interest_amount_recovered_adj,  @os_interest_amount_adj, @completed_installment,  @installment_amount,  @principal_amount_recovered_one_adj, 
 @remaing_amount_adj, @fest_recovery, @total_recovered_amount, @remaing_amount_adj_festival, @fm, @fy,@loanslno,@priority
end

--- inserting in loan adj priority 2 ---
if exists (select * from test2.dbo.PLoanDet where eid = (select eid from test2.dbo.PEMPMAST where Empid=@emp_code) and LoanId = @l_id and  loanslno = @loanslno
and priority = 2 )
begin
select 
 --@os_priciapl_open_amount=d.Amount-d.CumLoanRepaid-d.LoanRepaid,
  @os_priciapl_open_amount=d.Amount-d.CumLoanRepaid+d.LoanRepaid,
 @remaing_amount_adj_festival = d.Amount-d.CumLoanRepaid,
 @interest_accured_adj=d.IntAccured,
 @interest_open_amount = d.IntAmount-d.CumIntRepaid+d.IntRepaid,
 @total_interest_installments_adj=CAST(ceiling(cast(IntPeriod as int)/4*(3)) as varchar),
 @interest_amount_recovered_adj=d.IntRepaid,
 @os_interest_amount_adj=d.IntAmount-d.CumIntRepaid,
 @principal_amount_recovered_one_adj=d.IntAmount-d.CumIntRepaid-d.IntRepaid,
 @fest_recovery = l.recovery,
 @remaing_amount_adj = CAST(ceiling(cast(l.Amount as int))- (cast(d.CumLoanRepaid as int)) as int),
 @priority = d.priority,
 @principal_paid_amount=d.LoanRepaid
from [TEST2].dbo.PLoanDet d join [TEST2].dbo.PEMPMAST m on d.eid=m.eid join [TEST2].dbo.PLoan l on d.loanslno=l.LoanSlNo 
where 
d.Priority=2 and  m.EmpId = @emp_code  and  l.LoanId = @l_id and d.loanslno = @loanslno

exec sp_insert_loans_adjustments @emp_code, @loan_code, @os_priciapl_open_amount, @principal_paid_amount,   @interest_accured_adj,  @interest_open_amount,
 @interest_amount_recovered_adj,  @os_interest_amount_adj, @completed_installment,  @installment_amount,  @principal_amount_recovered_one_adj, 
 @remaing_amount_adj, @fest_recovery, @total_recovered_amount, @remaing_amount_adj_festival, @fm, @fy,@loanslno,@priority
end

--- check without subloans -----

if not exists (select * from test2.dbo.PLoanDET where eid = (select eid from test2.dbo.PEMPMAST where Empid=@emp_code) and loanslno = @loanslno)
  begin
select 
 --@os_priciapl_open_amount=d.Amount-d.CumLoanRepaid,
 @os_priciapl_open_amount=Amount-CumLoanRepaid+LoanRepaid,
 @remaing_amount_adj_festival = Amount-CumLoanRepaid,
 @interest_accured_adj=IntAccured,
 @interest_open_amount = IntAmount-CumIntRepaid+IntRepaid,
 @total_interest_installments_adj=CAST(ceiling(cast(IntPeriod as int)/4*(3)) as varchar),
 @interest_amount_recovered_adj=IntRepaid,
 @os_interest_amount_adj=IntAmount-CumIntRepaid,
 @principal_amount_recovered_one_adj=IntAmount-CumIntRepaid-IntRepaid,
 @fest_recovery = l.recovery,
 @remaing_amount_adj = CAST(ceiling(cast(l.Amount as int))- (cast(CumLoanRepaid as int)) as int),
 @priority = 0,
 @principal_paid_amount= LoanRepaid
from [TEST2].dbo.PLoan l join [TEST2].dbo.PEMPMAST m on l.eid=m.eid 
where  m.EmpId = @emp_code  and  l.LoanId = @l_id and loanslno = @loanslno

	 exec sp_insert_loans_adjustments @emp_code, @loan_code, @os_priciapl_open_amount, @principal_paid_amount,@interest_accured_adj, @interest_open_amount,
		 @interest_amount_recovered_adj, @os_interest_amount_adj,@completed_installment,   @installment_amount,   @principal_amount_recovered_one_adj,   
		 @remaing_amount_adj, @fest_recovery, @total_recovered_amount,  @remaing_amount_adj_festival, @fm, @fy,@loanslno,@priority
 end


---- Check for prevois loands data ----

 declare @pmn int;
 set @pmn = @fmn-1;
--check for prev. month adj.
 while(@pmn>=201904)
 begin 

 --- Check priority 1 ----
 if exists (select * from test2.dbo.poldloandet where loanslno = @loanslno and priority = 1)
 begin
 	if exists(select p.* from test2.dbo.PoldLoan p join test2.dbo.PEMPMAST m on p.eid=m.Eid where p.LoanId = @l_id and p.PMonth=@pmn and m.Empid=@emp_code and p.LoanSlNo=@loanslno)
	begin	

		 select 
		 @os_priciapl_open_amount=Amount-CumLoanRepaid+LoanRepaid,
		 @remaing_amount_adj_festival = Amount-CumLoanRepaid,
		 @interest_accured_adj=IntAccured,
		 @interest_open_amount = IntAmount-CumIntRepaid+IntRepaid,
		 @interest_amount_recovered_adj=IntRepaid,
		 @os_interest_amount_adj=IntAmount-CumIntRepaid,
		 @principal_amount_recovered_one_adj=IntAmount-CumIntRepaid-IntRepaid,
		 @fest_recovery = recovery,
		 @remaing_amount_adj = CAST(ceiling(cast(Amount as int))- (cast(CumLoanRepaid as int)) as int),
		 @fm = CAST((convert(date,convert(date,left(PMonth,4)+'-'+right(PMonth,2)+'-01'),102)) as varchar),
		 @priority = priority,
		 @principal_paid_amount=LoanRepaid
		 from [TEST2].dbo.PoldLoanDet
		 where LoanId=@l_id and loanslno = @loanslno and pmonth = @pmn and Priority = 1

		 exec sp_insert_loans_adjustments @emp_code, @loan_code, @os_priciapl_open_amount, @principal_paid_amount,   @interest_accured_adj,  
		     @interest_open_amount,@interest_amount_recovered_adj,  @os_interest_amount_adj, @completed_installment,  @installment_amount, 
			 @principal_amount_recovered_one_adj, @remaing_amount_adj, @fest_recovery, @total_recovered_amount, @remaing_amount_adj_festival, 
			 @fm, @fy,@loanslno,@priority

	end


 end

  --- Check priority 2 ----
  

 if exists (select * from test2.dbo.poldloandet where loanslno = @loanslno and priority = 2)
 begin
 	if exists(select p.* from test2.dbo.PoldLoan p join test2.dbo.PEMPMAST m on p.eid=m.Eid where p.LoanId = @l_id and p.PMonth=@pmn and m.Empid=@emp_code and p.LoanSlNo=@loanslno)
	begin	

		select 
		@os_priciapl_open_amount=Amount-CumLoanRepaid+LoanRepaid,
		@remaing_amount_adj_festival = Amount-CumLoanRepaid,
		@interest_accured_adj=IntAccured,
		@interest_open_amount = IntAmount-CumIntRepaid+IntRepaid,
		@interest_amount_recovered_adj=IntRepaid,
		@os_interest_amount_adj=IntAmount-CumIntRepaid,
		@principal_amount_recovered_one_adj=IntAmount-CumIntRepaid-IntRepaid,
		@fest_recovery = recovery,
		@remaing_amount_adj = CAST(ceiling(cast(Amount as int))- (cast(CumLoanRepaid as int)) as int),
		@fm = CAST((convert(date,convert(date,left(PMonth,4)+'-'+right(PMonth,2)+'-01'),102)) as varchar),
		@priority = priority,
		@principal_paid_amount=LoanRepaid
		from [TEST2].dbo.PoldLoanDet
		where LoanId=@l_id and loanslno = @loanslno and pmonth = @pmn and Priority = 2 

		 exec sp_insert_loans_adjustments @emp_code, @loan_code, @os_priciapl_open_amount, @principal_paid_amount,   @interest_accured_adj,  @interest_open_amount,
         @interest_amount_recovered_adj,  @os_interest_amount_adj, @completed_installment,  @installment_amount,  @principal_amount_recovered_one_adj, 
         @remaing_amount_adj, @fest_recovery, @total_recovered_amount, @remaing_amount_adj_festival, @fm, @fy,@loanslno,@priority

	end

	--- check without two subloans 

if not exists (select * from test2.dbo.PLoanDET where eid = (select eid from test2.dbo.PEMPMAST where Empid=@emp_code) and loanslno = @loanslno)
  begin

select 
 --@os_priciapl_open_amount=d.Amount-d.CumLoanRepaid,
 @os_priciapl_open_amount=Amount-CumLoanRepaid+LoanRepaid,
 @remaing_amount_adj_festival = Amount-CumLoanRepaid,
 @interest_accured_adj=IntAccured,
 @interest_open_amount = IntAmount-CumIntRepaid+IntRepaid,
 @total_interest_installments_adj=CAST(ceiling(cast(IntPeriod as int)/4*(3)) as varchar),
 @interest_amount_recovered_adj=IntRepaid,
 @os_interest_amount_adj=IntAmount-CumIntRepaid,
 @principal_amount_recovered_one_adj=IntAmount-CumIntRepaid-IntRepaid,
 @fest_recovery = l.recovery,
 @remaing_amount_adj = CAST(ceiling(cast(l.Amount as int))- (cast(CumLoanRepaid as int)) as int),
 @priority = 0,
 @principal_paid_amount= LoanRepaid
from [TEST2].dbo.POldLoan l join [TEST2].dbo.PEMPMAST m on l.eid=m.eid 
where  m.EmpId = @emp_code  and  l.LoanId = @l_id and loanslno = @loanslno

 exec sp_insert_loans_adjustments @emp_code, @loan_code, @os_priciapl_open_amount, @principal_paid_amount,@interest_accured_adj, @interest_open_amount,
		 @interest_amount_recovered_adj, @os_interest_amount_adj,@completed_installment,   @installment_amount,   @principal_amount_recovered_one_adj,   
		 @remaing_amount_adj, @fest_recovery, @total_recovered_amount,  @remaing_amount_adj_festival, @fm, @fy,@loanslno,@priority

end


end

 set @pmn = @pmn-1;

 end
delete from #two_sub where loan_id=@l_id;
end

--exec sp_dm_sub_loans 837,201909
