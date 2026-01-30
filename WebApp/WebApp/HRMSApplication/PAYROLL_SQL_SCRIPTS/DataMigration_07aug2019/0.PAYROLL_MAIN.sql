-- Run the  below scripts as a group
-- Payroll Scripts

:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\payroll_scripts\1.payroll_creations.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\payroll_scripts\2.payroll_rename.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\payroll_scripts\3.payroll_alter.sql

---------

:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\payroll_scripts\4.payroll_insertions.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\payroll_scripts\5.payroll_update.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\payroll_scripts\6.payroll_UnusedTables.sql

----------

-- Run the  below scripts individually

:r  D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\payroll_scripts\7.get_new_num.sql
:r  D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\payroll_scripts\8.exist.sql
:r  D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\payroll_scripts\9.nulls.sql
:r  D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\payroll_scripts\10.gen_new_transaction.sql
:r  D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\payroll_scripts\11.month_data.sql
:r  D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\payroll_scripts\12.customization.sql
:r  D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\payroll_scripts\13.indexsAndNotnull.sql
:r  D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\payroll_scripts\14.primarykey.sql

-- Payroll Migration Scripts

:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\1.dm_emp_mn_task.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\2.financialperiodmethod.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\3.financialyearmethod.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\3.sp_month_details.sql
exec sp_month_details '201904'
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\4.sp_dm_emp_master.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\5.sp_dm_payslip.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\6.sp_dm_per_ded.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\7.sp_dm_per_earn.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\8.sp_dm_rent.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\9.sp_dm_tax_source.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\10.othertdsdeductions.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\11.sp_insert_exgration_tds_data.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\12.sp_dm_promotions.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\13.sp_dm_increments.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\14.sp_dm_increments_monthend.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\15.sp_dm_jaiib_caiib.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\16.sp_dm_pfcontributioncard.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\17.sp_dm_obshare_main.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\18.sp_dm_pfnonpayabledata.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\19.sp_insert_fest_loans.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\20.sp_insert_fest_loans_child.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\21.sp_insert_festival_loans_adjustments.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\22.sp_insert_pfloans_loans.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\23.sp_insert_pfloans_loans_child.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\24.sp_insert_pfloans_adjustments.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\25.sp_insert_pfloans_loans_ht2.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\26.sp_insert_pfloans_loans_child_ht2.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\27.sp_insert_pfloans_adjustments_ht2.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\28.sp_insert_two_wheller_single_loans.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\29.sp_insert_two_wheller_single_child.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\30.sp_insert_two_wheller_single_addjustments.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\31.sp_insert_two_sub_loans.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\32.sp_insert_two_sub_Loans_priority1.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\33.sp_insert_two_sub_Loans_priority2.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\34.sp_insert_loans_adjustments.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\35.sp_dm_sub_loans.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\36.sp_dm_loan_fest.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\37.sp_dm_loan_pfloans.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\38.sp_dm_loan_pfloans_ht2.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\39.sp_dm_loan_single_two_wheller.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\40.pr_basic_stag_incr_master.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\41.pr_basic_anual_incr_master.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\44.sp_dm_sub_loans_four_wheeler.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\45.sp_dm_loan_pfloans_lt4.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\46.sp_dm_loan_pfloans_lt3.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\47.sp_dm_loan_pfloans_lt2.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\48.sp_dm_sub_loans_hladd.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\49.sp_dm_sub_loans_hl2bc.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\50.sp_dm_sub_loans_hl2a.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\51.sp_dm_loan_personal.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\52.sp_dm_encashment.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\53.AnualIncrementChild.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\53.sp_dm_adhoc.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\54.AnualIncrementMonthEnd.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\55.AnualIncrementMain.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\56.sm_dm_form12ba.sql
exec sp_dm_form12ba '201912'
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\57.getLeaveBalance.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\58.sm_dm_attendance.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\59.sp_dm_allowance.sql
exec sp_dm_allowance '201912'
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\60.sp_dm_encashment_present.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\61.sp_dm_pf_nominee.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigration_07aug2019\42.sp_dm_main.sql




--------------------
-- Run the  below scripts as a group

:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigrationData\payslipdata.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigrationData\fest.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigrationData\hl2a_loan.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigrationData\hl2bc_loan.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigrationData\hladd_loan.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigrationData\housingloan.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigrationData\pers_loan.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigrationData\pfdata.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigrationData\pfht1.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigrationData\pfht2.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigrationData\pflt2_loan.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigrationData\pflt3_loan.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigrationData\pflt4_loan.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigrationData\vehicle2.sql
:r	D:\HRMS\branches\MainBranch\HRMS\branches\TSCAB\WebApp\HRMSApplication\PAYROLL_SQL_SCRIPTS\DataMigrationData\vehicle4.sql




----------------------------------------------------------------------------------------------------

--	1.)ONLY ONCE
--	GENERAL DATA:
--	financial period method
--	financial year method
--	sp_month_details
--	sp_dm_emp_master
	
--	PAYSLIP AND TDS
--	sp_dm_per_ded
--	sp_dm_per_earn
--	sp_dm_rent
--	sp_insert_exgration_tds_data
--	sp_dm_tax_source
--	sp_dm_pfnonpayabledata
--	sp_dm_pfcontributioncard
--	other tds deductions
	
--	2.)EVERY MONTH
--	sp_dm_increments
--	sp_dm_increments_monthend
--	sp_dm_jaiib_caiib
--	sp_dm_obshare_main
--	sp_dm_payslip
--	sp_dm_promotions
--	sp_dm_loan_fest
--	sp_dm_sub_loans
--	sp_insert_fest_loans
--	sp_insert_fest_loans_child
--	sp_insert_loans_adjustments
--	sp_insert_two_sub_Loans
--	sp_insert_two_sub_Loans_priority1
--	sp_insert_two_sub_Loans_priority2



-- insert into dm_emp_mn_task values (793,,201910,'PS,RD,PE,PD,JA,PR,CB,OTDS,PROM,IIB,ENCASH,ADHOC,');
-- insert into dm_emp_mn_task values (793,,201909,'PS,PROM,IIB,ENCASH,ADHOC,');
-- insert into dm_emp_mn_task values (793,,201908,'PS,PROM,IIB,ENCASH,ADHOC,'); 
-- insert into dm_emp_mn_task values (793,,201907,'PS,PROM,IIB,ENCASH,ADHOC,');
-- insert into dm_emp_mn_task values (793,,201906,'PS,PROM,IIB,ENCASH,ADHOC,'); 
-- insert into dm_emp_mn_task values (793,,201905,'PS,PROM,IIB,ENCASH,ADHOC,');  
-- insert into dm_emp_mn_task values (793,,201904,'PS,PROM,IIB,ENCASH,ADHOC,TDS,');

---pf openingbal
--insert into dm_emp_mn_task values (271,2019,'PFOPEN,');

exec sp_dm_emp_master '2019-12-01',2020;

exec increments '2016-02-01'

exec sp_dm_main;

update new_num set last_num=(select max(id) from pr_emp_adv_loans) where table_name='pr_emp_adv_loans';
update new_num set last_num=(select max(id) from pr_emp_adv_loans_child) where table_name='pr_emp_adv_loans_child';
update new_num set last_num=(select max(id) from pr_emp_adv_loans_adjustments) where table_name='pr_emp_adv_loans_adjustments';

update pr_emp_adv_loans set active=0 where id in (select emp_adv_loans_mid from pr_emp_adv_loans_child where active=1 and os_principal_amount<0 or os_interest_amount<0);




 insert into pr_month_details 
values(209,2020,'2019-12-01',0,0,null,681,0.1,68.1,1,1000,31,0,0);

 update pr_month_details set active=0 where fm='2019-11-01';

 update new_num set last_num=209 where table_name='pr_month_details'; 

exec sm_dm_attendance;

exec sp_dm_pf_nominee '201912';


--Select distinct(emp_code) from dm_emp_mn_task;




--update pr_month_details set fm='2019-09-01',payment_date='2019-09-01',da_slabs=681,da_percent=68.1 where active=1; 

-- select * from pr_emp_payslip;
-- select * from pr_emp_payslip_allowance;
-- select * from pr_emp_payslip_deductions;
-- select * from pr_emp_rent_details;
-- select * from pr_emp_perdeductions;
-- select * from pr_emp_perearning;
 --select * from pr_emp_tds_process;



-- delete from pr_emp_payslip;
-- delete from pr_emp_payslip_allowance;
-- delete from pr_emp_payslip_deductions;
-- delete from pr_emp_rent_details;
-- delete from pr_emp_perdeductions;
-- delete from pr_emp_perearning;


-- delete from pr_emp_general;
-- delete from pr_emp_biological_field;
-- delete from pr_emp_pay_field;
-- delete from pr_emp_allowances_gen;
-- delete from pr_emp_allowances_spl;
-- delete from pr_emp_deductions;
-- delete from pr_emp_lic_details;
-- delete from pr_emp_hfc_details;

-- delete from pr_emp_payslip;
-- delete from pr_emp_payslip_allowance;
-- delete from pr_emp_payslip_deductions;
-- delete from pr_emp_tds_process;
-- delete from pr_emp_perearning;
-- delete from pr_emp_perdeductions;
-- delete from pr_emp_rent_details;
-- delete from pr_emp_other_tds_deductions;
-- delete from pr_emp_adv_loans;
-- delete from pr_emp_adv_loans_child;
-- delete from pr_emp_adv_loans_adjustments;
-- delete from pr_emp_jaib_caib_general;
-- delete from pr_emp_promotion;


-- select * from pr_emp_payslip where emp_code=175;
-- select * from pr_emp_payslip_allowance where emp_code=175;
-- select * from pr_emp_payslip_deductions where emp_code=175;
-- select * from pr_emp_tds_process where empcode=175;
-- select * from pr_emp_perearning where emp_code=175;
-- select * from pr_emp_perdeductions where emp_code=175;
-- select * from pr_emp_rent_details where emp_code=175;
-- select * from pr_emp_other_tds_deductions where emp_code=175;
-- select * from pr_emp_adv_loans where emp_code=175;
--select * from pr_emp_adv_loans_child where emp_code=175;
--select * from pr_emp_adv_loans_adjustments where emp_code=175;

-- exec sm_dm_attendance;

-- select * from pr_pfopeningbal;
-- select * from pr_ob_share;
-- select * from pr_emp_pf_nonrepayable_loan;

-- select * from pr_payroll_service_run;

-- delete from pr_payroll_service_run;

-- delete  from pr_month_attendance where active=1;




