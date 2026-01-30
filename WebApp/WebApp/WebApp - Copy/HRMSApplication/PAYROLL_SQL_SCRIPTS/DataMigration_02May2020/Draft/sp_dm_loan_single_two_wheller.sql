CREATE PROCEDURE [dbo].[sp_dm_loan_single_two_wheller] @emp_code integer,  @mn integer
AS  
begin

--* check loan exists or not *
	if not exists (select * from pr_emp_adv_loans where emp_code = @emp_code and loan_type_mid = 24)
	begin
		--create new loan
		print '**';
		--create new loan
		--** mail table insertion *
        declare  @total_amount integer;
		declare  @loan_code nvarchar(20);
		declare @method nvarchar(50);
		declare @interest_rate nvarchar(20);
		declare @total_installment nvarchar(20);
		declare @remaining_installment nvarchar(20);
		declare @principal_installment nvarchar(20);
		declare @interest_installment nvarchar(20);
		declare @completed_installment nvarchar(20);
        declare @sanction_date date;
		declare @installment_start_date date;
		declare @installment_end_date date;
		declare @installment_amount nvarchar(20);
		declare @total_recovered_amount nvarchar(20);
		declare @fy int;
		declare @fm date;
		declare @remaing_amount nvarchar(20);
		declare @interest_installment_amount  nvarchar(20);
		declare @vender_name  nvarchar(20);
		declare @loanslno int;

		 select 
		 @total_amount=l.Amount,
		 @loan_code= l.LoanId,
		 @method='Interest to be Recovered at End',
		 @interest_rate = CAST(l.IntRate as varchar),
		 @total_installment=CAST(l.Period as varchar),
		 @remaining_installment=CAST(ceiling(cast(Period as int))- (cast(l.InstNo as int)) as varchar),
		 @principal_installment=CAST(l.Period as varchar),
		 @interest_installment=CAST(ceiling(cast(IntPeriod as int)/4*(3)) as varchar),
		 @completed_installment=CAST(l.InstNo as varchar),
		 @sanction_date=cast(FORMAT (l.SanDate, 'yyyy-MM-dd') as varchar),
		 @installment_start_date=cast(CASE WHEN LoanStart = '' or LoanStart is null then cast(l.SanDate as Date) 
		 else convert(date,convert(date,left(LoanStart,4)+'-'+right(LoanStart,2)+'-01'),102) 
		 end as varchar),
		 @installment_end_date = CAST(+cast(case when LoanStart is null or LoanStart='' then SanDate else DATEADD(month, Period,convert(date,convert(date,left(LoanStart,4)+'-'+right(LoanStart,2)+'-01'),102))  end as Date) as varchar), 
		 @installment_amount=CAST(l.InstAmt as varchar),
		 @total_recovered_amount=CAST(l.Recovery as varchar),
		 @fm=CAST((convert(date,convert(date,left(PMonth,4)+'-'+right(PMonth,2)+'-01'),102)) as varchar),
		 @fy=CAST(year(convert(date,convert(date,left(PMonth,4)+'-'+right(PMonth,2)+'-01'),102)) as varchar),
		 @remaing_amount = CAST(ceiling(cast(Amount as int))- (cast(l.Recovery as int)) as varchar),
		 @loanslno = l.LoanSlNo
		from   TEST2.dbo.PLoan l   join TEST2.dbo.PEmpMast m on m.eid=l.eid
		--from PoldLoanDet d join PEMPMAST m on d.eid=m.eid join PoldLoan l on d.loanslno=l.LoanSlNo
		where m.EmpId = @emp_code and l.LoanId='VEH2W' order by m.EmpId

		 --- Execte procedure fest loans in Main Table
		exec sp_insert_two_wheller_single_loans @emp_code, @total_amount, @loan_code,  @method, @interest_rate, @total_installment,
		     @remaining_installment, @principal_installment, @interest_installment, @completed_installment, @sanction_date, 
			 @installment_start_date, @installment_end_date, @installment_amount, @total_recovered_amount, @fm, @fy,@loanslno

	
		--- Child table insertion ---declare @loan_amount int;  
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
			declare @date_disburse date;
			declare @os_principal_amount int; 
			declare @interest_amount_paid_month int;
			declare @loan_amount int;

			select 
			 @emp_code=CAST(m.Empid as varchar),
			 @loan_code=l.LoanId,
			 @date_disburse=cast(CASE WHEN LoanStart = '' or LoanStart is null then cast(l.SanDate as Date) 
			 else convert(date,convert(date,left(LoanStart,4)+'-'+right(LoanStart,2)+'-01'),102) 
			 end as varchar),
			 @loan_amount=l.Amount,
			 @interest_rate_one=l.IntRate,
			 @interest_accured=l.IntAmount,
			 @principal_amount_recovered_one=l.CumLoanRepaid,
			 @interest_amount_recovered_one=l.CumIntRepaid,
			 @total_amount_recovered_one=l.Recovery,
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
			 @total_principal_installments=CAST(ceiling(cast(Period as int)/4*(3)) as varchar),
			 @total_interest_installments=CAST(ceiling(cast(IntPeriod as int)/4*(3)) as varchar),
			 @os_principal_amount=l.Amount-l.CumLoanRepaid,
			 @os_interest_amount=l.IntAmount-l.CumIntRepaid
			 from   TEST2.dbo.PLoan l   join TEST2.dbo.PEmpMast m on m.eid=l.eid
			where 
			m.EmpId = @emp_code  and  l.LoanId = 'VEH2W'

			exec sp_insert_two_wheller_single_child @emp_code, @loan_code, @date_disburse, @loan_amount, @interest_rate_one,
			@interest_accured, @principal_amount_recovered_one, @interest_amount_recovered_one, @total_amount_recovered_one,
			@principal_start_date ,@principal_end_date,@interest_start_date_one,@interest_end_date_one,
			@total_principal_installments,@total_interest_installments, @os_principal_amount, @os_interest_amount,@loanslno 

   

	--- Execute procedure for loans in loans adjustment table 
        declare @total_interest_installments_adj int;
		declare @interest_accured_adj int;
		declare @installments_paid_amount_adj int;
		declare @interest_amount_recovered_adj int;
		declare @os_interest_amount_adj int;
		declare @intrest_balance_amount int;
		declare @remaing_amount_adj int;
		declare @fest_recovery int;
		declare @fm_prev date;
		declare @os_priciapl_open_amount float;
		declare @intrest_open_amount int;
		select 
		 @os_priciapl_open_amount=d.CumLoanRepaid,
		 @interest_accured_adj=l.IntAccured,
		 @intrest_open_amount = l.IntAmount,
		 @total_interest_installments_adj=CAST(ceiling(cast(IntPeriod as int)/4*(3)) as varchar),
		 @interest_amount_recovered_adj=l.CumIntRepaid,
		 @os_interest_amount_adj=d.IntAmount-d.CumIntRepaid,
		 @intrest_balance_amount=l.IntAmount-l.CumIntRepaid,
		 @fest_recovery = l.recovery,
		 @remaing_amount_adj = CAST(ceiling(cast(l.Amount as int))- (cast(d.CumLoanRepaid as int)) as int)
		from [TEST2].dbo.PLoanDet d join [TEST2].dbo.PEMPMAST m on d.eid=m.eid join [TEST2].dbo.PLoan l on d.loanslno=l.LoanSlNo 
		where 
		d.Priority=1 and  m.EmpId = @emp_code  and  l.LoanId ='VEH2W'

	

		if exists(select p.* from test2.dbo.PLoan p join test2.dbo.PEMPMAST m on p.eid=m.Eid where p.LoanId = 'VEH2W' and p.PMonth=@mn and m.Empid=@emp_code and p.LoanSlNo=@loanslno)
		begin

		exec sp_insert_two_wheller_single_addjustments @emp_code, @loan_code, @os_priciapl_open_amount, @interest_accured_adj,@intrest_open_amount, 
		 @interest_amount_recovered_adj,  @intrest_balance_amount, @os_interest_amount_adj, @completed_installment,  @installment_amount,  
		 @remaing_amount_adj, @fest_recovery, @total_recovered_amount, @remaing_amount_adj, @fm, @fy,@loanslno
		end

----- Inserting in prevo datsa

	declare @pmn int;
    set @pmn = @mn-1;
	while(@pmn>=201904)
	begin
 
 --check in poldloan with loanslid and @pmn
	select 
     @os_priciapl_open_amount=l.CumLoanRepaid,
	 @interest_accured_adj=l.IntAccured,
	 @intrest_open_amount = l.IntAmount,
	 @interest_amount_recovered_adj=l.CumIntRepaid,
	 @total_interest_installments_adj=CAST(ceiling(cast(IntPeriod as int)/4*(3)) as varchar),
	 @os_interest_amount_adj=d.IntAmount-d.CumIntRepaid,
	 @intrest_balance_amount=l.IntAmount-l.CumIntRepaid,
	 @fest_recovery = l.recovery,
	 @remaing_amount_adj = CAST(ceiling(cast(l.Amount as int))- (cast(d.CumLoanRepaid as int)) as int),
	 @fm = CAST((convert(date,convert(date,left(l.PMonth,4)+'-'+right(l.PMonth,2)+'-01'),102)) as varchar)
	from [TEST2].dbo.PoldLoanDet d join [TEST2].dbo.PEMPMAST m on d.eid=m.eid join [TEST2].dbo.PoldLoan l on d.loanslno=l.LoanSlNo 
	where d.priority = 1 and l.LoanId='VEH2W' and d.loanslno = @loanslno and l.pmonth = @pmn  order by l.Eid

	
	if exists(select p.* from test2.dbo.PoldLoan p join test2.dbo.PEMPMAST m on p.eid=m.Eid where p.LoanId = 'VEH2W' and p.PMonth=@pmn and m.Empid=@emp_code and p.LoanSlNo=@loanslno)
	begin
	exec sp_insert_two_wheller_single_addjustments @emp_code, @loan_code, @os_priciapl_open_amount, @interest_accured_adj, @intrest_open_amount,
	 @interest_amount_recovered_adj,  @intrest_balance_amount, @os_interest_amount_adj, @completed_installment,  @installment_amount, 
	 @remaing_amount_adj, @fest_recovery, @total_recovered_amount, @remaing_amount_adj, @fm, @fy,@loanslno
	end

   set @pmn = @pmn-1;
   end
   end
	else 
		begin
			print '*** Loan Existed ***';
		end

--**insert loan installment into loan adjustmnet table **
end;



--exec sp_dm_loan_single_two_wheller 499, 201908

