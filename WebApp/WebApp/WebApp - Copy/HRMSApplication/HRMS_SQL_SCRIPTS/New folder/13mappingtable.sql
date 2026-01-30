insert into Branch_Designation_Mapping select a.Id,b.Id from dbo.Designations b, dbo.Branches a where (
a.name in ('HeadOffice') and b.name in ('Managing Director','Chief General manager',
'General Manager','Deputy General Manager','Assistant General manager',
'Senior Manager','Manager Scale-1','Staff Assistant','Attender',
'Subordinate Staff',
'Attender Cum Watchman','Attender/J.C','Civil Engg Supervisor','Driver','IDO/Manager',
'JR Staff Assistant','Junior Clerk','Staff Assistant Cum Assistant Cashier','Stenographer',
'Telephone Operator Cum Receptionist','Typist','Watchman','Subordinate Staff(Substaff)','President','Manager','Manager Tech') )
or ( a.name  in ('OtherBranch') and b.name  in (
'Assistant General manager',
'Senior Manager','Manager Scale-1','Staff Assistant','Attender',
'Subordinate Staff',
'Attender Cum Watchman','Attender/J.C','Civil Engg Supervisor','Driver','IDO/Manager',
'JR Staff Assistant','Junior Clerk','Staff Assistant Cum Assistant Cashier','Stenographer',
'Telephone Operator Cum Receptionist','Typist','Watchman','Subordinate Staff(Substaff)','Manager','Manager Tech') )order by a.Id;

select * from Branch_Designation_Mapping