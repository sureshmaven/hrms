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

where EmpId in(846,564,338,983,563,5920,931,452,6191,337,121,361,373,565,547,357,6333,789,744,476,478,788,708,557,1998,590,6209,485,838,5809,981,721,331,6184,574,749,837,845,935,800)