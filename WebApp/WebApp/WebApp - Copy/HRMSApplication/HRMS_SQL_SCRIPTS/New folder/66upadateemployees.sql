update employees set Department=(select id from departments where name='IT Dept') where empid=463;	

update employees set Department=(select id from departments where name='L&A-PMU') where empid=204;

update employees set Department=(select id from departments where name='IT Dept - EPD') where empid=368;

update employees set Department=(select id from departments where name='Legal, Vigilance & RTI Act') where empid=371;

update employees set Department=(select id from departments where name='P&D Dept') where empid=832 ;


