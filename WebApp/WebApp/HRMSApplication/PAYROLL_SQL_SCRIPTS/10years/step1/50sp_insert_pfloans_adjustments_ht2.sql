CREATE PROCEDURE [dbo].[sp_insert_pfloans_adjustments_ht2] 
        @emp_code nvarchar(20) , 
		@loan_code varchar(20),
        @os_priciapl_open_amount integer,
		@interest_accured_adj int,
		@interest_amount_recovered_adj int,
		@interest_amount_paid_month int,
		@principal_amount_recovered_one_adj int,
		@completed_installment nvarchar(20), 
		@installment_amount nvarchar(20) , 
		@total_recovered_amount nvarchar(20), 
		@remaing_amount_adj nvarchar(20),
		@total_recovered_amount_festival  nvarchar(20),
		@remaing_amount_adj_festival int,
		@fm date,
		@fy int,
		@loanslno int
		AS

declare @transidnew int; 
--generating new trans id
exec gen_new_transaction 793, 'G JAGADISH', 'Payroll_Web', '1.0.0', 'N/A'; 

--selecting new trans id
set @transidnew = (select last_num from new_num where table_name = 'transaction_tbl'); 
declare @idnew0 int; 
--generating new number for pr_emp_adv_loans table
exec get_new_num 'pr_emp_adv_loans'; 
--selecting last id for pr_emp_adv_loans table

set @idnew0=(select  last_num from new_num where table_name = 'pr_emp_adv_loans');

declare @emp_id int;
set @emp_id=(select id from employees where EmpId=@emp_code);

declare @d_id int;
set @d_id=(select d.id from Designations d inner join Employees e on d.id=e.CurrentDesignation where e.EmpId=@emp_code);
--select * from pr_emp_adv_loans

declare @loan_id int ;
set @loan_id = (select id from pr_emp_adv_loans where loan_sl_no = @loanslno)

declare @adv_loans_child_id int;
set @adv_loans_child_id = (select id from pr_emp_adv_loans_child where loan_sl_no = @loanslno and priority = 1)

declare @adv_loans_child_id_two int;
set @adv_loans_child_id_two = (select id from pr_emp_adv_loans_child where loan_sl_no = @loanslno and priority = 2)

declare @paid_date date;
set @paid_date = (SELECT GETDATE());

declare @loanidnew int;
set @loanidnew = (select id from pr_loan_master where loan_id = @loan_code);

declare @status int;
set @status=(select active from pr_month_details where fm=@fm)
if(@status is null or @status=0)
begin
set @status=0;
end

Insert into pr_emp_adv_loans_adjustments(id,emp_adv_loans_mid, emp_adv_loans_child_mid, principal_open_amount, principal_paid_amount, 
principal_balance_amount, interest_accured, interest_open_amount, interest_paid_amount, interest_balance_amount,installments_paid,
installments_amount, installments_paid_date, cash_paid_on, covered_installments, amount_paid, active, trans_id, fm, fy, loan_sl_no, show) 
values
(@idnew0,@loan_id, @adv_loans_child_id, @os_priciapl_open_amount,case when @loan_code = 'FEST'  then @total_recovered_amount_festival else @total_recovered_amount end, 
@remaing_amount_adj_festival, @interest_accured_adj, @interest_amount_recovered_adj, @interest_amount_paid_month,
@principal_amount_recovered_one_adj, @completed_installment,  @installment_amount, @paid_date, 
@paid_date,@completed_installment, @installment_amount, @status, @transidnew, @fm, @fy, @loanslno,1);
update pr_emp_adv_loans_adjustments set payment_type='Installment',payment_mode='Cash Payment';
---End--