drop table tenp;
select * into tenp from employees ;
update employees set

       employees.Password=tenp.Password
      ,employees.Gender=tenp.Gender
      ,employees.MartialStatus=tenp.MartialStatus
      ,employees.PersonalEmailId=tenp.PersonalEmailId
      ,employees.FatherName='TBD'
      ,employees.MotherName='TBD'
      ,employees.MobileNumber=tenp.MobileNumber
      ,employees.HomeNumber=tenp.HomeNumber
      ,employees.PresentAddress='TBD'
      ,employees.PermanentAddress='TBD'
      ,employees.Graduation='TBD'
      ,employees.PostGraduation='TBD'
      ,employees.EmergencyName='TBD'
      ,employees.EmergencyContactNo=tenp.EmergencyContactNo
      ,employees.Category=tenp.Category
      ,employees.OfficalEmailId='TBD@TSCAB.COM'
      ,employees.TotalExperience=tenp.TotalExperience
      ,employees.RelievingDate=tenp.RelievingDate
      ,employees.ControllingAuthority=tenp.ControllingAuthority
      ,employees.SanctioningAuthority=tenp.SanctioningAuthority
      ,employees.UploadPhoto=tenp.UploadPhoto
      ,employees.UpdatedBy=tenp.UpdatedBy
      ,employees.UpdatedDate=tenp.UpdatedDate
      ,employees.RelievingReason=tenp.RelievingReason
      ,employees.SpouseName='TBD'
      ,employees.BloodGroup=tenp.BloodGroup
      ,employees.AadharCardNo=tenp.AadharCardNo
      ,employees.PanCardNo='TBD'
      ,employees.ProfessionalQualifications='TBD'
      ,employees.LoginMode=tenp.LoginMode
      ,employees.ControllingDepartment=tenp.ControllingDepartment
      ,employees.ControllingBranch=tenp.ControllingBranch
      ,employees.ControllingDesignation=tenp.ControllingDesignation
      ,employees.SanctioningDepartment=tenp.SanctioningDepartment
      ,employees.SanctioningBranch=tenp.SanctioningBranch
      ,employees.SanctioningDesignation=tenp.SanctioningDesignation
      ,employees.Branch_Value1=tenp.Branch_Value1
      ,employees.Branch_Value_2	=tenp.Branch_Value_2 
	  from tenp
	  WHERE tenp.EmpId=381;
	  
update employees set PanCardNo='AAAAAAAAAA';

update employees set employees.FirstName=tenp.LastName,employees.LastName=tenp.FirstName
from tenp where employees.id=tenp.id;

update employees set SpouseName='';

	  
update employees set  Gender='Female' where EmpId in (462,271,460,463,147,266,359,485,5809,361,287,340,373,478,377,331,5920,262,6184,6191,580,575,788,721,789,6190,835,838,841,836,846,943,945,947,983,6302,956,991,963,966,984,990,6319,999,6318,6342,827,790,749,714,775,733,549,730,559,783,599,725,830,763,556,572,796,6132,293,5908,505,899,848,865,842,889,902,867,903,884,893,862,860,881,909,885,891,854,905,892,890,855,861,857,858,898,874,914,920,918,928,933,934,936,938,939,513,445,944,948,949,970,958,962,975,6316,6312,977,978,973,996,964,979,6309,976,6307,6324,995,972,6329,6323,6310,997,6340,6335,6337,250,6198,490,509,510,511,523,543,832)
drop table tenp;