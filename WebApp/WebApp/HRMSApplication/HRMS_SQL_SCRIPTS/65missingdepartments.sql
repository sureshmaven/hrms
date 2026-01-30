insert into departments values('TBD','TBD','TBD','',GETDATE());
update employees set department='48' where empid in (463,204,368,250,5751,5740,5889,6198,532,832);
update employees set department='48' where empid in (356,6302,5908,861,914,926);
delete from EmpLeaveBalance where EmpId is null;
