--pr_emp_payslip
CREATE INDEX pr_emp_payslip_fm ON pr_emp_payslip (fm);
CREATE INDEX pr_emp_payslip_fy ON pr_emp_payslip (fy);
CREATE INDEX pr_emp_payslip_emp_code ON pr_emp_payslip (emp_code);
CREATE INDEX pr_emp_payslip_spl_type ON pr_emp_payslip(spl_type);
CREATE INDEX pr_emp_payslip_gross_amount ON pr_emp_payslip (gross_amount);
CREATE INDEX pr_emp_payslip_branch ON pr_emp_payslip (branch);
--all_constants
CREATE INDEX all_constants_constant ON all_constants (constant);
--pr_month_details
CREATE INDEX pr_month_details_fm ON pr_month_details (fm);
CREATE INDEX pr_month_details_activeON ON pr_month_details(active);
--pr_emp_hfc_details
CREATE INDEX pr_emp_hfc_details_fm ON pr_emp_hfc_details (fm);
CREATE INDEX pr_emp_hfc_details_fy ON pr_emp_hfc_details (fy);
CREATE INDEX pr_emp_hfc_details_code ON pr_emp_hfc_details (emp_code);
CREATE INDEX pr_emp_hfc_details_active ON pr_emp_hfc_details(active);
CREATE INDEX pr_emp_hfc_details_amount ON pr_emp_hfc_details (amount);
--pr_emp_lic_details
CREATE INDEX pr_emp_lic_details_fm ON pr_emp_lic_details (fm);
CREATE INDEX pr_emp_lic_details_fy ON pr_emp_lic_details (fy);
CREATE INDEX pr_emp_lic_details_code ON pr_emp_lic_details (emp_code);
CREATE INDEX pr_emp_lic_details_active ON pr_emp_lic_details(active);
CREATE INDEX pr_emp_lic_details_amount ON pr_emp_lic_details (amount);
--pr_emp_adv_loans
CREATE INDEX pr_emp_adv_loans_emp_code ON pr_emp_adv_loans (emp_code);
CREATE INDEX pr_emp_adv_loans_sanction_date ON pr_emp_adv_loans (sanction_date);
CREATE INDEX pr_emp_adv_loans_loan_type_mid ON pr_emp_adv_loans (loan_type_mid);
CREATE INDEX pr_emp_adv_loans_active ON pr_emp_adv_loans(active);
--pr_loan_master
CREATE INDEX pr_loan_masteractive ON pr_loan_master(active);
--pr_emp_tds_process
CREATE INDEX pr_emp_tds_process_fm ON pr_emp_tds_process (fm);
CREATE INDEX pr_emp_tds_process_fy ON pr_emp_tds_process (fy);
CREATE INDEX pr_emp_tds_process_active ON pr_emp_tds_process (active);
CREATE INDEX pr_emp_tds_process_sal_basic ON pr_emp_tds_process(sal_basic);
CREATE INDEX pr_emp_hfc_details_amount_emp_code ON pr_emp_tds_process (empcode);
--pr_emp_general
CREATE INDEX pr_emp_general_emp_code ON pr_emp_general (emp_code);
--pr_emp_tds_section_deductions
CREATE INDEX pr_emp_tds_section_deductions_active ON pr_emp_tds_section_deductions (active);
CREATE INDEX pr_emp_tds_section_deductions_empcode ON pr_emp_tds_section_deductions (empcode);
CREATE INDEX pr_emp_tds_section_deductions_m_id ON pr_emp_tds_section_deductions (m_id);
CREATE INDEX pr_emp_tds_section_deductions_section_type ON pr_emp_tds_section_deductions(section_type);
--pr_deduction_field_master
CREATE INDEX pr_deduction_field_master_type ON pr_deduction_field_master (type);
--pr_emp_branch_allowances
CREATE INDEX pr_emp_branch_allowances_emp_code ON pr_emp_branch_allowances (emp_code);
CREATE INDEX pr_emp_branch_allowances_allowance_mid ON pr_emp_branch_allowances (allowance_mid);
CREATE INDEX pr_emp_branch_allowances_active ON pr_emp_branch_allowances (active);
--pr_ob_share
CREATE INDEX pr_ob_share_fm ON pr_ob_share (fm);
CREATE INDEX pr_ob_share_emp_code ON pr_ob_share (emp_code);
CREATE INDEX pr_ob_share_active ON pr_ob_share(active);
--pr_emp_biological_field
CREATE INDEX pr_emp_biological_field_revision_of_date_change ON pr_emp_biological_field (revision_of_date_change);
--pr_emp_inc_anual_stag
CREATE INDEX pr_emp_inc_anual_stag_increment_date ON pr_emp_inc_anual_stag (increment_date);
CREATE INDEX pr_emp_inc_anual_stag_authorisation ON pr_emp_inc_anual_stag (authorisation);
CREATE INDEX pr_emp_inc_anual_stag_active ON pr_emp_inc_anual_stag(active);
--pr_emp_payslip_allowance
CREATE INDEX pr_emp_payslip_allowance_emp_code ON pr_emp_payslip_allowance (emp_code);
CREATE INDEX pr_emp_payslip_allowance_all_type ON pr_emp_payslip_allowance (all_type);
CREATE INDEX pr_emp_payslip_allowance_all_name ON pr_emp_payslip_allowance(all_name);
CREATE INDEX pr_emp_payslip_allowance_all_amount ON pr_emp_payslip_allowance (all_amount);
--pr_emp_payslip_deductions
CREATE INDEX pr_emp_payslip_deductions_emp_code ON pr_emp_payslip_deductions (emp_code);
--pr_emp_jaib_caib_general
CREATE INDEX pr_emp_jaib_caib_general_authorisation ON pr_emp_jaib_caib_general (authorisation);
CREATE INDEX pr_emp_jaib_caib_general_fm ON pr_emp_jaib_caib_general (fm);
CREATE INDEX pr_emp_jaib_caib_general_incr_incen_type ON pr_emp_jaib_caib_general(incr_incen_type);
--pr_allowance_field_master
CREATE INDEX pr_allowance_field_master_name ON pr_allowance_field_master (name);
CREATE INDEX pr_allowance_field_master_active ON pr_allowance_field_master (active);
--pr_emp_allowances_spl
CREATE INDEX pr_emp_allowances_spl_active ON pr_emp_allowances_spl (active);
CREATE INDEX pr_emp_allowances_spl_amount ON pr_emp_allowances_spl (amount);
--pr_branch_allowance_master
CREATE INDEX pr_branch_allowance_master_active ON pr_branch_allowance_master (active);
CREATE INDEX pr_branch_allowance_master_amount ON pr_branch_allowance_master (amount);
CREATE INDEX pr_branch_allowance_master_name ON pr_branch_allowance_master (name);
--pr_emp_allowances_gen
CREATE INDEX pr_emp_allowances_gen_active ON pr_emp_allowances_gen (active);
CREATE INDEX pr_emp_allowances_gen_amount ON pr_emp_allowances_gen (amount);
--pr_emp_deductions
CREATE INDEX pr_emp_deductions_amount ON pr_emp_deductions (amount);
--pr_emp_pf_nonrepayable_loan
CREATE INDEX pr_emp_pf_nonrepayable_loan_fm ON pr_emp_pf_nonrepayable_loan (fm);
CREATE INDEX pr_emp_pf_nonrepayable_loan_code ON pr_emp_pf_nonrepayable_loan (emp_code);
--pr_pfopeningbal
CREATE INDEX pr_pfopeningbal_loan_code ON pr_pfopeningbal (emp_code);
--pr_emp_adv_loans_adjustments
CREATE INDEX pr_emp_adv_loans_adjustments_fm ON pr_emp_adv_loans_adjustments (fm);
CREATE INDEX pr_emp_adv_loans_adjustments_active ON pr_emp_adv_loans_adjustments (active);
CREATE INDEX pr_emp_adv_loans_adjustments_cash_paid_on ON pr_emp_adv_loans_adjustments (cash_paid_on);
--pr_emp_adv_loans_child
CREATE INDEX pr_emp_adv_loans_child_active ON pr_emp_adv_loans_child (active);
CREATE INDEX pr_emp_adv_loans_child_priority ON pr_emp_adv_loans_child (priority);
--pr_emp_loans_projection
CREATE INDEX pr_emp_loans_projection_fm ON pr_emp_loans_projection (fm);
CREATE INDEX pr_emp_loans_projection_active ON pr_emp_loans_projection (active);
CREATE INDEX pr_emp_loans_projection_interest_rate ON pr_emp_loans_projection (interest_rate);


-----------------------------------------------------------------------

--Duplicate ids found in the following tables

--alter table	pr_emp_payslip	add primary key (	id	);
--alter table	pr_emp_hfc_details	add primary key (	id	);
--alter table	pr_emp_lic_details	add primary key (	id	);
--alter table	pr_deduction_field_master	add primary key (	id	);
--alter table	pr_ob_share	add primary key (	id	);
--alter table	pr_emp_payslip_deductions	add primary key (	id	);
--alter table	pr_emp_pf_nonrepayable_loan	add primary key (	id	);
--alter table	pr_emp_general	add primary key (	emp_code	);
--alter table	pr_emp_biological_field	add primary key (	emp_code	);
--alter table	pr_emp_general	add primary key (	emp_code	);

----------------------------------------------------------------------------------------------
ALTER TABLE	all_constants	ALTER COLUMN	id	INTEGER NOT NULL	;
ALTER TABLE	pr_month_details	ALTER COLUMN	id	INTEGER NOT NULL	;
ALTER TABLE	pr_emp_adv_loans	ALTER COLUMN	id	INTEGER NOT NULL	;
ALTER TABLE	pr_loan_master	ALTER COLUMN	id	INTEGER NOT NULL	;
ALTER TABLE	pr_emp_tds_process	ALTER COLUMN	id	INTEGER NOT NULL	;
ALTER TABLE	pr_emp_tds_section_deductions	ALTER COLUMN	id	INTEGER NOT NULL	;
ALTER TABLE	pr_emp_branch_allowances	ALTER COLUMN	id	INTEGER NOT NULL	;
ALTER TABLE	pr_emp_inc_anual_stag	ALTER COLUMN	id	INTEGER NOT NULL	;
ALTER TABLE	pr_emp_payslip_allowance	ALTER COLUMN	id	INTEGER NOT NULL	;
ALTER TABLE	pr_emp_jaib_caib_general	ALTER COLUMN	id	INTEGER NOT NULL	;
ALTER TABLE	pr_allowance_field_master	ALTER COLUMN	id	INTEGER NOT NULL	;
ALTER TABLE	pr_emp_allowances_spl	ALTER COLUMN	id	INTEGER NOT NULL	;
ALTER TABLE	pr_branch_allowance_master	ALTER COLUMN	id	INTEGER NOT NULL	;
ALTER TABLE	pr_emp_allowances_gen	ALTER COLUMN	id	INTEGER NOT NULL	;
ALTER TABLE	pr_emp_deductions	ALTER COLUMN	id	INTEGER NOT NULL	;
ALTER TABLE	pr_pfopeningbal	ALTER COLUMN	Id	INTEGER NOT NULL	;
ALTER TABLE	pr_emp_adv_loans_adjustments	ALTER COLUMN	id	INTEGER NOT NULL	;
ALTER TABLE	pr_emp_loans_projection	ALTER COLUMN	id	INTEGER NOT NULL	;
ALTER TABLE	pr_emp_adv_loans_child	ALTER COLUMN	id	INTEGER NOT NULL	;
