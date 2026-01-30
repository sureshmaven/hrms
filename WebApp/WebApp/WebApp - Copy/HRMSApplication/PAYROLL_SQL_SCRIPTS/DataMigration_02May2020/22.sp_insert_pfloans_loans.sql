CREATE PROCEDURE [dbo].[sp_insert_pfloans_loans] @emp_code nvarchar(20) , 
        @total_amount integer,
		@loan_code nvarchar(20),
		@method nvarchar(50), 
		@interest_rate nvarchar(20),  
		@total_installment nvarchar(20)  , 
		@remaining_installment nvarchar(20), 
		@principal_installment nvarchar(20),
		@interest_installment nvarchar(20),
		@completed_installment nvarchar(20), 
		@sanction_date date , 
		@installment_start_date date ,
		@installment_end_date date ,
		@installment_amount nvarchar(20), 
		@total_recovered_amount nvarchar(20), 
		@fm date,@fy int,
		@loanslno int,
		@loan_repaid nvarchar(20)
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


Insert into pr_emp_adv_loans(id, emp_id, emp_code, designation, loan_type_mid, 
total_amount, 
total_installment,remaining_installment,
principal_installment, interest_installment, completed_installment, sanction_date, installment_start_date,installment_end_date,method, 
interest_rate, installment_amount,interest_installment_amount, total_recovered_amount, code_master, active, trans_id,fm,fy,loan_sl_no) 
values
(@idnew0,@emp_id,@emp_code,@d_id,
(select id from pr_loan_master where loan_id=@loan_code),
@total_Amount,
@total_installment,@remaining_installment,@principal_installment,@interest_installment,@completed_installment,@sanction_date, 
@installment_start_date,@installment_end_date,case when @loan_code='HLADD' then 'Interest to be Recovered at End' when @loan_code = 'HL2A' then 'Interest to be Recovered at End' else  'Interest With Equal Installments' end,@interest_rate,
@installment_amount,@installment_amount,@total_recovered_amount,
NULL,1,@transidnew,@fm,@fy,@loanslno);
--inserting into transaction touch
 --print '1'
insert into transaction_touch([operation],[entity_name],[entity_oid],[key1],[trans_id]) values('I', 'pr_emp_adv_loans',@idnew0,'', @transidnew);






