CREATE PROCEDURE [dbo].[sp_insert_three_sub_Loans_priority4] @emp_code int, @loan_code varchar(20) ,@date_disburse_four date,
@loan_amount_four int , @interest_rate_four float, @interest_accured_four float, @principal_amount_recovered_four int,
@interest_amount_recovered_four float, @total_amount_recovered_four float,@principal_start_date_four date ,
@principal_end_date_four date ,@interest_start_date_four date,@interest_end_date_four date ,@total_principal_installments_four int ,
@total_interest_installments_four int, @os_principal_amount_four int,@os_interest_amount_four float, @os_total_balace_amount float,
@loanslno int
AS
declare @transidnew int; 
set @transidnew = (select last_num from new_num where table_name = 'transaction_tbl'); 
declare @idnew0 int; 
exec get_new_num 'pr_emp_adv_loans_child'; 
set @idnew0=(select last_num from new_num where table_name = 'pr_emp_adv_loans_child');
declare @emp_id int;
set @emp_id=(select id from employees where EmpId=@emp_code);
declare @d_id int;
set @d_id=(select d.id from Designations d inner join Employees e on d.id=e.CurrentDesignation where e.EmpId=@emp_code);

declare @loan_id int ;
set @loan_id=(select id from pr_emp_adv_loans where emp_code=@emp_code and active=1 and 
loan_type_mid=(select l.loan_type_mid from pr_emp_adv_loans l inner join pr_loan_master m on l.loan_type_mid=m.id 
where l.emp_code=@emp_code and m.loan_id=@loan_code and l.active=1));

if not exists (select * from pr_emp_adv_loans_child where loan_sl_no = @loanslno and priority = 4)
begin

Insert into pr_emp_adv_loans_child(id,emp_adv_loans_mid,slno,date_disburse,loan_amount,interest_rate,interest_accured,
principal_amount_recovered,interest_amount_recovered,total_amount_recovered,priority,principal_start_date,principal_end_date,
interest_start_date,interest_end_date,total_principal_installments,total_interest_installments,os_principal_amount,
os_interest_amount,os_this_month_interest,os_total_amount,principal_recovered_flag,interest_recovered_flag,active,trans_id,loan_sl_no) 
values(@idnew0,
@loan_id, 4 ,@date_disburse_four,@loan_amount_four,@interest_rate_four, @interest_accured_four, @principal_amount_recovered_four , 
@interest_amount_recovered_four ,@total_amount_recovered_four , 4 ,@principal_start_date_four, @principal_end_date_four,
@interest_start_date_four,@interest_end_date_four,@total_principal_installments_four,@total_interest_installments_four ,
@os_principal_amount_four, @os_interest_amount_four, 0 ,@os_total_balace_amount,
case when @os_principal_amount_four= 0 then 1 else 0 end,
case when @os_interest_amount_four= 0 then 1 else 0 end,
1,@transidnew, @loanslno);

insert into transaction_touch([operation],[entity_name],[entity_oid],[key1],[trans_id]) 
values('I', 'pr_emp_adv_loans_child',@idnew0,'', @transidnew); 

end