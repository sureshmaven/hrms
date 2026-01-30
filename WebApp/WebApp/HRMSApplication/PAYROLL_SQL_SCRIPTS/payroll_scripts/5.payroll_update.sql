update pr_allowance_field_master set benefit='WITH PF' where name='FPIIP';
update pr_allowance_field_master set benefit='WITH PF' where name='Fixed Personal Allowance';
update pr_allowance_field_master set benefit='NO PAY' where name='Personal Pay';
update pr_allowance_field_master set benefit='NO PAY' where name='SPL Spl.Alw.ACSTI';
update pr_allowance_field_master set benefit='NO PAY' where name='SPL Key';
update pr_allowance_field_master set benefit='NO PAY' where name='SPL Record room sub staff all';
update pr_allowance_field_master set benefit='NO PAY' where name='Br Manager Allowance';
update pr_allowance_field_master set benefit='NO PAY' where name='FPA-HRA Allowance';
update pr_allowance_field_master set benefit='DA HRA' where name='SPL Watchman';
update pr_allowance_field_master set benefit='DA HRA' where name='SPL Duplicating/xerox machine';
update pr_allowance_field_master set benefit='DA HRA' where name='SPL Cashier';
update pr_allowance_field_master set benefit='DA HRA' where name='SPL Dafter';
update pr_allowance_field_master set benefit='DA HRA' where name='SPL Driver';
update pr_allowance_field_master set benefit='DA HRA' where name='SPL Jamedar';
update pr_allowance_field_master set benefit='DA HRA' where name='Personal Qual Allowance';

Update all_constants set constant='MidCCAConditionalAmt' where id=2;
update all_constants set constant='Special_Allw_Min' where id=31;


update pr_emp_inc_anual_stag set fy=2019;
update pr_emp_payslip set sentmail=0,downloadpdf=0,final_process=0;

-- update pr_emp_inc_date_change,pr_emp_inc_anual_stag tables , pavan , 15/07/2019
Update pr_emp_inc_date_change set inc_date=DateAdd(yy,+1,inc_date) where year(inc_date) between '2017' and '2018';
update pr_emp_inc_anual_stag set increment_date=DateAdd(yy,+1,increment_date) where year(increment_date) between '2017' and '2018';



--in userdata these records status show as NO
UPDATE pr_payslip_customization SET cust_status='No' where m_id = (select id from pr_allowance_field_master where name='Medical Allowance' and type='EMPA') and field_type='EMPA';
UPDATE pr_payslip_customization SET cust_status='No' where m_id = (select id from pr_allowance_field_master where name='Faculty Allowance' and type='EMPSA') and field_type='EMPSA';

--in userdata data these records not there
UPDATE pr_payslip_customization SET cust_status='Yes' where m_id = (select id from pr_earn_field_master where name='Employee TDS' and type='pay_fields') and field_type='pay_fields';
UPDATE pr_payslip_customization SET cust_status='No' where m_id = (select id from pr_earn_field_master where name='LIC Premium' and type='pay_fields') and field_type='pay_fields';
UPDATE pr_payslip_customization SET cust_status='No' where m_id = (select id from pr_earn_field_master where name='Loss of Pay' and type='pay_fields') and field_type='pay_fields';
UPDATE pr_payslip_customization SET cust_status='No' where m_id = (select id from pr_earn_field_master where name='Special Pay' and type='pay_fields') and field_type='pay_fields';
UPDATE pr_payslip_customization SET cust_status='No' where m_id = (select id from pr_allowance_field_master where name='Interim Allowance' and type='EMPA') and field_type='EMPA';
UPDATE pr_payslip_customization SET cust_status='No' where m_id = (select id from pr_allowance_field_master where name='FP Incentive Recovery' and type='EMPA') and field_type='EMPA';
UPDATE pr_payslip_customization SET cust_status='No' where m_id = (select id from pr_allowance_field_master where name='Petrol & Paper' and type='EMPA') and field_type='EMPA';
UPDATE pr_payslip_customization SET cust_status='No' where m_id = (select id from pr_allowance_field_master where name='Petrol & Paper 1' and type='EMPA') and field_type='EMPA';
UPDATE pr_payslip_customization SET cust_status='No' where m_id = (select id from pr_allowance_field_master where name='Children Education Allowance' and type='EMPA') and field_type='EMPA';
UPDATE pr_payslip_customization SET cust_status='No' where m_id = (select id from pr_allowance_field_master where name='Fest. Advance' and type='EMPA') and field_type='EMPA';
UPDATE pr_payslip_customization SET cust_status='No' where m_id = (select id from pr_allowance_field_master where name='LT PF Loan' and type='EMPA') and field_type='EMPA';
UPDATE pr_payslip_customization SET cust_status='No' where m_id = (select id from pr_allowance_field_master where name='INCENTIVE' and type='EMPA') and field_type='EMPA';
UPDATE pr_payslip_customization SET cust_status='No' where m_id = (select id from pr_allowance_field_master where name='INCENTIVE DIFF' and type='EMPA') and field_type='EMPA';
UPDATE pr_payslip_customization SET cust_status='No' where m_id = (select id from pr_allowance_field_master where name='LTC ENCASHMENT' and type='EMPA') and field_type='EMPA';


-- to update code in allowance master
update pr_allowance_field_master set code='' where name='Fest. Advance';



--Encashment setup status updation
--in userdata these records status show as Yes
--pay_fields,earnings
UPDATE pr_encashment_earnings_customization SET cust_status='Yes' where m_id = (select id from pr_earn_field_master where name='Basic' and type='pay_fields') and field_type='pay_fields';
UPDATE pr_encashment_earnings_customization SET cust_status='Yes' where m_id = (select id from pr_earn_field_master where name='Special Increment' and type='pay_fields') and field_type='pay_fields';
UPDATE pr_encashment_earnings_customization SET cust_status='Yes' where m_id = (select id from pr_earn_field_master where name='Stagnation Increments' and type='pay_fields') and field_type='pay_fields';
UPDATE pr_encashment_earnings_customization SET cust_status='Yes' where m_id = (select id from pr_earn_field_master where name='CAIIB Increment' and type='pay_fields') and field_type='pay_fields';
UPDATE pr_encashment_earnings_customization SET cust_status='Yes' where m_id = (select id from pr_earn_field_master where name='JAIIB Increment' and type='pay_fields') and field_type='pay_fields';
UPDATE pr_encashment_earnings_customization SET cust_status='Yes' where m_id = (select id from pr_earn_field_master where name='Special Pay' and type='pay_fields') and field_type='pay_fields';
UPDATE pr_encashment_earnings_customization SET cust_status='Yes' where m_id = (select id from pr_earn_field_master where name='System Administrator Allowance' and type='pay_fields') and field_type='pay_fields';
UPDATE pr_encashment_earnings_customization SET cust_status='Yes' where m_id = (select id from pr_earn_field_master where name='Interm Relief' and type='pay_fields') and field_type='pay_fields';

--EMPA
UPDATE pr_encashment_earnings_customization SET cust_status='Yes' where m_id = (select id from pr_allowance_field_master where name='Physically Handicapped' and type='EMPA') and field_type='EMPA';
UPDATE pr_encashment_earnings_customization SET cust_status='Yes' where m_id = (select id from pr_allowance_field_master where name='Fixed Personal Allowance' and type='EMPA') and field_type='EMPA';
UPDATE pr_encashment_earnings_customization SET cust_status='Yes' where m_id = (select id from pr_allowance_field_master where name='FPA-HRA Allowance' and type='EMPA') and field_type='EMPA';
UPDATE pr_encashment_earnings_customization SET cust_status='Yes' where m_id = (select id from pr_allowance_field_master where name='FPIIP' and type='EMPA') and field_type='EMPA';
UPDATE pr_encashment_earnings_customization SET cust_status='Yes' where m_id = (select id from pr_allowance_field_master where name='NPSG Allowance' and type='EMPA') and field_type='EMPA';
UPDATE pr_encashment_earnings_customization SET cust_status='Yes' where m_id = (select id from pr_allowance_field_master where name='Personal Pay' and type='EMPA') and field_type='EMPA';
UPDATE pr_encashment_earnings_customization SET cust_status='Yes' where m_id = (select id from pr_allowance_field_master where name='Personal Qual Allowance' and type='EMPA') and field_type='EMPA';

--EMPSA
UPDATE pr_encashment_earnings_customization SET cust_status='Yes' where m_id = (select id from pr_allowance_field_master where name='SPL Personal Pay' and type='EMPSA') and field_type='EMPSA';
UPDATE pr_encashment_earnings_customization SET cust_status='Yes' where m_id = (select id from pr_allowance_field_master where name='SPL Care Taker' and type='EMPSA') and field_type='EMPSA';
UPDATE pr_encashment_earnings_customization SET cust_status='Yes' where m_id = (select id from pr_allowance_field_master where name='SPL Cashier' and type='EMPSA') and field_type='EMPSA';
UPDATE pr_encashment_earnings_customization SET cust_status='Yes' where m_id = (select id from pr_allowance_field_master where name='SPL Driver' and type='EMPSA') and field_type='EMPSA';
UPDATE pr_encashment_earnings_customization SET cust_status='Yes' where m_id = (select id from pr_allowance_field_master where name='SPL Jamedar' and type='EMPSA') and field_type='EMPSA';
UPDATE pr_encashment_earnings_customization SET cust_status='Yes' where m_id = (select id from pr_allowance_field_master where name='SPL Key' and type='EMPSA') and field_type='EMPSA';
UPDATE pr_encashment_earnings_customization SET cust_status='Yes' where m_id = (select id from pr_allowance_field_master where name='SPL Lift Operator' and type='EMPSA') and field_type='EMPSA';
UPDATE pr_encashment_earnings_customization SET cust_status='Yes' where m_id = (select id from pr_allowance_field_master where name='SPL Non Promotional' and type='EMPSA') and field_type='EMPSA';
UPDATE pr_encashment_earnings_customization SET cust_status='Yes' where m_id = (select id from pr_allowance_field_master where name='SPL Watchman' and type='EMPSA') and field_type='EMPSA';
UPDATE pr_encashment_earnings_customization SET cust_status='Yes' where m_id = (select id from pr_allowance_field_master where name='SPL Stenographer' and type='EMPSA') and field_type='EMPSA';
UPDATE pr_encashment_earnings_customization SET cust_status='Yes' where m_id = (select id from pr_allowance_field_master where name='SPL Bill Collector' and type='EMPSA') and field_type='EMPSA';
UPDATE pr_encashment_earnings_customization SET cust_status='Yes' where m_id = (select id from pr_allowance_field_master where name='SPL Despatch' and type='EMPSA') and field_type='EMPSA';
UPDATE pr_encashment_earnings_customization SET cust_status='Yes' where m_id = (select id from pr_allowance_field_master where name='SPL Electrician' and type='EMPSA') and field_type='EMPSA';
UPDATE pr_encashment_earnings_customization SET cust_status='Yes' where m_id = (select id from pr_allowance_field_master where name='SPL Dafter' and type='EMPSA') and field_type='EMPSA';
UPDATE pr_encashment_earnings_customization SET cust_status='Yes' where m_id = (select id from pr_allowance_field_master where name='SPL Arrear Incentive' and type='EMPSA') and field_type='EMPSA';
UPDATE pr_encashment_earnings_customization SET cust_status='Yes' where m_id = (select id from pr_allowance_field_master where name='SPL Split Duty - Managers' and type='EMPSA') and field_type='EMPSA';
UPDATE pr_encashment_earnings_customization SET cust_status='Yes' where m_id = (select id from pr_allowance_field_master where name='SPL Duplicating/xerox machine' and type='EMPSA') and field_type='EMPSA';
---diductions
UPDATE pr_encashment_deductions_customization SET cust_status='Yes' where m_id = (select id from pr_deduction_field_master where name='VPF Deduction' and type='EPD') and field_type='EPD';





--designation name update in pr_emp_designations

update pr_emp_designation set designation='Manager Scale-1' where id=5;

--customization updates

update new_num set last_num=51 where table_name='pr_encashment_deductions_customization';

update new_num set last_num=66 where table_name='pr_encashment_earnings_customization';

update new_num set last_num=64 where table_name='pr_payslip_customization';







----Update scripts for Loans Master Client 
update pr_loan_master set active=0 where loan_description='CD LOAN 2'
update pr_loan_master set active=0 where loan_description='Education Loan'
update pr_loan_master set active=0 where loan_description='PF Advance LT1'
update pr_loan_master set active=0 where loan_description=' PF Advance LT2' 
update pr_loan_master set active=0 where loan_description=' PF Advance LT3' 
update pr_loan_master set active=0 where loan_description='PF Advance LT4'
update pr_loan_master set active=0 where loan_description='Society Loan'
update pr_loan_master set active=0 where loan_description='Staff Benefit Fund Loan'

update pr_loan_master set active=0 where loan_description='Housing Loan - 2'
update pr_loan_master set active=0 where loan_description='Housing Loan 2A'
update pr_loan_master set active=0 where loan_description='Housing Loan 2B-2C'
update pr_loan_master set active=0 where loan_description='Housing Loan Commerical'
update pr_loan_master set active=0 where loan_description='Housing Loan 2'
update pr_loan_master set active=0 where loan_description='Housing Loan 3'
update pr_loan_master set active=0 where loan_description='Housing Loan Int'

update pr_loan_master set active=0 where loan_description='PF Loan ST 1'
update pr_loan_master set active=0 where loan_description='PF Loan ST2'




update pr_earn_field_master set emp_master_code='Basic' where name='Basic' and type='pay_fields';
update pr_earn_field_master set emp_master_code='SPLINCR' where name='Special Increment' and type='pay_fields';
update pr_earn_field_master set emp_master_code='Emptds' where name='Employee Tds' and type='pay_fields';
update pr_earn_field_master set emp_master_code='LIC' where name='LIC Premium' and type='pay_fields';
update pr_earn_field_master set emp_master_code='SPLPAY' where name='Special Pay' and type='pay_fields';
update pr_earn_field_master set emp_master_code='SYSADMN' where name='System Administrator Allowance' and type='pay_fields';
update pr_earn_field_master set emp_master_code='TEACH' where name='Teaching Allowance' and type='pay_fields';
update pr_earn_field_master set emp_master_code='WASHALLW' where name='Washing Allowance' and type='pay_fields';
update pr_earn_field_master set emp_master_code='IRLF' where name='Interm Relief' and type='pay_fields';
update pr_earn_field_master set emp_master_code='FXDALLW' where name='Fixed Allowance' and type='pay_fields';
update pr_earn_field_master set emp_master_code='TSINCR' where name='Telangana Increment' and type='pay_fields';
update pr_earn_field_master set emp_master_code='Lop' where name='Loss of Pay' and type='pay_fields';
update pr_earn_field_master set emp_master_code='JAIIB' where name='JAIIB Increment' and type='pay_fields';
update pr_earn_field_master set emp_master_code='CAIIB' where name='CAIIB Increment' and type='pay_fields';
update pr_earn_field_master set emp_master_code='STAGALLOW' where name='Stagnation Increments' and type='pay_fields';
update pr_earn_field_master set emp_master_Code='ANNUALALLOW' where name='Annual Increment' and type='pay_fields'


update pr_allowance_field_master set emp_master_code='PH' where name='Physically Handicapped' and type='EMPA'
update pr_allowance_field_master set emp_master_code='DEPU' where name='Deputation Allowance' and type='EMPA'
 update pr_allowance_field_master set emp_master_code='FPA' where name='Fixed Personal Allowance' and type='EMPA'
update pr_allowance_field_master set emp_master_code='FPHRA' where name='FPA-HRA Allowance' and type='EMPA'
 update pr_allowance_field_master set emp_master_code='INTERIM' where name='Interim Allowance' and type='EMPA'
 update pr_allowance_field_master set emp_master_code='FPIIP' where name='FPIIP' and type='EMPA'
 update pr_allowance_field_master set emp_master_code='MEDICAL' where name='Medical Allowance' and type='EMPA'
 update pr_allowance_field_master set emp_master_code='NPSGA' where name='NPSG Allowance' and type='EMPA'
update pr_allowance_field_master set emp_master_code='OFFI' where name='Officiating Allowance' and type='EMPA'
 update pr_allowance_field_master set emp_master_code='PERPAY' where name='Personal Pay' and type='EMPA'
update pr_allowance_field_master set emp_master_code='PERQPAY' where name='Personal Qual Allowance' and type='EMPA'
 update pr_allowance_field_master set emp_master_code='RESATTN' where name='Res. Attenders Allowance' and type='EMPA'
 update pr_allowance_field_master set emp_master_code='FPIR' where name='FP Incentive Recovery' and type='EMPA'
 update pr_allowance_field_master set emp_master_code='BRMGR' where name='Br Manager Allowance' and type='EMPA'
 update pr_allowance_field_master set emp_master_code='pep' where name='Petrol & Paper' and type='EMPA'
 update pr_allowance_field_master set emp_master_code='pep1' where name='Petrol & Paper 1' and type='EMPA'
 update pr_allowance_field_master set emp_master_code='FESTADV' where name='Fest. Advance' and type='EMPA'
update pr_allowance_field_master set emp_master_code='LTPFLOAN' where name='LT PF Loan' and type='EMPA'
 update pr_allowance_field_master set emp_master_code='INCENTIVE' where name='INCENTIVE' and type='EMPA'
update pr_allowance_field_master set emp_master_code='INCENTDIFF' where name='INCENTIVE DIFF' and type='EMPA'
update pr_allowance_field_master set emp_master_code='LTCENC' where name='LTC ENCASHMENT' and type='EMPA'
update pr_allowance_field_master set emp_master_code='CHILDLOAN' where name='Children Education Allowance' and type='EMPA'

----------Special allowances:

 
update pr_allowance_field_master set emp_master_code='SP_CARETAKE' where name='SPL Care Taker' and type='EMPSA'
 update pr_allowance_field_master set emp_master_code='SP_CASHIER' where name='SPL Cashier' and type='EMPSA'
 update pr_allowance_field_master set emp_master_code='SP_DRIVER' where name='SPL Driver' and type='EMPSA'
 update pr_allowance_field_master set emp_master_code='SP_JAMEDAR' where name='SPL Jamedar' and type='EMPSA'
 update pr_allowance_field_master set emp_master_code='SP_KEY' where name='SPL Key' and type='EMPSA'
 update pr_allowance_field_master set emp_master_code='SP_LIFT' where name='SPL Lift Operator' and type='EMPSA' 
 update pr_allowance_field_master set emp_master_code='SP_NONPROM' where name='SPL Non Promotional' and type='EMPSA'
 update pr_allowance_field_master set emp_master_code='SP_SD_AWARD' where name='SPL Split Duty -Award staff' and type='EMPSA'
 update pr_allowance_field_master set emp_master_code='SP_TYPIST' where name='SPL Typist' and type='EMPSA'
 update pr_allowance_field_master set emp_master_code='SP_STENO' where name='SPL Stenographer' and type='EMPSA'
 update pr_allowance_field_master set emp_master_code='SP_WATCHMAN' where name='SPL Watchman' and type='EMPSA'
 update pr_allowance_field_master set emp_master_code='SP_BILLCOLL' where name='SPL Bill Collector' and type='EMPSA'
 update pr_allowance_field_master set emp_master_code='SP_DESPATCH' where name='SPL Despatch' and type='EMPSA'
 update pr_allowance_field_master set emp_master_code='SP_ELEC' where name='SPL Electrician' and type='EMPSA'
update pr_allowance_field_master set emp_master_code='SP_DAFTARI' where name='SPL Dafter' and type='EMPSA'
 update pr_allowance_field_master set emp_master_code='SP_CASHCAB' where name='SPL Cash Cabin' and type='EMPSA'
 update pr_allowance_field_master set emp_master_code='SP_TELEPHONE' where name='SPL Telephone Operator' and type='EMPSA'
 update pr_allowance_field_master set emp_master_code='SP_LIBRARY' where name='SPL Library' and type='EMPSA'
 update pr_allowance_field_master set emp_master_code='SP_INCENTIVE' where name='SPL Incentive' and type='EMPSA'
 update pr_allowance_field_master set emp_master_code='SP_ARREAR' where name='SPL Arrear Incentive' and type='EMPSA'
 update pr_allowance_field_master set emp_master_code='SP_CONVEY' where name='SPL Conveyance' and type='EMPSA'
 update pr_allowance_field_master set emp_master_code='SP_SD_MGR' where name='SPL Split Duty - Managers' and type='EMPSA'
 update pr_allowance_field_master set emp_master_code='SP_XEROX' where name='SPL Duplicating/xerox machine' and type='EMPSA'
 update pr_allowance_field_master set emp_master_code='SP_RECASST' where name='SPL Record room asst allowance' and type='EMPSA'
 update pr_allowance_field_master set emp_master_code='SP_RECSUB' where name='SPL Record room sub staff all' and type='EMPSA'
  update pr_allowance_field_master set emp_master_code='SP_RECEPTION' where name='SPL Receptionist allowance' and type='EMPSA'
 update pr_allowance_field_master set emp_master_code='SP_ACSTI' where name='SPL Spl.Alw.ACSTI' and type='EMPSA'
 update pr_allowance_field_master set emp_master_code='SPLPAY' where name='SPL Personal Pay' and type='EMPSA'
 update pr_allowance_field_master set emp_master_code='FCLTYALLW' where name='FACULTY ALLOWANCE' and type='EMPSA'
update pr_allowance_field_master set emp_master_code='test' where name='test' and type='EMPSA'
update pr_allowance_field_master set emp_master_code='example1' where name='example1' and type='EMPSA'


---------Deductions:


 
 update pr_deduction_field_master set emp_master_code='APCOBHFCHO' where name='APCOB-HFC-LT' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='APCOBHFCLT' where name='APCOB-HFC-HO' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='VIJAYA' where name='VIJAYA Coop Society Deduction' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='VISAKHA' where name='VISAKHA Coop Society Deduction' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='GSLI' where name='GSLI' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='OFFASSN' where name='Officers Assn Fund' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='OFFASSNC' where name='Cadre Officers Assn Fund' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='SUBCLUB' where name='Club Subscription' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='SUBUNION' where name='Union Club Subscription' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='SUBLT' where name='SC/ST Assn LT Subscription' and type = 'EPD';
  update pr_deduction_field_master set emp_master_code='SUBST' where name='SC/ST Assn ST Subscription' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='SBF' where name='LT Staff Benefit Fund' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='VPF' where name='VPF Deduction'and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='VPFPER' where name='VPF Percentage' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='LICPENSION' where name='LIC - PENSION' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='JEEVAN' where name='JEEVAN SURAKSHA' and type = 'EPD';
update pr_deduction_field_master set emp_master_code='HDFC' where name='HDFC'  and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='CCSAP' where name='CCA - AP' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='CCSHYD' where name='CCS - HYD' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='ABHLFHYD' where name='AB-HLF(HYD)' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='APCCADBEMP' where name='APCCADB - EMP CCS' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='APCOBEDLHNR' where name='APCOB-ED.LOAN-HNR.BR' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='APCOBEDLHO' where name='APCOB-ED.LOAN-HO' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='APCOBHFCLT' where name='APSCB-LT-Emp Assn' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='DRCCSVIZAG' where name='DR-CCS-VIZAG' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='COURTVIZAG' where name='PRN-JR CVL JUDGE, VIZAG' and type = 'EPD';
  update pr_deduction_field_master set emp_master_code='COURTTANUKU' where name='1 ADDL JR CVL JUDGE, TANUKU' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='KADAPADCCB' where name='KADAPA DCCB' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='COURTSLRPET' where name='JR CVL JUDGE, SULLURPET' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='KMLCOOPBANK' where name='KML COOP BANK, VIZAG'and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='ESTT' where name='ESTT-EXCESS HRA REC' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='FESTADV' where name='FESTIVAL ADVANCE' and type = 'EPD'; 
 update pr_deduction_field_master set emp_master_code='ANDHBANK' where name='ANDHRA BANK, RAMANTHPUR' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='COURTKADAPA' where name='SR.CIVIL JUDGE, KADAPA' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='MEDICALADVAN' where name='MEDICAL ADVANCE RECOVERY' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='COURTHYD' where name='XIX JR.CVL JUDGE, C C COURT, HYDERABAD' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='TGASSN' where name='BANKS EMP ASSN TELANGANA' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='LICMACHILI' where name='LIC MACHILIPATNAM'  and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='VEHMACHILI' where name='VEHICLE LOAN MACHILIPATNAM' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='FAMEDAK' where name='FESTIVAL ADVANCE MEDAK' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='FESTMEDAK' where name='FEST ADV MEDAK' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='COURTSECBAD' where name='XI JR.CVL JUDGE, C C COURT, SEC' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='FAELURU' where name='FESTIVAL ADVANCE ELURU' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='TGUNION' where name='TELANGANA EMP UNION' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='DCCB' where name='DCCB DEDUCTION' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='COURTDED' where name='COURT DEDUCTION' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='LIFEINS' where name='LIFE INSURANCE' and type = 'EPD';
  update pr_deduction_field_master set emp_master_code='TOFA' where name='TELANGANA OFFICERS ASSN' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='TSCABOA' where name='TSCAB OFFICERS ASSN' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='MISCDED' where name='MISC. DEDUCTION' and type = 'EPD';
 update pr_deduction_field_master set emp_master_code='pernlon' where name='PERSONAL LOAN'and type = 'EPD';

 
update designations set code='MGRD',Name='MGRD',Description='MGRD',updatedby=123456,updateddate=getdate() where id=28;


update PLE_Type set authorisation=0 where fy is null;

---purpose of advance master updatequery
update pr_purpose_of_advance_master set purpose_code='AddationatoExistingHouse' where purpose_name='Additional to Existing House' and ptype='Nonrepay';
update pr_purpose_of_advance_master set purpose_code='HigherEducation' where purpose_name='Higher Education' and ptype='Nonrepay';
update pr_purpose_of_advance_master set purpose_code='LicPolicy' where purpose_name='LIC Policy' and ptype='Nonrepay';
update pr_purpose_of_advance_master set purpose_code='Marriage' where purpose_name='Marriage' and ptype='Nonrepay';
update pr_purpose_of_advance_master set purpose_code='MedicalExpenditure' where purpose_name='Medical Expenditure' and ptype='Nonrepay';
update pr_purpose_of_advance_master set purpose_code='PurchaseofHouse' where purpose_name='Purchage of House' and ptype='Nonrepay';
update pr_purpose_of_advance_master set purpose_code='PurchaseofSite' where purpose_name='Purchage of Site' and ptype='Nonrepay';
update pr_purpose_of_advance_master set purpose_code='NR 90% Withdrawl' where purpose_name='NR 90% Withdrawl' and ptype='Nonrepay';
update pr_purpose_of_advance_master set purpose_code='PF Final Settlement' where purpose_name='PF Final Settlement' and ptype='Nonrepay';
update pr_purpose_of_advance_master set purpose_code='Repayment of CCF Loans' where purpose_name='Repayment of CCF Loans' and ptype='Nonrepay';


--- Rental details Description / name Updation 
update pr_rentdetails_master set name= 'House Rent Paid Apr (04)' where id=2
update pr_rentdetails_master set name= 'House Rent Paid May (05)' where id=3
update pr_rentdetails_master set name= 'House Rent Paid Jun (06)' where id=4
update pr_rentdetails_master set name= 'House Rent Paid Jul (07)' where id=5
update pr_rentdetails_master set name= 'House Rent Paid Aug (08)' where id=6
update pr_rentdetails_master set name= 'House Rent Paid Sep (09)' where id=7
update pr_rentdetails_master set name= 'House Rent Paid Oct (10)' where id=8
update pr_rentdetails_master set name= 'House Rent Paid Nov (11)' where id=9
update pr_rentdetails_master set name= 'House Rent Paid Dec (12)' where id=10
update pr_rentdetails_master set name= 'House Rent Paid Jan (01)' where id=11
update pr_rentdetails_master set name= 'House Rent Paid Feb (02)' where id=12
update pr_rentdetails_master set name= 'House Rent Paid Mar (03)' where id=13