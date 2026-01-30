delete from ple_type where payslip_mid is not null and fy is not null and fm is not null;
delete from employee_transfer where senoirity_order is not null and authorisation is not null and active is not null;
drop table dm_emp_mn_task1
drop table dm_emp_mn_task2
drop table dm_emp_mn_task3
drop table dm_emp_mn_task4
--delete from designations where code in ('SysAdmin','JMDR','DFTR');
delete from userroles where pages in ('Payslip');
delete from departments where updatedby=123456 and active=0;

delete from designations where id=32
delete from designations where id=33
delete from designations where id=34