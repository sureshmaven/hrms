update employees set sanctioningauthority=(select id from employees where empid=381)

 where sanctioningauthority='';
 
 update employees set controllingauthority=(select id from employees where empid=878)

 where controllingauthority is null;
 
 
 update employees set branch_value1=42 where branch=(select id from branches where name='HeadOffice');
 
 update employees set branch_value1=43 where branch!=42;
 update employees set branch_value_2=1 where branch=42;
 update employees set branch_value_2=2 where branch!=42;
 
 update employees set sanctioningauthority=686 where empid=686;
 update employees set controllingauthority=686 where empid=686;
 
 update employees set loginmode='Admin';
  ---update employees set password=empid;
 update employees set Password=123456 where Password=123;
select * from employees where Department is null;
update employees set Department=47 where Department=1;

update employees set ControllingAuthority=1 ,SanctioningAuthority=1 where empid=686