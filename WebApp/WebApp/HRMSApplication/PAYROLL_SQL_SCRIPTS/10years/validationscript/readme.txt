1. For every new goodwill DB restore from their production/any backup we need to run 14.goodwill_scripts-onetime.sql under validation scripts to create few columns and tables
2. Create indexes listed under Test folder to drop and create the indexes in goodwill db//check the db credentials in runall.bat
3. point to right DB in V1.0 runall.bat
4. run cp to copy the same runall.bat  in all places
5. run step1, step2, mains, step3   r
6. run validation script and validation script values