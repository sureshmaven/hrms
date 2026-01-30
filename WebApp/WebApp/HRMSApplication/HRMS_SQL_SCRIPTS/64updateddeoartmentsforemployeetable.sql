update employees set department=
			(select id from departments where name='P&D Dept') where empid in (262,
949,460,
993);

update employees set department=
			(select id from departments where name='Clearing (incl. CCAC, CTS, RTGS/NEFT)')
where empid in (
6182,
586,
822,
5910,
989,
947,
783,
319,
6323);


update employees set department=
			(select id from departments where name='HRD-Administration') where empid in (356,
6302,
5908,
861,
914,
926);

update employees set department=
			(select id from departments where name='Premises') where empid in (457,
5768,
5866,
922,
6308);

update employees set department=
			(select id from departments where name='Reconciliation Cell')
where empid in (322,
5757,
979);


update employees set department=
			(select id from departments where name='EPD Dept')
where empid in (480,
580,
984,
844,
6309);


update employees set department=
			(select id from departments where name='IT Dept - EPD')
where empid in (363,
365,
575,
548,
985,
847,
877,
859);


update employees set department=
			(select id from departments where name='Legal, Vigilance & RTI Act')
where empid in (265,
815);