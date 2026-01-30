update Employees SET SanctioningAuthority=(select id from Employees where LastName = 'D NAGA' and EmpId = 381) where EmpId in (725,1930,1862,178,250,249,5751,5740,301,302,308,5862,5796,5869,5884,5889,5909,422,440,6193,6196,6198,474,491,492,499,5890,510,511,512,523,532,535,539,544,831,832,
833,940,950);

update Employees SET SanctioningAuthority=(select id from Employees where FirstName = 'MURALIDHAR' and EmpId = 686) Where EmpId in (462,354,405,271,463,368,383);

update Employees SET SanctioningAuthority=(select id from Employees where FirstName = 'Bala' and EmpId = 271) Where EmpId in (460,204,266,371,279,377);

update Employees SET SanctioningAuthority=(select id from Employees where lastName='RAVINDER RAO') Where EmpId = 686;