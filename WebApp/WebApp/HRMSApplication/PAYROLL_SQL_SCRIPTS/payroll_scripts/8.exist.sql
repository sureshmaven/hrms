
--procedure for emp_general

CREATE procedure [dbo].[exist] @empid int as   
begin  
SELECT CASE  
WHEN EXISTS(SELECT emp_code  
FROM pr_emp_general  
WHERE emp_code=@empid)  
THEN (select emp_code  
FROM pr_emp_general  
WHERE emp_code=@empid)  
ELSE (select EmpId  
FROM Employees  
WHERE empid=@empid)  
END as emp_code,CASE  
WHEN EXISTS(SELECT sex  
FROM pr_emp_general  
WHERE emp_code=@empid)  
THEN (select sex  
FROM pr_emp_general  
WHERE emp_code=@empid)  
ELSE (select Gender  
FROM Employees  
WHERE empid=@empid)  
END as sex,CASE  
WHEN EXISTS(SELECT martial_status  
FROM pr_emp_general  
WHERE emp_code=@empid)  
THEN (select martial_status  
FROM pr_emp_general  
WHERE emp_code=@empid)  
ELSE (select MartialStatus  
FROM Employees  
WHERE empid=@empid)  
END as martial_status,(select zone from pr_emp_general where emp_code=@empid) as zone,  
(select designation_category from pr_emp_general where emp_code=@empid) as designation_category,
CASE WHEN EXISTS(SELECT Designation   
FROM pr_emp_general  
WHERE emp_code=@empid)  
THEN (select Designation   
FROM pr_emp_general  
WHERE emp_code=@empid)  
ELSE (select d.name as designation FROM Employees e join Designations d on e.CurrentDesignation = d.Id WHERE empid=@empid) END as designation,  
(select region_for_p_tax from pr_emp_general where emp_code=@empid) as region_for_p_tax,  
(select p_tax_region from pr_emp_general where emp_code=@empid) as p_tax_region,  
CASE WHEN EXISTS(SELECT address   
FROM pr_emp_general  
WHERE emp_code=@empid)  
THEN (select address   
FROM pr_emp_general  
WHERE emp_code=@empid)  
ELSE (select PresentAddress   
FROM Employees  
WHERE empid=@empid) end as address,
CASE WHEN EXISTS(SELECT per_address   
FROM pr_emp_general  
WHERE emp_code=@empid)  
THEN (select per_address   
FROM pr_emp_general  
WHERE emp_code=@empid)  
ELSE (select PermanentAddress   
FROM Employees  
WHERE empid=@empid) end as per_address,  
CASE WHEN EXISTS(SELECT per_phoneno   
FROM pr_emp_general  
WHERE emp_code=@empid)  
THEN (select per_phoneno   
FROM pr_emp_general  
WHERE emp_code=@empid)  
ELSE (select MobileNumber   
FROM Employees  
WHERE empid=@empid) end as per_phoneno,  
(select native_place from pr_emp_general where emp_code=@empid) as native_place,  
(select division from pr_emp_general where emp_code=@empid) as division,  
CASE WHEN EXISTS(SELECT FORMAT (dob, 'yyyy-MM-dd') as dob   
FROM pr_emp_general  
WHERE emp_code=@empid)  
THEN (select FORMAT (dob, 'yyyy-MM-dd') as dob  
FROM pr_emp_general  
WHERE emp_code=@empid)  
ELSE (select FORMAT (dob, 'yyyy-MM-dd') as dob  
FROM Employees  
WHERE empid=@empid) end as dob,
CASE WHEN EXISTS(SELECT emp_age   
FROM pr_emp_general  
WHERE emp_code=@empid)  
THEN (select emp_age   
FROM pr_emp_general  
WHERE emp_code=@empid)  
ELSE (select (year(getdate()) - year(dob)) 
FROM Employees  
WHERE empid=@empid) end as emp_age,
CASE WHEN EXISTS(SELECT exp   
FROM pr_emp_general  
WHERE emp_code=@empid)  
THEN (select exp   
FROM pr_emp_general  
WHERE emp_code=@empid)  
ELSE (select TotalExperience   
FROM Employees  
WHERE empid=@empid) end as exp,  
(select pf_no from pr_emp_general where emp_code=@empid) as pf_no,  
(select uan_no from pr_emp_general where emp_code=@empid) as uan_no,  
(select FORMAT (doj_pf, 'yyyy-MM-dd') as doj_pf from pr_emp_general where emp_code=@empid) as doj_pf,  
CASE WHEN EXISTS(SELECT email_id   
FROM pr_emp_general  
WHERE emp_code=@empid)  
THEN (select email_id   
FROM pr_emp_general  
WHERE emp_code=@empid)  
ELSE (select PersonalEmailId   
FROM Employees  
WHERE empid=@empid) end as email_id,  
(select identify_mark1 from pr_emp_general where emp_code=@empid) as identify_mark1,  
(select identify_mark2 from pr_emp_general where emp_code=@empid) as identify_mark2,CASE  
WHEN EXISTS(SELECT blood_group  
FROM pr_emp_general  
WHERE emp_code=@empid)  
THEN (select blood_group  
FROM pr_emp_general  
WHERE emp_code=@empid)  
ELSE (select BloodGroup  
FROM Employees  
WHERE empid=@empid)  
END as blood_group,(select religion from pr_emp_general where emp_code=@empid) as religion,  
(select cur_reservation from pr_emp_general where emp_code=@empid) as cur_reservation,  
(select reg_order from pr_emp_general where emp_code=@empid) as reg_order ,  
(select join_reservation from pr_emp_general where emp_code=@empid) as join_reservation,
CASE  
WHEN EXISTS(SELECT pan_no  
FROM pr_emp_general  
WHERE emp_code=@empid)  
THEN (select pan_no  
FROM pr_emp_general  
WHERE emp_code=@empid)  
ELSE (select PanCardNo  
FROM Employees  
WHERE empid=@empid)  
END as pan_no,
(select branch_code from pr_emp_general where emp_code=@empid) as branch_code,
(select pay_bank from pr_emp_general where emp_code=@empid) as pay_bank,  
(select account_code from pr_emp_general where emp_code=@empid) as account_code,  
(select bank_accno from pr_emp_general where emp_code=@empid) as bank_accno,  
(select customer_id from pr_emp_general where emp_code=@empid) as customer_id,  
(select acc_with_dccb from pr_emp_general where emp_code=@empid) as acc_with_dccb,  
(select phy_handicapped from pr_emp_general where emp_code=@empid) as phy_handicapped,  
(select house_provided from pr_emp_general where emp_code=@empid) as house_provided,  
(select designation_no from pr_emp_general where emp_code=@empid) as designation_no,  
(select stl_temp from pr_emp_general where emp_code=@empid) as stl_temp,  
(select fest_adv from pr_emp_general where emp_code=@empid) as fest_adv,  
(select artr_emp from pr_emp_general where emp_code=@empid) as artr_emp,CASE  
WHEN EXISTS(SELECT aadhaar_no  
FROM pr_emp_general  
WHERE emp_code=@empid)  
THEN (select aadhaar_no  
FROM pr_emp_general  
WHERE emp_code=@empid)  
ELSE (select AadharCardNo  
FROM Employees  
WHERE empid=@empid)  
END as aadhaar_no;  
end; 

