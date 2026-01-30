ALTER TABLE pr_earn_field_master ADD code varchar(50),emp_master_Code VARCHAR (50);
ALTER TABLE pr_deduction_field_master ADD code varchar(50),emp_master_Code VARCHAR (50);
ALTER TABLE pr_allowance_field_master ADD code varchar(50),emp_master_Code VARCHAR (50);

ALTER table pr_pfopeningbal add PFreturn int,VPFreturn int,Bankreturn int,pfintcurr int,
vpfintcurr int,bankintcurr int,intDate date ,pfintrate int, active bit, trans_id int;

alter table ple_type add fy int,fm date,payslip_mid int;

-- Alter table pr_ob_share, satya , 17/07/2019
alter table pr_ob_share add own_share_open float,own_share_total float,vpf_open  float,vpf_total float,bank_share_open float,
bank_share_total float,pension_open float NOT NULL DEFAULT 0,pension_total float NOT NULL DEFAULT 0,pension_intrest_amount float NOT NULL DEFAULT 0,bank_share_intrst_open float,bank_share_intrst_total float,own_share_intrst_open float,own_share_intrst_total float,vpf_intrst_open float,vpf_intrst_total float;

-- alter id in pr_month_attendance_temp
alter table pr_month_attendance alter column id int null;

-- revision_of_date_change for increments
alter table pr_emp_biological_field add revision_of_date_change varchar(50);


--for form 12b
alter table pr_emp_incometax_12ba_master add type varchar(20);


--Added the new column CompletedEmpCodes in pr_payroll_service_run for getting codes which is processed
ALTER TABLE [dbo].pr_payroll_service_run ADD CompletedEmpCodes NVARCHAR(max)



--Added new columns in  Employee_Transfer
alter table Employee_Transfer add fy int,fm date,category varchar(70),new_basic float,old_basic float,incre_due_date date,senoirity_order int,authorisation bit,active bit;



--interest calculated or not in ob share

alter table pr_month_details add is_interest_calculated [bit] NULL;
alter table pr_month_details add interest_percent [float] NULL;

--Raji, 18-09-2019, adding new column in loan related tables
Alter table pr_emp_adv_loans add loan_sl_no int  DEFAULT(0);
Alter table pr_emp_adv_loans_child add loan_sl_no int  DEFAULT(0);
Alter table pr_emp_adv_loans_adjustments add loan_sl_no int  DEFAULT(0);

--Samanth, 21-09-2019, alter table pr_emp_loans_ledger

Alter table pr_emp_loans_projection add loan_sl_no int  DEFAULT(0);

--Samanth, 26-09-2019 
alter table pr_emp_adv_loans_adjustments add show bit;
---Added the column for payslip
ALTER TABLE pr_payroll_service_run
ADD username nvarchar(max);

--payroll login access
alter table all_constants alter column constant varchar(300)

--alter table pr_emp_pf_nonrepayable_loan
alter table pr_emp_pf_nonrepayable_loan add 
bankshare_interest float NOT NULL default '0.00',
ownshare_interest  float NOT NULL default '0.00',
vpf_interest  float NOT NULL default '0.00';
alter table pr_emp_pf_nonrepayable_loan add is_interest_caculated float not null default 0;

--alter table pr_ob_share
alter table pr_ob_share add is_interest_caculated float not null default 0;


-- Alter table pr_pfopeningbal for changing datatypes
alter table pr_pfopeningbal alter column pfintrate float;
alter table pr_pfopeningbal alter column PFreturn float;
alter table pr_pfopeningbal alter column VPFreturn float;
alter table pr_pfopeningbal alter column Bankreturn float;
alter table pr_pfopeningbal alter column pfintcurr float;
alter table pr_pfopeningbal alter column vpfintcurr float;
alter table pr_pfopeningbal alter column bankintcurr float;

-- Alter table pr_emp_adhoc_det_field change datatype
alter table pr_emp_adhoc_det_field alter column cheque_no nvarchar(max);

-- Alter table pr_payroll_service_run alter column err_desc
alter table pr_payroll_service_run alter column err_desc nvarchar(max);

--ALTER table pr_month_attendance Add sus_per 
ALTER table pr_month_attendance Add sus_per float;

-- Added new column in pr_purpose_of_advance_master
alter table pr_purpose_of_advance_master add purpose_code nvarchar(max);

--to increase the address size

alter table pr_emp_general alter column address nvarchar(max);
alter table pr_emp_general alter column per_address nvarchar(max);