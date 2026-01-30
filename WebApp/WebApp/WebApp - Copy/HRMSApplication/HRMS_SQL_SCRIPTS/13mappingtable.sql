insert into Branch_Designation_Mapping select a.Id,b.Id from dbo.Designations b, dbo.Branches a where (
a.name in ('HeadOffice') and b.name in ('Managing Director','Chief General manager',
'General Manager','Deputy General Manager','Assistant General manager',
'Senior Manager','Manager Scale-1','Staff Assistant',
'Subordinate Staff') )or ( a.name  in ('OtherBranch') and b.name  in (
'Assistant General manager','Senior Manager',
'Manager Scale-1','Staff Assistant','Subordinate Staff') )order by a.Id;
