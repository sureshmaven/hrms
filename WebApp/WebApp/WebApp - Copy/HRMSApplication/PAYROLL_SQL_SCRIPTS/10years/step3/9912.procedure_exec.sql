exec sp_payslip_allowances

exec [sp_dm_pfcontributioncard] 2010

exec sm_dm_attendance;

exec sp_dm_pf_nominee '202010';

exec sm_dm_attendance_stopsal
drop table pr_emp_adv_loans_bef_monthend;
drop table pr_emp_adv_loans_child_bef_monthend;
drop table pr_emp_adv_loans_adjustments_bef_monthend;


select * into pr_emp_adv_loans_bef_monthend from pr_emp_adv_loans;
select * into pr_emp_adv_loans_child_bef_monthend from pr_emp_adv_loans_child;
select * into pr_emp_adv_loans_adjustments_bef_monthend from pr_emp_adv_loans_adjustments;

delete from pr_ob_share where fm=(select fm from pr_month_details where active=1)

-- New num id Update statemnt
update new_num set last_num=(select max(id) from pr_allowance_field_master )where table_name = 'pr_allowance_field_master';
update new_num set last_num=(select max(id) from pr_basic_anual_incr_master )where table_name = 'pr_basic_anual_incr_master';
update new_num set last_num=(select max(id) from pr_basic_stag_incr_master )where table_name = 'pr_basic_stag_incr_master';
update new_num set last_num=(select max(id) from pr_branch_allowance_master )where table_name = 'pr_branch_allowance_master';
update new_num set last_num=(select max(id) from pr_contribution_field_master )where table_name = 'pr_contribution_field_master';
update new_num set last_num=(select max(id) from pr_deduction_field_master )where table_name = 'pr_deduction_field_master';
update new_num set last_num=(select max(id) from pr_earn_field_master )where table_name = 'pr_earn_field_master';
update new_num set last_num=(select max(id) from pr_emp_adhoc_contribution_field )where table_name = 'pr_emp_adhoc_contribution_field';
update new_num set last_num=(select max(id) from pr_emp_adhoc_deduction_field )where table_name = 'pr_emp_adhoc_deduction_field';
update new_num set last_num=(select max(id) from pr_emp_adhoc_det_field )where table_name = 'pr_emp_adhoc_det_field';
update new_num set last_num=(select max(id) from pr_emp_adhoc_earn_field )where table_name = 'pr_emp_adhoc_earn_field';
update new_num set last_num=(select max(id) from pr_emp_adj_contribution_field )where table_name = 'pr_emp_adj_contribution_field';
update new_num set last_num=(select max(id) from pr_emp_adj_deduction_field )where table_name = 'pr_emp_adj_deduction_field';
update new_num set last_num=(select max(id) from pr_emp_adj_earn_field )where table_name = 'pr_emp_adj_earn_field';
update new_num set last_num=(select max(id) from pr_emp_adv_loan_type )where table_name = 'pr_emp_adv_loan_type';
update new_num set last_num=(select max(id) from pr_emp_adv_loans )where table_name = 'pr_emp_adv_loans';
update new_num set last_num=(select max(id) from pr_emp_adv_loans_adjustments )where table_name = 'pr_emp_adv_loans_adjustments';
update new_num set last_num=(select max(id) from pr_emp_adv_loans_adjustments_bef_monthend )where table_name = 'pr_emp_adv_loans_adjustments_bef_monthend';
update new_num set last_num=(select max(id) from pr_emp_adv_loans_bef_monthend )where table_name = 'pr_emp_adv_loans_bef_monthend';
update new_num set last_num=(select max(id) from pr_emp_adv_loans_child )where table_name = 'pr_emp_adv_loans_child';
update new_num set last_num=(select max(id) from pr_emp_adv_loans_child_bef_monthend )where table_name = 'pr_emp_adv_loans_child_bef_monthend';
update new_num set last_num=(select max(id) from pr_emp_allowance_process )where table_name = 'pr_emp_allowance_process';
update new_num set last_num=(select max(id) from pr_emp_allowances )where table_name = 'pr_emp_allowances';
update new_num set last_num=(select max(id) from pr_emp_allowances_gen )where table_name = 'pr_emp_allowances_gen';
update new_num set last_num=(select max(id) from pr_emp_allowances_spl )where table_name = 'pr_emp_allowances_spl';
update new_num set last_num=(select max(id) from pr_emp_branch_allowances )where table_name = 'pr_emp_branch_allowances';
update new_num set last_num=(select max(id) from pr_emp_categories )where table_name = 'pr_emp_categories';
update new_num set last_num=(select max(id) from pr_emp_deductions )where table_name = 'pr_emp_deductions';
update new_num set last_num=(select max(id) from pr_emp_deput_contribution_field )where table_name = 'pr_emp_deput_contribution_field';
update new_num set last_num=(select max(id) from pr_emp_deput_deduction_field )where table_name = 'pr_emp_deput_deduction_field';
update new_num set last_num=(select max(id) from pr_emp_deput_det_field )where table_name = 'pr_emp_deput_det_field';
update new_num set last_num=(select max(id) from pr_emp_designation )where table_name = 'pr_emp_designation';
update new_num set last_num=(select max(id) from pr_emp_epf_deduction_field )where table_name = 'pr_emp_epf_deduction_field';
update new_num set last_num=(select max(id) from pr_emp_epf_earn_field )where table_name = 'pr_emp_epf_earn_field';
update new_num set last_num=(select max(id) from pr_emp_hfc_details )where table_name = 'pr_emp_hfc_details';
update new_num set last_num=(select max(id) from pr_emp_inc_anual_stag )where table_name = 'pr_emp_inc_anual_stag';
update new_num set last_num=(select max(id) from pr_emp_inc_date_change )where table_name = 'pr_emp_inc_date_change';
update new_num set last_num=(select max(id) from pr_emp_incometax_12b )where table_name = 'pr_emp_incometax_12b';
update new_num set last_num=(select max(id) from pr_emp_incometax_12ba )where table_name = 'pr_emp_incometax_12ba';
update new_num set last_num=(select max(id) from pr_emp_incometax_12ba_master )where table_name = 'pr_emp_incometax_12ba_master';
update new_num set last_num=(select max(id) from pr_emp_jaib_caib_general )where table_name = 'pr_emp_jaib_caib_general';
update new_num set last_num=(select max(id) from pr_emp_lic_details )where table_name = 'pr_emp_lic_details';
update new_num set last_num=(select max(id) from pr_emp_loans_projection )where table_name = 'pr_emp_loans_projection';
update new_num set last_num=(select max(id) from pr_emp_other_tds_deductions )where table_name = 'pr_emp_other_tds_deductions';
update new_num set last_num=(select max(id) from pr_emp_pay_field )where table_name = 'pr_emp_pay_field';
update new_num set last_num=(select max(id) from pr_emp_payslip )where table_name = 'pr_emp_payslip';
update new_num set last_num=(select max(id) from pr_emp_payslip_allowance )where table_name = 'pr_emp_payslip_allowance';
update new_num set last_num=(select max(id) from pr_emp_payslip_deductions )where table_name = 'pr_emp_payslip_deductions';
update new_num set last_num=(select max(id) from pr_emp_payslip_netSalary )where table_name = 'pr_emp_payslip_netSalary';
update new_num set last_num=(select max(id) from pr_emp_perdeductions )where table_name = 'pr_emp_perdeductions';
update new_num set last_num=(select max(id) from pr_emp_perearning )where table_name = 'pr_emp_perearning';
update new_num set last_num=(select max(id) from pr_emp_pf_non_cert_elg )where table_name = 'pr_emp_pf_non_cert_elg';
update new_num set last_num=(select max(id) from pr_emp_pf_nonrepayable_loan )where table_name = 'pr_emp_pf_nonrepayable_loan';
update new_num set last_num=(select max(id) from pr_emp_pf_repayable_loan )where table_name = 'pr_emp_pf_repayable_loan';
update new_num set last_num=(select max(id) from pr_emp_promotion )where table_name = 'pr_emp_promotion';
update new_num set last_num=(select max(id) from pr_emp_rent_details )where table_name = 'pr_emp_rent_details';
update new_num set last_num=(select max(id) from pr_emp_tds_process )where table_name = 'pr_emp_tds_process';
update new_num set last_num=(select max(id) from pr_emp_tds_process_allowances )where table_name = 'pr_emp_tds_process_allowances';
update new_num set last_num=(select max(id) from pr_emp_tds_section_deductions )where table_name = 'pr_emp_tds_section_deductions';
update new_num set last_num=(select max(id) from pr_encashment_deductions_customization )where table_name = 'pr_encashment_deductions_customization';
update new_num set last_num=(select max(id) from pr_encashment_earnings_customization )where table_name = 'pr_encashment_earnings_customization';
update new_num set last_num=(select max(id) from pr_form16_codes )where table_name = 'pr_form16_codes';
update new_num set last_num=(select max(id) from pr_incometax_bank_payment )where table_name = 'pr_incometax_bank_payment';
update new_num set last_num=(select max(id) from pr_list_of_documents_master )where table_name = 'pr_list_of_documents_master';
update new_num set last_num=(select max(id) from pr_loan_master )where table_name = 'pr_loan_master';
update new_num set last_num=(select max(id) from pr_month_attendance )where table_name = 'pr_month_attendance';
update new_num set last_num=(select max(id) from pr_month_details )where table_name = 'pr_month_details';
update new_num set last_num=(select max(id) from pr_ob_share )where table_name = 'pr_ob_share';
update new_num set last_num=(select max(id) from pr_ob_share_adhoc )where table_name = 'pr_ob_share_adhoc';
update new_num set last_num=(select max(id) from pr_ob_share_encashment )where table_name = 'pr_ob_share_encashment';
update new_num set last_num=(select max(id) from pr_payroll_service_run )where table_name = 'pr_payroll_service_run';
update new_num set last_num=(select max(id) from pr_payslip_customization )where table_name = 'pr_payslip_customization';
update new_num set last_num=(select max(id) from pr_pf_nominee )where table_name = 'pr_pf_nominee';
update new_num set last_num=(select max(id) from pr_purpose_of_advance_master )where table_name = 'pr_purpose_of_advance_master';
update new_num set last_num=(select max(id) from pr_rentdetails_master )where table_name = 'pr_rentdetails_master';
update new_num set last_num=(select max(trans_id) from transaction_tbl )where table_name = 'transaction_tbl';



update loans set installment_amount=ploan.instamt from pr_emp_adv_loans loans join
TEST2.dbo.PEMPMAST m on loans.emp_code=m.empid join TEST2.dbo.Ploan ploan on m.eid=ploan.eid
where loans.loan_type_mid=3 and ploan.loanid='FEST'

update loans set installment_amount=ploan.instamt from pr_emp_adv_loans loans join
TEST2.dbo.PEMPMAST m on loans.emp_code=m.empid join TEST2.dbo.Ploan ploan on m.eid=ploan.eid
where loans.loan_type_mid=7 and ploan.loanid='HLADD'

update loans set installment_amount=ploan.instamt from pr_emp_adv_loans loans join
TEST2.dbo.PEMPMAST m on loans.emp_code=m.empid join TEST2.dbo.Ploan ploan on m.eid=ploan.eid
where loans.loan_type_mid=9 and ploan.loanid='HLPLT'

update loans set installment_amount=ploan.instamt from pr_emp_adv_loans loans join
TEST2.dbo.PEMPMAST m on loans.emp_code=m.empid join TEST2.dbo.Ploan ploan on m.eid=ploan.eid
where loans.loan_type_mid=10 and ploan.loanid='HOUS1'

update loans set installment_amount=ploan.instamt from pr_emp_adv_loans loans join
TEST2.dbo.PEMPMAST m on loans.emp_code=m.empid join TEST2.dbo.Ploan ploan on m.eid=ploan.eid
where loans.loan_type_mid=14 and ploan.loanid='MARR'

update loans set installment_amount=ploan.instamt from pr_emp_adv_loans loans join
TEST2.dbo.PEMPMAST m on loans.emp_code=m.empid join TEST2.dbo.Ploan ploan on m.eid=ploan.eid
where loans.loan_type_mid=15 and ploan.loanid='PERS'

update loans set installment_amount=ploan.instamt from pr_emp_adv_loans loans join
TEST2.dbo.PEMPMAST m on loans.emp_code=m.empid join TEST2.dbo.Ploan ploan on m.eid=ploan.eid
where loans.loan_type_mid=16 and ploan.loanid='PFHT1'

update loans set installment_amount=ploan.instamt from pr_emp_adv_loans loans join
TEST2.dbo.PEMPMAST m on loans.emp_code=m.empid join TEST2.dbo.Ploan ploan on m.eid=ploan.eid
where loans.loan_type_mid=17 and ploan.loanid='PFHT2'

update loans set installment_amount=ploan.instamt from pr_emp_adv_loans loans join
TEST2.dbo.PEMPMAST m on loans.emp_code=m.empid join TEST2.dbo.Ploan ploan on m.eid=ploan.eid
where loans.loan_type_mid=18 and ploan.loanid='PFLT1'

update loans set installment_amount=ploan.instamt from pr_emp_adv_loans loans join
TEST2.dbo.PEMPMAST m on loans.emp_code=m.empid join TEST2.dbo.Ploan ploan on m.eid=ploan.eid
where loans.loan_type_mid=19 and ploan.loanid='PFLT2'

update loans set installment_amount=ploan.instamt from pr_emp_adv_loans loans join
TEST2.dbo.PEMPMAST m on loans.emp_code=m.empid join TEST2.dbo.Ploan ploan on m.eid=ploan.eid
where loans.loan_type_mid=20 and ploan.loanid='PFLT3'

update loans set installment_amount=ploan.instamt from pr_emp_adv_loans loans join
TEST2.dbo.PEMPMAST m on loans.emp_code=m.empid join TEST2.dbo.Ploan ploan on m.eid=ploan.eid
where loans.loan_type_mid=21 and ploan.loanid='PFLT4'

update loans set installment_amount=ploan.instamt from pr_emp_adv_loans loans join
TEST2.dbo.PEMPMAST m on loans.emp_code=m.empid join TEST2.dbo.Ploan ploan on m.eid=ploan.eid
where loans.loan_type_mid=24 and ploan.loanid='VEH2W'

update loans set installment_amount=ploan.instamt from pr_emp_adv_loans loans join
TEST2.dbo.PEMPMAST m on loans.emp_code=m.empid join TEST2.dbo.Ploan ploan on m.eid=ploan.eid
where loans.loan_type_mid=25 and ploan.loanid='VEH4W'

update loans set installment_amount=ploan.instamt from pr_emp_adv_loans loans join
TEST2.dbo.PEMPMAST m on loans.emp_code=m.empid join TEST2.dbo.Ploan ploan on m.eid=ploan.eid
where loans.loan_type_mid=26 and ploan.loanid='PFL1'

update loans set installment_amount=ploan.instamt from pr_emp_adv_loans loans join
TEST2.dbo.PEMPMAST m on loans.emp_code=m.empid join TEST2.dbo.Ploan ploan on m.eid=ploan.eid
where loans.loan_type_mid=27 and ploan.loanid='PFL2'

update pr_loan_master set active=1 where id=6
update pr_loan_master set interest_rate=8 where id=24
update pr_loan_master set interest_rate=8 where id=25

UPDATE pr_emp_adv_loans_child  
SET interest_recovered_flag  = 0  
FROM pr_emp_adv_loans_child ch  
INNER JOIN pr_emp_adv_loans l ON ch.emp_adv_loans_mid = l.id  
WHERE ch.principal_recovered_flag=0 and ch.interest_recovered_flag=1 and l.active=1  