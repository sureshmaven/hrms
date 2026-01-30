insert into new_num values('transaction_tbl','trans_id',0,0);
insert into new_num values('pr_adhoc_payments','id',0,0);
insert into new_num values('pr_adhoc_payments_det','id',0,0);
insert into new_num values('pr_adjust_payments','id',0,0);
insert into new_num values('pr_adv_loans_adj','id',0,0);
insert into new_num values('pr_allowance_field_master','id',52,0);
insert into new_num values('pr_basic_anual_incr_master','id',0,0);
insert into new_num values('pr_basic_stag_incr_master','id',0,0);
insert into new_num values('pr_branch_allowance_master','id',5,0);
insert into new_num values('pr_contribution_field_master','id',41,0);
insert into new_num values('pr_deduction_field_emp','id',0,0);
insert into new_num values('pr_deduction_field_master','id',149,0);
insert into new_num values('pr_deputation','id',0,0);
insert into new_num values('pr_deputation_det','id',0,0);
insert into new_num values('pr_earn_field_master','id',97,0);
insert into new_num values('pr_eligibility_amount_master','id',0,0);
insert into new_num values('pr_emp_adhoc_contribution_field','id',0,0);
insert into new_num values('pr_emp_adhoc_deduction_field','id',0,0);
insert into new_num values('pr_emp_adhoc_det_field','id',0,0);
insert into new_num values('pr_emp_adhoc_earn_field','id',0,0);
insert into new_num values('pr_emp_adj_contribution_field','id',0,0);
insert into new_num values('pr_emp_adj_deduction_field','id',0,0);
insert into new_num values('pr_emp_adj_earn_field','id',0,0);
insert into new_num values('pr_emp_adv_loan_type','id',0,0);
insert into new_num values('pr_emp_adv_loans','id',90000,0);
insert into new_num values('pr_emp_adv_loans_adjustments','id',720000,0);
insert into new_num values('pr_emp_adv_loans_child','id',180000,0);
insert into new_num values('pr_emp_allowance_process','id',0,0);
insert into new_num values('pr_emp_allowances','id',0,0);
insert into new_num values('pr_emp_allowances_gen','id',0,0);
insert into new_num values('pr_emp_allowances_spl','id',0,0);
insert into new_num values('pr_emp_biological_field','id',0,0);
insert into new_num values('pr_emp_branch_allowances','id',1500,0);
insert into new_num values('pr_emp_categories','id',8,0);
insert into new_num values('pr_emp_deductions','id',0,0);
insert into new_num values('pr_emp_deput_contribution_field','id',0,0);
insert into new_num values('pr_emp_deput_deduction_field','id',0,0);
insert into new_num values('pr_emp_deput_det_field','id',0,0);
insert into new_num values('pr_emp_designation','id',10,0);
insert into new_num values('pr_emp_encashment','id',0,0);
insert into new_num values('pr_emp_epf_deduction_field','id',0,0);
insert into new_num values('pr_emp_epf_earn_field','id',0,0);
insert into new_num values('pr_emp_general','id',0,0);
insert into new_num values('pr_emp_hfc_details','id',0,0);
insert into new_num values('pr_emp_inc_anual_stag','id',439,0);
insert into new_num values('pr_emp_inc_anual_stag_date_change','id',0,0);
insert into new_num values('pr_emp_inc_date_change','id',439,0);
insert into new_num values('pr_emp_incometaxdet','id',0,0);
insert into new_num values('pr_emp_jaib_caib_general','id',0,0);
insert into new_num values('pr_emp_lic_details','id',0,0);
insert into new_num values('pr_emp_loan_rate_select','id',0,0);
insert into new_num values('pr_emp_loans_projection','id',1,0);
insert into new_num values('pr_emp_pay_field','id',0,0);
insert into new_num values('pr_emp_payslip','id',50000,0);
insert into new_num values('pr_emp_payslip_allowance','id',1500000,0);
insert into new_num values('pr_emp_payslip_deductions','id',3000000,0);
insert into new_num values('pr_emp_perdeductions','id',25000,0);
insert into new_num values('pr_emp_perearning','id',25000,0);
insert into new_num values('pr_emp_pf_nominee','id',0,0);
insert into new_num values('pr_emp_pf_non_cert_elg','id',0,0);
insert into new_num values('pr_emp_pf_nonrepayable_loan','id',0,0);
insert into new_num values('pr_emp_pf_repayable_loan','id',0,0);
insert into new_num values('pr_emp_promotion','id',0,0);
insert into new_num values('pr_emp_rent_details','id',1200,0);
insert into new_num values('pr_emp_roi','id',0,0);
insert into new_num values('pr_emp_tds_process','id',0,0);
insert into new_num values('pr_emp_tds_section_deductions','id',0,0);
insert into new_num values('pr_incometax_bank_payment','id',0,0);
insert into new_num values('pr_list_of_documents_master','id',18,0);
insert into new_num values('pr_loan_master','id',27,0);
insert into new_num values('pr_month_attendance','id',0,0);
insert into new_num values('pr_month_details','id',200,0);
insert into new_num values('pr_ob_share','id',0,0);
insert into new_num values('pr_payroll_service_run','id',0,0);
insert into new_num values('pr_pf_loan_type_master','id',0,0);
insert into new_num values('pr_pf_nominee','id',459,0);
insert into new_num values('pr_purpose_of_advance_master','id',16,0);
insert into new_num values('pr_rentdetails_master','id',13,0);
insert into new_num values('pr_spl_allowance_field_master','id',0,0);
insert into new_num values('pr_emp_other_tds_deductions','id',0,0);
insert into new_num values('pr_emp_incometax_12ba','id',12000,0);
insert into new_num values('pr_emp_incometax_12ba_master','id',18,0);
insert into new_num values('pr_form16_codes','id',1,0);
insert into new_num values('pr_emp_adv_loans_bef_monthend','id',(select max(id) from pr_emp_adv_loans_bef_monthend),0);
insert into new_num values('pr_emp_adv_loans_child_bef_monthend','id',(select max(id) from pr_emp_adv_loans_child_bef_monthend),0);
insert into new_num values('pr_emp_adv_loans_adjustments_bef_monthend','id',(select max(id) from pr_emp_adv_loans_adjustments_bef_monthend),0);






---- start  earning fields script change   pr_earn_field_master-------



insert into pr_earn_field_master values (1,'Annual Increment','EPF',1,121,'','INCREMENT','');
insert into pr_earn_field_master values (2,'CAIIB Increment','EPF',1,122,'','','');
insert into pr_earn_field_master values (3,'JAIIB Increment','EPF',1,123,'','','');
insert into pr_earn_field_master values (4,'HRA DIFF-AUG-10','EPF',1,124,'','','');
insert into pr_earn_field_master values (5,'Arrear Gross Amount','EPF',1,125,'','','');
insert into pr_earn_field_master values (6,'Basic','Adj_Pay',1,126,'','','');
insert into pr_earn_field_master values (7,'Special Increment','Adj_Pay',1,127,'','','');
insert into pr_earn_field_master values (8,'Stagnation Increments','Adj_Pay',1,128,'','','');
insert into pr_earn_field_master values (9,'Annual Increment','Adj_Pay',1,129,'','','');
insert into pr_earn_field_master values (10,'CAIIB Increment','Adj_Pay',1,130,'','','');
insert into pr_earn_field_master values (11,'Basic','pay_fields',1,131,'','','');
insert into pr_earn_field_master values (12,'Special Increment','pay_fields',1,132,'','','');
insert into pr_earn_field_master values (13,'Stagnation Increments','pay_fields',1,133,'','STAGALLOW','');
insert into pr_earn_field_master values (14,'Annual Increment','pay_fields',1,134,'','INCREMENT','');
insert into pr_earn_field_master values (15,'CAIIB Increment','pay_fields',1,135,'','CAIIB','');
insert into pr_earn_field_master values (16,'JAIIB Increment','pay_fields',1,136,'','JAIIB','');
insert into pr_earn_field_master values (17,'Employee TDS','pay_fields',1,137,'','Tds','');
insert into pr_earn_field_master values (18,'LIC Premium','pay_fields',0,138,'','','');
insert into pr_earn_field_master values (19,'Loss of Pay','pay_fields',0,139,'','','');
insert into pr_earn_field_master values (20,'Special Pay','pay_fields',1,140,'WITH PAY,','SPLPAY','');
insert into pr_earn_field_master values (21,'System Administrator Allowance','pay_fields',1,141,'NO PAY','SYSADMN','');
insert into pr_earn_field_master values (22,'Teaching Allowance','pay_fields',1,142,'','','');
insert into pr_earn_field_master values (23,'Washing Allowance','pay_fields',1,143,'','','');
Insert into pr_earn_field_master values (24,'Interm Relief','pay_fields',1,144,'','','IRLF');
Insert into pr_earn_field_master values (25,'Fixed Allowance','pay_fields',1,145,'','','');
Insert into pr_earn_field_master values (26,'Telangana Increment','pay_fields',1,146,'NO PAY','','TSINCR');
insert into pr_earn_field_master values (27,'PFPerks','per_earn',1,147,'','','');
insert into pr_earn_field_master values (28,'LOANPerks','per_earn',1,148,'','','');
insert into pr_earn_field_master values (29,'Exgratia','per_earn',1,149,'','','');
insert into pr_earn_field_master values (30,'LTC','per_earn',1,150,'','','');
insert into pr_earn_field_master values (31,'Interest On NSC (Earning)','per_earn',1,151,'','','');
insert into pr_earn_field_master values (32,'Incentive','per_earn',1,152,'','','');
insert into pr_earn_field_master values (33,'Interim Relief','per_earn',1,153,'','','');
insert into pr_earn_field_master values (34,'Basic','adhoc',1,154,NULL,'','');
insert into pr_earn_field_master values (35,'Special Increment','adhoc',1,155,NULL,'','');
insert into pr_earn_field_master values (36,'Stagnation Increments','adhoc',1,156,NULL,'STAGALLOW','');
insert into pr_earn_field_master values (37,'Annual Increment','adhoc',1,157,NULL,'','');
insert into pr_earn_field_master values (38,'CAIIB Increment','adhoc',1,158,NULL,'','CAIIB');
insert into pr_earn_field_master values (39,'JAIIB Increment','adhoc',1,159,NULL,'','JAIIB');
insert into pr_earn_field_master values (40,'Special Pay','adhoc',1,160,NULL,'','');
insert into pr_earn_field_master values (41,'Physically Handicapped','adhoc',1,161,NULL,'','');
insert into pr_earn_field_master values (42,'System Administrator Allowance','adhoc',1,162,NULL,'','');
insert into pr_earn_field_master values (43,'Deputation Allowance','adhoc',1,163,NULL,'','');
insert into pr_earn_field_master values (44,'Fixed Personal Allowance','adhoc',1,164,NULL,'','');
insert into pr_earn_field_master values (45,'FPA-HRA Allowance','adhoc',1,165,NULL,'','');
insert into pr_earn_field_master values (46,'Interim Allowance','adhoc',1,166,NULL,'','');
insert into pr_earn_field_master values (47,'FPIIP','adhoc',1,167,NULL,'','');
insert into pr_earn_field_master values (48,'Medical Allowance','adhoc',1,168,NULL,'','');
insert into pr_earn_field_master values (49,'NPSG Allowance','adhoc',1,169,NULL,'','');
insert into pr_earn_field_master values (50,'Officiating Allowance','adhoc',1,170,NULL,'','');
insert into pr_earn_field_master values (51,'Personal Pay','adhoc',1,171,NULL,'','');
insert into pr_earn_field_master values (52,'SPL Personal Pay','adhoc',1,172,NULL,'','');
insert into pr_earn_field_master values (53,'Personal Qual Allowance','adhoc',1,173,NULL,'','');
insert into pr_earn_field_master values (54,'Res. Attenders Allowance','adhoc',1,174,NULL,'','');
insert into pr_earn_field_master values (55,'Teaching Allowance','adhoc',1,175,NULL,'','');
insert into pr_earn_field_master values (56,'SPL Care Taker','adhoc',1,176,NULL,'','');
insert into pr_earn_field_master values (57,'SPL Cashier','adhoc',1,177,NULL,'','');
insert into pr_earn_field_master values (58,'SPL Driver','adhoc',1,178,NULL,'','');
insert into pr_earn_field_master values (59,'SPL Jamedar','adhoc',1,179,NULL,'','');
insert into pr_earn_field_master values (60,'SPL Key','adhoc',1,180,NULL,'','');
insert into pr_earn_field_master values (61,'SPL Lift Operator','adhoc',1,181,NULL,'','');
insert into pr_earn_field_master values (62,'SPL Non Promotional','adhoc',1,182,NULL,'','');
insert into pr_earn_field_master values (63,'SPL Split Duty -Award staff','adhoc',1,183,NULL,'','');
insert into pr_earn_field_master values (64,'SPL Typist','adhoc',1,184,NULL,'','');
insert into pr_earn_field_master values (65,'SPL Watchman','adhoc',1,185,NULL,'','');
insert into pr_earn_field_master values (66,'SPL Stenographer','adhoc',1,186,NULL,'','');
insert into pr_earn_field_master values (67,'SPL Bill Collector','adhoc',1,187,NULL,'','');
insert into pr_earn_field_master values (68,'SPL Despatch','adhoc',1,188,NULL,'','');
insert into pr_earn_field_master values (69,'SPL Electrician','adhoc',1,189,NULL,'','');
insert into pr_earn_field_master values (70,'SPL Daftar','adhoc',1,190,NULL,'','');
insert into pr_earn_field_master values (71,'SPL Cash Cabin','adhoc',1,191,NULL,'','');
insert into pr_earn_field_master values (72,'SPL Telephone Operator','adhoc',1,192,NULL,'','');
insert into pr_earn_field_master values (73,'SPL Library','adhoc',1,193,NULL,'','');
insert into pr_earn_field_master values (74,'SPL Incentive','adhoc',1,194,NULL,'','');
insert into pr_earn_field_master values (75,'SPL Arrear Incentive','adhoc',1,195,NULL,'','');
insert into pr_earn_field_master values (76,'SPL Conveyance','adhoc',1,196,NULL,'','');
insert into pr_earn_field_master values (77,'SPL Split Duty - Managers','adhoc',1,197,NULL,'','');
insert into pr_earn_field_master values (78,'SPL Duplicating/xerox machine','adhoc',1,198,NULL,'','');
insert into pr_earn_field_master values (79,'SPL Record room asst allowance','adhoc',1,199,NULL,'','');
insert into pr_earn_field_master values (80,'SPL Record room sub.staff allowance','adhoc',1,200,NULL,'','');
insert into pr_earn_field_master values (81,'SPL Receptionist allowance','adhoc',1,201,NULL,'','');
insert into pr_earn_field_master values (82,'SPL Spl.Alw.ACSTI','adhoc',1,202,NULL,'','');
insert into pr_earn_field_master values (83,'Br Manager Allowance','adhoc',1,203,NULL,'','');
insert into pr_earn_field_master values (84,'DA','adhoc',1,204,NULL,'','DA');
insert into pr_earn_field_master values (85,'HRA','adhoc',1,205,NULL,'','');
insert into pr_earn_field_master values (86,'CCA','adhoc',1,206,NULL,'','CCA');
insert into pr_earn_field_master values (87,'HRA DIFF-AUG-10','adhoc',1,207,NULL,'','');
insert into pr_earn_field_master values (88,'Washing Allowance','adhoc',1,208,NULL,'','');
insert into pr_earn_field_master values (89,'Arrear Gross Amount','adhoc',1,209,NULL,'','');
insert into pr_earn_field_master values (90,'Interm Relief','adhoc',1,210,NULL,'','');
insert into pr_earn_field_master values (91,'Fixed Allowance','adhoc',1,211,NULL,'','');
insert into pr_earn_field_master values (92,'Telangana Increment','adhoc',1,212,NULL,'','');
insert into pr_earn_field_master values (93,'CEO Allowance','adhoc',1,213,NULL,'','');
insert into pr_earn_field_master values (94,'Shift Duty Allowance','adhoc',1,214,NULL,'','SP_SHFTDUTY');
insert into pr_earn_field_master values (95,'Spl. Allow','adhoc',1,215,NULL,'','');
insert into pr_earn_field_master values (96,'Spcl. DA','adhoc',1,216,NULL,'','SBASICDA');
insert into pr_earn_field_master values (97,'Faculty Allowance','adhoc',1,217,NULL,'','');
------------------------------- end-------------------------



insert into pr_deduction_field_master values (1,'FGPRM','EPF',1,121,null,'','');
insert into pr_deduction_field_master values (2,'VEH-INS','EPF',1,122,null,'','');
insert into pr_deduction_field_master values (3,'PHONE BILL','EPF',1,123,null,'','');
insert into pr_deduction_field_master values (4,'APCOB STAFF UNION LEVY','EPF',1,124,null,'','');
insert into pr_deduction_field_master values (5,'APCOB EMP ASSN LEVY','EPF',1,125,null,'','');
insert into pr_deduction_field_master values (6,'ESI Deduction','Adj_Pay',1,126,null,'','');
insert into pr_deduction_field_master values (7,'Income Tax','Adj_Pay',1,127,null,'','');
insert into pr_deduction_field_master values (8,'LIC','Adj_Pay',1,128,null,'','');
insert into pr_deduction_field_master values (9,'COURT Deductions','Adj_Pay',1,129,null,'COURT','');
insert into pr_deduction_field_master values (10,'Provident Fund','Adj_Pay',1,130,null,'','');
insert into pr_deduction_field_master values (11,'INCOME TAX','Dep_Ent',1,131,null,'','');
insert into pr_deduction_field_master values (12,'LIC','Dep_Ent',1,132,null,'','');
insert into pr_deduction_field_master values (13,'COURT Deductions','Dep_Ent',1,133,null,'COURT','');
insert into pr_deduction_field_master values (14,'Provident Fund','Dep_Ent',1,134,null,'','');
insert into pr_deduction_field_master values (15,'GSLI','Dep_Ent',1,135,null,'','');
insert into pr_deduction_field_master values (16,'APCOB-HFC-LT','EPD',1,136,null,'','');
insert into pr_deduction_field_master values (17,'APCOB-HFC-HO','EPD',1,137,null,'','');
insert into pr_deduction_field_master values (18,'VIJAYA Coop Society Deduction','EPD',1,138,null,'','');
insert into pr_deduction_field_master values (19,'VISAKHA Coop Society Deduction','EPD',1,139,null,'','');
insert into pr_deduction_field_master values (20,'GSLI','EPD',1,140,null,'','');
insert into pr_deduction_field_master values (21,'Officers Assn Fund','EPD',1,141,null,'','');
insert into pr_deduction_field_master values (22,'Cadre Officers Assn Fund','EPD',1,142,null,'','');
insert into pr_deduction_field_master values (23,'Club Subscription','EPD',1,143,null,'','');
insert into pr_deduction_field_master values (24,'Union Club Subscription','EPD',1,144,null,'','');
insert into pr_deduction_field_master values (25,'SC/ST Assn LT Subscription','EPD',1,145,null,'','');
insert into pr_deduction_field_master values (26,'SC/ST Assn ST Subscription','EPD',1,146,null,'','');
insert into pr_deduction_field_master values (27,'LT Staff Benefit Fund','EPD',1,147,null,'','');
insert into pr_deduction_field_master values (28,'VPF Deduction','EPD',1,148,null,'VPF','');
insert into pr_deduction_field_master values (29,'VPF Percentage','EPD',1,149,null,'','');
insert into pr_deduction_field_master values (30,'LIC - PENSION','EPD',1,150,null,'','');
insert into pr_deduction_field_master values (31,'JEEVAN SURAKSHA','EPD',1,151,null,'','');
insert into pr_deduction_field_master values (32,'HDFC','EPD',1,152,null,'','');
insert into pr_deduction_field_master values (33,'CCA - AP','EPD',1,153,null,'','');
insert into pr_deduction_field_master values (34,'CCS - HYD','EPD',1,154,null,'CCSHYD','');
insert into pr_deduction_field_master values (35,'AB-HLF(HYD)','EPD',1,155,null,'','');
insert into pr_deduction_field_master values (36,'APCCADB - EMP CCS','EPD',1,156,null,'APCCADBEMP','');
insert into pr_deduction_field_master values (37,'APCOB-ED.LOAN-HNR.BR','EPD',1,157,null,'','');
insert into pr_deduction_field_master values (38,'APCOB-ED.LOAN-HO','EPD',1,158,null,'','');
insert into pr_deduction_field_master values (39,'APSCB-LT-Emp Assn','EPD',1,159,null,'','');
insert into pr_deduction_field_master values (40,'DR-CCS-VIZAG','EPD',1,160,null,'','');
insert into pr_deduction_field_master values (41,'PRN-JR CVL JUDGE, VIZAG','EPD',1,161,null,'','');
insert into pr_deduction_field_master values (42,'1 ADDL JR CVL JUDGE, TANUKU','EPD',1,162,null,'','');
insert into pr_deduction_field_master values (43,'KADAPA DCCB','EPD',1,163,null,'','');
insert into pr_deduction_field_master values (44,'JR CVL JUDGE, SULLURPET','EPD',1,164,null,'','');
insert into pr_deduction_field_master values (45,'KML COOP BANK, VIZAG','EPD',1,165,null,'','');
insert into pr_deduction_field_master values (46,'ESTT-EXCESS HRA REC','EPD',1,166,null,'','');
insert into pr_deduction_field_master values (47,'FESTIVAL ADVANCE','EPD',1,167,null,'','');
insert into pr_deduction_field_master values (48,'ANDHRA BANK, RAMANTHPUR','EPD',1,168,null,'','');
insert into pr_deduction_field_master values (49,'SR.CIVIL JUDGE, KADAPA','EPD',1,169,null,'','');
insert into pr_deduction_field_master values (50,'MEDICAL ADVANCE RECOVERY','EPD',1,170,null,'','');
insert into pr_deduction_field_master values (51,'XIX JR.CVL JUDGE, C C COURT, HYDERABAD','EPD',1,171,null,'','');
insert into pr_deduction_field_master values (52,'BANKS EMP ASSN TELANGANA','EPD',1,172,null,'TGASSN','');
insert into pr_deduction_field_master values (53,'LIC MACHILIPATNAM','EPD',1,173,null,'','');
insert into pr_deduction_field_master values (54,'VEHICLE LOAN MACHILIPATNAM','EPD',1,174,null,'','');
insert into pr_deduction_field_master values (55,'FESTIVAL ADVANCE MEDAK','EPD',1,175,null,'','');
insert into pr_deduction_field_master values (56,'FEST ADV MEDAK','EPD',1,176,null,'','');
insert into pr_deduction_field_master values (57,'XI JR.CVL JUDGE, C C COURT, SEC','EPD',1,177,null,'','');
insert into pr_deduction_field_master values (58,'FESTIVAL ADVANCE ELURU','EPD',1,178,null,'','');
insert into pr_deduction_field_master values (59,'TELANGANA EMP UNION','EPD',1,179,null,'TGUNION','');
insert into pr_deduction_field_master values (60,'DCCB DEDUCTION','EPD',1,180,null,'','');
insert into pr_deduction_field_master values (61,'COURT DEDUCTION','EPD',1,181,null,'COURTDED','');
insert into pr_deduction_field_master values (62,'LIFE INSURANCE','EPD',1,182,null,'','');
insert into pr_deduction_field_master values (63,'TELANGANA OFFICERS ASSN','EPD',1,183,null,'TOFA','');
insert into pr_deduction_field_master values (64,'TSCAB OFFICERS ASSN','EPD',1,184,null,'TSCABOA','');
insert into pr_deduction_field_master values (65,'MISC. DEDUCTION','EPD',1,185,null,'MISCDED','');
insert into pr_deduction_field_master values (66,'PERSONAL LOAN','EPD',1,186,null,'','');
insert into pr_deduction_field_master values (67,'HFC','per_ded',1,187,null,'','');
insert into pr_deduction_field_master values (68,'HFC INT','per_ded',1,188,null,'','');
insert into pr_deduction_field_master values (69,'Interest On NSC (Deduction)','per_ded',1,189,null,'','');
insert into pr_deduction_field_master values (70,'PPF','per_ded',1,190,null,'','');
insert into pr_deduction_field_master values (71,'TAX SAVER FD','per_ded',1,191,null,'','');
insert into pr_deduction_field_master values (72,'INTEREST ON EDUCATION LOAN','per_ded',1,192,null,'','');
insert into pr_deduction_field_master values (73,'Housing Loan principle (financial Year recovered amount)','per_ded',1,193,null,'','');
insert into pr_deduction_field_master values (74,'Housing Loan Interest (financial interest amount)','per_ded',1,194,null,'','');
insert into pr_deduction_field_master values (75,'ESI Deduction','adhoc',1,195,null,'','');
insert into pr_deduction_field_master values (76,'Income Tax','adhoc',1,196,null,'','');
insert into pr_deduction_field_master values (77,'LIC','adhoc',1,197,null,'','');
insert into pr_deduction_field_master values (78,'COURT Deductions','adhoc',1,198,null,'COURTDED','');
insert into pr_deduction_field_master values (79,'Provident Fund','adhoc',1,199,null,'','');
insert into pr_deduction_field_master values (80,'VIJAYA Coop Society Deduction','adhoc',1,200,null,'','');
insert into pr_deduction_field_master values (81,'VISAKHA Coop Society Deduction','adhoc',1,201,null,'','');
insert into pr_deduction_field_master values (82,'GSLI','adhoc',1,202,null,'','');
insert into pr_deduction_field_master values (83,'Officers Assn Fund','adhoc',1,203,null,'','');
insert into pr_deduction_field_master values (84,'Cadre Officers Assn Fund','adhoc',1,204,null,'','');
insert into pr_deduction_field_master values (85,'Club Subscription','adhoc',1,205,null,'','');
insert into pr_deduction_field_master values (86,'Union Subscription','adhoc',1,206,null,'','');
insert into pr_deduction_field_master values (87,'SC/ST Assn LT Subscription','adhoc',1,207,null,'','');
insert into pr_deduction_field_master values (88,'SC/ST Assn ST Subscription','adhoc',1,208,null,'SUBST','');
insert into pr_deduction_field_master values (89,'FP Incentive Recovery','adhoc',1,209,null,'','');
insert into pr_deduction_field_master values (90,'LT Staff Benefit Fund','adhoc',1,210,null,'','');
insert into pr_deduction_field_master values (91,'VPF','adhoc',1,211,null,'','');
insert into pr_deduction_field_master values (92,'Prof. Tax','adhoc',1,212,null,'','');
insert into pr_deduction_field_master values (93,'LIC - PENSION','adhoc',1,213,null,'','');
insert into pr_deduction_field_master values (94,'JEEVAN SURAKSHA','adhoc',1,214,null,'','');
insert into pr_deduction_field_master values (95,'HDFC','adhoc',1,215,null,'','');
insert into pr_deduction_field_master values (96,'CCA - AP','adhoc',1,216,null,'','');
insert into pr_deduction_field_master values (97,'CCS - HYD','adhoc',1,217,null,'','');
insert into pr_deduction_field_master values (98,'AB-HLF(HYD)','adhoc',1,218,null,'','');
insert into pr_deduction_field_master values (99,'APCCADB - EMP CC','adhoc',1,219,null,'','');
insert into pr_deduction_field_master values (100,'APCOB-ED.LOAN-HNR-BR','adhoc',1,220,null,'','');
insert into pr_deduction_field_master values (101,'APCOB-ED-LOAN-HO','adhoc',1,221,null,'','');
insert into pr_deduction_field_master values (102,'APCOB-HFC-HO','adhoc',1,222,null,'','');
insert into pr_deduction_field_master values (103,'APCOBHFCLT','adhoc',1,223,null,'','');
insert into pr_deduction_field_master values (104,'One day Sal Recovery','adhoc',1,224,null,'','');
insert into pr_deduction_field_master values (105,'Miscellaneous Deduction ','adhoc',1,225,null,'MISCDED','');
insert into pr_deduction_field_master values (106,'HFC','adhoc',1,226,null,'','');
insert into pr_deduction_field_master values (107,'One day Salary Recovery','adhoc',1,227,null,'','');
insert into pr_deduction_field_master values (108,'HOU-INSU-PRE','adhoc',1,228,null,'','');
insert into pr_deduction_field_master values (109,'EMP_ASSN_SUB','adhoc',1,229,null,'','');
insert into pr_deduction_field_master values (110,'COD_INS_PRM','adhoc',1,230,null,'','');
insert into pr_deduction_field_master values (111,'COD-INS-PRM1','adhoc',1,231,null,'','');
insert into pr_deduction_field_master values (112,'FGPRM','adhoc',1,232,null,'','');
insert into pr_deduction_field_master values (113,'VEH-INS','adhoc',1,233,null,'','');
insert into pr_deduction_field_master values (114,'PHONE BILL','adhoc',1,234,null,'','');
insert into pr_deduction_field_master values (115,'APSCB-LT-Emp ASSN','adhoc',1,235,null,'','');
insert into pr_deduction_field_master values (116,'DR-CCS-VIZAG','adhoc',1,236,null,'','');
insert into pr_deduction_field_master values (117,'PRN-JR CVL JUDGE, VIZAG','adhoc',1,237,null,'','');
insert into pr_deduction_field_master values (118,'1 ADDL JR CVL JUDGE, TANUKU','adhoc',1,238,null,'','');
insert into pr_deduction_field_master values (119,'KADAPA DCCB','adhoc',1,239,null,'','');
insert into pr_deduction_field_master values (120,'JR CVL JUDGE, SULLURPET','adhoc',1,240,null,'','');
insert into pr_deduction_field_master values (121,'KML COOP BANK, VIZAG','adhoc',1,241,null,'','');
insert into pr_deduction_field_master values (122,'ESTT-EXCESS HRA REC','adhoc',1,242,null,'','');
insert into pr_deduction_field_master values (123,'FESTIVAL ADVANCE','adhoc',1,243,null,'','');
insert into pr_deduction_field_master values (124,'ANDHRA BANK, RAMANTHPUR','adhoc',1,244,null,'','');
insert into pr_deduction_field_master values (125,'SR.CIVIL JUDGE, KADAPA','adhoc',1,245,null,'','');
insert into pr_deduction_field_master values (126,'MEDICAL ADVANCE RECOVERY','adhoc',1,246,null,'','');
insert into pr_deduction_field_master values (127,'APCOB STAFF UNION LEVY','adhoc',1,247,null,'','');
insert into pr_deduction_field_master values (128,'APCOB EMP ASSN LEVY','adhoc',1,248,null,'','');
insert into pr_deduction_field_master values (129,'XIX JR.CVL COURT, C C COURT, HYDERABAD','adhoc',1,249,null,'','');
insert into pr_deduction_field_master values (130,'XIX JR CVL COURT, C C COURT HYDERABAD','adhoc',1,250,null,'','');
insert into pr_deduction_field_master values (131,'LTC ADVANCE RECOVERY','adhoc',1,251,null,'','');
insert into pr_deduction_field_master values (132,'BANKS EMP ASSN TELANGANA','adhoc',1,252,null,'','');
insert into pr_deduction_field_master values (133,'LIC ADVANCE RECOVERY','adhoc',1,253,null,'','');
insert into pr_deduction_field_master values (134,'BANKS EMP ASSN TELANGANA','adhoc',1,254,null,'','');
insert into pr_deduction_field_master values (135,'LIC MACHILIPATNAM','adhoc',1,255,null,'','');
insert into pr_deduction_field_master values (136,'VEHICLE LOAN MACHILIPATNAM','adhoc',1,256,null,'','');
insert into pr_deduction_field_master values (137,'FESTIVAL ADVANCE MEDAK','adhoc',1,257,null,'','');
insert into pr_deduction_field_master values (138,'XI JR.CVL JUDGE, C C COURT, SEC','adhoc',1,258,null,'','');
insert into pr_deduction_field_master values (139,'FESTIVAL ADVANCE ELURU','adhoc',1,259,null,'','');
insert into pr_deduction_field_master values (140,'FESTIVAL ADVANCE ONGOLE','adhoc',1,260,null,'','');
insert into pr_deduction_field_master values (141,'TELANGANA EMP UNION','adhoc',1,261,null,'','');
insert into pr_deduction_field_master values (142,'LIFE INSURANCE','adhoc',1,262,null,'','');
insert into pr_deduction_field_master values (143,'DCCB DEDUCTION','adhoc',1,263,null,'','');
insert into pr_deduction_field_master values (144,'COURT DEDUCTIONS','adhoc',1,264,null,'','');
insert into pr_deduction_field_master values (145,'TELANGANA OFFICERS ASSN','adhoc',1,265,null,'','');
insert into pr_deduction_field_master values (146,'HOUSE PROPERTY INSURANCE','adhoc',1,266,null,'','');
insert into pr_deduction_field_master values (147,'TSCAB OFFICERS ASSN','adhoc',1,267,null,'','');
insert into pr_deduction_field_master values (148,'MISC. DEDUCTION','adhoc',1,268,null,'','');
insert into pr_deduction_field_master values (149,'PERSONAL LOAN1','adhoc',1,269,null,'','');
insert into pr_deduction_field_master values (150,'Officers Assn Fund','Dep_Ent',1,270,null,'','');
insert into pr_deduction_field_master values (151,'Cadre Officers Assn Fund','Dep_Ent',1,271,null,'','');
insert into pr_deduction_field_master values (152,'Club Subscription','Dep_Ent',1,272,null,'','');
insert into pr_deduction_field_master values (153,'Union Subscription','Dep_Ent',1,273,null,'','');
insert into pr_deduction_field_master values (154,'SC/ST Assn LT Subscription','Dep_Ent',1,274,null,'','');
insert into pr_deduction_field_master values (155,'SC/ST Assn ST Subscription','Dep_Ent',1,275,null,'','');
insert into pr_deduction_field_master values (156,'VPF','Dep_Ent',1,276,null,'','');
insert into pr_deduction_field_master values (157,'CCS-AP','Dep_Ent',1,277,null,'','');
insert into pr_deduction_field_master values (158,'CCS-HYD','Dep_Ent',1,278,null,'','');
insert into pr_deduction_field_master values (159,'APCCADB-EMP CC','Dep_Ent',1,279,null,'','');
insert into pr_deduction_field_master values (160,'APCOB-HFC-HO','Dep_Ent',1,280,null,'','');
insert into pr_deduction_field_master values (161,'APCOBHFCLT','Dep_Ent',1,281,null,'','');
insert into pr_deduction_field_master values (162,'BANKS EMP ASSN TELANGANA','Dep_Ent',1,232,null,'','');



insert into pr_contribution_field_master values (1,'HRA Rate','Adj_Pay',1,121);
insert into pr_contribution_field_master values (2,'MAX CCA','Adj_Pay',1,122);
insert into pr_contribution_field_master values (3,'CCA Lower','Adj_Pay',1,123);
insert into pr_contribution_field_master values (4,'Earned Bonus','Adj_Pay',1,124);
insert into pr_contribution_field_master values (5,'PF Wages','Adj_Pay',1,125);
insert into pr_contribution_field_master values (6,'Max Pension','Dep_Ent',1,126);
insert into pr_contribution_field_master values (7,'PF Contribution','Dep_Ent',1,127);
insert into pr_contribution_field_master values (8,'HRA Rate','adhoc',1,128);
insert into pr_contribution_field_master values (9,'MAX CCA','adhoc',1,129);
insert into pr_contribution_field_master values (10,'CCA Lower','adhoc',1,130);
insert into pr_contribution_field_master values (11,'Earned Bonus','adhoc',1,131);
insert into pr_contribution_field_master values (12,'PF Wages','adhoc',1,132);
insert into pr_contribution_field_master values (13,'FPF Contribution','adhoc',1,133);
insert into pr_contribution_field_master values (14,'PF Adminstration Charges','adhoc',1,134);
insert into pr_contribution_field_master values (15,'EDLI Salary','adhoc',1,135);
insert into pr_contribution_field_master values (16,'EDLI Adminstration Charges','adhoc',1,136);
insert into pr_contribution_field_master values (17,'EDLI Contribution','adhoc',1,137);
insert into pr_contribution_field_master values (18,'PF Total Deduction','adhoc',1,138);
insert into pr_contribution_field_master values (19,'ESI Wages','adhoc',1,139);
insert into pr_contribution_field_master values (20,'ESI Contribution','adhoc',1,140);
insert into pr_contribution_field_master values (21,'ESI Total Deduction','adhoc',1,141);
insert into pr_contribution_field_master values (22,'ESI Daily Wages','adhoc',1,142);
insert into pr_contribution_field_master values (23,'Current Round Amount','adhoc',1,143);
insert into pr_contribution_field_master values (24,'Previous Round Amount','adhoc',1,144);
insert into pr_contribution_field_master values (25,'Max Pension','adhoc',1,145);
insert into pr_contribution_field_master values (26,'PF Contribution','adhoc',1,146);
insert into pr_contribution_field_master values (27,'Ptax Gross Pay','adhoc',1,147);
insert into pr_contribution_field_master values (28,'CPtax Deduction','adhoc',1,148);
insert into pr_contribution_field_master values (29,'Petrol & Paper','adhoc',1,149);
insert into pr_contribution_field_master values (30,'All Allowances','adhoc',1,150);
insert into pr_contribution_field_master values (31,'STD Allowances','adhoc',1,151);
insert into pr_contribution_field_master values (32,'STD Increments','adhoc',1,152);
insert into pr_contribution_field_master values (33,'MIS Allowances','adhoc',1,153);
insert into pr_contribution_field_master values (34,'Loans Amount','adhoc',1,154);
insert into pr_contribution_field_master values (35,'STD Deductions','adhoc',1,155);
insert into pr_contribution_field_master values (36,'MIS Deductions','adhoc',1,156);
insert into pr_contribution_field_master values (37,'itbsrcode','adhoc',1,157);
insert into pr_contribution_field_master values (38,'itchallan no','adhoc',1,158);
insert into pr_contribution_field_master values (39,'it cheque no','adhoc',1,159);
insert into pr_contribution_field_master values (40,'itchallan date','adhoc',1,160);
insert into pr_contribution_field_master values (41,'Spl. Allowance Rate','adhoc',1,161);


-------------------Start Allowance fields master script change pr_allowance_field_master ---------------------------


insert into pr_allowance_field_master values(1,'Physically Handicapped','EMPA',1,1,'','','');
insert into pr_allowance_field_master values(2,'Deputation Allowance','EMPA',1,2,'','','');
insert into pr_allowance_field_master values(3,'Fixed Personal Allowance','EMPA',1,3,'','','');
insert into pr_allowance_field_master values(4,'FPA-HRA Allowance','EMPA',1,4,'','','');
insert into pr_allowance_field_master values(5,'Interim Allowance','EMPA',1,5,'','','');
insert into pr_allowance_field_master values(6,'FPIIP','EMPA',1,6,'','FPIIP','');
insert into pr_allowance_field_master values(7,'Medical Allowance','EMPA',1,7,'','','');
insert into pr_allowance_field_master values(8,'NPSG Allowance','EMPA',1,8,'','','');
insert into pr_allowance_field_master values(9,'Officiating Allowance','EMPA',1,9,'','','');
insert into pr_allowance_field_master values(10,'Personal Pay','EMPA',1,10,'','','');
insert into pr_allowance_field_master values(11,'Personal Qual Allowance','EMPA',1,11,'','',''); 
insert into pr_allowance_field_master values(12,'Res. Attenders Allowance','EMPA',1,12,'','','');
insert into pr_allowance_field_master values(13,'FP Incentive Recovery','EMPA',1,13,'','','');
insert into pr_allowance_field_master values(14,'Br Manager Allowance','EMPA',1,14,'','BR_MGR','');
insert into pr_allowance_field_master values(15,'Petrol & Paper','EMPA',1,15,'','','');
insert into pr_allowance_field_master values(16,'Petrol & Paper 1','EMPA',1,16,'','','');
insert into pr_allowance_field_master values(17,'Children Education Allowance','EMPA',1,17,'','CEOALLW','');
insert into pr_allowance_field_master values(18,'Fest. Advance','EMPA',1,18,'','','');
insert into pr_allowance_field_master values(19,'LT PF Loan','EMPA',1,19,'','','');
insert into pr_allowance_field_master values(20,'INCENTIVE','EMPA',1,20,'','','');
insert into pr_allowance_field_master values(21,'INCENTIVE DIFF','EMPA',1,21,'','','');
insert into pr_allowance_field_master values(22,'LTC ENCASHMENT','EMPA',1,22,'','','');
insert into pr_allowance_field_master values(23,'SPL Care Taker','EMPSA',1,23,'','','');
insert into pr_allowance_field_master values(24,'SPL Cashier','EMPSA',1,24,'','SP_CASHIER','');
insert into pr_allowance_field_master values(25,'SPL Driver','EMPSA',1,25,'','SP_DRIVER','');
insert into pr_allowance_field_master values(26,'SPL Jamedar','EMPSA',1,26,'','','SP_JAMEDAR');
insert into pr_allowance_field_master values(27,'SPL Key','EMPSA',1,27,'','SP_KEY','');
insert into pr_allowance_field_master values(28,'SPL Lift Operator','EMPSA',1,28,'','','');
insert into pr_allowance_field_master values(29,'SPL Non Promotional','EMPSA',1,29,'','','');
insert into pr_allowance_field_master values(30,'SPL Split Duty -Award staff','EMPSA',1,30,'','','');
insert into pr_allowance_field_master values(31,'SPL Typist','EMPSA',1,31,'','','');
insert into pr_allowance_field_master values(32,'SPL Watchman','EMPSA',1,32,'','SP_WATCHMAN','');
insert into pr_allowance_field_master values(33,'SPL Stenographer','EMPSA',1,33,'','','');
insert into pr_allowance_field_master values(34,'SPL Bill Collector','EMPSA',1,34,'','','');
insert into pr_allowance_field_master values(35,'SPL Despatch','EMPSA',1,35,'','','');
insert into pr_allowance_field_master values(36,'SPL Electrician','EMPSA',1,36,'','SP_ELEC','');
insert into pr_allowance_field_master values(37,'SPL Dafter','EMPSA',1,37,'','SP_DAFTARI','');
insert into pr_allowance_field_master values(38,'SPL Cash Cabin','EMPSA',1,38,'','','');
insert into pr_allowance_field_master values(39,'SPL Telephone Operator','EMPSA',1,39,'','','');
insert into pr_allowance_field_master values(40,'SPL Library','EMPSA',1,40,'','','');
insert into pr_allowance_field_master values(41,'SPL Incentive','EMPSA',1,41,'','','');
insert into pr_allowance_field_master values(42,'SPL Arrear Incentive','EMPSA',1,42,'','','');
insert into pr_allowance_field_master values(43,'SPL Conveyance','EMPSA',1,43,'','','');
insert into pr_allowance_field_master values(44,'SPL Split Duty - Managers','EMPSA',1,44,'','SP_SD_MGR','');
insert into pr_allowance_field_master values(45,'SPL Duplicating/xerox machine','EMPSA',1,45,'','SP_XEROX','');
insert into pr_allowance_field_master values(46,'SPL Record room asst allowance','EMPSA',1,46,'','','');
insert into pr_allowance_field_master values(47,'SPL Record room sub staff all','EMPSA',1,47,'','','');
insert into pr_allowance_field_master values(48,'SPL Receptionist allowance','EMPSA',1,48,'','','');
insert into pr_allowance_field_master values(49,'SPL Spl.Alw.ACSTI','EMPSA',1,49,'','','');
insert into pr_allowance_field_master values(50,'SPL Personal Pay','EMPSA',1,50,'','','');
insert into pr_allowance_field_master values(51,'FACULTY ALLOWANCE','EMPSA',1,51,'','FACULTYALLW','');

-------------------------------------end pr_allowance_field_master---------------------------



Insert into pr_rentdetails_master values(1,'Section 89 Amount',1,150);
Insert into pr_rentdetails_master values(2,'House Rent Paid 04',1,150);
Insert into pr_rentdetails_master values(3,'House Rent Paid 05',1,150);
Insert into pr_rentdetails_master values(4,'House Rent Paid 06',1,150);
Insert into pr_rentdetails_master values(5,'House Rent Paid 07',1,150);
Insert into pr_rentdetails_master values(6,'House Rent Paid 08',1,150);
Insert into pr_rentdetails_master values(7,'House Rent Paid 09',1,150);
Insert into pr_rentdetails_master values(8,'House Rent Paid 10',1,150);
Insert into pr_rentdetails_master values(9,'House Rent Paid 11',1,150);
Insert into pr_rentdetails_master values(10,'House Rent Paid 12',1,150);
Insert into pr_rentdetails_master values(11,'House Rent Paid 01',1,150);
Insert into pr_rentdetails_master values(12,'House Rent Paid 02',1,150);
Insert into pr_rentdetails_master values(13,'House Rent Paid 03',1,150);


--all_masters(lvn1)


insert into all_masters values('AB1','AB-HFL(HYD)','Loan Vendor Name','PR_Loans_Advances',0,0,1);
insert into all_masters values('CCS1','CCS - AP','Loan Vendor Name','PR_Loans_Advances',0,0,1);
insert into all_masters values('CCS2','CCS - HYD','Loan Vendor Name','PR_Loans_Advances',0,0,1);
insert into all_masters values('CCS3','APCCADB - EMP CCS','Loan Vendor Name','PR_Loans_Advances',0,0,1);
insert into all_masters values('EDHNR','APCOB ED.LOAN MNR Branch','Loan Vendor Name','PR_Loans_Advances',0,0,1);
insert into all_masters values('EDHO','APCOB ED.LOAN HO','Loan Vendor Name','PR_Loans_Advances',0,0,1);
insert into all_masters values('HDFC','H D F C','Loan Vendor Name','PR_Loans_Advances',0,0,1);
insert into all_masters values('HFC1','APCOB HFC-1(LT)','Loan Vendor Name','PR_Loans_Advances',0,0,1);
insert into all_masters values('HFC2','APCOB HFC-2(FT)','Loan Vendor Name','PR_Loans_Advances',0,0,1);
insert into all_masters values('HFC3','APCOB HFC (HO)','Loan Vendor Name','PR_Loans_Advances',0,0,1);
insert into all_masters values('HFC4','APCOB HFC (DSNR BR)','Loan Vendor Name','PR_Loans_Advances',0,0,1);


--pr_loan_master

insert into pr_loan_master values(1,'CD2','CD LOAN 2',0,1,1);
insert into pr_loan_master values(2,'EDUC','Education Loan',0,1,2);
insert into pr_loan_master values(3,'FEST','Festival Advance',0,1,3);
insert into pr_loan_master values(4,'HL2 ','Housing Loan - 2',6,1,4);
insert into pr_loan_master values(5,'HL2A','Housing Loan 2A',11,1,5);
insert into pr_loan_master values(6,'HL2BC','Housing Loan 2B-2C',11,1,6);
insert into pr_loan_master values(7,'HLADD','Housing Addl.Loan - 2D',11,1,7);
insert into pr_loan_master values(8,'HLCOM','Housing Loan Commerical',14.5,1,8);
insert into pr_loan_master values(9,'HLPLT','Housing Loan-Plot',0,1,9);
insert into pr_loan_master values(10,'HOUS1','Housing Loan Main',5,1,10);
insert into pr_loan_master values(11,'HOUS2','Housing Loan 2',0,1,11);
insert into pr_loan_master values(12,'HOUS3','Housing Loan 3',0,1,12);
insert into pr_loan_master values(13,'HOUSI','Housing Loan Int',0,1,13);
insert into pr_loan_master values(14,'MARR','Marriage Loan',0,1,14);
insert into pr_loan_master values(15,'PERS','Personal Loan',0,1,15);
insert into pr_loan_master values(16,'PFHT1','PF Loan ST 1',11.5,1,16);
insert into pr_loan_master values(17,'PFHT2','PF Loan ST2',11.5,1,17);
insert into pr_loan_master values(18,'PFLT1','PF Advance LT1',11.5,1,18);
insert into pr_loan_master values(19,'PFLT2',' PF Advance LT2',11.5,1,19);
insert into pr_loan_master values(20,'PFLT3',' PF Advance LT3',11.5,1,20);
insert into pr_loan_master values(21,'PFLT4','PF Advance LT4',11.5,1,21);
insert into pr_loan_master values(22,'SBFLN','Staff Benefit Fund Loan',0,1,22);
insert into pr_loan_master values(23,'SOCIE','Society Loan',0,1,23);
insert into pr_loan_master values(24,'VEH2W','Vehicle Loan (2W)',0,1,24);
insert into pr_loan_master values(25,'VEH4W','Vehicle Loan (4W)',0,1,25);
insert into pr_loan_master values(26,'PFL1','PF Loan 1',11.5,1,26);
insert into pr_loan_master values(27,'PFL2','PF Loan 2',11.5,1,27);



insert into pr_purpose_of_advance_master values (1,'Additional to Existing House',12,1,1,'NONREPAY','');
insert into pr_purpose_of_advance_master values (2,'Higher Education',1,1,2,'NONREPAY','');
insert into pr_purpose_of_advance_master values (3,'LIC Policy',1,1,3,'NONREPAY','');
insert into pr_purpose_of_advance_master values (4,'Marriage',1,1,4,'NONREPAY','');
insert into pr_purpose_of_advance_master values (5,'Medical Expenditure',6,1,5,'NONREPAY','');
insert into pr_purpose_of_advance_master values (6,'Purchage of House',36,1,6,'NONREPAY','');
insert into pr_purpose_of_advance_master values (7,'Purchage of Site',24,1,7,'NONREPAY','');
insert into pr_purpose_of_advance_master values (8,'NR 90% Withdrawl',1, 0,8,'NONREPAY','');
insert into pr_purpose_of_advance_master values (9,'PF Final Settlement',1, 0,9,'NONREPAY','');
insert into pr_purpose_of_advance_master values (10,'Repayment of CCF Loans',1, 1,10,'NONREPAY','');
Insert into pr_purpose_of_advance_master values (11,'Religious',0,1,11,'REPAY','');
Insert into pr_purpose_of_advance_master values (12,'Medical',0,1,12,'REPAY','');
Insert into pr_purpose_of_advance_master values (13,'Personal',0,1,13,'REPAY','');
Insert into pr_purpose_of_advance_master values (14,'Domestic',0,1,14,'REPAY','');
Insert into pr_purpose_of_advance_master values (15,'Education',0,1,15,'REPAY','');
Insert into pr_purpose_of_advance_master values (16,'Housing',0,1,16,'REPAY','');
insert into pr_purpose_of_advance_master values (17,'Others',0,1,17,'REPAY','');
insert into pr_purpose_of_advance_master (id,purpose_name,month,active	,trans_id,ptype,purpose_code) 
values (18,'Covid',	3,	1,18,'REPAY','Covid');
insert into pr_purpose_of_advance_master (id,purpose_name,month,active	,trans_id,ptype,purpose_code) 
values (19,'Covid',	3,	1,19,'NONREPAY','Covid');



insert into pr_list_of_documents_master values(1,1,'Title Deed_Non_Encumberance Certificate',1,1);
insert into pr_list_of_documents_master values(2,1,'An estimate of Cost of Construction',1,2);
insert into pr_list_of_documents_master values(3,1,'Sanction/Approved plan',1,3);
insert into pr_list_of_documents_master values(4,2,'Admission Letter',1,4);
insert into pr_list_of_documents_master values(5,2,'Fees Requirement details',1,5);
insert into pr_list_of_documents_master values(6,3,'Proposed Amount',1,6);
insert into pr_list_of_documents_master values(7,3,'Premium Amount',1,7);
insert into pr_list_of_documents_master values(8,3,'Name of Policy',1,8);
insert into pr_list_of_documents_master values(9,4,'Wedding Card',1,9);
insert into pr_list_of_documents_master values(10,5,'Doctors Certificate from the Hospital',1,10);
insert into pr_list_of_documents_master values(11,6,'Title Deed & Non_Encumberance Certificate',1,11);
insert into pr_list_of_documents_master values(12,6,'An estimate of Cost Construction',1,12);
insert into pr_list_of_documents_master values(13,6,'Sanction/Approved plan',1,13);
insert into pr_list_of_documents_master values(14,7,'Title Deed proposed seller',1,14);
insert into pr_list_of_documents_master values(15,7,'Agreement with vendor',1,15);
insert into pr_list_of_documents_master values(16,7,'Non_encumberance certificate',1,16);
insert into pr_list_of_documents_master values(17,8,'Withdrawl',1,17);
insert into pr_list_of_documents_master values(18,10,'Certificate from CCS',1,18);
insert into pr_list_of_documents_master (id,loan_id,document_name,active,trans_id) 
values (19,19,'Title Deed proposed seller',1,19);


insert into pr_emp_designation values(1,1,'Assistant General Manager',1,1200);
insert into pr_emp_designation values(2,2,'Chief General Manager',1,1200);
insert into pr_emp_designation values(3,3,'Deputy General Manager',1,1200);
insert into pr_emp_designation values(4,4,'General Manager',1,1200);
insert into pr_emp_designation values(5,5,'Manager Scale I',1,1200);
insert into pr_emp_designation values(6,5,'Senior Manager',1,1200);
insert into pr_emp_designation values(7,6,'Managing Director',1,1200);
insert into pr_emp_designation values(8,7,'Staff Assistant',1,1200);
insert into pr_emp_designation values(9,8,'Attender',1,1200);
insert into pr_emp_designation values(10,8,'Driver',1,1200);

insert into pr_emp_categories values(1,'Assistant General Manager',1,121);
insert into pr_emp_categories values(2,'Chief General Manager',1,122);
insert into pr_emp_categories values(3,'Deputy General Manager',1,123);
insert into pr_emp_categories values(4,'General Manager',1,124);
insert into pr_emp_categories values(5,'Manager',1,125);
insert into pr_emp_categories values(6,'Managing Director',1,126);
insert into pr_emp_categories values(7,'Staff Assistant',1,127);
insert into pr_emp_categories values(8,'Subordinate staff',1,128);

Insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','GenPayslip','CCAPercentage','0.04',1,111);
Insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','GenPayslip','CCAConditionalAmt','470',1,111);
Insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','GenPayslip','HRAStaffAsstAttenderPercentage','0.15',1,111);
Insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','GenPayslip','HRAOthersPercentage','0.17',1,111);
Insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','GenPayslip','ProfTaxMinMaxAmts','15000,20000',1,111);
Insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','GenPayslip','ProfTaxMinMaxAmtVals','150,200',1,111);
Insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','GenPayslip','PFPercentage','0.125',1,111);
Insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','GenPayslip','IncomeTaxSlabMinConditionalAmt','250000,499999',1,111);
Insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','GenPayslip','IncomeTaxSlabMid','0.20',1,111);
Insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','GenPayslip','IncomeTaxSlabMax','0.30',1,111);
Insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','GenPayslip','IncomeTaxSlabMidConditionalAmt','500000,1000000',1,111);
Insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','GenPayslip','IncomeTaxSlabMaxConditionalAmt','1000001,5000000',1,111);
Insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','GenPayslip','IncomeTaxSlabMin','0.05',1,111);
Insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','GenPayslip','IncomeTaxCessPercentage','0.04',1,111);
Insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','GenPayslip','IncomeTaxSurcharge','0.10',1,111);
Insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','IncomeTax','Section80CD','50000',1,111);
Insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','IncomeTax','Section80CCC','150000',1,111);
Insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','IncomeTax','Section80C','150000',1,111);
Insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','IncomeTax','Section80EE_HL1','200000',1,111);
Insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','IncomeTax','Section80EE_HL2','250000',1,111);
Insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','IncomeTax','Section80D_F','25000',1,111);
Insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','IncomeTax','Section80D_P','30000',1,111);
Insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','IncomeTax','Section80DD','125000',1,111);
Insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','IncomeTax','Section80DDB','60000',1,111);
Insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','IncomeTax','Section80U','125000',1,111);
Insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','IncomeTax','Section80G','0.50',1,111);
Insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','IncomeTax','Section80GGB','1',1,111);
insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','IncomeTax','Section80CCD',50000,1,111);
insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','IncomeTax','Section80E','NULL',1,111);
insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','IncomeTax','Section80EE',300000,1,111);
Insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','IncomeTax','Section80CCF','1',1,111);
Insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','IncomeTax','24(b)','1',1,111);
Insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','IncomeTax','Section24(b)','300000',1,111);
Insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','GenPayslip','MinCCAConditionalAmt','400',1,111);
Insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','GenPayslip','MaxCCAConditionalAmt','870',1,111);
Insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','GenPayslip','Special_DA','0.0775',1,111); 
Insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','GenPayslip','Special_Allw','0.0775',1,111);
Insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','GenPayslip','Special_Allw_Mid','0.10',1,111);
Insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','GenPayslip','Special_Allw_Max','0.11',1,111);
Insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','GenPayslip','MidCCAConditionalAmt','470',1,111);
Insert into all_constants (app_type,functionality,constant,value,active,trans_id) values('payroll','GenPayslip','Special_Allw_Min','0.0775',1,111);



--Insertion into pr_emp_incometax_12ba_master table 

insert into pr_emp_incometax_12ba_master values(1,'Accommodation',1,101,'12ba');
insert into pr_emp_incometax_12ba_master values(2,'Cars',1,102,'12ba');
insert into pr_emp_incometax_12ba_master values(3,'Sweeper,Gardener,Watchman or Personal Attendant',1,103,'12ba');
insert into pr_emp_incometax_12ba_master values(4,'Gas,Electricy,Water',1,104,'12ba');
insert into pr_emp_incometax_12ba_master values(5,'Interest Free or Concessional Loans',1,105,'12ba');
insert into pr_emp_incometax_12ba_master values(6,'Holiday Expenses',1,106,'12ba');
insert into pr_emp_incometax_12ba_master values(7,'Free Meals',1,107,'12ba');
insert into pr_emp_incometax_12ba_master values(8,'Free or Concessional travel',1,108,'12ba');
insert into pr_emp_incometax_12ba_master values(9,'Education',1,109,'12ba');
insert into pr_emp_incometax_12ba_master values(10,'Gifts,Vouchers',1,110,'12ba');
insert into pr_emp_incometax_12ba_master values(11,'Credit Card Expenses',1,111,'12ba');
insert into pr_emp_incometax_12ba_master values(12,'Club Expenses',1,112,'12ba');
insert into pr_emp_incometax_12ba_master values(13,'Use of Movable assets by Employees',1,113,'12ba');
insert into pr_emp_incometax_12ba_master values(14,'Transfer of Assets to Employees',1,114,'12ba');
insert into pr_emp_incometax_12ba_master values(15,'Value of any other benifts/amenity/services/privilege',1,115,'12ba');
insert into pr_emp_incometax_12ba_master values(16,'Stock Options',1,116,'12ba');
insert into pr_emp_incometax_12ba_master values(17,'Other Benifits or Amenities',1,117,'12ba');
insert into pr_emp_incometax_12ba_master values(18,'Total Value of Perquisites',1,118,'12ba');
insert into pr_emp_incometax_12ba_master values(19,'Total Value of Profits in lieu of Salary as per 17(3)',1,119,'12ba');
insert into pr_emp_incometax_12ba_master values(20,'TAN no',1,120,'12b');
insert into pr_emp_incometax_12ba_master values(21,'PAN no',1,121,'12b');
insert into pr_emp_incometax_12ba_master values(22,'Salary (including HRA, perquisite and others)',1,122,'12b');
insert into pr_emp_incometax_12ba_master values(23,'HRA Other Allowances',1,123,'12b');
insert into pr_emp_incometax_12ba_master values(24,'Perquisite Amount',1,124,'12b');
insert into pr_emp_incometax_12ba_master values(25,'Gross Salary',1,125,'12b');
insert into pr_emp_incometax_12ba_master values(26,'LIC or PF contribution',1,126,'12b');
insert into pr_emp_incometax_12ba_master values(27,'Amount Under Sec 80C TDS paid',1,127,'12b');
insert into pr_emp_incometax_12ba_master values(28,'Remarks',1,128,'12b');



--pr_branch_allowance_master


insert into [pr_branch_allowance_master] values (1,'SP_SHFTDUTY','SHIFT DUTY ALLOWANCE',750,getdate(),1,121,NULL);
insert into [pr_branch_allowance_master] values (2,'SP_KEY','SPL KEY',500,getdate(),1,122,NULL);
insert into [pr_branch_allowance_master] values (3,'SP_CASHIER','SPL Cashier',1000,getdate(),1,123,NULL);
insert into [pr_branch_allowance_master] values (4,'BR_MGR','Br Manager Allowance',1500,getdate(),1,124,NULL);

Insert into pr_incometax_bank_payment(id,fy,fm,trans_id) values(1,2019,'2019-04-01',121);

Insert into pr_emp_adv_loan_type values (1,2020,'2019-04-20',315,175,'CD_Loans 2',1,121);
Insert into pr_emp_adv_loan_type values (2,2020,'2019-04-20',197,176,'Education Loan',1,122);
Insert into pr_emp_adv_loan_type values (3,2020,'2019-04-20',381,346,'Housing Loans-2',1,123);



Insert into all_constants(app_type,functionality,constant,value,active,trans_id) values('payroll','Per_Earings','PFPerks','0.05',1,111);





 insert into new_num values('pr_payslip_customization','id',0,0);
 Insert into new_num values('pr_emp_tds_process_allowances','id',0,0);
 --new num for encashment customization--
insert into new_num values('pr_encashment_earnings_customization','id',0,0);
insert into new_num values('pr_encashment_deductions_customization','id',0,0);
--new num--
insert into new_num values('pr_emp_incometax_12b','id',0,0);
insert into new_num values('pr_pfopeningbal','id',10082,0);
insert into new_num values('loan_sl_no','loan_sl_no',10000,0);

Insert into designations values('SysAdmin','System Administrator','System Administrator',123456,getdate());
Insert into designations values('JMDR','JAMEDAR','JAMEDAR',123456,getdate());
Insert into designations values('DFTR','DAFTARI','DAFTARI',123456,getdate());




--loans payslip data migration

--insert into dm_payslip_loans values('House Loan Principal','HOUS1Princ','Loan');
insert into dm_payslip_loans values('Personal Loan','PERNLON1','Loan');
insert into dm_payslip_loans values('House Insurance Premium','HOUINSUPRE','Loan');

insert into dm_payslip_loans values('Housing Loan 2A','HL2A','Loan');
insert into dm_payslip_loans values('Housing Loan 2B-2C','HL2BC','Loan');
insert into dm_payslip_loans values('PF Loan ST 1','PFHT1','Loan');


insert into dm_payslip_loans values('PF Loan ST2','PFHT2','Loan');
insert into dm_payslip_loans values(' PF Advance LT2','PFLT2','Loan');
insert into dm_payslip_loans values(' PF Advance LT3','PFLT3','Loan');

insert into dm_payslip_loans values('PF Advance LT4','PFLT4','Loan');





--allconstants for login
insert into all_constants(app_type,functionality,constant,value,active,trans_id) values('payroll','LoginAccess','793,929,6305,6336,6326,123456',1,1,111);




delete from pr_encashment_deductions_customization;

delete from pr_encashment_earnings_customization;

delete from pr_payslip_customization;

--customization data


INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (1, 16, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (2, 17, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (3, 18, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (4, 19, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (5, 20, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (6, 21, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (7, 22, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (8, 23, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (9, 24, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (10, 25, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (11, 26, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (12, 27, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (13, 28, N'EPD', N'Yes', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (14, 29, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (15, 30, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (16, 31, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (17, 32, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (18, 33, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (19, 34, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (20, 35, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (21, 36, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (22, 37, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (23, 38, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (24, 39, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (25, 40, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (26, 41, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (27, 42, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (28, 43, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (29, 44, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (30, 45, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (31, 46, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (32, 47, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (33, 48, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (34, 49, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (35, 50, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (36, 51, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (37, 52, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (38, 53, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (39, 54, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (40, 55, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (41, 56, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (42, 57, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (43, 58, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (44, 59, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (45, 60, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (46, 61, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (47, 62, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (48, 63, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (49, 64, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (50, 65, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_deductions_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (51, 66, N'EPD', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (1, 11, N'pay_fields', N'Yes', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (2, 12, N'pay_fields', N'Yes', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (3, 13, N'pay_fields', N'Yes', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (4, 14, N'pay_fields', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (5, 15, N'pay_fields', N'Yes', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (6, 16, N'pay_fields', N'Yes', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (7, 18, N'pay_fields', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (8, 19, N'pay_fields', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (9, 20, N'pay_fields', N'Yes', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (10, 21, N'pay_fields', N'Yes', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (11, 22, N'pay_fields', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (12, 23, N'pay_fields', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (13, 24, N'pay_fields', N'Yes', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (14, 25, N'pay_fields', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (15, 26, N'pay_fields', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (16, 1, N'EMPA', N'Yes', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (17, 2, N'EMPA', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (18, 3, N'EMPA', N'Yes', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (19, 4, N'EMPA', N'Yes', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (20, 5, N'EMPA', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (21, 6, N'EMPA', N'Yes', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (22, 7, N'EMPA', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (23, 8, N'EMPA', N'Yes', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (24, 9, N'EMPA', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (25, 10, N'EMPA', N'Yes', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (26, 11, N'EMPA', N'Yes', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (27, 12, N'EMPA', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (28, 13, N'EMPA', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (29, 14, N'EMPA', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (30, 15, N'EMPA', N'', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (31, 16, N'EMPA', N'', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (32, 17, N'EMPA', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (33, 18, N'EMPA', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (34, 19, N'EMPA', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (35, 20, N'EMPA', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (36, 21, N'EMPA', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (37, 22, N'EMPA', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (38, 23, N'EMPSA', N'Yes', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (39, 24, N'EMPSA', N'Yes', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (40, 25, N'EMPSA', N'Yes', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (41, 26, N'EMPSA', N'Yes', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (42, 27, N'EMPSA', N'Yes', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (43, 28, N'EMPSA', N'Yes', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (44, 29, N'EMPSA', N'Yes', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (45, 30, N'EMPSA', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (46, 31, N'EMPSA', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (47, 32, N'EMPSA', N'Yes', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (48, 33, N'EMPSA', N'Yes', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (49, 34, N'EMPSA', N'Yes', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (50, 35, N'EMPSA', N'Yes', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (51, 36, N'EMPSA', N'Yes', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (52, 37, N'EMPSA', N'Yes', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (53, 38, N'EMPSA', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (54, 39, N'EMPSA', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (55, 40, N'EMPSA', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (56, 41, N'EMPSA', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (57, 42, N'EMPSA', N'Yes', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (58, 43, N'EMPSA', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (59, 44, N'EMPSA', N'Yes', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (60, 45, N'EMPSA', N'Yes', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (61, 46, N'EMPSA', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (62, 47, N'EMPSA', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (63, 48, N'EMPSA', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (64, 49, N'EMPSA', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (65, 50, N'EMPSA', N'Yes', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (66, 51, N'EMPSA', N'No', 1, 2004)
INSERT [dbo].[pr_encashment_earnings_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (67, 17, N'pay_fields', N'Yes', 1, 2004)

INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (1, 11, N'pay_fields', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (2, 12, N'pay_fields', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (3, 13, N'pay_fields', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (4, 14, N'pay_fields', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (5, 15, N'pay_fields', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (6, 16, N'pay_fields', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (7, 18, N'pay_fields', N'No', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (8, 19, N'pay_fields', N'No', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (9, 20, N'pay_fields', N'No', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (10, 21, N'pay_fields', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (11, 22, N'pay_fields', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (12, 23, N'pay_fields', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (13, 24, N'pay_fields', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (14, 25, N'pay_fields', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (15, 26, N'pay_fields', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (16, 1, N'EMPA', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (17, 2, N'EMPA', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (18, 3, N'EMPA', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (19, 4, N'EMPA', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (20, 5, N'EMPA', N'No', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (21, 6, N'EMPA', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (22, 7, N'EMPA', N'No', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (23, 8, N'EMPA', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (24, 9, N'EMPA', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (25, 10, N'EMPA', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (26, 11, N'EMPA', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (27, 12, N'EMPA', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (28, 13, N'EMPA', N'No', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (29, 14, N'EMPA', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (30, 17, N'EMPA', N'No', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (31, 18, N'EMPA', N'No', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (32, 19, N'EMPA', N'No', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (33, 20, N'EMPA', N'No', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (34, 21, N'EMPA', N'No', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (35, 22, N'EMPA', N'No', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (36, 23, N'EMPSA', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (37, 24, N'EMPSA', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (38, 25, N'EMPSA', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (39, 26, N'EMPSA', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (40, 27, N'EMPSA', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (41, 28, N'EMPSA', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (42, 29, N'EMPSA', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (43, 30, N'EMPSA', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (44, 31, N'EMPSA', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (45, 32, N'EMPSA', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (46, 33, N'EMPSA', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (47, 34, N'EMPSA', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (48, 35, N'EMPSA', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (49, 36, N'EMPSA', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (50, 37, N'EMPSA', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (51, 38, N'EMPSA', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (52, 39, N'EMPSA', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (53, 40, N'EMPSA', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (54, 41, N'EMPSA', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (55, 42, N'EMPSA', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (56, 43, N'EMPSA', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (57, 44, N'EMPSA', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (58, 45, N'EMPSA', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (59, 46, N'EMPSA', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (60, 47, N'EMPSA', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (61, 48, N'EMPSA', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (62, 49, N'EMPSA', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (63, 50, N'EMPSA', N'Yes', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (64, 51, N'EMPSA', N'No', 1, 2003)
INSERT [dbo].[pr_payslip_customization] ([id], [m_id], [field_type], [cust_status], [active], [trans_id]) VALUES (65, 17, N'pay_fields', N'Yes', 1, 2003);



--adhoc payslip data migration
	insert into dm_adhoc_earn values('Washing Allowance','WASHALLW','EMPSA');
	insert into dm_adhoc_earn values('CEO Allowance','CEOALLW','EMPSA');
	insert into dm_adhoc_earn values('Shift Duty Allowance','SP_SHFTDUTY','EMPSA');
	
-----no sufficient salary
insert into new_num values('pr_emp_payslip_netSalary','id',0,0);


-- Added new record in userroles
insert into userroles values(12,'Payslip','','Yes','Yes','Yes','Yes','Yes','Yes',123456,getdate());

insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIC',
 'adhoc',1,(select max(trans_id)+1 from pr_deduction_field_master),null,'LIC','LIC');

 
  insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'PERSONAL LOAN',
 'adhoc',1,(select max(trans_id)+1 from pr_deduction_field_master),null,'PERNLON1','PERNLON1');


insert into departments values('Anakapalli CSF','Anakapalli CSF','Anakapalli CSF',123456,getdate(),0);
insert into departments values('Audit','Audit','Audit',123456,getdate(),0);
insert into departments values('BANKING-Accounts','BANKING-Accounts','BANKING-Accounts',123456,getdate(),0);
insert into departments values('BANKING-BRCC','BANKING-BRCC','BANKING-BRCC',123456,getdate(),0);
insert into departments values('BANKING-CCAC','BANKING-CCAC','BANKING-CCAC',123456,getdate(),0);
insert into departments values('BANKING-CCAC,CLG','BANKING-CCAC,CLG','BANKING-CCAC,CLG',123456,getdate(),0);
insert into departments values('BANKING-Clg','BANKING-Clg','BANKING-Clg',123456,getdate(),0);
insert into departments values('BANKING-CLPC/HFC','BANKING-CLPC/HFC','BANKING-CLPC/HFC',123456,getdate(),0);
insert into departments values('BANKING-Investments','BANKING-Investments','BANKING-Investments',123456,getdate(),0);
insert into departments values('BANKING-Operations','BANKING-Operations','BANKING-Operations',123456,getdate(),0);
insert into departments values('BANKING-Recon','BANKING-Recon','BANKING-Recon',123456,getdate(),0);
insert into departments values('BANKING-RMD','BANKING-RMD','BANKING-RMD',123456,getdate(),0);
insert into departments values('BANKING-RTGS','BANKING-RTGS','BANKING-RTGS',123456,getdate(),0);
insert into departments values('BANKING-Stationery','BANKING-Stationery','BANKING-Stationery',123456,getdate(),0);
insert into departments values('BKG-KYC/AML','BKG-KYC/AML','BKG-KYC/AML',123456,getdate(),0);
insert into departments values('BKG-TREASURY OPERATIONS','BKG-TREASURY OPERATIONS','BKG-TREASURY OPERATIONS',123456,getdate(),0);
insert into departments values('Board Sectt.','Board Sectt.','Board Sectt.',123456,getdate(),0);
insert into departments values('Board Sectt./HRD','Board Sectt./HRD','Board Sectt./HRD',123456,getdate(),0);
insert into departments values('CGM Peshi','CGM Peshi','CGM Peshi',123456,getdate(),0);
insert into departments values('Chittoor CSF','Chittoor CSF','Chittoor CSF',123456,getdate(),0);
insert into departments values('chodavaram sugar factory','chodavaram sugar factory','chodavaram sugar factory',123456,getdate(),0);
insert into departments values('CMI & AD (DOS)','CMI & AD (DOS)','CMI & AD (DOS)',123456,getdate(),0);
insert into departments values('Co-op Minister Peshi','Co-op Minister Peshi','Co-op Minister Peshi',123456,getdate(),0);
insert into departments values('CSP & Board Secretariat','CSP & Board Secretariat','CSP & Board Secretariat',123456,getdate(),0);
insert into departments values('CTI - Rajendranagar','CTI - Rajendranagar','CTI - Rajendranagar',123456,getdate(),0);
insert into departments values('Dept. of Supervision (DOS)','Dept. of Supervision (DOS)','Dept. of Supervision (DOS)',123456,getdate(),0);
insert into departments values('DoS/Audit','DoS/Audit','DoS/Audit',123456,getdate(),0);
insert into departments values('Engineering Cell','Engineering Cell','Engineering Cell',123456,getdate(),0);
insert into departments values('Etikoppaka CSF-VSP','Etikoppaka CSF-VSP','Etikoppaka CSF-VSP',123456,getdate(),0);
insert into departments values('GM Peshi','GM Peshi','GM Peshi',123456,getdate(),0);
insert into departments values('HRD-Payments','HRD-Payments','HRD-Payments',123456,getdate(),0);
insert into departments values('HRD-Terminal Benfits','HRD-Terminal Benfits','HRD-Terminal Benfits',123456,getdate(),0);
insert into departments values('HRD/DoS','HRD/DoS','HRD/DoS',123456,getdate(),0);
insert into departments values('IF,LEGAL,PR','IF,LEGAL,PR','IF,LEGAL,PR',123456,getdate(),0);
insert into departments values('IFC-RABO Project','IFC-RABO Project','IFC-RABO Project',123456,getdate(),0);
insert into departments values('INDUSTRIAL FINANCE','INDUSTRIAL FINANCE','INDUSTRIAL FINANCE',123456,getdate(),0);
insert into departments values('IT CARDS PROJECT','IT CARDS PROJECT','IT CARDS PROJECT',123456,getdate(),0);
insert into departments values('IT DEPT- Computers','IT DEPT- Computers','IT DEPT- Computers',123456,getdate(),0);
insert into departments values('IT DEPT-PACS Computerisation','IT DEPT-PACS Computerisation','IT DEPT-PACS Computerisation',123456,getdate(),0);
insert into departments values('IT(APCOB-CBS)','IT(APCOB-CBS)','IT(APCOB-CBS)',123456,getdate(),0);
insert into departments values('IT(DCCB-CBS)','IT(DCCB-CBS)','IT(DCCB-CBS)',123456,getdate(),0);
insert into departments values('Jampani CSF,GNT','Jampani CSF,GNT','Jampani CSF,GNT',123456,getdate(),0);
insert into departments values('Kovvur CSF','Kovvur CSF','Kovvur CSF',123456,getdate(),0);
insert into departments values('L & A - LT','L & A - LT','L & A - LT',123456,getdate(),0);
insert into departments values('L & A - ST','L & A - ST','L & A - ST',123456,getdate(),0);
insert into departments values('LIBRARY','LIBRARY','LIBRARY',123456,getdate(),0);
insert into departments values('MD Peshi','MD Peshi','MD Peshi',123456,getdate(),0);
insert into departments values('O/o SCDR/OSD','O/o SCDR/OSD','O/o SCDR/OSD',123456,getdate(),0);
insert into departments values('P&D','P&D','P&D',123456,getdate(),0);
insert into departments values('P&D-Proj.Mont.Unit','P&D-Proj.Mont.Unit','P&D-Proj.Mont.Unit',123456,getdate(),0);
insert into departments values('Personal Banking Department','Personal Banking Department','Personal Banking Department',123456,getdate(),0);
insert into departments values('PLG & DEV/RMD & IDD','PLG & DEV/RMD & IDD','PLG & DEV/RMD & IDD',123456,getdate(),0);
insert into departments values('PR','PR','PR',123456,getdate(),0);
insert into departments values('PRINTING&STATIONERY','PRINTING&STATIONERY','PRINTING&STATIONERY',123456,getdate(),0);
insert into departments values('PUTLIBOWLI  PREMISES','PUTLIBOWLI  PREMISES','PUTLIBOWLI  PREMISES',123456,getdate(),0);
insert into departments values('Sri Venkateswara CSF,TPT','Sri Venkateswara CSF,TPT','Sri Venkateswara CSF,TPT',123456,getdate(),0);
insert into departments values('Tandava CSF','Tandava CSF','Tandava CSF',123456,getdate(),0);
insert into departments values('VijRamGajapati CSF,VZM','VijRamGajapati CSF,VZM','VijRamGajapati CSF,VZM',123456,getdate(),0);


insert into new_num values('pr_ob_share_encashment','id',0,0);
insert into new_num values('pr_ob_share_adhoc','id',0,0);



insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'APCOBHFCLT',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'EDUC',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'ESTT',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'SP_SD_AWARD',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'CCSAP',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'FA',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'CEOALLW',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'HYDCOURT',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'MISCDED',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'PFHT1',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'SP_CONVEY',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'SP_SD_MGR',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'CAIIB',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'JAIIB',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'SP_CARETAKE',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'LICMACHILI',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'PF5',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'PFLT1',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'TOFA',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'SPLINCR',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'INTERIM',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'SP_WATCHMAN',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'SP_CASHCAD',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'FAELURU',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'MEDICALADVANCE',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'HFC',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'HLADD',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'DRCCSVIZAG',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'KMLCOOPBANK',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'SUBCLUB',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'WASHALLW',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'Famedhak',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'MARR',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'SUBUNION',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'VISAKHA',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'FGPRM',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'SBFLN',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'SP_DRIVER',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'COURTVIZAG',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'PHONE',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'EASSNSUB',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'HLPLT',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'CD2',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'HOUS1',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'SP_TYPIST',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'VEH4W',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'PERS',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'KADAPADCCB',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'SP_ACSTI',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'APCOBEDLHO',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'RESATTN',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'LIFEINS',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'SP_XEROX',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'VIJAYA',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'PFLT3',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'LICPENSION',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'HOUS3',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'FXDALLW',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'SP_RECEPTION',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'LTCADVREC',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'TSCABOA',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'SP_BILLCOLL',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'MISCEARN',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'PF',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'STAGALLOW',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'TGUNION',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'FEST',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'HOUSI',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'ONDAYSAL',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'SP_JAMEDAR',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'ESI',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'HOUPROPINS',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'BR_MGR',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'COURT',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'MEDICAL',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'COURTDED',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'SBF',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'UNIONLEVY',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'VEH2W',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'CCSHYD',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'ABHLFHYD',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'LIC',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'SP_DAFTARI',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'SP_CASHIER',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'SUBST',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'HLCOM',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'PERQPAY',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'VPF',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'SP_STENO',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'COURTSLRPET',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'DEPU',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'APSCBLTASSN',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'TEACH',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'JEEVAN',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'VEHINS',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'CODINSPRM',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'COURTHYD',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'HOUINSUPRE',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'SP_RECASST',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'PH',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'COURTAB',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'FCLTYALLW',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'FPIR',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'HDCF',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'SP_ARREAR',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'PERPAY',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'Tds',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'PFHT2',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'OFFASSN',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'SP_SHFTDUTY',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'PFLT2',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'SP_KEY',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'HOUS2',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'LTASSNLEVY',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'APCCADBEMP',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'MISCELLAN',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'GSLI',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'SP_LIBRARY',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'COURTTANUKU',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'SP_TELEPHONE',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'SP_ELEC',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'CODINSPRM1',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'PERNLON1',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'SYSADMN',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'HL2BC',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'SP_LIFT',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'SP_PERPAY',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'SUBLT',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'TGASSN',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'SP_NONPROM',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'APCOBEDLHNR',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'Onedaysal',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'ARR_GR_AMT',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'OFFASSNC',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'SP_INCENTIVE',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'HL2A',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'NPSGA',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'SOCIE',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'APCOBHFCHO',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'COURTKADAPA',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'HL2',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'PFLT4',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'COURTSECBAD',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'VEHMACHILI',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'SP_DESPATCH',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'OFFI',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'SP_RECSUB',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'DCCB',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'FAONGOLE',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'SPLPAY',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'INCREMENT',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'ENCASHMENT',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);



insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIC HOUSING LOAN INT',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'HFC-INT',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIP -Max Newyork',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIC HFL HOUSING LOAN',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'INT ON HOUSING LOAN LIC',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'BHAGYANAGAR GAU SEVA SADAN',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'NIA MEDICAL INS',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'EDU LOAN PRIN-SBI',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIP-Max Newyork',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'MEDICAL INSURANCE-NICL',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'SBI HOUSING LOAN PRINCIPAL',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'MUTUAL FUND (ELSS) ',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'HLINT-LIC HOUSING FIN LTD',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIC HOUSING FINANCE PRINCIPLE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'GIC HOUSING LOAN-INT',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan - HUDCO',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'CORPORATION BANK EDUCATION LOAN',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIC HOUSING LOAN PRINC',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'BIRLA SUNLIFE TAX PLAN',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Can Fin Homes Ltd Housing loan Principal',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'SBI EDU LOAN INT',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'ICICI LOMBARD MEDICAL INSURANCE PREMIUM',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'INT ON HOUSING LOAN(UCO BANK)',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'HOUSING LOAN-HUDCO NIWAS',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'ANNUAL HEALTH CHECKUP',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'UCO-HOUSING LOAN',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIC HFL HOUSING LOAN PRINCIPLE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIP-Max New York Life',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'APCOB HOUSING LOAN INTEREST',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIP-SBI LIFE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'EDUCATION LOAN INTEREST',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'VISUALLY HANDICAPPED-75%',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Int on Education Loan',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'IIFL HOUSLING LOAN INTEREST',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'REGISTRAION CHARGES & STAMP DUTY',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIP-Sriram Life',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIP SBI LIFE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'ICICI HOUSING LOAN PRINCIPAL',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'HEARING DISABILITY',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'HDFC HOUSING LOAN INT',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'STAR HEALTH MEDICAL INSURANCE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'INS PREMIUM-HDFC',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'HDFC CREDILA EDUCATION LOAN INTEREST  ',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'NIA MED INSU',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'CORP-EDU LOAN',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIP-ICICI Prudential',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'VISUAL DIABILITY',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'AXIS MF',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'DONATION(2)',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan-Indian Bank',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan- SBI',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan - APCOB',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'JAMAI NIZAMIA',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'SOS CHILDRENS VILLAGES INDIA DONATION',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'HDFC HOUSING LOAN INTEREST\',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'MAX LIP',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'HOUSIN LOAN LIC HFL',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'HOUSIN LOAN INT LIC HFL',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'ICICI HOUSING LOAN INTEREST',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'PNB EDUCATION LOAN INTEREST',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'uco bank housing loan principle',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'IDBI HOUSING LOAN PRINCIPLE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'MAX CURE ANNUAL HEALTH CHECKUP',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIC HFL HOUSING LOAN INTEREST',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIP-Prudetial Life',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'EDU LOAN INT TSCAB',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housin Loan-LIC HFL',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIP - Max New York',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),' UCO HOUSING LOAN INTEREST',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'PHYSICAL DISABILITY',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan-DHFC',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Interest on educational loan-Indian Bank',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'NIACL MEDICAL INSURANCE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'INTEREST ON HOUSING LOAN-ICICI',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'INTEREST ON EDUCATION LOAN-TSCAB',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'IFFCO TOKIO MEDICAL INSURANCE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'VIZAG COOP BANK HOUSING LOAN',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'SBI HOUSING LOAN INTEREST',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'DONATION TO JAMIA NIZAMLA',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'MEDICAL POLICY',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIP - Birla Sun Life',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'SYNDICATE BANK HOUSING LOAN INTEREST',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIP - ICICI PRU',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'INT ON EDU LOAN-PNB',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'INT ON EDU LOAN-ANDHRA BANK',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'DSP BLACKROCK MF',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'ICICI PRU MF',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'GIC HOUSING LOAN PRIN',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan - SBI',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'ICICI PRUDENTIAL MF',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'STAMP DUTY & REGISTRAIION CHARGES',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan - LIC-HFL',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'ADITYA BIRLA SIP',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIP MEDICAL',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'HOUSING LOAN INT HDFC',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'BOM HOUSING LOAN INTEREST',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'NICL MED INSURANCE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'HOUSING LOAN LIC HFL PRINCIPAL',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'HL INTEREST OF ICICI',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'HEARING DISABILITY(100%)',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'AXIS MUTUAL FUND',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'DONATION TO SAVE THE CHILDREN',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan-NTHFL',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan-Sundaram BNP',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'SBI PPF',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'SUKANYA SAMRIDHI A/C',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIP PLI',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan-LIC HFL',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'NEW INDIA HEALTH ISURANCE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'HDFC HOUSING LOAN PRINCIPLE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'NICL HOSPITALIZATION INS',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan-Sundaram BNP Paribas',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan - LIC HFL',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'ANADHA VIDYARTHI GRIHA DONATION',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'HDFC ELSS SCHEME',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'DSP MUTUAL FUND',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'HEALTH INS-MAX BUPA',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'MEDICAL INSURANCE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'HelpAge India Donation',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'HOUSING LOAN LIC',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'ICICI BANK HL INT',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIP KOTAK',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'IDBI HOUSING LOAN INTEREST',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'UCO-HOUSING LOAN-INT',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Physical Disability(75%)',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'HOUSING LOAN INT ICICI',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'EDU LOAN PRIN- BOB',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'HEALTH INSURANCE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Pension -  Max New York Life',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'NEW INDIA INSURANCE MEDICLAIM',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIP -Birla Sun',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan-Andhra Bank',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'IIFL PRINCIPLE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIC HFL HOUSING LOAN-INT',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan - City Bank',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIC HFC HOUSING LOAN PRINCIPLE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'TUTUION FEE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'VISUAL DIABILITY(75%)',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Can Fin Homes Ltd Housing Loan Int',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan-APCOB',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'BOM HOUSING LOAN PRINCIPLE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'SBI LIFE INSU',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'NIA MEDICAL INSURANCE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'APCOB HOUSING LOAN PRINCIPLE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'STAR HELATH INSURANCE ',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'HOUSING LOAN DEWAN HFC',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'CORPORATION BANK EDU LOAN INT',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'HFC PRINCIPAL',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'ICICI HOUSING LOAN-INT',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'NEW INDIA ASSURANCE MEDICAL INSURANCE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan PNB',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'PHYSICALL HANDICAPPED(40%)',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'NPS SCHEME',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'BIRLA SUNLIFE MF',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'CORPORATION EDUCATION LOAN INTEREST',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIP-ING VYSYA',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan-HDFC',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'PNB HOUSING LOAN PRINCIPAL',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan - LIC HFC',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'TUTION FEE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'SBI HOUSING LOAN PRINCIPLE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Diability',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIC',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'NEW INDIA MEDICAL INSUARNCE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan-Dewan HFC',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'EDUCATION LOAN INT/ANDHRA BANK',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'ICICI PRU LIFE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Physically handicapped dependent',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan-LIC HFC',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIC HFL INT',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'INT ON EDU LOAN-BOB',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'APOLLO MUNCIH HEALTH POLICY',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Donation to GAU SEVA SADAN',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'NIACL-MEDICAL INS',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'HDFC HOUSING LOAN',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'NIA MEDICAL INSUARANCE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'PGIMER DONATION',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIC HOUSING LOAN PRINCIPLE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'PPF POST OFFICE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Visually Impaired (PH - 100%)',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'DONATION',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIP-Shriram LIC',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'SBI LIFE INSURANCE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan Int - Indian bank',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'INT ON EDUCN LOAN',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'CORP-EDU LOAN-INT',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'DISABILITY,VH',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'DHFL HOUSING LOAN',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'THENKAILAYA BAKTHI PERAVAI DONATION',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'HOUSING LOAN INDIAN BANK PRINICIPAL',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'SBO HOUSING LOAN INT',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'SBI HOUSING LOAN',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'EDELWEISS TOKIO LIP',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan - DHFL',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'ICICI HOUSING LOAN PRINCIPLE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan-Canara Bank',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'INT ON EDU LOAN SBI',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'ICICI Prudential Life',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Donations - ISHA',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'hdfc credila edu loan interest',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Tution Fees',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan-NATIONAL TRUST HOUSING FIN',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'ICICI PRUDENTIAL LIP',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'visual diabilty 100%',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Mentally retarded dependent',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'DSP BLACK ROCK MUTUAL FUND',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIP-Max Life',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'HOUSING LOAN PRIN ICICI',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'visually handicapped',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan-LIC Housing Finance',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIP-ICICI',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'DONATION TO SIGHT RESTORATION SURGERIES',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing loan-IDBI & LIC HFL',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'ARUNODAYA FOUNDATION',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Tuition Fee',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIP-Birla Sun Life',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'HOUSING LOAN INT-LIC HFL',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'EDU LOAN INT ANDHRA BANK',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'PNB HOUSING LOAN-INT',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'HelpAge India',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'DHFL HOUSING LOAN PRINCIPLE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIC HOUSING LOAN PRIN',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'INTEREST ON HOUSING LOAN',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIC HOUSING FINANCE INTEREST',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),' UCO HOUSING LOAN PRINCIPLE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan SBI Principal',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan-SBI',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'NPS',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),' Tution Fee',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'TATA AIA LIP',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIC MUTUAL FUND',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'RELIANCE MUTUAL FUND',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'ICICI BANK HL PRINCIPAL',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'ANDHRA BANK HOUSING LOAN INTEREST',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan -LIC HFL',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'uco bank housing loan interest',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Pension - HDFC',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'IIFL INTEREST',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan-PNB',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan - IDBI',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'HOUSING LOAN INDIAN BANK INT',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Health Insurance Policy',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'HDFC LIP',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'CORPORATION BANK EDU LOAN PRINCIPAL',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housig Loan-DHFC',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIP(MAY&NOV)',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'ICICI LIP',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan - LIC HF',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'SYNDICATE BANK HOUSING LOAN PRINCIPLE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'HDFC LIFE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'DHFL HOUSING LOAN INT',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'EDUCATION LOAN INT TSCAB',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'MEDICAL INS PERMIUM',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'INT OF HOUSE LOAN-SBI',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'SAHRUDHAYA OLD AGE HOME',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan - LIC',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Tution Fee - TKR Edu. College',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'SBI LIP',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIP-Kotak',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'EDUCATION LOAN INTEREST(SBI)',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'INT OF VIZAG COOP BANK HOUSING LOAN',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'MAX HEALTH INSURANCE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'TUTUION FEES',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'DHFL HOUSING LOAN PRINCIPAL',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'ED LOAN- CORP BANK INTEREST',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'NSC',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan-United Bank of India',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'GIC HOUSING LOAN',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIP RELIANCE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'EDUCATION LOAN PRINCIPAL(SBI)',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'HL-LIC HOUSING FIN LTD',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Physically hadicapped dependent',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan-IDBI Bank',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan LIC HFL INT',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan PNB INT',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'MEDICLAIM POLICY-NIACL',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIP-Bajaj  Allianz',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'DONATIONS',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan - HDFC',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'FRANK TEMP TAX SHIELD MF',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'PNB HOUSING LOAN INTEREST',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'DISABILITY OF LEFT HAND',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'ICICI HOUSING LOAN',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIC HFL PRINICIPLE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'HL-UCO BANK',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'IDBI HOUSING LOAN',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'VISUALLY HANDICAPED',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'IIFL HOUSLING LOAN PRINCIPLE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'NATIONAL INSURANCE MEDICLAIM',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'SBI HOUSING LOAN-INT',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIC HOUSING LOAN INTEREST',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Tax saver FDR',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'HL INT- UCO BANK',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIP - ICICI PRODENTIAL',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'MALLIKAMBA INST OF MEN HANDICAPPED',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'HOUSING LOAN PRI-SBI',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'INT ON EDU LOAN-IOB',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'PNB HOUSING LOAN',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'PNB HOUSING LOAN PRINCIPLE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'DHFL HOUSING LOAN INTEREST',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIC HFC HOUSING LOAN INTEREST',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'SUKANYA SAMRUDHI YOJANA',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Tutition fees',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'MUTUAL FUNDS-AXIS',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Mediclim Policy',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'DONATION TO ISHA EDUCATION',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIC HOUSING FIN LTD-INTEREST',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIC HOUSING FIN LTD- PRINCIPAL',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'PNB HOUSING LOAN INT',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan-NTHF',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Tuition Fees',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan HDFC',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIC HFL',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'NICL MEDIACL INSURANCE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan SBI Int',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan-CITI Bank',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'HDFL HOUSING LOAN INT',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan LIC HFL',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'PHYSICALLY HANDICAPPED(HH)',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'HOUSING LOAN ICICI PRIN',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'SBI LIFE SMART WEALTH BUILDER',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'LIC HFL INEREST',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'HDFC HOUSING LOAN INTEREST',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan-IDBI',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Mediclaim Policy',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'BHAGYANAGAR GAU-SEVA SADAN',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Standard deduction',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Interest on educational loan',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'GIC HOUSING LOAN INT',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'ICIC LOMBARD - MED INSURANCE',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Power Finance Corporation',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'HOUSING LOAN HDFC INT',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Can Fin Homes',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'Housing Loan - PNB',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_deduction_field_master values ((select max(id)+1 from pr_deduction_field_master),'TAX SAVER GLD',
'per_ded',1,(select max(trans_id)+1 from pr_deduction_field_master),null,null,null);	
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'Leave Encashment',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);
insert into pr_earn_field_master values ((select max(id)+1 from pr_earn_field_master),'codperks',
'per_earn',1,(select max(trans_id)+1 from pr_earn_field_master),null,null,null);

