CREATE PROCEDURE [dbo].[sp_insert_fest_loans_child] @emp_code int ,@loan_code nvarchar(20),@date_disburse date, @total_amount integer ,@principal_amount_recovered integer,@total_amount_recovered integer,
 @principal_start_date date ,@principal_end_date date ,@total_principal_installments int , @os_principal_amount integer,@loanslno int,@os_remaining_amount_paid int
AS

declare @transidnew int; 
set @transidnew = (select last_num from new_num where table_name = 'transaction_tbl'); 

declare @idnew0 int; 
exec get_new_num 'pr_emp_adv_loans_child'; 
set @idnew0=(select  last_num from new_num where table_name = 'pr_emp_adv_loans_child');

declare @emp_id int;
set @emp_id=(select id from employees where EmpId=@emp_code);

declare @d_id int;
set @d_id=(select d.id from Designations d inner join Employees e on d.id=e.CurrentDesignation where e.EmpId=@emp_code);

declare @loan_id int ;
set @loan_id=(select id from pr_emp_adv_loans where emp_code=@emp_code and active=1 and 
loan_type_mid=(select l.loan_type_mid from pr_emp_adv_loans l inner join pr_loan_master m on l.loan_type_mid=m.id 
where l.emp_code=@emp_code and m.loan_id=@loan_code and l.active=1));

--if not exists (select * from pr_emp_adv_loans_child where loan_sl_no = @loanslno)
--begin

Insert into pr_emp_adv_loans_child(id,emp_adv_loans_mid,slno,date_disburse,loan_amount,interest_rate,interest_accured,
principal_amount_recovered,interest_amount_recovered,total_amount_recovered,priority,principal_start_date,principal_end_date,
interest_start_date,interest_end_date,total_principal_installments,total_interest_installments,os_principal_amount,os_interest_amount,
os_this_month_interest,os_total_amount,principal_recovered_flag,interest_recovered_flag,active,trans_id,loan_sl_no) 
values
(@idnew0,@loan_id, 1 ,@date_disburse,@total_amount,0, 0 , @principal_amount_recovered , 0 ,@total_amount_recovered , 1 ,
@principal_start_date,@principal_end_date,null,null,@total_principal_installments,0 ,@os_principal_amount,0  ,0 ,
@os_remaining_amount_paid,
case when @os_principal_amount= 0 then 1 else 0 end,0,1,@transidnew, @loanslno);
--inserting into transaction touch 
insert into transaction_touch([operation],[entity_name],[entity_oid],[key1],[trans_id]) 
values('I', 'pr_emp_adv_loans_child',@idnew0,'', @transidnew);

--end