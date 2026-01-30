alter PROCEDURE [dbo].[sp_dm_loan_fest] @emp_code integer,  @mn integer
AS  
begin

--print'dm loan fest';
--* check loan exists or not *
	if not exists (select * from pr_emp_adv_loans where emp_code = @emp_code and loan_type_mid = 3)
	begin
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
		 @method='FEST',
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
		from   TEST2.dbo.Poldloan l   join TEST2.dbo.PEmpMast m on m.eid=l.eid
		--from PoldLoanDet d join PEMPMAST m on d.eid=m.eid join PoldLoan l on d.loanslno=l.LoanSlNo
		where m.EmpId = @emp_code and l.LoanId='FEST' and PMonth=@mn-1 order by m.EmpId

	
		--- Child table insertion ---
        declare @date_disburse date;
		declare @principal_amount_recovered integer;
		declare @total_amount_recovered integer;
		declare @principal_start_date date;
		declare @principal_end_date date;
		declare @total_principal_installments  nvarchar(20);
		declare @os_principal_amount integer;
		declare @os_remaining_amount_paid int;
		
		select  
		 @loan_code=l.LoanId,
		 @date_disburse=cast(FORMAT (l.SanDate, 'yyyy-MM-dd') as varchar),
		 @principal_amount_recovered = l.LoanRepaid+l.CumLoanRepaid,
		 @total_amount_recovered=l.LoanRepaid+l.CumLoanRepaid+l.IntRepaid+l.CumIntRepaid,
		 @principal_start_date=cast(CASE WHEN LoanStart = '' or LoanStart is null then cast(l.SanDate as Date) 
		 else convert(date,convert(date,left(LoanStart,4)+'-'+right(LoanStart,2)+'-01'),102) 
		 end as varchar),
		 @principal_end_date=
		 CAST(Dateadd(month,period, cast(CASE WHEN LoanStart = '' or LoanStart is null then cast(l.SanDate as Date) else convert(date,convert(date,left(LoanStart,4)+'-'+right(LoanStart,2)
		 +'-01'),102) end as Date)) as varchar),
		 @total_principal_installments=floor(cast(Period as int)),
		 @os_principal_amount=l.Amount-(l.LoanRepaid+l.CumLoanRepaid),
		 @loanslno = l.LoanSlNo,
		 @os_remaining_amount_paid = l.Amount-(l.LoanRepaid+l.CumLoanRepaid)
		 --from   TEST2.dbo.PEmpMast m  join TEST2.dbo.Poldloan l on m.Eid=l.Eid
		 from   TEST2.dbo.Poldloan l   join TEST2.dbo.PEmpMast m on m.eid=l.eid
		 where  l.LoanId='FEST' and  m.EmpId = @emp_code and PMonth=@mn-1 order by l.Eid 

    --- Execte procedure fest loans in Main Table
		exec sp_insert_fest_loans @emp_code, @total_amount, @loan_code,  @method, @interest_rate, @total_installment,
		     @remaining_installment, @principal_installment, @interest_installment, @completed_installment, @sanction_date, 
			 @installment_start_date, @installment_end_date, @installment_amount, @total_recovered_amount, @fm, @fy,@loanslno

	 --- Execue procedire fest loans child in child Table

	      exec sp_insert_fest_loans_child @emp_code, @loan_code, @date_disburse, @total_amount, @principal_amount_recovered, @total_amount_recovered,
          @principal_start_date, @principal_end_date, @total_principal_installments, @os_principal_amount,@loanslno,@os_remaining_amount_paid

	--- Execute procedure for loans in loans adjustment table 
        declare @total_interest_installments_adj int;
		declare @interest_accured_adj int;
		declare @installments_paid_amount_adj int;
		declare @interest_amount_recovered_adj int;
		declare @os_interest_amount_adj int;
		declare @principal_amount_recovered_one_adj int;
		declare @remaing_amount_adj int;
		declare @fest_recovery int;
		declare @fm_prev date;
		declare @os_priciapl_open_amount float;
		select 
		 @os_priciapl_open_amount= Amount-Recovery+InstAmt,
		 @interest_accured_adj=IntAmount,
		 @total_interest_installments_adj=CAST(ceiling(cast(IntPeriod as int)/4*(3)) as varchar),
		 @interest_amount_recovered_adj=CumIntRepaid,
		 @os_interest_amount_adj=IntAmount-CumIntRepaid,
		 @principal_amount_recovered_one_adj=CumLoanRepaid,
		 @fest_recovery = recovery,
		 @remaing_amount_adj = CAST(ceiling(cast(l.Amount as int))- (cast(CumLoanRepaid as int)) as int)
		 from   TEST2.dbo.PEmpMast m  join  TEST2.dbo.PoldLoan l  on m.Eid=l.Eid 
		where  m.EmpId = @emp_code  and  l.LoanId ='FEST'


		declare @total_recovered_amount_festival float;
		declare @remaing_amount_adj_festival int;
		select 
		@os_priciapl_open_amount= Amount-Recovery+InstAmt,
		@total_recovered_amount_festival=LoanRepaid,
		@remaing_amount_adj_festival = Amount-CumLoanRepaid
		 from   TEST2.dbo.PEmpMast m  join  TEST2.dbo.Poldloan l  on m.Eid=l.Eid 
		where l.LoanId='FEST' and  m.EmpId = @emp_code order by l.Eid


----- Inserting in prevo datsa

	declare @pmn int;
    set @pmn = @mn-1;
	while(@pmn>=201904)
	begin
	select 
	@os_priciapl_open_amount= Amount-Recovery+InstAmt,
	@total_recovered_amount_festival=LoanRepaid,
	@remaing_amount_adj_festival = Amount-CumLoanRepaid,
	@fm = CAST((convert(date,convert(date,left(l.PMonth,4)+'-'+right(l.PMonth,2)+'-01'),102)) as varchar)
	 from   TEST2.dbo.PEmpMast m  join  TEST2.dbo.PoldLoan l  on m.Eid=l.Eid 
	where l.LoanId='FEST' and loanslno = @loanslno and pmonth = @pmn  order by l.Eid

	
	if exists(select p.* from test2.dbo.PoldLoan p join test2.dbo.PEMPMAST m on p.eid=m.Eid where p.LoanId = 'FEST' and p.PMonth=@pmn and m.Empid=@emp_code and p.LoanSlNo=@loanslno)
	begin
	exec sp_insert_festival_loans_adjustments @emp_code, @loan_code, @os_priciapl_open_amount, @interest_accured_adj, 
	 @interest_amount_recovered_adj,  @os_interest_amount_adj, @completed_installment,  @installment_amount,  @principal_amount_recovered_one_adj, 
	 @remaing_amount_adj, @fest_recovery, @total_recovered_amount_festival, @remaing_amount_adj_festival, @fm, @fy,@loanslno
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
