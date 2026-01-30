update employees set branch_value1=42 where branch_value1='HeadOffice'

update employees set branch_value1=43 where branch_value1 !=42

select count(empid) as empid,controllingauthority from employees group by controllingauthority
select count(empid) as empid, sanctioningauthority from employees group by sanctioningauthority
