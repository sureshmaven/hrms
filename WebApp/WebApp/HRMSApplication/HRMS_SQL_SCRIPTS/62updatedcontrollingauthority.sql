

update employees set ControllingAuthority=8 where id=165;
update employees set ControllingAuthority=14 where id=14;
update employees set ControllingAuthority=4 where id=8;
update employees set ControllingAuthority=14 where controllingauthority='';

update employees set category='OC' where category='General';
update Departments set code='OtherDepartment',Name='OtherDepartment',Description='OtherDepartment' where id=47;
--update employees set department=
			--(select id from departments where name='OtherDepartment') where department is null;