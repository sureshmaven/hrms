update employees set shift_id=91  where branch=43 and empid=790
update employees set shift_id=91  where branch=43 and empid=842
update employees set shift_id=91  where branch=43 and empid=881
update employees set shift_id=91  where branch=43 and empid=857
update employees set shift_id=91  where branch=43 and empid=914
update employees set shift_id=91  where branch=43 and empid=949
update employees set shift_id=91  where branch=43 and empid=990
update employees set shift_id=91  where branch=43 and empid=999
update employees set shift_id=91  where branch=43 and empid=6333
update employees set shift_id=91  where branch=43 and empid=961
update employees set shift_id=91  where branch=43 and empid=973
update employees set shift_id=91  where branch=43 and empid=6316
update employees set shift_id=91  where branch=43 and empid=6353
update employees set shift_id=91  where branch=43 and empid=6362
update employees set shift_id=10  where branch=10 and empid=868
update employees set shift_id=27  where branch=27 and empid=966
update employees set shift_id=52  where branch=7  and empid=429
update employees set shift_id=13  where branch=13 and empid=6194
update employees set shift_id=26  where branch=26 and empid=6199
update employees set shift_id=68  where branch=23 and empid=490
update employees set shift_id=106 where branch=6  and empid=6331
update employees set shift_id=92  where branch=44 and empid=6335
update employees set shift_id=21  where branch=21 and empid=6350
update employees set shift_id=109 where branch=40 and empid=6387
update employees set shift_id=99  where branch=17 and empid=6398
update employees set shift_id=41  where branch=41 and empid=6414
update employees set shift_id=1   where branch=1  and empid=6442
update employees set shift_id=104 where branch=3  and empid=6467
update employees set shift_id=34  where branch=34 and empid=6468

----Join query for employee shift timings---
select e.empid,e.ShortName,b.Name as Branch,d.name as CurrentDesignation,e.Shift_Id as empshift_id,sm.Id as shift_id,sm.InTime as Shift_StartTime,sm.OutTime as Shift_EndTime from employees as e 
join Branches as b on b.Id=e.Branch
join shift_master as sm on sm.id=e.shift_id
join Designations as d on d.id=e.currentdesignation
 where e.retirementdate>=getdate() order by b.name
 
 
 