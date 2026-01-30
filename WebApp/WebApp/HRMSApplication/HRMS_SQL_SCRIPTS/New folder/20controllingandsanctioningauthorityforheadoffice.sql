update employees set Department=47 where Department=1 and 'OtherDepartment'='OtherDepartment';
update employees set 
ControllingAuthority=(select Id from employees where EmpId=5660),
department=(select Id from departments where name='PRINTING & STATIONERY'),
ControllingBranch=(select Id from Branches where Name='HeadOffice'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=5660),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),
Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2 where EmpId in(5660,6313);
update employees set department=(select Id from departments where name='Stationery') where EmpId=6313;
update employees set 
ControllingAuthority=(select id from employees where EmpId=358),
Department=(select Id from departments where Name='Investments-Front Office'),
ControllingBranch=(select Id from Branches where Name='HeadOffice'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=358),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),
Branch_Value1=(select Id from Branches where Name='HeadOffice'),
Branch_Value_2=1 where EmpId in(358,6334,955,6310);
update employees set department=(select Id from Branches where Name='FINANCE & ACCOUNTS') where EmpId=358;
update employees set 
ControllingAuthority=(select id from employees where EmpId=358),
Department=(select Id from departments where Name='Banking – Accounts'),
ControllingBranch=(select Id from Branches where Name='HeadOffice'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=358),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),
Branch_Value1=(select Id from Branches where Name='HeadOffice'),
Branch_Value_2=1 where EmpId in(358,785,841,942,6336);
update employees set department=(select Id from Branches where Name='Banking') where EmpId=841;
update employees set 
ControllingAuthority=(select id from employees where EmpId=5780),
Department=(select Id from departments where Name='BRCC'),
ControllingBranch=(select Id from Branches where Name='HeadOffice'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=5780),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),
Branch_Value1=(select Id from Branches where Name='HeadOffice'),
Branch_Value_2=1 where EmpId in(5780,6317,962,977);
update employees set ControllingAuthority=(select id from employees where EmpId=5780),
Department=(select Id from departments where Name='RMD-Risk Management Dept'),
ControllingBranch=(select Id from Branches where Name='HeadOffice'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=5780),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),
Branch_Value1=(select Id from Branches where Name='HeadOffice'),
Branch_Value_2=1 where EmpId in(5780,6342,6337,6331);
update employees set 
ControllingAuthority=(select id from employees where EmpId=377),
Department=(select Id from departments where Name='CLPC & HFC'),
ControllingBranch=(select Id from Branches where Name='HeadOffice'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=377),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),
Branch_Value1=(select Id from Branches where Name='HeadOffice'),
Branch_Value_2=1 where EmpId in(377,956,6325,856,433,438);
update employees set department=(select Id from Branches where name='CLPC') where EmpId in(377,6325);
update employees set department=(select Id from Branches where name='CLPC RECOVERY') where EmpId=438;
update employees set ControllingAuthority=(select id from employees where EmpId=377),Department=(select Id from departments where Name='KYC/AML'),ControllingBranch=(select Id from Branches where Name='HeadOffice'),ControllingDesignation=(select CurrentDesignation from Employees where EmpId=377),SanctioningAuthority=(select Id from employees where EmpId=381),SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),SanctioningBranch=(select Id from Branches where Name='HeadOffice'),SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),Branch_Value1=(select Id from Branches where Name='HeadOffice'),Branch_Value_2=1where EmpId in(377,817,6314);
update employees set ControllingAuthority=(select id from employees where EmpId=359),Department=(select Id from departments where Name='Loans & Advances – ST'),ControllingBranch=(select Id from Branches where Name='HeadOffice'),ControllingDesignation=(select CurrentDesignation from Employees where EmpId=359),SanctioningAuthority=(select Id from employees where EmpId=381),SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),SanctioningBranch=(select Id from Branches where Name='HeadOffice'),SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),Branch_Value1=(select Id from Branches where Name='HeadOffice'),Branch_Value_2=1where EmpId in(359,864,943,872,572);
update employees set ControllingAuthority=(select id from employees where EmpId=340),Department=(select Id from departments where Name='Loans & Advances – LT'),ControllingBranch=(select Id from Branches where Name='HeadOffice'),ControllingDesignation=(select CurrentDesignation from Employees where EmpId=340),SanctioningAuthority=(select Id from employees where EmpId=381),SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),SanctioningBranch=(select Id from Branches where Name='HeadOffice'),SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),Branch_Value1=(select Id from Branches where Name='HeadOffice'),Branch_Value_2=1where EmpId in(340,991,902);
update employees set ControllingAuthority=(select id from employees where EmpId=555),Department=(select Id from departments where Name='Loans & Advances – IF'),ControllingBranch=(select Id from Branches where Name='HeadOffice'),ControllingDesignation=(select CurrentDesignation from Employees where EmpId=555),SanctioningAuthority=(select Id from employees where EmpId=381),SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),SanctioningBranch=(select Id from Branches where Name='HeadOffice'),SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),Branch_Value1=(select Id from Branches where Name='HeadOffice'),Branch_Value_2=1where EmpId in(555,934);
update employees set 	ControllingAuthority=(select id from employees where EmpId=265),	Department=(select Id from departments where Name='Legal,Vigilance & RTI Act'),	ControllingBranch=(select Id from Branches where Name='HeadOffice'),	ControllingDesignation=(select CurrentDesignation from Employees where EmpId=265),	SanctioningAuthority=(select Id from employees where EmpId=381),	SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),	SanctioningBranch=(select Id from Branches where Name='HeadOffice'),	SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),	Branch_Value1=(select Id from Branches where Name='HeadOffice'),	Branch_Value_2=1	where EmpId in(265,815);
update employees set 	ControllingAuthority=(select id from employees where EmpId=176),	Department=(select Id from departments where Name='DOS'),	ControllingBranch=(select Id from Branches where Name='HeadOffice'),	ControllingDesignation=(select CurrentDesignation from Employees where EmpId=176),	SanctioningAuthority=(select Id from employees where EmpId=381),	SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),	SanctioningBranch=(select Id from Branches where Name='HeadOffice'),	SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),	Branch_Value1=(select Id from Branches where Name='HeadOffice'),	Branch_Value_2=1	where EmpId in(176,286,777,562,876,6332,886,896,6306,6180,6338);
update employees set department=(select Id from Branches where name='DEPT OF SUPERVISION') where EmpId=6332;
update employees set 	ControllingAuthority=(select id from employees where EmpId=356),	Department=(select Id from departments where Name='HRD-Administration'),	ControllingBranch=(select Id from Branches where Name='HeadOffice'),	ControllingDesignation=(select CurrentDesignation from Employees where EmpId=356),	SanctioningAuthority=(select Id from employees where EmpId=381),	SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),	SanctioningBranch=(select Id from Branches where Name='HeadOffice'),	SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),	Branch_Value1=(select Id from Branches where Name='HeadOffice'),	Branch_Value_2=1	where EmpId in(356,6302,5908,861,914,926);
update employees set department=(select Id from Branches where name='HRD LEAVE') where EmpId=5908;
update employees set 	ControllingAuthority=(select id from employees where EmpId=790),	Department=(select Id from departments where Name='Board Secretariat'),	ControllingBranch=(select Id from Branches where Name='HeadOffice'),	ControllingDesignation=(select CurrentDesignation from Employees where EmpId=790),	SanctioningAuthority=(select Id from employees where EmpId=381),	SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),	SanctioningBranch=(select Id from Branches where Name='HeadOffice'),	SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),	Branch_Value1=(select Id from Branches where Name='HeadOffice'),	Branch_Value_2=1	where EmpId in(790,878);
update employees set 	ControllingAuthority=(select id from employees where EmpId=376),	Department=(select Id from departments where Name='HRD – Gratuity and PF'),	ControllingBranch=(select Id from Branches where Name='HeadOffice'),	ControllingDesignation=(select CurrentDesignation from Employees where EmpId=376),	SanctioningAuthority=(select Id from employees where EmpId=381),	SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),	SanctioningBranch=(select Id from Branches where Name='HeadOffice'),	SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),	Branch_Value1=(select Id from Branches where Name='HeadOffice'),	Branch_Value_2=1	where EmpId in(376,6305,793,6326,929,912);
update employees set department=(select Id from Branches where name='HRD–Payments') where EmpId=376;
update employees set 	ControllingAuthority=(select id from employees where EmpId=457),	Department=(select Id from departments where Name='Premises'),	ControllingBranch=(select Id from Branches where Name='HeadOffice'),	ControllingDesignation=(select CurrentDesignation from Employees where EmpId=457),	SanctioningAuthority=(select Id from employees where EmpId=381),	SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),	SanctioningBranch=(select Id from Branches where Name='HeadOffice'),	SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),	Branch_Value1=(select Id from Branches where Name='HeadOffice'),	Branch_Value_2=1	where EmpId in(457,5768,5866,922,6308);
update employees set 	ControllingAuthority=(select id from employees where EmpId=287),	Department=(select Id from departments where Name='TSCAB-CTI'),	ControllingBranch=(select Id from Branches where Name='HeadOffice'),	ControllingDesignation=(select CurrentDesignation from Employees where EmpId=287),	SanctioningAuthority=(select Id from employees where EmpId=381),	SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),	SanctioningBranch=(select Id from Branches where Name='HeadOffice'),	SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),	Branch_Value1=(select Id from Branches where Name='HeadOffice'),	Branch_Value_2=1	where EmpId in(287,913);
update employees set department=(select id from departments where name='TBD') where EmpId=287;
update employees set 	ControllingAuthority=(select id from employees where EmpId=5770),	Department=(select Id from departments where Name='Inward-Outward'),	ControllingBranch=(select Id from Branches where Name='HeadOffice'),	ControllingDesignation=(select CurrentDesignation from Employees where EmpId=5770),	SanctioningAuthority=(select Id from employees where EmpId=381),	SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),	SanctioningBranch=(select Id from Branches where Name='HeadOffice'),	SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),	Branch_Value1=(select Id from Branches where Name='HeadOffice'),	Branch_Value_2=1	where EmpId in(5770,505,5824);
update employees set 	ControllingAuthority=(select id from employees where EmpId=262),	Department=(select Id from departments where Name='P&D Dept'),	ControllingBranch=(select Id from Branches where Name='HeadOffice'),	ControllingDesignation=(select CurrentDesignation from Employees where EmpId=262),	SanctioningAuthority=(select Id from employees where EmpId=381),	SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),	SanctioningBranch=(select Id from Branches where Name='HeadOffice'),	SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),	Branch_Value1=(select Id from Branches where Name='HeadOffice'),	Branch_Value_2=1	where EmpId in(262,949,993);
update employees set 	ControllingAuthority=(select id from employees where EmpId=363),	Department=(select Id from departments where Name='IT Dept-CBS'),	ControllingBranch=(select Id from Branches where Name='HeadOffice'),	ControllingDesignation=(select CurrentDesignation from Employees where EmpId=363),	SanctioningAuthority=(select Id from employees where EmpId=381),	SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),	SanctioningBranch=(select Id from Branches where Name='HeadOffice'),	SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),	Branch_Value1=(select Id from Branches where Name='HeadOffice'),	Branch_Value_2=1	where EmpId in(363,365,575,548,985,847,877,859);
update employees set department=(select Id from Branches where name='ITSD') where EmpId in(363,877);
update employees set 	ControllingAuthority=(select id from employees where EmpId=480),	Department=(select Id from departments where Name='IT Dept-EPD'),	ControllingBranch=(select Id from Branches where Name='HeadOffice'),	ControllingDesignation=(select CurrentDesignation from Employees where EmpId=480),	SanctioningAuthority=(select Id from employees where EmpId=381),	SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),	SanctioningBranch=(select Id from Branches where Name='HeadOffice'),	SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),	Branch_Value1=(select Id from Branches where Name='HeadOffice'),	Branch_Value_2=1	where EmpId in(480,580,984,844,6309);
update employees set 	ControllingAuthority=(select id from employees where EmpId=322),	Department=(select Id from departments where Name='Clearing (incl. CCAC, CTS, RTGS & NEFT)'),	ControllingBranch=(select Id from Branches where Name='HeadOffice'),	ControllingDesignation=(select CurrentDesignation from Employees where EmpId=322),	SanctioningAuthority=(select Id from employees where EmpId=381),	SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),	SanctioningBranch=(select Id from Branches where Name='HeadOffice'),	SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),	Branch_Value1=(select Id from Branches where Name='HeadOffice'),	Branch_Value_2=1	where EmpId in(322,6182,586,822,5910,989,947,783,319,6323,5757,979);
update employees set department=(select Id from Branches where name='HO-CLG') where EmpId=783;
update employees set department=(select Id from Branches where name='Reconciliation Cell') where EmpId=5757;
update employees set department=(select id from Departments where name='TBD') where EmpId=5910;
update employees set 	ControllingAuthority=(select id from employees where EmpId=320),	Department=(select Id from departments where Name='RTGS & NEFT'),	ControllingBranch=(select Id from Branches where Name='HeadOffice'),	ControllingDesignation=(select CurrentDesignation from Employees where EmpId=320),	SanctioningAuthority=(select Id from employees where EmpId=381),	SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),	SanctioningBranch=(select Id from Branches where Name='HeadOffice'),	SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),	Branch_Value1=(select Id from Branches where Name='HeadOffice'),	Branch_Value_2=1	where EmpId in(320,566,782,598,982,6341);
update employees set 	ControllingAuthority=(select id from employees where EmpId=320),	Department=(select Id from departments where Name='Investments-Back Office'),	ControllingBranch=(select Id from Branches where Name='HeadOffice'),	ControllingDesignation=(select CurrentDesignation from Employees where EmpId=320),	SanctioningAuthority=(select Id from employees where EmpId=381),	SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),	SanctioningBranch=(select Id from Branches where Name='HeadOffice'),	SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),	Branch_Value1=(select Id from Branches where Name='HeadOffice'),	Branch_Value_2=1	where EmpId in(320,554,973);
