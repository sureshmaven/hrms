drop table dm_emp_mn_task1 ;
drop table dm_emp_mn_task2 ;
drop table dm_emp_mn_task3 ;

select * into dm_emp_mn_task1 from dm_emp_mn_task;
select * into dm_emp_mn_task2 from dm_emp_mn_task;
select * into dm_emp_mn_task3 from dm_emp_mn_task;
 -- delete from dm_emp_mn_task1 where emp_code=452 and mn='202007' and task='HOUS1,'
 -- delete from dm_emp_mn_task2 where emp_code=452 and mn='202007' and task='HOUS1,'
 -- delete from dm_emp_mn_task3 where emp_code=452 and mn='202007' and task='HOUS1,'