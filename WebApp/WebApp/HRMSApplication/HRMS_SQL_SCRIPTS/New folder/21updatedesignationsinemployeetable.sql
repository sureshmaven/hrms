update employees set Branch=(select Id from Branches where name='HO Branch'),
ControllingAuthority=(select Id from employees where EmpId=6177),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='HO Branch'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=6177),

SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2
 where EmpId in(6177,5150,6319,299,814,827,840,858,6321,428,421,6194,432,495,6311,293);
update employees set Branch=(select id from branches where name='HeadOffice') where EmpId in (5150,6319,299,814,827,858,428,421,6194,6311,293);

/* update employees set Branch=(select Id from Branches where name='Stationery'),
ControllingAuthority=(select Id from employees where EmpId=5660),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='Stationery'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=5660),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2
 where EmpId in(5660,6313);
 */

update employees set Branch=(select Id from Branches where name='ALWAL'),
ControllingAuthority=(select Id from employees where EmpId=846),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='ALWAL'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=846),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2
 where EmpId in(846,959,887,995,915,240);
 
 update employees set Branch=(select Id from Branches where name='AMBERPET'),
ControllingAuthority=(select Id from employees where EmpId=564),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='AMBERPET'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=564),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2
 where EmpId in(564,988,6173,860,490);

update employees set Branch=(select Id from Branches where name='AMEERPET'),
ControllingAuthority=(select Id from employees where EmpId=338),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='AMEERPET'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=338),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2
 where EmpId in(338,971,760,874,944,964,891);
update employees set Branch=(select Id from Branches where name='HO Branch') where EmpId=891;
update employees set Branch=(select Id from Branches where name='ATTAPUR'),
ControllingAuthority=(select Id from employees where EmpId=983),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='ATTAPUR'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=983),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2
 where EmpId in(983,763,968,6343,304);
 
 update employees set Branch=(select Id from Branches where name='BAGHLINGAMPALLY'),
ControllingAuthority=(select Id from employees where EmpId=563),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='BAGHLINGAMPALLY'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=563),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2
 where EmpId in(563,550,986,2005,890,6329,6340);
 
 
 
 update employees set Branch=(select Id from Branches where name='BANDLAGUDA'),
ControllingAuthority=(select Id from employees where EmpId=5920),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='BANDLAGUDA'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=5920),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2
 where EmpId in (5920,835,6132,849,972,288);
update employees set Branch=(select Id from Branches where name='TBD') where EmpId=5920;
 update employees set Branch=(select Id from Branches where name='BHARATNAGAR'),
ControllingAuthority=(select Id from employees where EmpId=931),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='BHARATNAGAR'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=931),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2
 where EmpId in (931,775,938,946,969);
 
 
update employees set Branch=(select Id from Branches where name='BOUDHANAGAR'),
ControllingAuthority=(select Id from employees where EmpId=452),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='BOUDHANAGAR'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=452),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2
 where EmpId in (452,5738,425,906,6327,853);
 update employees set Branch=(select Id from Branches where name='TBD') where EmpId=452;
 update employees set Branch=(select Id from Branches where name='CHAMPAPET'),
ControllingAuthority=(select Id from employees where EmpId=6191),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='CHAMPAPET'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=6191),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2
 where EmpId in (6191,836,786,854,881,992,530);
 update employees set Branch=(select Id from Branches where name='HO Branch') where EmpId=836;
  update employees set Branch=(select Id from Branches where name='MALAKPET') where EmpId=881;
  update employees set Branch=(select Id from Branches where name='CHANDANAGAR'),
ControllingAuthority=(select Id from employees where EmpId=337),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='CHANDANAGAR'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=337),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2
 where EmpId in (337,556,6324,892,5785);
 
 
 update employees set Branch=(select Id from Branches where name='CHARMINAR'),
ControllingAuthority=(select Id from employees where EmpId=121),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='CHARMINAR'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=121),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2
 where EmpId in (121,6303,5741,5781,930,961,5782);
 
 update employees set Branch=(select Id from Branches where name='DILSUKHNAGAR'),
ControllingAuthority=(select Id from employees where EmpId=361),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='DILSUKHNAGAR'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=361),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2
 where EmpId in (361,952,599,868,898,177,303);


update employees set Branch=(select Id from Branches where name='GACHIBOWLI'),
ControllingAuthority=(select Id from employees where EmpId=373),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='GACHIBOWLI'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=373),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2
 where EmpId in (373,6301,953,5802);

update employees set Branch=(select Id from Branches where name='HIMAYATNAGAR'),
ControllingAuthority=(select Id from employees where EmpId=565),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='HIMAYATNAGAR'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=565),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2
 where EmpId in (565,830,434,865,517,239);

update employees set Branch=(select Id from Branches where name='JUBILEE HILLS'),
ControllingAuthority=(select Id from employees where EmpId=547),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='JUBILEE HILLS'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=547),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2
 where EmpId in (547,957,960,939,842,527,871);

update employees set Branch=(select Id from Branches where name='KAMALANAGAR'),
ControllingAuthority=(select Id from employees where EmpId=357),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='KAMALANAGAR'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=357),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2
 where EmpId in (357,954,730,857,889,980,516,5743);
update employees set Branch=(select Id from Branches where name='TBD') where EmpId=357;
update employees set Branch=(select Id from Branches where name='AMBERPET') where EmpId=730;
update employees set Branch=(select Id from Branches where name='KUKATPALLY'),
ControllingAuthority=(select Id from employees where EmpId=6333),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='KUKATPALLY'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=6333),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2
 where EmpId in (6333,581,958,893,508);

update employees set Branch=(select Id from Branches where name='LALAPET'),
ControllingAuthority=(select Id from employees where EmpId=789),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='LALAPET'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=789),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2
 where EmpId  in(789,987,427,936,520);


update employees set Branch=(select Id from Branches where name='LOTHKUNTA'),
ControllingAuthority=(select Id from employees where EmpId=744),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='LOTHKUNTA'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=744),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2
 where EmpId  in(744,6318,5746,884,967,6328,8053);

update employees set Branch=(select Id from Branches where name='MALAKPET'),
ControllingAuthority=(select Id from employees where EmpId=476),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='MALAKPET'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=476),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2
 where EmpId in(476,963,526,905,445,928);

update employees set Branch=(select Id from Branches where name='MALKAJGIRI'),
ControllingAuthority=(select Id from employees where EmpId=478),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='MALKAJGIRI'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=478),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2 where EmpId in (478,999,866,514,975,486);
update employees set Branch=(select Id from Branches where name='LOTHKUNTA') where EmpId=866;

update employees set Branch=(select Id from Branches where name='MARUTINAGAR'),
ControllingAuthority=(select Id from employees where EmpId=788),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='MARUTINAGAR'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=788),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2 where EmpId in (788,723,6339,909,885,493);

update employees set Branch=(select Id from Branches where name='MASABTANK'),
ControllingAuthority=(select Id from employees where EmpId=708),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='MASABTANK'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=708),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2 where EmpId in (708,965,570,911,17,970);
update employees set Branch=(select Id from Branches where name='HeadOffice') where EmpId=965;
update employees set Branch=(select Id from Branches where name='MOULALI'),
ControllingAuthority=(select Id from employees where EmpId=557),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='MOULALI'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=557),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2 where EmpId in (557,945,549,921,918,941,6307);
update employees set Branch=(select Id from Branches where name='SECUNDERABAD') where EmpId=557;
update employees set Branch=(select Id from Branches where name='NARAYANAGUDA'),
ControllingAuthority=(select Id from employees where EmpId=1998),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='NARAYANAGUDA'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=1998),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2 where EmpId in (1998,801,513,528,997,5148);

update employees set Branch=(select Id from Branches where name='UPPAL'),
ControllingAuthority=(select Id from employees where EmpId=590),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='UPPAL'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=590),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2
where EmpId in(590,5750,6192,910,867,509);


update employees set Branch=(select Id from Branches where name='SAIDABAD COLONY'),
ControllingAuthority=(select Id from employees where EmpId=6209),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='SAIDABAD COLONY'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=6209),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2
where EmpId in(6209,507,990,292,951,848,521,6312);

update employees set Branch=(select Id from Branches where name='SECUNDERABAD'),
ControllingAuthority=(select Id from employees where EmpId=485),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='SECUNDERABAD'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=485),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2
where EmpId in(485,974,805,6195,899,862,184,560);
update employees set Branch=(select Id from Branches where name='VENGALRAONAGAR') where EmpId=485;
update employees set Branch=(select Id from Branches where name='SAROORNAGAR') where EmpId=184;
update employees set Branch=(select Id from Branches where name='SERILINGAMPALLY'),
ControllingAuthority=(select Id from employees where EmpId=838),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='SERILINGAMPALLY'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=838),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2
where EmpId in(838,6300,903,6335,435);


update employees set Branch=(select Id from Branches where name='TARNAKA'),
ControllingAuthority=(select Id from employees where EmpId=5809),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='TARNAKA'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=5809),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2
where EmpId in(5809,6304,733,468,933,538,545,429);

update employees set Branch=(select Id from Branches where name='TOLICHOWKI'),
ControllingAuthority=(select Id from employees where EmpId=981),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='TOLICHOWKI'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=981),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2
where EmpId in(981,731,901,920,996,834);

update employees set Branch=(select Id from Branches where name='VENGALRAONAGAR'),
ControllingAuthority=(select Id from employees where EmpId=721),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='VENGALRAONAGAR'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=721),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2
where EmpId in(721,6190,709,895,916,948,444);

update employees set Branch=(select Id from Branches where name='VIDYANAGAR'),
ControllingAuthority=(select Id from employees where EmpId=331),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='VIDYANAGAR'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=331),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2
where EmpId in(331,420,702,937,519,6315,175,5892);

update employees set Branch=(select Id from Branches where name='VIDYUTHSOUDHA'),
ControllingAuthority=(select Id from employees where EmpId=6184),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='VIDYUTHSOUDHA'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=6184),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2
where EmpId in(6184,966,5888,924,6330,254);
update employees set Branch=(select Id from Branches where name='TBD') where EmpId=6184;
update employees set Branch=(select Id from Branches where name='NEREDMET'),
ControllingAuthority=(select Id from employees where EmpId=574),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='NEREDMET'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=574),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2
where EmpId in(574,6320,543);


update employees set Branch=(select Id from Branches where name='YOUSUFGUDA'),
ControllingAuthority=(select Id from employees where EmpId=749),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='YOUSUFGUDA'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=749),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2
where EmpId in(749,6322,484);

update employees set Branch=(select Id from Branches where name='SUCHITRA'),
ControllingAuthority=(select Id from employees where EmpId=837),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='SUCHITRA'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=837),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2
where EmpId in(837,6316,932,536);


update employees set Branch=(select Id from Branches where name='SAROORNAGAR'),
ControllingAuthority=(select Id from employees where EmpId=845),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='SAROORNAGAR'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=845),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2
where EmpId in(845,917,233);

update employees set Branch=(select Id from Branches where name='BADANGPET'),
ControllingAuthority=(select Id from employees where EmpId=935),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='BADANGPET'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=935),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2
where EmpId in(935,925,6199,978);
update employees set Branch=(select Id from Branches where name='TBD') where EmpId=978;

update employees set Branch=(select Id from Branches where name='MEERPET'),
ControllingAuthority=(select Id from employees where EmpId=800),
department=(select Id from departments where Code='OtherDepartment'),
ControllingBranch=(select Id from Branches where Name='MEERPET'),
ControllingDesignation=(select CurrentDesignation from Employees where EmpId=800),
SanctioningAuthority=(select Id from employees where EmpId=381),
SanctioningDepartment=(select Id from departments where Name='HRD-Administration'),
SanctioningBranch=(select Id from Branches where Name='HeadOffice'),
SanctioningDesignation=(select CurrentDesignation from Employees where EmpId=381),

Branch_Value1=(select Id from Branches where Name='OtherBranch'),
Branch_Value_2=2
where EmpId in(800,855,518);
