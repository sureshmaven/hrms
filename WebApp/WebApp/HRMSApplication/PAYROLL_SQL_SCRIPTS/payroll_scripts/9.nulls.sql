--procedure for emp_general

CREATE procedure [dbo].[nulls] @empid int as   
begin  
select case when emp_code is null then (SELECT EmpId FROM Employees WHERE EmpId=@empid)   
else emp_code end as emp_code,   
case when sex is null then (SELECT Gender FROM Employees WHERE EmpId=@empid) else sex end as sex,  
case when martial_status is null then (SELECT MartialStatus FROM Employees WHERE EmpId=@empid) else martial_status end as martial_status,zone,
case when Designation is null then (select d.name as designation FROM Employees e join Designations d on e.CurrentDesignation = d.Id WHERE empid=@empid) else Designation end as designation,  
designation_category,  
region_for_p_tax,p_tax_region,address,per_address,  
case when per_phoneno is null then (SELECT per_phoneno FROM Employees WHERE EmpId=@empid) else per_phoneno end as per_phoneno,native_place,division,  
case when dob is null then (SELECT FORMAT (dob, 'yyyy-MM-dd') as dob FROM Employees WHERE EmpId=@empid) else FORMAT (dob, 'yyyy-MM-dd') end as dob,   
case when exp is null then (SELECT TotalExperience FROM Employees WHERE EmpId=@empid) else exp end as exp,pf_no,uan_no,FORMAT (doj_pf, 'yyyy-MM-dd') as doj_pf,  
case when email_id is null then (SELECT email_id FROM Employees WHERE EmpId=@empid) else email_id end as email_id,identify_mark1,identify_mark2,
case when blood_group is null then (SELECT BloodGroup FROM Employees WHERE EmpId=@empid)   
else blood_group end as blood_group,religion,cur_reservation,join_reservation,case when pan_no is null   
then (SELECT PanCardNo FROM Employees WHERE EmpId=@empid)   
else pan_no end as pan_no,reg_order,branch_code,pay_bank,account_code,bank_accno,  
customer_id,acc_with_dccb,phy_handicapped,house_provided,emp_age,designation_no,stl_temp,  
fest_adv,artr_emp,case when aadhaar_no is null then (SELECT AadharCardNo FROM Employees WHERE EmpId=@empid)   
else aadhaar_no end as aadhaar_no  
from pr_emp_general where emp_code=@empid;  
end; 
GO