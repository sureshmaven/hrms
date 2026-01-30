exec sp_dm_retd_employees
update employees set branch=42 where branch is null
update employees set currentdesignation=24 where currentdesignation is null
update employees set joineddesignation=24 where joineddesignation is null

update employees set retirementdate='2015-04-01 00:00:00:00' where empid in (185
,244
,256
,260
,290
,317
,336
,341
,344
,349
,372
,375
,379
,423
,424
,426
,430
,431
,446
,447
,451
,458
,459
,464
,488
,496
,497
,498
,5156
,5747
,5753
,5754
,5755
,5759
,5760
,5764
,5765
,5766
,5773
,5778
,5797
,5801
,5807
,5822
,5864
,5865
,5871
,5891
,5900
,5993
,6124
,6131
,6140
,6171
,6181
,6197
,6203
,753
,769
,923);
