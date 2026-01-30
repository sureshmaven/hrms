update employees set DOB='1950-01-01' where DOB='' or dob is null;
update employees set PersonalEmailId='tpb@gmail.com' where PersonalEmailId	='' or 	PersonalEmailId	is null;
update employees set FatherName='TBD' where FatherName='' or FatherName is null;
update employees set MotherName='TBD' where MotherName='' or MotherName	is null;
update employees set TotalExperience='3years' where TotalExperience='' or TotalExperience	is null;
update employees set JoinedDesignation='24' where JoinedDesignation='' or JoinedDesignation is null;
update employees set CurrentDesignation='24' where CurrentDesignation='' or CurrentDesignation is null;
update employees set DOJ='1990-01-01' where doj='' or doj is null;
update employees set RelievingDate='2100-01-01' where RelievingDate='' or RelievingDate is null;
update employees set RetirementDate='2100-01-01' where RetirementDate='' or RetirementDate is null;
update employees set ControllingAuthority=(select id from employees where empid=147) where ControllingAuthority='' or ControllingAuthority is null;
update employees set FirstName='TBD' where FirstName='' or FirstName is null;
update employees set LastName='TBD' where LastName='' or LastName is null;
update employees set UpdatedDate='2018-01-18' where UpdatedDate='' or UpdatedDate is null;
update employees set BloodGroup	='O+' where BloodGroup='' or BloodGroup is null;
update employees set PanCardNo='AAAAAAAAAA' where PanCardNo='' or PanCardNo is null;
update employees set AadharCardNo='123456789012' where AadharCardNo='' or AadharCardNo is null;
update employees set EmergencyName='TBD' where EmergencyName='' or EmergencyName is null;
update employees set ControllingDepartment=(select id from departments where name='TBD') where ControllingDepartment='' or ControllingDepartment is null;
update employees set ControllingBranch=(select id from branches where name='TBD') where ControllingBranch='' or ControllingBranch is null;
update employees set ControllingDesignation=(select id from designations where name='TBD') where ControllingDesignation='' or ControllingDesignation is null;
update employees set SanctioningDepartment=(select id from departments where name='TBD') where SanctioningDepartment='' or SanctioningDepartment is null;
update employees set SanctioningBranch=(select id from branches where name='TBD') where SanctioningBranch='' or SanctioningBranch is null;
update employees set SanctioningDesignation=(select id from designations where name='TBD') where SanctioningDesignation='' or SanctioningDesignation is null;
update employees set SpouseName='TBD' where SpouseName='' or SpouseName is null;




