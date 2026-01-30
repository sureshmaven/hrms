 select * into employees4apr from employees

UPDATE employees SET branch=T.oldbranch,department=t.OldDepartment FROM employees e join Employee_Transfer  t on e.id = T.EmpId 
where Type='TemporaryTransfer' and convert(date,Effectivefrom)<convert(Date, GETDATE()) and convert(date,Effectiveto)<convert(Date,GETDATE());

UPDATE employees SET branch=T.oldbranch,department=t.OldDepartment FROM employees e join Employee_Transfer  t on e.id = T.EmpId 
and convert(date,Effectivefrom)<=convert(Date, GETDATE()) and convert(date,Effectiveto)<=convert(Date, GETDATE()) and 
DATEDIFF(d,convert(date,EffectiveTo),convert(Date, GETDATE()))=1;



select * from employees where concat(year(dob),' ',month(dob),' ',day(dob))= concat(year(getdate()),' ',month(getdate()),' ',day(getdate()));

select * from employees where concat(year(doj),' ',month(doj),' ',day(doj))=concat(year(getdate()),' ',month(getdate()),' ',day(getdate()));


select * from Employees where  DATEDIFF(d,RetirementDate,GETDATE())<0; 

update employees set password='tscabret1red' where password='123456' and DATEDIFF(d,RetirementDate,GETDATE())<0; 



