update employees set JoinedDesignation=7 where empid=5770;

update employees set branch=(select id from branches where name='SAROORNAGAR'),department=
(select id from departments where code='OtherDepartment'),Branch_Value1=42,branch_value_2=1 where empid=845;
 
update employees set branch=(select id from branches where name='OtherBranch'),department=
(select id from departments where code='Inward-Outward'),Branch_Value1=42,branch_value_2=1 where empid=5770;



insert into DB_Script(DT_Time,Type,FileName) values(Getdate(),'DDL-DML','02072018.SQL');