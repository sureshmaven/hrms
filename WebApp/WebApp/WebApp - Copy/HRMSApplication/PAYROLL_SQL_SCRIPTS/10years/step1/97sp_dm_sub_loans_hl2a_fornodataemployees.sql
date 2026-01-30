create PROCEDURE [dbo].[sp_dm_sub_loans_hl2a_fornodataemployees] @emp_code integer,  @fmn integer
AS  
--all two sub loans codes
create table #two_sub(
loan_id varchar(20)
);
print(concat('5 ', CAST(SYSDATETIME() AS TIME)))
declare @l_id varchar(20);
--select * from pr_loan_master;
insert into #two_sub select distinct(LoanId) as loan_id from [TEST2].dbo.PoldLoanDet where LoanId='HL2A';
while exists(select top 1 * from #two_sub)
begin

select top 1 @l_id = loan_id from #two_sub;

declare @loan_code_mid int;
set @loan_code_mid = (select id from pr_loan_master where loan_id=@l_id);


declare @loanslnonull int;
select @loanslnonull=l.LoanSlNo 
from   TEST2.dbo.PEmpMast m  join  TEST2.dbo.PoldLoanDet l  on m.Eid=l.Eid 
where l.LoanId=@l_id and  m.EmpId = @emp_code and l.pmonth= @fmn-1 order by l.Eid

if (@loanslnonull is null)
begin
update l set l.LoanSlNo=(select max(LoanSlNo)+1 from TEST2.dbo.PoldLoanDet)  
from   TEST2.dbo.PEmpMast m  join  TEST2.dbo.PoldLoanDet l  on m.Eid=l.Eid 
where l.LoanId=@l_id and  m.EmpId = @emp_code
end


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
@total_installment=0,
@remaining_installment=0,
@principal_installment=0,
@interest_installment=0,
@completed_installment=0,
@sanction_date=cast(l.SanDate as Date),
@installment_start_date=null,
@installment_end_date = null,
@interest_rate = CAST(l.IntRate as varchar),
@installment_amount= CAST((case when  (l.InstAmt='0.00' and l.loanrepaid >'0.00' )then l.loanrepaid  else l.InstAmt end) as varchar),
@interest_installment_amount = CAST(l.IntAmount as varchar),
@total_recovered_amount=CAST(l.Recovery as varchar),
@principal_recoverd = CAST(l.CumLoanRepaid as int),
@remaing_amount = CAST(ceiling(cast(Amount as int))- (cast(l.Recovery as int)) as int),
@vender_name = null,
@os_principal_amount=Amount-CumLoanRepaid,
@date_disburse=cast(l.SanDate as Date),
@fm= CAST((convert(date,convert(date,left(PMonth,4)+'-'+right(PMonth,2)+'-01'),102)) as varchar),
@fy=CAST(year(convert(date,convert(date,left(PMonth,4)+'-'+right(PMonth,2)+'-01'),102)) as varchar),
@loanslno = l.LoanSlNo
 from   TEST2.dbo.PEmpMast m  join  TEST2.dbo.PoldLoanDet l  on m.Eid=l.Eid 
where l.LoanId=@l_id and  m.EmpId = @emp_code and l.pmonth= @fmn-1 order by l.Eid




if not exists (select * from pr_emp_adv_loans p join pr_loan_master m on p.loan_type_mid=m.id where p.emp_code = @emp_code and m.loan_id=@l_id and loan_sl_no = @loanslno)
	begin
--select * from Employees;
  if exists (select * from test2.dbo.PoldLoanDet p join test2.dbo.PEMPMAST m on p.Eid=m.eid where p.LoanId=@l_id and m.Empid=@emp_code and p.pmonth= @fmn-1)
  begin

exec sp_insert_two_sub_Loans @emp_code ,@loan_code_mid,  @total_amount, @total_installment, @remaining_installment, 
@principal_installment, @interest_installment,@completed_installment, @sanction_date, @installment_start_date,
@installment_end_date, @method, @interest_rate, @installment_amount, @interest_installment_amount, 
@total_recovered_amount, @vender_name, @fm, @fy,@loanslno

--- Priority 1----
if exists (select * from test2.dbo.PoldLoanDet where eid = (select eid from test2.dbo.PEMPMAST where Empid=@emp_code) and LoanId = @l_id and priority=1 and pmonth= @fmn-1)
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
 @date_disburse=cast(d.SanDate as Date),
 @loan_amount=d.Amount,
 @interest_rate_one=d.IntRate,
 @interest_accured=d.intamount+d.intaccured,
 @principal_amount_recovered_one=d.LoanRepaid+d.CumLoanRepaid,
 @interest_amount_recovered_one=d.IntRepaid+d.CumIntRepaid,
 @total_amount_recovered_one=d.LoanRepaid+d.CumLoanRepaid+d.IntRepaid+d.CumIntRepaid,
 @principal_start_date=cast(d.SanDate as Date),
 @principal_end_date=cast(d.SanDate as Date),
 @interest_start_date_one=cast(d.SanDate as Date),
 @interest_end_date_one=cast(d.SanDate as Date),
 @total_principal_installments=0,
 @total_interest_installments=0,
 @os_principal_amount=(d.Amount-(d.LoanRepaid+d.CumLoanRepaid)),
 @os_interest_amount=((d.intamount+d.intaccured)-(d.IntRepaid+d.CumIntRepaid)),
 @os_total_balace_amount = d.Amount-d.CumLoanRepaid-d.LoanRepaid+d.IntAmount-d.CumIntRepaid-d.IntRepaid
from [TEST2].dbo.PoldLoandet d join [TEST2].dbo.PEMPMAST m on d.eid=m.eid 
where 
d.Priority=1 and  m.EmpId = @emp_code  and  d.LoanId = @l_id and d.pmonth= @fmn-1

exec sp_insert_two_sub_Loans_priority1 @emp_code, @loan_code, @date_disburse, @loan_amount, @interest_rate_one,
@interest_accured, @principal_amount_recovered_one, @interest_amount_recovered_one, @total_amount_recovered_one,
@principal_start_date ,@principal_end_date,@interest_start_date_one,@interest_end_date_one,
@total_principal_installments,@total_interest_installments, @os_principal_amount, @os_interest_amount, @os_total_balace_amount, @loanslno 


end
--- Priority 2----
if exists (select * from test2.dbo.PoldLoanDet where eid = (select eid from test2.dbo.PEMPMAST where Empid=@emp_code) and LoanId = @l_id and priority=2 and pmonth= @fmn-1)
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
 @date_disburse_two=cast(d.SanDate as Date),
 @loan_amount_two=d.Amount,
 @interest_rate_two=d.IntRate,
 @interest_accured_two=d.intamount+d.intaccured,
 @principal_amount_recovered_two=d.LoanRepaid+d.CumLoanRepaid,
 @interest_amount_recovered_two=d.IntRepaid+d.CumIntRepaid,
 @total_amount_recovered_two=d.LoanRepaid+d.CumLoanRepaid+d.IntRepaid+d.CumIntRepaid,
 @principal_start_date_two=cast(d.SanDate as Date),
 @principal_end_date_two=cast(d.SanDate as Date),
 @interest_start_date_two=cast(d.SanDate as Date),
 @interest_end_date_two=cast(d.SanDate as Date),
 @total_principal_installments_two=0,
 @total_interest_installments_two=0,
 @os_principal_amount_two=(d.Amount-(d.LoanRepaid+d.CumLoanRepaid)),
 @os_interest_amount_two=((d.intamount+d.intaccured)-(d.IntRepaid+d.CumIntRepaid)),
 @os_total_balace_amount = d.Amount-d.CumLoanRepaid-d.LoanRepaid+d.IntAmount-d.CumIntRepaid-d.IntRepaid
from [TEST2].dbo.PoldLoandet d join [TEST2].dbo.PEMPMAST m on d.eid=m.eid 
where 
d.Priority=2 and  m.EmpId = @emp_code  and  d.LoanId = @l_id and d.pmonth= @fmn-1

exec sp_insert_two_sub_Loans_priority2 @emp_code, @loan_code, @date_disburse_two, @loan_amount_two, @interest_rate_two,
@interest_accured_two, @principal_amount_recovered_two, @interest_amount_recovered_two, @total_amount_recovered_two,
@principal_start_date_two ,@principal_end_date_two, @interest_start_date_two,@interest_end_date_two,
@total_principal_installments_two,@total_interest_installments_two, @os_principal_amount_two, @os_interest_amount_two,@os_total_balace_amount,@loanslno 
end 
--- Priority 3----
if exists (select * from test2.dbo.PoldLoanDet where eid = (select eid from test2.dbo.PEMPMAST where Empid=@emp_code) and LoanId = @l_id and priority=3 and pmonth= @fmn-1)
begin
declare @date_disburse_three date; 
declare @loan_amount_three int;  
declare @interest_rate_three float;
declare @interest_accured_three float; 
declare @principal_amount_recovered_three int;
declare @interest_amount_recovered_three float; 
declare @total_amount_recovered_three float;
declare @principal_start_date_three date;
declare @principal_end_date_three date;
declare @interest_start_date_three date; 
declare @interest_end_date_three date;
declare @total_principal_installments_three int;
declare @total_interest_installments_three int; 
declare @os_principal_amount_three int; 
declare @os_interest_amount_three float;


select 
 @emp_code=CAST(m.Empid as varchar),
 @loan_code=d.LoanId,
 @date_disburse_three=cast(d.SanDate as Date),
 @loan_amount_three=d.Amount,
 @interest_rate_three=d.IntRate,
 @interest_accured_three=d.intamount+d.intaccured,
 @principal_amount_recovered_three=d.LoanRepaid+d.CumLoanRepaid,
 @interest_amount_recovered_three=d.IntRepaid+d.CumIntRepaid,
 @total_amount_recovered_three=d.LoanRepaid+d.CumLoanRepaid+d.IntRepaid+d.CumIntRepaid,
 @principal_start_date_three=cast(d.SanDate as Date),
 @principal_end_date_three=cast(d.SanDate as Date),
 @interest_start_date_three=cast(d.SanDate as Date),
 @interest_end_date_three=cast(d.SanDate as Date),
 @total_principal_installments_three=0,
 @total_interest_installments_three=0,
 @os_principal_amount_three=(d.Amount-(d.LoanRepaid+d.CumLoanRepaid)),
 @os_interest_amount_three=((d.intamount+d.intaccured)-(d.IntRepaid+d.CumIntRepaid)),
 @os_total_balace_amount = d.Amount-d.CumLoanRepaid+d.IntAmount-d.CumIntRepaid
from [TEST2].dbo.PoldLoandet d join [TEST2].dbo.PEMPMAST m on d.eid=m.eid  
where 
d.Priority=3 and  m.EmpId = @emp_code  and  d.LoanId = @l_id and d.pmonth= @fmn-1

exec sp_insert_three_sub_Loans_priority3 @emp_code, @loan_code, @date_disburse_three, @loan_amount_three, @interest_rate_three,
@interest_accured_three, @principal_amount_recovered_three, @interest_amount_recovered_three, @total_amount_recovered_three,
@principal_start_date_three ,@principal_end_date_three, @interest_start_date_three,@interest_end_date_three,
@total_principal_installments_three,@total_interest_installments_three, @os_principal_amount_three, @os_interest_amount_three,@os_total_balace_amount,@loanslno 
end 
--- check without two subloans and insert in child table ---

if not exists (select * from test2.dbo.PoldLoanDet where eid = (select eid from test2.dbo.PEMPMAST where Empid=@emp_code)  and pmonth= @fmn-1)
  begin
 
select 
 @emp_code=CAST(m.Empid as varchar),
 @loan_code=LoanId,
 @date_disburse=cast(CASE WHEN LoanStart = '' or LoanStart is null then cast(l.SanDate as Date) 
 else convert(date,convert(date,left(LoanStart,4)+'-'+right(LoanStart,2)+'-01'),102) 
 end as varchar),
 @loan_amount=l.Amount,
 @interest_rate_one=l.IntRate,
 @interest_accured=l.intamount+l.intaccured,
 @principal_amount_recovered_one=l.LoanRepaid+l.CumLoanRepaid,
 @interest_amount_recovered_one=l.IntRepaid+l.CumIntRepaid,
 @total_amount_recovered_one=l.LoanRepaid+l.CumLoanRepaid+l.IntRepaid+l.CumIntRepaid,
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
 @os_principal_amount=(l.Amount-(l.LoanRepaid+l.CumLoanRepaid)),
 @os_interest_amount=((l.intamount+l.intaccured)-(l.IntRepaid+l.CumIntRepaid)),
 @os_total_balace_amount = Amount-CumLoanRepaid-LoanRepaid+IntAmount-CumIntRepaid-IntRepaid
from [TEST2].dbo.poldloan l join [TEST2].dbo.PEMPMAST m on l.eid=m.eid
where 
  m.EmpId = @emp_code  and  l.LoanId = @l_id and  l.pmonth= @fmn-1

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
@remaing_amount_adj_festival = Amount-CumLoanRepaid-LoanRepaid
from   TEST2.dbo.PEmpMast m  join  TEST2.dbo.PLoan l  on m.Eid=l.Eid 
where l.LoanId=@l_id and  m.EmpId = @emp_code order by l.Eid


---- Check for prevois loands data ----

 declare @pmn int;
 set @pmn = @fmn-1;
--check for prev. month adj.
 while(@pmn>=201104)
 begin 

 --- Check priority 1 ----
 if exists (select * from test2.dbo.poldloandet where loanslno = @loanslno and priority = 1 and pmonth = @pmn)
 begin
 	if exists(select p.* from test2.dbo.PoldLoanDet p join test2.dbo.PEMPMAST m on p.eid=m.Eid where p.LoanId = @l_id and p.PMonth=@pmn and m.Empid=@emp_code and p.LoanSlNo=@loanslno )
	begin	

		 select 
		 @os_priciapl_open_amount=Amount-CumLoanRepaid,
		 @remaing_amount_adj_festival = Amount-CumLoanRepaid-LoanRepaid,
		 @interest_accured_adj=IntAccured,
		 @interest_open_amount = IntAmount-CumIntRepaid,
		 @interest_amount_recovered_adj=IntRepaid,
		 @os_interest_amount_adj=IntAmount+IntAccured-CumIntRepaid-IntRepaid,
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
 if exists (select * from test2.dbo.poldloandet where loanslno = @loanslno and priority = 2 and pmonth = @pmn)
 begin
 	if exists(select p.* from test2.dbo.PoldLoanDet p join test2.dbo.PEMPMAST m on p.eid=m.Eid where p.LoanId = @l_id and p.PMonth=@pmn and m.Empid=@emp_code and p.LoanSlNo=@loanslno)
	begin	

		select 
		@os_priciapl_open_amount=Amount-CumLoanRepaid,
		@remaing_amount_adj_festival = Amount-CumLoanRepaid-LoanRepaid,
		@interest_accured_adj=IntAccured,
		@interest_open_amount = IntAmount-CumIntRepaid,
		@interest_amount_recovered_adj=IntRepaid,
		@os_interest_amount_adj=IntAmount+IntAccured-CumIntRepaid-IntRepaid,
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
	
end
 --- Check priority 3 ----
 if exists (select * from test2.dbo.poldloandet where loanslno = @loanslno and priority = 3 and pmonth = @pmn)
 begin
 	if exists(select p.* from test2.dbo.PoldLoanDet p join test2.dbo.PEMPMAST m on p.eid=m.Eid where p.LoanId = @l_id and p.PMonth=@pmn and m.Empid=@emp_code and p.LoanSlNo=@loanslno)
	begin	

		select 
		@os_priciapl_open_amount=Amount-CumLoanRepaid,
		@remaing_amount_adj_festival = Amount-CumLoanRepaid-LoanRepaid,
		@interest_accured_adj=IntAccured,
		@interest_open_amount = IntAmount-CumIntRepaid,
		@interest_amount_recovered_adj=IntRepaid,
		@os_interest_amount_adj=IntAmount+IntAccured-CumIntRepaid-IntRepaid,
		@principal_amount_recovered_one_adj=IntAmount-CumIntRepaid-IntRepaid,
		@fest_recovery = recovery,
		@remaing_amount_adj = CAST(ceiling(cast(Amount as int))- (cast(CumLoanRepaid as int)) as int),
		@fm = CAST((convert(date,convert(date,left(PMonth,4)+'-'+right(PMonth,2)+'-01'),102)) as varchar),
		@priority = priority,
		@principal_paid_amount=LoanRepaid
		from [TEST2].dbo.PoldLoanDet
		where LoanId=@l_id and loanslno = @loanslno and pmonth = @pmn and Priority = 3 

		 exec sp_insert_loans_adjustments @emp_code, @loan_code, @os_priciapl_open_amount, @principal_paid_amount,   @interest_accured_adj,  @interest_open_amount,
         @interest_amount_recovered_adj,  @os_interest_amount_adj, @completed_installment,  @installment_amount,  @principal_amount_recovered_one_adj, 
         @remaing_amount_adj, @fest_recovery, @total_recovered_amount, @remaing_amount_adj_festival, @fm, @fy,@loanslno,@priority
		
	end
	
end
--- check without two subloans 
if not exists (select * from test2.dbo.poldloandet where eid = (select eid from test2.dbo.PEMPMAST where Empid=@emp_code) and loanslno = @loanslno)
  begin

select 
 --@os_priciapl_open_amount=d.Amount-d.CumLoanRepaid,
 @os_priciapl_open_amount=Amount-CumLoanRepaid,
 @remaing_amount_adj_festival = Amount-CumLoanRepaid-LoanRepaid,
 @interest_accured_adj=IntAccured,
 @interest_open_amount = IntAmount-CumIntRepaid,
 @total_interest_installments_adj=0,
 @interest_amount_recovered_adj=IntRepaid,
 @os_interest_amount_adj=IntAmount+IntAccured-CumIntRepaid-IntRepaid,
 @principal_amount_recovered_one_adj=IntAmount-CumIntRepaid-IntRepaid,
 @fest_recovery = l.recovery,
 @remaing_amount_adj = CAST(ceiling(cast(l.Amount as int))- (cast(CumLoanRepaid as int)) as int),
 @fm= CAST((convert(date,convert(date,left(PMonth,4)+'-'+right(PMonth,2)+'-01'),102)) as varchar),
 @priority = 0,
 @principal_paid_amount= LoanRepaid
from [TEST2].dbo.PoldLoan l join [TEST2].dbo.PEMPMAST m on l.eid=m.eid 
where  m.EmpId = @emp_code  and  l.LoanId = @l_id and loanslno = @loanslno and pmonth = @pmn

 exec sp_insert_loans_adjustments @emp_code, @loan_code, @os_priciapl_open_amount, @principal_paid_amount,@interest_accured_adj, @interest_open_amount,
		 @interest_amount_recovered_adj, @os_interest_amount_adj,@completed_installment,   @installment_amount,   @principal_amount_recovered_one_adj,   
		 @remaing_amount_adj, @fest_recovery, @total_recovered_amount,  @remaing_amount_adj_festival, @fm, @fy,@loanslno,@priority

end

 set @pmn = @pmn-1;

 end
delete from #two_sub where loan_id=@l_id;
end

--exec sp_dm_sub_loans 837,201909
