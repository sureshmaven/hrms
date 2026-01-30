CREATE PROCEDURE [dbo].[sp_insert_three_sub_Loans_priority3] @emp_code int, @loan_code varchar(20) ,@date_disburse_three date,
@loan_amount_three int , @interest_rate_three float, @interest_accured_three float, @principal_amount_recovered_three int,
@interest_amount_recovered_three float, @total_amount_recovered_three float,@principal_start_date_three date ,
@principal_end_date_three date ,@interest_start_date_three date,@interest_end_date_three date ,@total_principal_installments_three int ,
@total_interest_installments_three int, @os_principal_amount_three int,@os_interest_amount_three float, @os_total_balace_amount float,
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
set @loan_id=(select id from pr_emp_adv_loans where emp_code=@emp_code and --active=1 and 
loan_type_mid=(select l.loan_type_mid from pr_emp_adv_loans l inner join pr_loan_master m on l.loan_type_mid=m.id 
where l.emp_code=@emp_code and m.loan_id=@loan_code ));--and l.active=1

if not exists (select * from pr_emp_adv_loans_child where loan_sl_no = @loanslno and priority = 3)
begin

Insert into pr_emp_adv_loans_child(id,emp_adv_loans_mid,slno,date_disburse,loan_amount,interest_rate,interest_accured,
principal_amount_recovered,interest_amount_recovered,total_amount_recovered,priority,principal_start_date,principal_end_date,
interest_start_date,interest_end_date,total_principal_installments,total_interest_installments,os_principal_amount,
os_interest_amount,os_this_month_interest,os_total_amount,principal_recovered_flag,interest_recovered_flag,active,trans_id,loan_sl_no) 
values(@idnew0,
@loan_id, 3 ,@date_disburse_three,@loan_amount_three,@interest_rate_three, @interest_accured_three, @principal_amount_recovered_three , 
@interest_amount_recovered_three ,@total_amount_recovered_three , 3 ,@principal_start_date_three, @principal_end_date_three,
@interest_start_date_three,@interest_end_date_three,@total_principal_installments_three,@total_interest_installments_three ,
@os_principal_amount_three, @os_interest_amount_three, 0 ,@os_total_balace_amount,
case when @os_principal_amount_three= 0 then 1 else 0 end,
case when @os_interest_amount_three= 0 then 1 else 0 end,
0,@transidnew, @loanslno);

insert into transaction_touch([operation],[entity_name],[entity_oid],[key1],[trans_id]) 
values('I', 'pr_emp_adv_loans_child',@idnew0,'', @transidnew); 

end