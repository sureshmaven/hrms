truncate table 	pr_allowance_field_master	;
truncate table 	pr_basic_anual_incr_master	;
truncate table 	pr_basic_stag_incr_master	;
truncate table 	pr_branch_allowance_master	;
truncate table 	pr_contribution_field_master	;
truncate table 	pr_deduction_field_master	;
truncate table 	pr_earn_field_master	;
truncate table 	pr_emp_adhoc_contribution_field	;
truncate table 	pr_emp_adhoc_deduction_field	;
truncate table 	pr_emp_adhoc_det_field	;
truncate table 	pr_emp_adhoc_earn_field	;
truncate table 	pr_emp_adj_contribution_field	;
truncate table 	pr_emp_adj_deduction_field	;
truncate table 	pr_emp_adj_earn_field	;
truncate table 	pr_emp_adv_loan_type	;
truncate table 	pr_emp_adv_loans	;
truncate table 	pr_emp_adv_loans_adjustments	;
truncate table 	pr_emp_adv_loans_child	;
truncate table 	pr_emp_allowance_process	;
truncate table 	pr_emp_allowances	;
truncate table 	pr_emp_allowances_gen	;
truncate table 	pr_emp_allowances_spl	;
truncate table 	pr_emp_biological_field	;
truncate table 	pr_emp_branch_allowances	;
truncate table 	pr_emp_categories	;
truncate table 	pr_emp_deductions	;
truncate table 	pr_emp_deput_contribution_field	;
truncate table 	pr_emp_deput_deduction_field	;
truncate table 	pr_emp_deput_det_field	;
truncate table 	pr_emp_designation	;
truncate table 	pr_emp_epf_deduction_field	;
truncate table 	pr_emp_epf_earn_field	;
truncate table 	pr_emp_general	;
truncate table 	pr_emp_hfc_details	;
truncate table 	pr_emp_inc_anual_stag	;
truncate table 	pr_emp_inc_date_change	;
truncate table 	pr_emp_incometax_12b	;
truncate table 	pr_emp_incometax_12ba	;
truncate table 	pr_emp_incometax_12ba_master	;
truncate table 	pr_emp_jaib_caib_general	;
truncate table 	pr_emp_lic_details	;
truncate table 	pr_emp_loans_projection	;
truncate table 	pr_emp_other_tds_deductions	;
truncate table 	pr_emp_pay_field	;
truncate table 	pr_emp_payslip	;
truncate table 	pr_emp_payslip_allowance	;
truncate table 	pr_emp_payslip_allowance_Old	;
truncate table 	pr_emp_payslip_deductions	;
truncate table 	pr_emp_payslip_deductions_Old	;
truncate table 	pr_emp_payslip_netSalary	;
truncate table 	pr_emp_payslip_Old	;
truncate table 	pr_emp_perdeductions	;
truncate table 	pr_emp_perearning	;
truncate table 	pr_emp_pf_non_cert_elg	;
truncate table 	pr_emp_pf_nonrepayable_loan	;
truncate table 	pr_emp_pf_repayable_loan	;
truncate table 	pr_emp_promotion	;
truncate table 	pr_emp_rent_details	;
truncate table 	pr_emp_tds_process	;
truncate table 	pr_emp_tds_process_allowances	;
truncate table 	pr_emp_tds_section_deductions	;
truncate table 	pr_encashment_deductions_customization	;
truncate table 	pr_encashment_earnings_customization	;
truncate table 	pr_form16_codes	;
truncate table 	pr_incometax_bank_payment	;
truncate table 	pr_list_of_documents_master	;
truncate table 	pr_loan_master	;
truncate table 	pr_month_attendance	;
truncate table 	pr_month_details	;
truncate table 	pr_ob_share	;
truncate table 	pr_payroll_service_run	;
truncate table 	pr_payslip_customization	;
truncate table 	pr_pf_nominee	;
truncate table 	pr_purpose_of_advance_master	;
truncate table 	pr_rentdetails_master	;
truncate table 	new_num	;
truncate table 	transaction_tbl	;
truncate table 	transaction_touch	;
truncate table 	dm_emp_mn_task	;
truncate table 	dm_payslip_loans	;
truncate table 	dm_adhoc_earn	;
truncate table  all_constants;
truncate table pr_ob_share_encashment;
truncate table pr_ob_share_adhoc;
drop table pr_PAYROLL_SAMANTH_service_run;




delete from ple_type where payslip_mid is not null and fy is not null and fm is not null;
delete from employee_transfer where senoirity_order is not null and authorisation is not null and active is not null;

:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\payroll_insertions.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\payroll_update.sql


run the goodwill script  9.goodwill_scripts file individually

----------

-- Run the  below scripts individually
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\payroll_month_data.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\payroll_customization.sql

drop procedure 	sp_dm_adhoc	;
drop procedure 	sp_dm_allowance	;
drop procedure 	sp_dm_encashment	;
drop procedure 	sp_dm_encashment_present	;
drop procedure 	sp_dm_Exgraticas_tds	;
drop procedure 	sp_dm_form12ba	;
drop procedure 	sp_dm_increments	;
drop procedure 	sp_dm_increments_monthend	;
drop procedure 	sp_dm_jaiib_caiib	;
drop procedure 	sp_dm_loan_fest	;
drop procedure 	sp_dm_loan_personal	;
drop procedure 	sp_dm_loan_pfloans	;
drop procedure 	sp_dm_loan_pfloans_ht2	;
drop procedure 	sp_dm_loan_pfloans_lt2	;
drop procedure 	sp_dm_loan_pfloans_lt3	;
drop procedure 	sp_dm_loan_pfloans_lt4	;
drop procedure 	sp_dm_loan_single_two_wheller	;
drop procedure 	sp_dm_main	;
drop procedure 	sp_dm_obshare_main	;
drop procedure 	sp_dm_other_tds	;
drop procedure 	sp_dm_payslip	;
drop procedure 	sp_dm_per_ded	;
drop procedure 	sp_dm_per_earn	;
drop procedure 	sp_dm_pf_nominee	;
drop procedure 	sp_dm_pfcontributioncard	;
drop procedure 	sp_dm_pfnonrepayable	;
drop procedure 	sp_dm_promotions	;
drop procedure 	sp_dm_rent	;
drop procedure 	sp_dm_sub_loans	;
drop procedure 	sp_dm_sub_loans_four_wheeler	;
drop procedure 	sp_dm_sub_loans_hl2a	;
drop procedure 	sp_dm_sub_loans_hl2bc	;
drop procedure 	sp_dm_sub_loans_hladd	;
drop procedure 	sp_dm_tax_source	;
drop procedure 	sp_insert_fest_loans	;
drop procedure 	sp_insert_fest_loans_child	;
drop procedure 	sp_insert_festival_loans_adjustments	;
drop procedure 	sp_insert_loans_adjustments	;
drop procedure 	sp_insert_pfloans_adjustments	;
drop procedure 	sp_insert_pfloans_adjustments_ht2	;
drop procedure 	sp_insert_pfloans_loans	;
drop procedure 	sp_insert_pfloans_loans_child	;
drop procedure 	sp_insert_pfloans_loans_child_ht2	;
drop procedure 	sp_insert_pfloans_loans_ht2	;
drop procedure 	sp_insert_two_sub_Loans	;
drop procedure 	sp_insert_two_sub_Loans_priority1	;
drop procedure 	sp_insert_two_sub_Loans_priority2	;
drop procedure 	sp_insert_two_wheller_single_addjustments	;
drop procedure 	sp_insert_two_wheller_single_child	;
drop procedure 	sp_insert_two_wheller_single_loans	;
drop procedure 	sp_month_details	;
drop procedure 	increments	;
drop procedure  pr_emp_repayable;
drop procedure 	increments_month_end	;
drop procedure 	sm_dm_attendance	;
drop procedure  a;
drop procedure sp_dm_loan_single_two_wheller_fornodataemployees ;
drop procedure sp_dm_sub_loans_hl2a_fornodataemployees ;
drop procedure sp_dm_sub_loans_hl2bc_fornodataemployees;
drop procedure sp_dm_sub_loans_fornodataemployees ;
drop procedure sp_insert_three_sub_Loans_priority4;
drop procedure sp_dm_house_plot;
drop procedure 	sp_dm_emp_master	;
drop procedure GetLoansEmpData;
drop procedure sp_dm_tdsprocress;
drop procedure sp_dm_tdsprocess_earn_ded;
drop procedure sp_dm_pf_enash_adhoc;



--Retd employees data
--alter table employees add eid int;
--:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\62.sp_dm_retd_employees
--insert into pwinfile values(400,9999,'Other Branch',null,null);
--insert into pwinfile values(401,8888,'Other Desig',null,null);
--insert into pwinfile values(402,7777,'Other bloodgroup',null,null);
--insert into pwinfile values(403,6666,'Other Dept',null,null);
--exec sp_dm_retd_employees
--update employees set branch=42 where branch is null
--update employees set currentdesignation=24 where currentdesignation is null
--update employees set joineddesignation=24 where joineddesignation is null



-- Payroll Migration Scripts

:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\3.sp_month_details.sql
exec sp_month_details '201904','202006'
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\4.sp_dm_emp_master.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\5.sp_dm_payslip.sql 
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\6.sp_dm_per_ded.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\7.sp_dm_per_earn.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\8.sp_dm_rent.sql
--:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\9.sp_dm_tax_source.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\9.sp_dm_tdsprocress.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\9.sp_dm_tdsprocess_earn_ded.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\10.othertdsdeductions.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\11.sp_insert_exgration_tds_data.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\12.sp_dm_promotions.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\13.sp_dm_increments.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\14.sp_dm_increments_monthend.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\15.sp_dm_jaiib_caiib.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\16.sp_dm_pfcontributioncard.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\17.sp_dm_obshare_main.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\17.sp_dm_pf_enash_adhoc.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\18.sp_dm_pfnonpayabledata.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\18.sp_dm_pfpayabledata.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\19.sp_insert_fest_loans.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\20.sp_insert_fest_loans_child.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\21.sp_insert_festival_loans_adjustments.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\22.sp_insert_pfloans_loans.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\23.sp_insert_pfloans_loans_child.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\24.sp_insert_pfloans_adjustments.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\25.sp_insert_pfloans_loans_ht2.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\26.sp_insert_pfloans_loans_child_ht2.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\27.sp_insert_pfloans_adjustments_ht2.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\28.sp_insert_two_wheller_single_loans.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\29.sp_insert_two_wheller_single_child.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\30.sp_insert_two_wheller_single_addjustments.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\31.sp_insert_two_sub_loans.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\32.sp_insert_two_sub_Loans_priority1.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\33.sp_insert_two_sub_Loans_priority2.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\sp_insert_three_sub_Loans_priority0.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\sp_insert_three_sub_Loans_priority3.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\sp_insert_three_sub_Loans_priority4.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\34.sp_insert_loans_adjustments.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\35.sp_dm_sub_loans.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\36.sp_dm_loan_fest.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\37.sp_dm_loan_pfloans.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\38.sp_dm_loan_pfloans_ht2.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\39.sp_dm_loan_single_two_wheller.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\40.pr_basic_stag_incr_master.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\41.pr_basic_anual_incr_master.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\44.sp_dm_sub_loans_four_wheeler.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\45.sp_dm_loan_pfloans_lt4.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\46.sp_dm_loan_pfloans_lt3.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\47.sp_dm_loan_pfloans_lt2.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\48.sp_dm_sub_loans_hladd.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\49.sp_dm_sub_loans_hl2bc.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\50.sp_dm_sub_loans_hl2a.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\51.sp_dm_loan_personal.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\52.sp_dm_encashment.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\53.AnualIncrementChild.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\53.sp_dm_adhoc.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\54.AnualIncrementMonthEnd.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\55.AnualIncrementMain.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\56.sm_dm_form12ba.sql
exec sp_dm_form12ba '202006'
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\58.sm_dm_attendance.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\59.sp_dm_allowance.sql
exec sp_dm_allowance '202006'
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\60.sp_dm_encashment_present.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\61.sp_dm_pf_nominee.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\62.sp_dm_payslip_emp_data.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\63.sp_dm_GetEmpLoansData.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\64.sp_dm_ob_share_emp_data.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\42.sp_dm_main.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\Loans_08062020\39.sp_dm_loan_single_two_wheller_fornodataemployees.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\Loans_08062020\50.sp_dm_sub_loans_hl2a_fornodataemployees.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\Loans_08062020\10.sp_dm_sub_loans_hl2bc_fornodataemployees.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\Loans_08062020\35.sp_dm_sub_loans_fornodataemployees.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\Loans_08062020\sp_dm_house_plot.sql

exec sp_dm_payslip_emp_data 201904,1
exec sp_dm_payslip_emp_data 201905,2
exec sp_dm_payslip_emp_data 201906,2
exec sp_dm_payslip_emp_data 201907,2
exec sp_dm_payslip_emp_data 201908,2
exec sp_dm_payslip_emp_data 201909,2
exec sp_dm_payslip_emp_data 201910,2
exec sp_dm_payslip_emp_data 201911,2
exec sp_dm_payslip_emp_data 201912,2
exec sp_dm_payslip_emp_data 202001,2
exec sp_dm_payslip_emp_data 202002,2
exec sp_dm_payslip_emp_data 202003,2
exec sp_dm_payslip_emp_data 202004,2
exec sp_dm_payslip_emp_data 202005,3

exec sp_dm_pfnonrepayable 202006

exec pr_emp_repayable 202006

exec sp_dm_ob_share_emp_data 201904

exec GetLoansEmpData 202006,'FEST'
exec GetLoansEmpData 202006,'HL2A'
exec GetLoansEmpData 202006,'HL2BC'
exec GetLoansEmpData 202006,'HLADD'
exec GetLoansEmpData 202006,'HOUS1'
exec GetLoansEmpData 202006,'PERS'
exec GetLoansEmpData 202006,'PFHT1'
exec GetLoansEmpData 202006,'PFHT2'
exec GetLoansEmpData 202006,'PFLT2'
exec GetLoansEmpData 202006,'PFLT3'
exec GetLoansEmpData 202006,'PFLT4'
exec GetLoansEmpData 202006,'VEH2W'
exec GetLoansEmpData 202006,'VEH4W'
---emps not in poldloan
exec sp_dm_loan_single_two_wheller_fornodataemployees 239,202006
exec sp_dm_sub_loans_hl2a_fornodataemployees 443,202006
EXEC sp_dm_sub_loans_hl2a_fornodataemployees 360,202006
exec sp_dm_sub_loans_hl2bc_fornodataemployees 177,202006
exec sp_dm_sub_loans_hl2bc_fornodataemployees 5750,202006

EXEC sp_dm_sub_loans_fornodataemployees 360,202006
exec sp_dm_sub_loans_fornodataemployees	177,202006
exec sp_dm_sub_loans_fornodataemployees	332,202006
exec sp_dm_sub_loans_fornodataemployees	371,202006
exec sp_dm_sub_loans_fornodataemployees	438,202006
exec sp_dm_sub_loans_fornodataemployees	452,202006
exec sp_dm_sub_loans_fornodataemployees	5768,202006
exec sp_dm_sub_loans_fornodataemployees	5780,202006
exec sp_dm_sub_loans_fornodataemployees	5794,202006
exec sp_dm_sub_loans_fornodataemployees	5888,202006
exec sp_dm_sub_loans_fornodataemployees	5892,202006
exec sp_dm_sub_loans_fornodataemployees	5908,202006
exec sp_dm_sub_loans_fornodataemployees	6177,202006

exec sp_dm_house_plot 425,202006

---emps not in poldloan

update pr_emp_adv_loans set active=0  where emp_Code=239 and loan_type_mid=24 and sanction_date='2008-01-31';

exec sp_dm_emp_master '2020-06-01',2020;

exec increments '2016-02-01'

exec sp_dm_tdsprocress 202003,'2019-20'

exec sp_dm_tdsprocess_earn_ded 

exec sp_dm_pf_enash_adhoc

exec sp_dm_main;

:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\65.payroll_delete.sql
:r	F:\Mavensoft\hrms\prod\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_02May2020\66.payroll_update.sql

exec [sp_dm_pfcontributioncard] 2019

exec [sp_dm_pfcontributioncard] 2020


exec sm_dm_attendance;

exec sp_dm_pf_nominee '202006';


drop table pr_emp_adv_loans_bef_monthend;
drop table pr_emp_adv_loans_child_bef_monthend;
drop table pr_emp_adv_loans_adjustments_bef_monthend;


select * into pr_emp_adv_loans_bef_monthend from pr_emp_adv_loans;
select * into pr_emp_adv_loans_child_bef_monthend from pr_emp_adv_loans_child;
select * into pr_emp_adv_loans_adjustments_bef_monthend from pr_emp_adv_loans_adjustments;