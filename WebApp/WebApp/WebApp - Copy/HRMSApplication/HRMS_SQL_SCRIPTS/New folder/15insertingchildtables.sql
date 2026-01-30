update Employees set Branch=(select Id from Branches where Name='Headoffice') where EmpId in (777,785,822,554,575,566,817,584,779,782,790,804,572,598,815,876,864,841,847,877,878,902,896,856,859,913,844,955,956,982,989,985,984,991,6317,6325,6334,947,176,319,340,358,363,364,365,371,376,377,381,438,440,457,460,474,480,492,510,511,523,548,539,5909,940,6193,5882,5751,5660,6198,5757,6182,5161,6180,5770,5884,5768,5866,5794,1862,6196,5908,1930,5796,832,374,383,405,287,5910,433,505,5824,178,422,308,250,5740,535,544,831,525,249,512,5862,443,5869,5890,436,439,950,833,962,973,976,977,979,993,998,994,6309,6306,6308,6313,6310,6314,6323,6326,6331,6336,6337,6338,6341,6344,6345,6351,6352,6353,6346,6350,6348,6355,6358,6359,6361,6366,6367,6368,6363,6365,6370,6372,534);
update Employees set Branch_Value1=(select Id from Branches where Name='Headoffice') where EmpId in (822,554,575,566,584,779,782,790,804,598,847,877,859,844,982,989,985,984,947,319,363,364,365,371,381,440,460,474,480,492,510,511,523,548,539,5909,940,6193,5882,5751,6198,5757,6182,5161,5770,5884,1862,6196,5908,1930,5796,832,374,383,405,5910,505,5824,178,422,308,250,5740,535,544,831,525,249,512,5862,443,5869,5890,436,439,950,833,962,973,976,977,979,993,994,6309,6341,6344,6345,6351,6352,6353,6346,6350,6348,6355,6358,6359,6361,6366,6367,6368,6363,6365,6370,6372,534);
update Employees Set Department=(Select Id from Departments where Name='TBD') where EmpId in (785,822,554,575,566,817,548,779,782,793,804,572,598,864,841,847,877,842,902,872,912,859,844,929,942,934,943,982,989,985,984,991,6305,6325,6332,947,147,319,340,358,363,364,365,371,376,381,438,480,510,527,532,547,548,5882,5751,6198,5757,6182,5161,5770,5908,1930,5889,374,368,463,359,320,322,262,318,5910,505,5824,250,525,443,436,439,973,976,979,993,994,6309,6314,6323,6326,6336,6341,6344,6345,6354,6351,6352,6353,6346,6350,6347,6348,6349,6357,6355,6356,6358,6359,6360,6361,6366,6362,6367,6368,6363,6364,6365,6370,6372,534);
UPDATE Employees Set Department=(Select Id from Departments where Name='L&A-PMU') where EmpId=5751UPDATE Employees Set Department=(Select Id from Departments where Name='Clearing (incl. CCAC, CTS, RTGS & NEFT)') where EmpId=6198UPDATE Employees Set Department=(Select Id from Departments where Name='Reconciliation Cell') where EmpId=5757UPDATE Employees Set Department=(Select Id from Departments where Name='Clearing (incl. CCAC, CTS, RTGS & NEFT)') where EmpId=6182UPDATE Employees Set Department=(Select Id from Departments where Name='Premises') where EmpId=5161UPDATE Employees Set Department=(Select Id from Departments where Name='Inward-Outward') where EmpId=5770UPDATE Employees Set Department=(Select Id from Departments where Name='HRD LEAVE') where EmpId=5908UPDATE Employees Set Department=(Select Id from Departments where Name='P&D Dept') where EmpId=993UPDATE Employees Set Department=(Select Id from Departments where Name='IT Dept-EPD') where EmpId=6309UPDATE Employees Set Department=(Select Id from Departments where Name='Clearing (incl. CCAC, CTS, RTGS & NEFT)') where EmpId=6323;

insert into leavetypes values('Casual Leave','Casual Leave','',GETDATE(),'CL'),
('Medical/Sick Leave','Medical/Sick Leave','',GETDATE(),'ML'),
('Privilege Leave','Privilege Leave','',GETDATE(),'PL'),
('Maternity Leave','Maternity Leave','',GETDATE(),'MTL'),
('Paternity Leave','Paternity Leave','',GETDATE(),'PTL'),
('Extraordinary Leave','Extraordinary Leave','',GETDATE(),'EOL'),
('Special Casual Leave','Special Casual Leave','',GETDATE(),'SCL');


insert into holidaylist values ('Sankranti/Pongal ','2018-01-15','',GETDATE(),''),
('Republic Day','2018-01-26','',GETDATE(),''),
('Maha Shivaratri ','2018-02-13','',GETDATE(),''),
('Holi ','2018-03-01','',GETDATE(),''),
('Sri Rama Navami','2018-03-26','',GETDATE(),''),
('Good Friday  ','2018-03-30','',GETDATE(),''),
('Babu Jagjivan Ram’s Birthday ','2018-04-05','',GETDATE(),''),
('May Day ','2018-05-01','',GETDATE(),''),
('Eid-Ul-Fitar (Ramzan)','2018-06-16','',GETDATE(),''),
('Independence Day','2018-08-15','',GETDATE(),''),
('Eidul Azha (Bakrid)','2018-08-22','',GETDATE(),''),
('Sri Krishna Astami  ','2018-09-03','',GETDATE(),''),
('Vinayaka Chavithi','2018-09-13','',GETDATE(),''),
('Shahadat Imam Hussain (A.S) 10th Moharam, 1493 Hijri  ','2018-09-21','',GETDATE(),''),
('Mahtma Gandhi Jayanthi','2018-10-02','',GETDATE(),''),
('Durgastami','2018-10-17','',GETDATE(),''),
('Vijaya Dasami ','2018-10-18','',GETDATE(),''),
('Deepavali','2018-11-07','',GETDATE(),''),
('Eid Miladun Nabi','2018-11-21','',GETDATE(),''),
('Kartika Purnima / Guru Nanak’s Birthday ','2018-11-23','',GETDATE(),''),
('Christmas ','2018-12-25','',GETDATE(),'');




