update pr_emp_payslip set active=1, final_process=0 where fm=(select fm from pr_month_details where id in 
(select max(id)-1 from pr_month_details )) and spl_type='Regular';

update pr_pf_open_bal_year set os_cur=0 where os_cur is null;
update pr_pf_open_bal_year set os_cur_int=0 where os_cur_int is null;
update pr_pf_open_bal_year set bs_cur=0 where bs_cur is null;
update pr_pf_open_bal_year set bs_cur_int=0 where bs_cur_int is null;
update pr_pf_open_bal_year set vpf_cur=0 where vpf_cur is null;
update pr_pf_open_bal_year set vpf_cur_int=0 where vpf_cur_int is null;

update new_num set last_num=(select max(id) from pr_emp_adv_loans) where table_name='pr_emp_adv_loans';
update new_num set last_num=(select max(id) from pr_emp_adv_loans_child) where table_name='pr_emp_adv_loans_child';
update new_num set last_num=(select max(id) from pr_emp_adv_loans_adjustments) where table_name='pr_emp_adv_loans_adjustments';

update pr_emp_adv_loans set active=0 where id in (select emp_adv_loans_mid from pr_emp_adv_loans_child where active=1 and os_principal_amount<0 or os_interest_amount<0);

update pr_emp_adv_loans set active=0 where id in(select emp_adv_loans_mid from pr_emp_adv_loans_child ch join pr_emp_adv_loans l on l.id=ch.emp_adv_loans_mid where os_principal_amount=0 and os_interest_amount=0);

update new_num set last_num=(select max(id) from pr_month_details) where table_name='pr_month_details'; 