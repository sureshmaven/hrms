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


insert into dm_emp_mn_task values (271,201908,'PS,RD,PE,PD,JA,PR,CB,TDS,PETDS,OTDS,NRL,FEST,PFHT1,PFHT2,VEH2W,HOUS1,VEH4W,')
insert into dm_emp_mn_task values (271,201907,'PS')
insert into dm_emp_mn_task values (271,201906,'PS')
insert into dm_emp_mn_task values (271,201905,'PS')
insert into dm_emp_mn_task values (271,201904,'PS,OB,')



insert into dm_emp_mn_task values (304,201908,'PS,RD,PE,PD,JA,PR,CB,TDS,PETDS,OTDS,NRL,ENCASH,')
insert into dm_emp_mn_task values (304,201907,'PS,ENCASH,')
insert into dm_emp_mn_task values (304,201906,'PS,ENCASH,')
insert into dm_emp_mn_task values (304,201905,'PS,ENCASH,')
insert into dm_emp_mn_task values (304,201904,'PS,,ENCASH,')


select * from dm_emp_mn_task;
delete from dm_emp_mn_task;

 select * from pr_emp_payslip where emp_code=304;
 select * from pr_emp_payslip_allowance where emp_code=304;
 select * from pr_emp_payslip_deductions where emp_code=304;
 select * from pr_emp_tds_process where empcode=304;
 select * from pr_emp_perearning where emp_code=304;
 select * from pr_emp_perdeductions where emp_code=304;
 select * from pr_emp_rent_details where emp_code=304;
 select * from pr_emp_other_tds_deductions where emp_code=304;
 select * from pr_emp_adv_loans where emp_code=304;


 delete from pr_emp_payslip where emp_code=304;
 delete from pr_emp_payslip_allowance where emp_code=304;
 delete from pr_emp_payslip_deductions where emp_code=304;
 delete from pr_emp_tds_process where empcode=304;
 delete from pr_emp_perearning where emp_code=304;
 delete from pr_emp_perdeductions where emp_code=304;
 delete from pr_emp_rent_details where emp_code=304;
 delete from pr_emp_other_tds_deductions where emp_code=304;
 delete from pr_emp_adv_loans where emp_code=304;

 exec sp_dm_main;

---pf openingbal
insert into dm_emp_mn_task values (271,2019,'PFOPEN,');

exec sp_dm_emp_master '2019-10-01',2020;


--Select distinct(emp_code) from dm_emp_mn_task;

delete from dm_emp_mn_task;

select Empid from test2.dbo.PEMPMAST where dor>GETDATE();


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



exec sp_dm_main;


-- exec sm_dm_attendance;

-- select * from pr_pfopeningbal;
-- select * from pr_ob_share;
-- select * from pr_emp_pf_nonrepayable_loan;

-- select * from pr_payroll_service_run;

-- delete from pr_payroll_service_run;

-- delete  from pr_month_attendance where active=1;


select * from pr_month_details;
insert into pr_month_details 
values(7,2020,'2019-09-01',0,0,null,681,0.1,68.1,1,1000,30,0,0);

update pr_month_details set active=0 where id=6;

select * from new_num where table_name='pr_month_details';

update new_num set last_num=7 where table_name='pr_month_details'; 


