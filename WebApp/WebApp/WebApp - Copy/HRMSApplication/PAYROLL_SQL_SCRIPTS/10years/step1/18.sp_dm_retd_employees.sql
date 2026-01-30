create procedure sp_dm_retd_employees
as 

select * into test2.dbo.pempmastold_temp from test2.dbo.pempmastold
select * into test2.dbo.pempmast_temp from test2.dbo.pempmast

update test2.dbo.pempmastold_temp set Bloodgroup='14' where Bloodgroup is null
 update test2.dbo.pempmastold_temp set Dept='345' where Dept is null
 update test2.dbo.pempmastold_temp set branch='75' where branch is null
 
 update test2.dbo.pempmastold_temp set dept=400 where dept not in (select winid from test2.dbo.pwinfile)
update test2.dbo.pempmastold_temp set desgn=401 where desgn not in (select winid from test2.dbo.pwinfile)
update test2.dbo.pempmastold_temp set bloodgroup=402 where bloodgroup not in (select winid from test2.dbo.pwinfile)
update test2.dbo.pempmastold_temp set branch=403 where branch not in (select winid from test2.dbo.pwinfile)



select Empname,
 Empname as e1,
'pP2TLUp8ksrkrTSP24c+xw=='	as password,
w1.winvalue as sex	,
case when w2.winvalue='Unmarried' then 'Single' else w2.winvalue end as marital_status	,
Dob	,
EMail	as mail,
Fname	,
	'' as mother,
PPhoneNo as phone1	,
PPhoneNo	as phone2,	
 concat(concat( Add1,' ', add2),' ', add3)	as addresss,
concat(concat( pAdd1,' ', padd2),' ', padd3)	as paddress	,
	'' as graduation,
	'' as post_graduation,
EmpShName	as emergency_contact_name,
PhoneNo	as emergency_contact_number,
	'' as cat,
Empid	,
 case when w3.winvalue='ALWAL' then	1
 when w3.winvalue='AMBERPET' then	2
 when w3.winvalue='AMEERPET' then	3
 when w3.winvalue='ATTAPUR' then	4
 when w3.winvalue='BADANGPET' then	5
 when w3.winvalue='BAGHLIMGAMPALLY' then	6
 when w3.winvalue='BANDLAGUDA' then	7
 when w3.winvalue='BHARAT NAGAR' then	8
 when w3.winvalue='BOUDHANAGAR' then	9
 when w3.winvalue='CHAMPAPET' then	10
 when w3.winvalue='CHANDANAGAR' then	11
 when w3.winvalue='CHARMINAR' then	12
 when w3.winvalue='DILSUKHNAGAR' then	13
 when w3.winvalue='GACHIBOWLI' then	14
 when w3.winvalue='HIMAYATNAGAR' then	15
 when w3.winvalue='JUBILEEHILLS' then	16
 when w3.winvalue='KAMALANAGAR' then	17
 when w3.winvalue='KUKATPALLY' then	18
 when w3.winvalue='LOTHUKUNTA' then	20
 when w3.winvalue='MALAKPET' then	21
 when w3.winvalue='MALKAJGIRI' then	22
 when w3.winvalue='MARUTHINAGAR' then	23
 when w3.winvalue='MASABTANK' then	24
 when w3.winvalue='MEERPET' then	25
 when w3.winvalue='MOULALI' then	26
 when w3.winvalue='NARAYANAGUDA' then	27
 when w3.winvalue='NEREDMET' then	28
 when w3.winvalue='SAIDABAD' then	29
 when w3.winvalue='SAROORNAGAR' then	30
 when w3.winvalue='SECUNDERABAD' then	31
 when w3.winvalue='SERILINGAMPALLY' then	32
 when w3.winvalue='SUCHITRA' then	33
 when w3.winvalue='TARNAKA' then	34
 when w3.winvalue='TARNAKA EXTN.COUNTER' then	34
 when w3.winvalue='TOLICHOWKI' then	35
 when w3.winvalue='UPPAL' then	37
 when w3.winvalue='VENGALRAONAGAR' then	38
 when w3.winvalue='VIDYA NAGAR' then	39
 when w3.winvalue='VIDYUTHSOUDHA' then	40
 when w3.winvalue='YOUSUFGUDA' then	41
 when w3.winvalue='HEAD OFFICE' then	43
 when w3.winvalue='MANSOORABAD' then	48
	end as join_branch,
 case when w4.winvalue like 'CHIEF GENERAL MANAGER.' then	2
 when w4.winvalue like 'AGM%' then	5
 when w4.winvalue like 'ASST.GEN.MANAGER%' then	5
 when w4.winvalue like 'ATDR' then	9
 when w4.winvalue like 'ATTENDER.' then	9
 when w4.winvalue like 'B.M.' then	25
 when w4.winvalue like 'DAFTARI..' then	34
 when w4.winvalue like 'DFTR' then	37
 when w4.winvalue like 'DGM' then	4
 when w4.winvalue like 'DRVR' then	13
 when w4.winvalue like 'DY.GENERAL  MANAGER.' then	4
 when w4.winvalue like 'ELECT' then	16
 when w4.winvalue like 'GENERAL  MANAGER.' then	3
 when w4.winvalue like 'GM' then	3
 when w4.winvalue like 'JAMEDAR' then	36
 when w4.winvalue like 'JMDR' then	36
 when w4.winvalue like 'JSA' then	15
 when w4.winvalue like 'MANAGING  DIRECTOR..' then	1
 when w4.winvalue like 'MANAGING DIRECTOR.' then	1
 when w4.winvalue like 'MD' then	1
 when w4.winvalue like 'MGR SC 1' then	7
 when w4.winvalue like 'MGR.SC 1' then	7
 when w4.winvalue like 'MGRD.' then	7
 when w4.winvalue like 'SA' then	8
 when w4.winvalue like 'SA-O' then	8
 when w4.winvalue like 'SA.' then	8
 when w4.winvalue like 'SR.MGR' then	6
 when w4.winvalue like 'SR.MGR-EN' then	6
 when w4.winvalue like 'SR.MGR-ENG' then	6
 when w4.winvalue like 'STAFF  ASSISTANT' then	8
 when w4.winvalue like 'STENO' then	18
 when w4.winvalue like 'SYS.ADMINISTRATOR' then	29
 when w4.winvalue like 'SYS.ADMN' then	32
 when w4.winvalue like 'TYP' then	20
 when w4.winvalue like 'WATCH' then	21 
 when w4.winvalue='z-DAFTARI.----' then	34
 when w4.winvalue='z-MD .' then	1
 when w4.winvalue='z-DAFT.--------' then	34 end as joined_Desgn,
case when w4.winvalue like 'CHIEF GENERAL MANAGER.' then	2
 when w4.winvalue like 'AGM%' then	5
 when w4.winvalue like 'ASST.GEN.MANAGER%' then	5
 when w4.winvalue like 'ATDR' then	9
 when w4.winvalue like 'ATTENDER.' then	9
 when w4.winvalue like 'B.M.' then	25
 when w4.winvalue like 'DAFTARI..' then	34
 when w4.winvalue like 'DFTR' then	37
 when w4.winvalue like 'DGM' then	4
 when w4.winvalue like 'DRVR' then	13
 when w4.winvalue like 'DY.GENERAL  MANAGER.' then	4
 when w4.winvalue like 'ELECT' then	16
 when w4.winvalue like 'GENERAL  MANAGER.' then	3
 when w4.winvalue like 'GM' then	3
 when w4.winvalue like 'JAMEDAR' then	36
 when w4.winvalue like 'JMDR' then	36
 when w4.winvalue like 'JSA' then	15
 when w4.winvalue like 'MANAGING  DIRECTOR..' then	1
 when w4.winvalue like 'MANAGING DIRECTOR.' then	1
 when w4.winvalue like 'MD' then	1
 when w4.winvalue like 'MGR SC 1' then	7
 when w4.winvalue like 'MGR.SC 1' then	7
 when w4.winvalue like 'MGRD.' then	7
 when w4.winvalue like 'SA' then	8
 when w4.winvalue like 'SA-O' then	8
 when w4.winvalue like 'SA.' then	8
 when w4.winvalue like 'SR.MGR' then	6
 when w4.winvalue like 'SR.MGR-EN' then	6
 when w4.winvalue like 'SR.MGR-ENG' then	6
 when w4.winvalue like 'STAFF  ASSISTANT' then	8
 when w4.winvalue like 'STENO' then	18
 when w4.winvalue like 'SYS.ADMINISTRATOR' then	29
 when w4.winvalue like 'SYS.ADMN' then	32
 when w4.winvalue like 'TYP' then	20
 when w4.winvalue like 'WATCH' then	21 
 when w4.winvalue='z-DAFTARI.----' then	34
 when w4.winvalue='z-MD .' then	1
 when w4.winvalue='z-DAFT.--------' then	34 end	as curr_Desgn	,
case when w5.winvalue like 'Anakapalli CSF' then	73
 when w5.winvalue like 'Audit' then	74
 when w5.winvalue like 'BANKING-Accounts' then	75
 when w5.winvalue like 'BANKING-BRCC' then	76
 when w5.winvalue like 'BANKING-CCAC' then	77
 when w5.winvalue like 'BANKING-CCAC,CLG' then	78
 when w5.winvalue like 'BANKING-Clg' then	79
 when w5.winvalue like 'BANKING-CLPC/HFC' then	80
 when w5.winvalue like 'BANKING-Investments' then	81
 when w5.winvalue like 'BANKING-Operations' then	82
 when w5.winvalue like 'BANKING-Recon' then	83
 when w5.winvalue like 'BANKING-RMD' then	84
 when w5.winvalue like 'BANKING-RTGS' then	85
 when w5.winvalue like 'BANKING-Stationery' then	86
 when w5.winvalue like 'BKG-KYC/AML' then	87
 when w5.winvalue like 'BKG-TREASURY OPERATIONS' then	88
 when w5.winvalue like 'Board Sectt.' then	89
 when w5.winvalue like 'Board Sectt./HRD' then	90
 when w5.winvalue like 'CGM Peshi' then	91
 when w5.winvalue like 'Chittoor CSF' then	92
 when w5.winvalue like 'chodavaram sugar factory' then	93
 when w5.winvalue like 'CMI & AD (DOS)' then	94
 when w5.winvalue like 'Co-op Minister Peshi' then	95
 when w5.winvalue like 'CSP & Board Secretariat' then	96
 when w5.winvalue like 'CTI - Rajendranagar' then	97
 when w5.winvalue like 'Dept. of Supervision (DOS)' then	98
 when w5.winvalue like 'DoS/Audit' then	99
 when w5.winvalue like 'Engineering Cell' then	100
 when w5.winvalue like 'Etikoppaka CSF-VSP' then	101
 when w5.winvalue like 'GM Peshi' then	102
 when w5.winvalue like 'HRD-Payments' then	103
 when w5.winvalue like 'HRD-Terminal Benfits' then	104
 when w5.winvalue like 'HRD/DoS' then	105
 when w5.winvalue like 'IF,LEGAL,PR' then	106
 when w5.winvalue like 'IFC-RABO Project' then	107
 when w5.winvalue like 'INDUSTRIAL FINANCE' then	108
 when w5.winvalue like 'IT CARDS PROJECT' then	109
 when w5.winvalue like 'IT DEPT- Computers' then	110
 when w5.winvalue like 'IT DEPT-PACS Computerisation' then	111
 when w5.winvalue like 'IT(APCOB-CBS)' then	112
 when w5.winvalue like 'IT(DCCB-CBS)' then	113
 when w5.winvalue like 'Jampani CSF,GNT' then	114
 when w5.winvalue like 'Kovvur CSF' then	115
 when w5.winvalue like 'L & A - LT' then	116
 when w5.winvalue like 'L & A - ST' then	117
 when w5.winvalue like 'LIBRARY' then	118
 when w5.winvalue like 'MD Peshi' then	119
 when w5.winvalue like 'O/o SCDR/OSD' then	120
 when w5.winvalue like 'P&D' then	121
 when w5.winvalue like 'P&D-Proj.Mont.Unit' then	122
 when w5.winvalue like 'Personal Banking Department' then	123
 when w5.winvalue like 'PLG & DEV/RMD & IDD' then	124
 when w5.winvalue like 'PR' then	125
 when w5.winvalue like 'PRINTING&STATIONERY' then	126
 when w5.winvalue like 'PUTLIBOWLI  PREMISES' then	127
 when w5.winvalue like 'Sri Venkateswara CSF,TPT' then	128
 when w5.winvalue like 'Tandava CSF' then	129
 when w5.winvalue like 'VijRamGajapati CSF,VZM' then	130
 when w5.winvalue like 'PREMISES' then	23
 when w5.winvalue like 'Legal,Vigilance & RTI Act' then 20
 when w5.winvalue like 'HRD-Admn' then 16
 when w5.winvalue like 'Branches' then  46
 when w5.winvalue like 'HEAD OFFICE.' then  42
 when w5.winvalue like 'Inward-Outward' then 12
 when w5.winvalue in ('0','55','56','57','58','59','69','7') then 63
 when w5.winvalue='' then 63
 else 46
end	as joined_Dept	,
4	as role1,
EMail	,
Expr	,
Doj	,
	'' as reliving_date,
Dor	,
325	as ca,
325	as sa,
	'' as photo,
123456	as updated_by,
getdate()	as updated_time,
	'' as reliving_reason,
	'' as spouse,
w6.winvalue as BloodGroup	,
AADHAAR	,
PANNO	,
	'' as profession_qualfication,
'Employee'	as role,
16	as C_D,
27	as CB,
5	as CD,
16	as SD,
	'' as SB,
4	as S_D,
43	as BV1,
2	as BV2,
EmpShName	,
	'' as other_qualification,
32	as per_branch,
1	as per_dept,
'' as branch	,
'Retd' as type	,
Eid	into employees_temp1
 from test2.dbo.pempmast_temp e 
 join test2.dbo.PWinfile w1 on e.sex=w1.winid
 join test2.dbo.PWinfile w2 on e.mstatus=w2.winid
 join test2.dbo.PWinfile w3 on e.branch=w3.winid
 join test2.dbo.PWinfile w4 on e.Desgn=w4.winid
 join test2.dbo.PWinfile w5 on e.Dept=w5.winid
 join test2.dbo.PWinfile w6 on e.Bloodgroup=w6.winid 
---------------------------------------------------------------

select Empname,
 Empname as e1,
'pP2TLUp8ksrkrTSP24c+xw=='	as password,
 w1.winvalue as sex	,
case when w2.winvalue='Unmarried' then 'Single' else w2.winvalue end as marital_status	,
Dob	,
EMail	as mail,
Fname	,
	'' as mother,
PPhoneNo as phone1	,
PPhoneNo	as phone2,	
 concat(concat( Add1,' ', add2),' ', add3)	as addresss,
concat(concat( pAdd1,' ', padd2),' ', padd3)	as paddress	,
	'' as graduation,
	'' as post_graduation,
EmpShName	as emergency_contact_name,
PhoneNo	as emergency_contact_number,
	'' as cat,
Empid	,
 case when w3.winvalue='ALWAL' then	1
 when w3.winvalue='AMBERPET' then	2
 when w3.winvalue='AMEERPET' then	3
 when w3.winvalue='ATTAPUR' then	4
 when w3.winvalue='BADANGPET' then	5
 when w3.winvalue='BAGHLIMGAMPALLY' then	6
 when w3.winvalue='BANDLAGUDA' then	7
 when w3.winvalue='BHARAT NAGAR' then	8
 when w3.winvalue='BOUDHANAGAR' then	9
 when w3.winvalue='CHAMPAPET' then	10
 when w3.winvalue='CHANDANAGAR' then	11
 when w3.winvalue='CHARMINAR' then	12
 when w3.winvalue='DILSUKHNAGAR' then	13
 when w3.winvalue='GACHIBOWLI' then	14
 when w3.winvalue='HIMAYATNAGAR' then	15
 when w3.winvalue='JUBILEEHILLS' then	16
 when w3.winvalue='KAMALANAGAR' then	17
 when w3.winvalue='KUKATPALLY' then	18
 when w3.winvalue='LOTHUKUNTA' then	20
 when w3.winvalue='MALAKPET' then	21
 when w3.winvalue='MALKAJGIRI' then	22
 when w3.winvalue='MARUTHINAGAR' then	23
 when w3.winvalue='MASABTANK' then	24
 when w3.winvalue='MEERPET' then	25
 when w3.winvalue='MOULALI' then	26
 when w3.winvalue='NARAYANAGUDA' then	27
 when w3.winvalue='NEREDMET' then	28
 when w3.winvalue='SAIDABAD' then	29
 when w3.winvalue='SAROORNAGAR' then	30
 when w3.winvalue='SECUNDERABAD' then	31
 when w3.winvalue='SERILINGAMPALLY' then	32
 when w3.winvalue='SUCHITRA' then	33
 when w3.winvalue='TARNAKA' then	34
 when w3.winvalue='TARNAKA EXTN.COUNTER' then	34
 when w3.winvalue='TOLICHOWKI' then	35
 when w3.winvalue='UPPAL' then	37
 when w3.winvalue='VENGALRAONAGAR' then	38
 when w3.winvalue='VIDYA NAGAR' then	39
 when w3.winvalue='VIDYUTHSOUDHA' then	40
 when w3.winvalue='YOUSUFGUDA' then	41
 when w3.winvalue='HEAD OFFICE' then	43
 when w3.winvalue='MANSOORABAD' then	48
	end as join_branch,
 case when w4.winvalue like 'CHIEF GENERAL MANAGER.' then	2
 when w4.winvalue like 'AGM%' then	5
 when w4.winvalue like 'ASST.GEN.MANAGER%' then	5
 when w4.winvalue like 'ATDR' then	9
 when w4.winvalue like 'ATTENDER.' then	9
 when w4.winvalue like 'B.M.' then	25
 when w4.winvalue like 'DAFTARI..' then	34
 when w4.winvalue like 'DFTR' then	37
 when w4.winvalue like 'DGM' then	4
 when w4.winvalue like 'DRVR' then	13
 when w4.winvalue like 'DY.GENERAL  MANAGER.' then	4
 when w4.winvalue like 'ELECT' then	16
 when w4.winvalue like 'GENERAL  MANAGER.' then	3
 when w4.winvalue like 'GM' then	3
 when w4.winvalue like 'JAMEDAR' then	36
 when w4.winvalue like 'JMDR' then	36
 when w4.winvalue like 'JSA' then	15
 when w4.winvalue like 'MANAGING  DIRECTOR..' then	1
 when w4.winvalue like 'MANAGING DIRECTOR.' then	1
 when w4.winvalue like 'MD' then	1
 when w4.winvalue like 'MGR SC 1' then	7
 when w4.winvalue like 'MGR.SC 1' then	7
 when w4.winvalue like 'MGRD.' then	7
 when w4.winvalue like 'SA' then	8
 when w4.winvalue like 'SA-O' then	8
 when w4.winvalue like 'SA.' then	8
 when w4.winvalue like 'SR.MGR' then	6
 when w4.winvalue like 'SR.MGR-EN' then	6
 when w4.winvalue like 'SR.MGR-ENG' then	6
 when w4.winvalue like 'STAFF  ASSISTANT' then	8
 when w4.winvalue like 'STENO' then	18
 when w4.winvalue like 'SYS.ADMINISTRATOR' then	29
 when w4.winvalue like 'SYS.ADMN' then	32
 when w4.winvalue like 'TYP' then	20
 when w4.winvalue like 'WATCH' then	21 
 when w4.winvalue like 'z-SY AD' then 32
 when w4.winvalue='z-DAFTARI.----' then	34
 when w4.winvalue='z-MD .' then	1
 when w4.winvalue='z-DAFT.--------' then	34 end as joined_Desgn,
case when w4.winvalue like 'CHIEF GENERAL MANAGER.' then	2
 when w4.winvalue like 'AGM%' then	5
 when w4.winvalue like 'ASST.GEN.MANAGER%' then	5
 when w4.winvalue like 'ATDR' then	9
 when w4.winvalue like 'ATTENDER.' then	9
 when w4.winvalue like 'B.M.' then	25
 when w4.winvalue like 'DAFTARI..' then	34
 when w4.winvalue like 'DFTR' then	37
 when w4.winvalue like 'DGM' then	4
 when w4.winvalue like 'DRVR' then	13
 when w4.winvalue like 'DY.GENERAL  MANAGER.' then	4
 when w4.winvalue like 'ELECT' then	16
 when w4.winvalue like 'GENERAL  MANAGER.' then	3
 when w4.winvalue like 'GM' then	3
 when w4.winvalue like 'JAMEDAR' then	36
 when w4.winvalue like 'JMDR' then	36
 when w4.winvalue like 'JSA' then	15
 when w4.winvalue like 'MANAGING  DIRECTOR..' then	1
 when w4.winvalue like 'MANAGING DIRECTOR.' then	1
 when w4.winvalue like 'MD' then	1
 when w4.winvalue like 'MGR SC 1' then	7
 when w4.winvalue like 'MGR.SC 1' then	7
 when w4.winvalue like 'MGRD.' then	7
 when w4.winvalue like 'SA' then	8
 when w4.winvalue like 'SA-O' then	8
 when w4.winvalue like 'SA.' then	8
 when w4.winvalue like 'SR.MGR' then	6
 when w4.winvalue like 'SR.MGR-EN' then	6
 when w4.winvalue like 'SR.MGR-ENG' then	6
 when w4.winvalue like 'STAFF  ASSISTANT' then	8
 when w4.winvalue like 'STENO' then	18
 when w4.winvalue like 'SYS.ADMINISTRATOR' then	29
 when w4.winvalue like 'SYS.ADMN' then	32
 when w4.winvalue like 'TYP' then	20
 when w4.winvalue like 'WATCH' then	21 
 when w4.winvalue like 'z-SY AD' then 32
 when w4.winvalue='z-DAFTARI.----' then	34
 when w4.winvalue='z-MD .' then	1
 when w4.winvalue='z-DAFT.--------' then	34 end	as curr_Desgn	,
case when w5.winvalue like 'Anakapalli CSF' then	73
 when w5.winvalue like 'Audit' then	74
 when w5.winvalue like 'BANKING-Accounts' then	75
 when w5.winvalue like 'BANKING-BRCC' then	76
 when w5.winvalue like 'BANKING-CCAC' then	77
 when w5.winvalue like 'BANKING-CCAC,CLG' then	78
 when w5.winvalue like 'BANKING-Clg' then	79
 when w5.winvalue like 'BANKING-CLPC/HFC' then	80
 when w5.winvalue like 'BANKING-Investments' then	81
 when w5.winvalue like 'BANKING-Operations' then	82
 when w5.winvalue like 'BANKING-Recon' then	83
 when w5.winvalue like 'BANKING-RMD' then	84
 when w5.winvalue like 'BANKING-RTGS' then	85
 when w5.winvalue like 'BANKING-Stationery' then	86
 when w5.winvalue like 'BKG-KYC/AML' then	87
 when w5.winvalue like 'BKG-TREASURY OPERATIONS' then	88
 when w5.winvalue like 'Board Sectt.' then	89
 when w5.winvalue like 'Board Sectt./HRD' then	90
 when w5.winvalue like 'CGM Peshi' then	91
 when w5.winvalue like 'Chittoor CSF' then	92
 when w5.winvalue like 'chodavaram sugar factory' then	93
 when w5.winvalue like 'CMI & AD (DOS)' then	94
 when w5.winvalue like 'Co-op Minister Peshi' then	95
 when w5.winvalue like 'CSP & Board Secretariat' then	96
 when w5.winvalue like 'CTI - Rajendranagar' then	97
 when w5.winvalue like 'Dept. of Supervision (DOS)' then	98
 when w5.winvalue like 'DoS/Audit' then	99
 when w5.winvalue like 'Engineering Cell' then	100
 when w5.winvalue like 'Etikoppaka CSF-VSP' then	101
 when w5.winvalue like 'GM Peshi' then	102
 when w5.winvalue like 'HRD-Payments' then	103
 when w5.winvalue like 'HRD-Terminal Benfits' then	104
 when w5.winvalue like 'HRD/DoS' then	105
 when w5.winvalue like 'IF,LEGAL,PR' then	106
 when w5.winvalue like 'IFC-RABO Project' then	107
 when w5.winvalue like 'INDUSTRIAL FINANCE' then	108
 when w5.winvalue like 'IT CARDS PROJECT' then	109
 when w5.winvalue like 'IT DEPT- Computers' then	110
 when w5.winvalue like 'IT DEPT-PACS Computerisation' then	111
 when w5.winvalue like 'IT(APCOB-CBS)' then	112
 when w5.winvalue like 'IT(DCCB-CBS)' then	113
 when w5.winvalue like 'Jampani CSF,GNT' then	114
 when w5.winvalue like 'Kovvur CSF' then	115
 when w5.winvalue like 'L & A - LT' then	116
 when w5.winvalue like 'L & A - ST' then	117
 when w5.winvalue like 'LIBRARY' then	118
 when w5.winvalue like 'MD Peshi' then	119
 when w5.winvalue like 'O/o SCDR/OSD' then	120
 when w5.winvalue like 'P&D' then	121
 when w5.winvalue like 'P&D-Proj.Mont.Unit' then	122
 when w5.winvalue like 'Personal Banking Department' then	123
 when w5.winvalue like 'PLG & DEV/RMD & IDD' then	124
 when w5.winvalue like 'PR' then	125
 when w5.winvalue like 'PRINTING&STATIONERY' then	126
 when w5.winvalue like 'PUTLIBOWLI  PREMISES' then	127
 when w5.winvalue like 'Sri Venkateswara CSF,TPT' then	128
 when w5.winvalue like 'Tandava CSF' then	129
 when w5.winvalue like 'VijRamGajapati CSF,VZM' then	130
 when w5.winvalue like 'PREMISES' then	23
 when w5.winvalue like 'Legal,Vigilance & RTI Act' then 20
 when w5.winvalue like 'HRD-Admn' then 16
 when w5.winvalue like 'Branches' then  46
 when w5.winvalue like 'HEAD OFFICE.' then  42
 when w5.winvalue like 'Inward-Outward' then 12
 when w5.winvalue in ('0','55','56','57','58','59','69','7') then 63
 when w5.winvalue='' then 63
 else 46
end	as joined_Dept	,
4	as role1,
EMail	,
Expr	,
Doj	,
	'' as reliving_date,
Dor	,
325	as ca,
325	as sa,
	'' as photo,
123456	as updated_by,
getdate()	as updated_time,
	'' as reliving_reason,
	'' as spouse,
w6.winvalue as BloodGroup	,
'' as AADHAAR	,
PANNO	,
	'' as profession_qualfication,
'Employee'	as role,
16	as C_D,
27	as CB,
5	as CD,
16	as SD,
	'' as SB,
4	as S_D,
43	as BV1,
2	as BV2,
EmpShName	,
	'' as other_qualification,
32	as per_branch,
1	as per_dept,
'' as branch	,
'Retd' as type	,
Eid	into employees_temp2
 from test2.dbo.pempmastold_temp e 
 join test2.dbo.PWinfile w1 on e.sex=w1.winid
 join test2.dbo.PWinfile w2 on e.mstatus=w2.winid
 join test2.dbo.PWinfile w3 on e.branch=w3.winid
 join test2.dbo.PWinfile w4 on e.Desgn=w4.winid
 join test2.dbo.PWinfile w5 on e.Dept=w5.winid
 join test2.dbo.PWinfile w6 on e.Bloodgroup=w6.winid 
 
 
 
 update employees_temp1 set curr_desgn=(select id from designations where code='SysAdmin') where curr_desgn in (32,36)
update employees_temp1 set curr_desgn=(select id from designations where code='DFTR') where curr_desgn in (34,37)

update employees_temp1 set joined_desgn=(select id from designations where code='SysAdmin') where joined_desgn in (32,36)
update employees_temp1 set joined_desgn=(select id from designations where code='DFTR') where joined_desgn in (34,37)

 
update employees_temp2 set curr_desgn=(select id from designations where code='SysAdmin') where curr_desgn in (32,36)
update employees_temp2 set curr_desgn=(select id from designations where code='DFTR') where curr_desgn in (34,37)

update employees_temp2 set joined_desgn=(select id from designations where code='SysAdmin') where joined_desgn in (32,36)
update employees_temp2 set joined_desgn=(select id from designations where code='DFTR') where joined_desgn in (34,37)

update employees_temp1 set curr_desgn=8 where curr_desgn is  null
 update employees_temp1 set joined_desgn=8 where joined_desgn is  null
update employees_temp2 set curr_desgn=8 where curr_desgn is  null
 update employees_temp2 set joined_desgn=8 where joined_desgn is  null
 
 update employees_temp1 set joined_dept=(select id from departments where name='Anakapalli CSF')  where  joined_dept=74
update employees_temp1 set joined_dept=(select id from departments where name='Audit')  where  joined_dept=75
update employees_temp1 set joined_dept=(select id from departments where name='BANKING-Accounts')  where  joined_dept=76
update employees_temp1 set joined_dept=(select id from departments where name='BANKING-BRCC')  where  joined_dept=77
update employees_temp1 set joined_dept=(select id from departments where name='BANKING-CCAC')  where  joined_dept=78
update employees_temp1 set joined_dept=(select id from departments where name='BANKING-CCAC,CLG')  where  joined_dept=79
update employees_temp1 set joined_dept=(select id from departments where name='BANKING-Clg')  where  joined_dept=80
update employees_temp1 set joined_dept=(select id from departments where name='BANKING-CLPC/HFC')  where  joined_dept=81
update employees_temp1 set joined_dept=(select id from departments where name='BANKING-Investments')  where  joined_dept=82
update employees_temp1 set joined_dept=(select id from departments where name='BANKING-Operations')  where  joined_dept=83
update employees_temp1 set joined_dept=(select id from departments where name='BANKING-Recon')  where  joined_dept=84
update employees_temp1 set joined_dept=(select id from departments where name='BANKING-RMD')  where  joined_dept=85
update employees_temp1 set joined_dept=(select id from departments where name='BANKING-RTGS')  where  joined_dept=86
update employees_temp1 set joined_dept=(select id from departments where name='BANKING-Stationery')  where  joined_dept=87
update employees_temp1 set joined_dept=(select id from departments where name='BKG-KYC/AML')  where  joined_dept=88
update employees_temp1 set joined_dept=(select id from departments where name='BKG-TREASURY OPERATIONS')  where  joined_dept=89
update employees_temp1 set joined_dept=(select id from departments where name='Board Sectt.')  where  joined_dept=90
update employees_temp1 set joined_dept=(select id from departments where name='Board Sectt./HRD')  where  joined_dept=91
update employees_temp1 set joined_dept=(select id from departments where name='CGM Peshi')  where  joined_dept=92
update employees_temp1 set joined_dept=(select id from departments where name='Chittoor CSF')  where  joined_dept=93
update employees_temp1 set joined_dept=(select id from departments where name='chodavaram sugar factory')  where  joined_dept=94
update employees_temp1 set joined_dept=(select id from departments where name='CMI & AD (DOS)')  where  joined_dept=95
update employees_temp1 set joined_dept=(select id from departments where name='Co-op Minister Peshi')  where  joined_dept=96
update employees_temp1 set joined_dept=(select id from departments where name='CSP & Board Secretariat')  where  joined_dept=97
update employees_temp1 set joined_dept=(select id from departments where name='CTI - Rajendranagar')  where  joined_dept=98
update employees_temp1 set joined_dept=(select id from departments where name='Dept. of Supervision (DOS)')  where  joined_dept=99
update employees_temp1 set joined_dept=(select id from departments where name='DoS/Audit')  where  joined_dept=100
update employees_temp1 set joined_dept=(select id from departments where name='Engineering Cell')  where  joined_dept=101
update employees_temp1 set joined_dept=(select id from departments where name='Etikoppaka CSF-VSP')  where  joined_dept=102
update employees_temp1 set joined_dept=(select id from departments where name='GM Peshi')  where  joined_dept=103
update employees_temp1 set joined_dept=(select id from departments where name='HRD-Payments')  where  joined_dept=104
update employees_temp1 set joined_dept=(select id from departments where name='HRD-Terminal Benfits')  where  joined_dept=105
update employees_temp1 set joined_dept=(select id from departments where name='HRD/DoS')  where  joined_dept=106
update employees_temp1 set joined_dept=(select id from departments where name='IF,LEGAL,PR')  where  joined_dept=107
update employees_temp1 set joined_dept=(select id from departments where name='IFC-RABO Project')  where  joined_dept=108
update employees_temp1 set joined_dept=(select id from departments where name='INDUSTRIAL FINANCE')  where  joined_dept=109
update employees_temp1 set joined_dept=(select id from departments where name='IT CARDS PROJECT')  where  joined_dept=110
update employees_temp1 set joined_dept=(select id from departments where name='IT DEPT- Computers')  where  joined_dept=111
update employees_temp1 set joined_dept=(select id from departments where name='IT DEPT-PACS Computerisation')  where  joined_dept=112
update employees_temp1 set joined_dept=(select id from departments where name='IT(APCOB-CBS)')  where  joined_dept=113
update employees_temp1 set joined_dept=(select id from departments where name='IT(DCCB-CBS)')  where  joined_dept=114
update employees_temp1 set joined_dept=(select id from departments where name='Jampani CSF,GNT')  where  joined_dept=115
update employees_temp1 set joined_dept=(select id from departments where name='Kovvur CSF')  where  joined_dept=116
update employees_temp1 set joined_dept=(select id from departments where name='L & A - LT')  where  joined_dept=117
update employees_temp1 set joined_dept=(select id from departments where name='L & A - ST')  where  joined_dept=118
update employees_temp1 set joined_dept=(select id from departments where name='LIBRARY')  where  joined_dept=119
update employees_temp1 set joined_dept=(select id from departments where name='MD Peshi')  where  joined_dept=120
update employees_temp1 set joined_dept=(select id from departments where name='O/o SCDR/OSD')  where  joined_dept=121
update employees_temp1 set joined_dept=(select id from departments where name='P&D')  where  joined_dept=122
update employees_temp1 set joined_dept=(select id from departments where name='P&D-Proj.Mont.Unit')  where  joined_dept=123
update employees_temp1 set joined_dept=(select id from departments where name='Personal Banking Department')  where  joined_dept=124
update employees_temp1 set joined_dept=(select id from departments where name='PLG & DEV/RMD & IDD')  where  joined_dept=125
update employees_temp1 set joined_dept=(select id from departments where name='PR')  where  joined_dept=126
update employees_temp1 set joined_dept=(select id from departments where name='PRINTING&STATIONERY')  where  joined_dept=127
update employees_temp1 set joined_dept=(select id from departments where name='PUTLIBOWLI  PREMISES')  where  joined_dept=128
update employees_temp1 set joined_dept=(select id from departments where name='Sri Venkateswara CSF,TPT')  where  joined_dept=129
update employees_temp1 set joined_dept=(select id from departments where name='Tandava CSF')  where  joined_dept=130
update employees_temp1 set joined_dept=(select id from departments where name='VijRamGajapati CSF,VZM')  where  joined_dept=131

update employees_temp2 set joined_dept=(select id from departments where name='Anakapalli CSF')  where  joined_dept=74
update employees_temp2 set joined_dept=(select id from departments where name='Audit')  where  joined_dept=75
update employees_temp2 set joined_dept=(select id from departments where name='BANKING-Accounts')  where  joined_dept=76
update employees_temp2 set joined_dept=(select id from departments where name='BANKING-BRCC')  where  joined_dept=77
update employees_temp2 set joined_dept=(select id from departments where name='BANKING-CCAC')  where  joined_dept=78
update employees_temp2 set joined_dept=(select id from departments where name='BANKING-CCAC,CLG')  where  joined_dept=79
update employees_temp2 set joined_dept=(select id from departments where name='BANKING-Clg')  where  joined_dept=80
update employees_temp2 set joined_dept=(select id from departments where name='BANKING-CLPC/HFC')  where  joined_dept=81
update employees_temp2 set joined_dept=(select id from departments where name='BANKING-Investments')  where  joined_dept=82
update employees_temp2 set joined_dept=(select id from departments where name='BANKING-Operations')  where  joined_dept=83
update employees_temp2 set joined_dept=(select id from departments where name='BANKING-Recon')  where  joined_dept=84
update employees_temp2 set joined_dept=(select id from departments where name='BANKING-RMD')  where  joined_dept=85
update employees_temp2 set joined_dept=(select id from departments where name='BANKING-RTGS')  where  joined_dept=86
update employees_temp2 set joined_dept=(select id from departments where name='BANKING-Stationery')  where  joined_dept=87
update employees_temp2 set joined_dept=(select id from departments where name='BKG-KYC/AML')  where  joined_dept=88
update employees_temp2 set joined_dept=(select id from departments where name='BKG-TREASURY OPERATIONS')  where  joined_dept=89
update employees_temp2 set joined_dept=(select id from departments where name='Board Sectt.')  where  joined_dept=90
update employees_temp2 set joined_dept=(select id from departments where name='Board Sectt./HRD')  where  joined_dept=91
update employees_temp2 set joined_dept=(select id from departments where name='CGM Peshi')  where  joined_dept=92
update employees_temp2 set joined_dept=(select id from departments where name='Chittoor CSF')  where  joined_dept=93
update employees_temp2 set joined_dept=(select id from departments where name='chodavaram sugar factory')  where  joined_dept=94
update employees_temp2 set joined_dept=(select id from departments where name='CMI & AD (DOS)')  where  joined_dept=95
update employees_temp2 set joined_dept=(select id from departments where name='Co-op Minister Peshi')  where  joined_dept=96
update employees_temp2 set joined_dept=(select id from departments where name='CSP & Board Secretariat')  where  joined_dept=97
update employees_temp2 set joined_dept=(select id from departments where name='CTI - Rajendranagar')  where  joined_dept=98
update employees_temp2 set joined_dept=(select id from departments where name='Dept. of Supervision (DOS)')  where  joined_dept=99
update employees_temp2 set joined_dept=(select id from departments where name='DoS/Audit')  where  joined_dept=100
update employees_temp2 set joined_dept=(select id from departments where name='Engineering Cell')  where  joined_dept=101
update employees_temp2 set joined_dept=(select id from departments where name='Etikoppaka CSF-VSP')  where  joined_dept=102
update employees_temp2 set joined_dept=(select id from departments where name='GM Peshi')  where  joined_dept=103
update employees_temp2 set joined_dept=(select id from departments where name='HRD-Payments')  where  joined_dept=104
update employees_temp2 set joined_dept=(select id from departments where name='HRD-Terminal Benfits')  where  joined_dept=105
update employees_temp2 set joined_dept=(select id from departments where name='HRD/DoS')  where  joined_dept=106
update employees_temp2 set joined_dept=(select id from departments where name='IF,LEGAL,PR')  where  joined_dept=107
update employees_temp2 set joined_dept=(select id from departments where name='IFC-RABO Project')  where  joined_dept=108
update employees_temp2 set joined_dept=(select id from departments where name='INDUSTRIAL FINANCE')  where  joined_dept=109
update employees_temp2 set joined_dept=(select id from departments where name='IT CARDS PROJECT')  where  joined_dept=110
update employees_temp2 set joined_dept=(select id from departments where name='IT DEPT- Computers')  where  joined_dept=111
update employees_temp2 set joined_dept=(select id from departments where name='IT DEPT-PACS Computerisation')  where  joined_dept=112
update employees_temp2 set joined_dept=(select id from departments where name='IT(APCOB-CBS)')  where  joined_dept=113
update employees_temp2 set joined_dept=(select id from departments where name='IT(DCCB-CBS)')  where  joined_dept=114
update employees_temp2 set joined_dept=(select id from departments where name='Jampani CSF,GNT')  where  joined_dept=115
update employees_temp2 set joined_dept=(select id from departments where name='Kovvur CSF')  where  joined_dept=116
update employees_temp2 set joined_dept=(select id from departments where name='L & A - LT')  where  joined_dept=117
update employees_temp2 set joined_dept=(select id from departments where name='L & A - ST')  where  joined_dept=118
update employees_temp2 set joined_dept=(select id from departments where name='LIBRARY')  where  joined_dept=119
update employees_temp2 set joined_dept=(select id from departments where name='MD Peshi')  where  joined_dept=120
update employees_temp2 set joined_dept=(select id from departments where name='O/o SCDR/OSD')  where  joined_dept=121
update employees_temp2 set joined_dept=(select id from departments where name='P&D')  where  joined_dept=122
update employees_temp2 set joined_dept=(select id from departments where name='P&D-Proj.Mont.Unit')  where  joined_dept=123
update employees_temp2 set joined_dept=(select id from departments where name='Personal Banking Department')  where  joined_dept=124
update employees_temp2 set joined_dept=(select id from departments where name='PLG & DEV/RMD & IDD')  where  joined_dept=125
update employees_temp2 set joined_dept=(select id from departments where name='PR')  where  joined_dept=126
update employees_temp2 set joined_dept=(select id from departments where name='PRINTING&STATIONERY')  where  joined_dept=127
update employees_temp2 set joined_dept=(select id from departments where name='PUTLIBOWLI  PREMISES')  where  joined_dept=128
update employees_temp2 set joined_dept=(select id from departments where name='Sri Venkateswara CSF,TPT')  where  joined_dept=129
update employees_temp2 set joined_dept=(select id from departments where name='Tandava CSF')  where  joined_dept=130
update employees_temp2 set joined_dept=(select id from departments where name='VijRamGajapati CSF,VZM')  where  joined_dept=131

 

 
 insert into employees select * from employees_temp1 where empid COLLATE DATABASE_DEFAULT not in (select empid from employees)

 insert into employees select * from employees_temp2 where empid COLLATE DATABASE_DEFAULT not in (select empid from employees)
 
 
 update e set e.eid=t.eid from employees e join employees_temp1 t on e.empid=t.empid  COLLATE DATABASE_DEFAULT where  e.eid is null

drop table test2.dbo.pempmastold_temp 
drop table test2.dbo.pempmast_temp 

drop table employees_temp1
drop table employees_temp2


 --select * from employees where eid is not null

 --select * from employees_temp where empid=6363
--select * from employees where empid=6363

-- select * from departments order by name


--------------------------------------------------------------------------------------------------------------------------------------------------------

-- insert into pempmast(Eid	,
-- Empid	,
-- Empname	,
-- Desgn	,
-- Basic	,
-- Dept	,
-- Lop	,
-- Sex	,
-- Fname	,
-- Doj	,
-- Doc	,
-- Expr	,
-- Div	,
-- Pfno	,
-- Esicno	,
-- Dol	,
-- Dob	,
-- Age	,
-- Add1	,
-- Add2	,
-- Add3	,
-- Padd1	,
-- Padd2	,
-- Padd3	,
-- Fdob	,
-- Mstatus	,
-- Pfjoin	,
-- Emptds	,
-- LIC	,
-- Region	,
-- EMail	,
-- Frel	,
-- Hra40	,
-- Branch	,
-- Zone	,
-- EmpShName	,
-- PhoneNo	,
-- PPhoneNo	,
-- NativePlace	,
-- IdMark1	,
-- IdMark2	,
-- BloodGroup	,
-- Religion	,
-- JoinReservation	,
-- ResvCode	,
-- ResvCodeJoin	,
-- PtaxRegion	,
-- DesgnCat	,
-- SPLPAY	,
-- PH	,
-- SYSADMN	,
-- DEPU	,
-- FPA	,
-- FPHRA	,
-- INTERIM	,
-- FPIIP	,
-- MEDICAL	,
-- NPSGA	,
-- OFFI	,
-- PERPAY	,
-- PERQPAY	,
-- RESATTN	,
-- TEACH	,
-- SP_CARETAKE	,
-- SP_CASHIER	,
-- SP_DRIVER	,
-- SP_JAMEDAR	,
-- SP_KEY	,
-- SP_LIFT	,
-- SP_NONPROM	,
-- SP_SD_AWARD	,
-- SP_TYPIST	,
-- SP_WATCHMAN	,
-- SP_STENO	,
-- SP_BILLCOLL	,
-- SP_DESPATCH	,
-- SP_ELEC	,
-- SP_DAFTARI	,
-- SP_CASHCAB	,
-- SP_TELEPHONE	,
-- SP_LIBRARY	,
-- SP_INCENTIVE	,
-- SP_ARREAR	,
-- SP_CONVEY	,
-- SP_SD_MGR	,
-- SP_XEROX	,
-- SP_RECASST	,
-- SP_RECSUB	,
-- SP_RECEPTION	,
-- SP_ACSTI	,
-- VIJAYA	,
-- VISAKHA	,
-- GSLI	,
-- OFFASSN	,
-- OFFASSNC	,
-- SUBCLUB	,
-- SUBUNION	,
-- SUBLT	,
-- SUBST	,
-- FPIR	,
-- SBF	,
-- VPF	,
-- PANNO	,
-- Regord	,
-- Dor	,
-- STAGALLOW	,
-- SP_PERPAY	,
-- relation	,
-- BrRoZo	,
-- TRANSDATE	,
-- incdate	,
-- VPFPER	,
-- Paybank	,
-- Emplgname	,
-- Phc	,
-- Houseprovid) select * from pempmastold where empid not in (select empid from pempmast);