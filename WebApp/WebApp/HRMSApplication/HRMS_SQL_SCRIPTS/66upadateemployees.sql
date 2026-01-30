update employees set firstname='SEKHAR' where lastName like '%P CHANDRA%' and  firstname like  '%SHEKAR%' and empid=356;

update employees set FirstName='SRINIVASA RAO',LastName='M R' where empid=371;

update employees set FirstName='SRINIVASA RAO',LastName='TBVR' where empid=5882;

update employees set Department=(select id from departments where name='IT Dept') where empid=463;	

update employees set Department=(select id from departments where name='L&A-PMU') where empid=204;

update employees set Department=(select id from departments where name=' IT Dept - EPD') where empid=368;

update employees set Department=(select id from departments where name='Legal, Vigilance & RTI Act') where empid=371;

update employees set Department=(select id from departments where name='P&D Dept') where empid=832 ;

Update employees set controllingauthority=(select id from employees where lastName like '%TBVR%' and  firstname like  '%SRINIVASA RAO%') WHERE empid=491; 							

Update employees set controllingauthority=(select id from employees where  LastName like 'M.' and  firstname like  '%RAYAPPA%') WHERE empid=499;

Update employees set controllingauthority=(select id from employees where lastName like '%CH CHINNA%' and  firstname like  '%RAO%') WHERE empid=5890;  

Update employees set controllingauthority=(select id from employees where lastName like '%ANANTHJEET%' and  firstname like  '%KAUR%') WHERE empid=510;

Update employees set controllingauthority=(select id from employees where lastName like '%P CHANDRA%' and  firstname like  '%SEKHAR%') WHERE empid=511;						
			
Update employees set controllingauthority=(select id from employees where lastName like '%P CHANDRA%' and  firstname like  '%SEKHAR%') WHERE empid=512						

Update employees set controllingauthority=(select id from employees where FirstName='SRINIVASA RAO' and LastName='M R') WHERE empid=532;						

Update employees set controllingauthority=(select id from employees where lastName like '%P CHANDRA%' and  firstname like  '%SEKHAR%') WHERE empid=1930;	

Update employees set controllingauthority=(select id from employees where lastName like '%Y NIRMALA%' and  firstname like 'KUMARI') WHERE empid=250; 						

