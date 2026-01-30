update employees set 
	ControllingAuthority=(select id from employees where EmpId=5794),
	Department=(select Id from departments where Name='MD Peshi'),
	ControllingBranch=(select Id from Branches where Name='HeadOffice'),
	ControllingDesignation=(select CurrentDesignation from Employees where EmpId=5794),
	SanctioningAuthority=(select Id from employees where EmpId=381),
	SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
	SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
	SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),
	Branch_Value1=(select Id from Branches where Name='HeadOffice'),
	Branch_Value_2=1
	where EmpId in(998,5794);
	
	update employees set 
	ControllingAuthority=(select id from employees where EmpId=863),
	Department=(select Id from departments where Name='Infrastructure Management (IMISS)'),
	ControllingBranch=(select Id from Branches where Name='HeadOffice'),
	ControllingDesignation=(select CurrentDesignation from Employees where EmpId=863),
	SanctioningAuthority=(select Id from employees where EmpId=381),
	SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
	SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
	SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),
	Branch_Value1=(select Id from Branches where Name='HeadOffice'),
	Branch_Value_2=1
	where EmpId in(863,804);
	
	update employees set 
	ControllingAuthority=(select id from employees where EmpId=364),
	Department=(select Id from departments where Name='PR & Protocal'),
	ControllingBranch=(select Id from Branches where Name='HeadOffice'),
	ControllingDesignation=(select CurrentDesignation from Employees where EmpId=364),
	SanctioningAuthority=(select Id from employees where EmpId=381),
	SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
	SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
	SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),
	Branch_Value1=(select Id from Branches where Name='HeadOffice'),
	Branch_Value_2=1
	where EmpId in(364,994);
	
	
	update employees set 
	ControllingAuthority=(select id from employees where EmpId=976),
	Department=(select Id from departments where Name='L&A-PMU'),
	ControllingBranch=(select Id from Branches where Name='HeadOffice'),
	ControllingDesignation=(select CurrentDesignation from Employees where EmpId=976),
	SanctioningAuthority=(select Id from employees where EmpId=381),
	SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
	SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
	SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),
	Branch_Value1=(select Id from Branches where Name='HeadOffice'),
	Branch_Value_2=1
	where EmpId in(976);
	
	update employees set 
	ControllingAuthority=(select id from employees where EmpId=5882),
	Department=(select Id from departments where Name='President Peshi'),
	ControllingBranch=(select Id from Branches where Name='HeadOffice'),
	ControllingDesignation=(select CurrentDesignation from Employees where EmpId=5882),
	SanctioningAuthority=(select Id from employees where EmpId=381),
	SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
	SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
	SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),
	Branch_Value1=(select Id from Branches where Name='HeadOffice'),
	Branch_Value_2=1
	where EmpId in(976);
	
update  Employees set

ControllingAuthority=(select id from employees where EmpId=147),
ControllingBranch=(select Id from Branches where Name='HeadOffice'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=147),

SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=43,
Branch_Value_2=1

where EmpId in (5660,
358,
5780,
377,
359,
340,
555,
976,
265,
364,
176,
356,
790,
376,
457,
287,
5770,
262,
363,
863,
480,
322,
320,
5794,
5882);




