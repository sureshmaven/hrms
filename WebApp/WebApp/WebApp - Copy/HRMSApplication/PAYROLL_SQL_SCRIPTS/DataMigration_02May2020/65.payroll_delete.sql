delete  from pr_emp_adv_loans_adjustments  where emp_adv_loans_mid in (select id from pr_emp_adv_loans  where loan_type_mid is NULL)
delete  from pr_emp_adv_loans_child WHERE emp_adv_loans_mid in (select id from pr_emp_adv_loans  where loan_type_mid is NULL)
delete from pr_emp_adv_loans  where loan_type_mid is NULL
