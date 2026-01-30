--Select * from Branches
Create  table dim_branch(
dim_bid int primary key,
Name nvarchar(50),
BranchCode nvarchar(50)
)
--Select * from designations
Create  table dim_designation(
dim_desid int primary key,
Code nvarchar(50),
Name nvarchar(50)
)
--Select * from Departments
Create  table dim_department(
dim_depid int primary key,
Code nvarchar(50),
Name nvarchar(50)
)
--Select * from pr_deduction_field_master
Create  table dim_deduction_types(
dim_deductid int primary key,
Name nvarchar(500),
Type nvarchar(50)
)
--Select * from pr_earn_field_master
Create  table dim_earning_types(
dim_earnid int primary key,
Name nvarchar(500),
Type nvarchar(50)
)

Create  table dim_time(
dim_tid int primary key,
Year int,
Quarter nvarchar(20),
Month nvarchar(20) ,
Week nvarchar(20),
Day int,
)

Create Table dim_emp (
dim_eid int primary key, 
Empid int NOT NULL UNIQUE,
FirstName nvarchar(100),
LastName nvarchar(100),
DoJ Datetime,
RetirementDate Datetime,
Designation_id int Foreign key references dim_designation (dim_desid),
Department_id int Foreign key references dim_department (dim_depid),
Branch_id int Foreign key references dim_branch (dim_bid),
Shift_id int,
Is_NPS nvarchar(20)
)


Insert into dim_designation(dim_desid,Code,Name) select Id,Code,Name from designations
Insert into dim_department(dim_depid,Code,Name) select Id,Code,Name from departments --select Id,count(id) from departments group by id having count(id)>1
Insert into dim_branch(dim_bid,Name,BranchCode) select Id,Name,BranchCode from branches
Insert into dim_deduction_types(dim_deductid,Name,Type) select Id,Name,Type from pr_deduction_field_master where active=1
Insert into dim_earning_types(dim_earnid,Name,Type) select Id,Name,Type from pr_earn_field_master where active=1
Insert into dim_emp(dim_eid,Empid,FirstName,LastName,DoJ,RetirementDate,Designation_id,Department_id,Branch_id,Shift_Id) select Id,Empid,FirstName,LastName,DoJ,RetirementDate,CurrentDesignation,Department,Branch,Shift_Id from Employees where retirementdate>'2019-04-01'


----------------------- Inserting records into dim_time for 2020 and 2021 years ---------------------
DECLARE @FIRSTDAY DATE;
DECLARE @LASTDAY DATE;
DECLARE @Date_Counter DATE;
DECLARE @Calendar_Month NVARCHAR(20);
DECLARE @Calendar_Day NVARCHAR(20);
DECLARE @Calendar_Year INT;
DECLARE @Calendar_Quarter INT;
DECLARE @Day_of_Week INT;
DECLARE @Day_of_Year INT;
DECLARE @Week_of_Year INT;
DECLARE @ID INT;

SET @FIRSTDAY=(SELECT  DATEADD(yy, DATEDIFF(yy, 0, '2019-01-01'), 0))
SET @LASTDAY=(SELECT  DATEADD(yy, DATEDIFF(yy, 0, '2019-01-01') + 4, -1))
SET @ID=1;

WHILE (@FIRSTDAY<=@LASTDAY)
BEGIN

SET @Date_Counter=@FIRSTDAY;
SELECT @Calendar_Month = DATENAME(MONTH, @Date_Counter);
SELECT @Calendar_Day = DATENAME(WEEKDAY, @Date_Counter);
SELECT @Calendar_Year = DATEPART(YEAR, @Date_Counter);
SELECT @Calendar_Quarter = DATEPART(QUARTER, @Date_Counter);
SELECT @Day_of_Week = DATEPART(WEEKDAY, @Date_Counter);
SELECT @Day_of_Year = DATEPART(DAYOFYEAR, @Date_Counter);
SELECT @Week_of_Year = DATEPART(WEEK, @Date_Counter);

INSERT INTO dim_time (dim_tid,Year,Quarter,Month,Week,Day)VALUES(@ID, @Calendar_Year, CONCAT('Q',@Calendar_Quarter), @Calendar_Month, CONCAT('W',@Week_of_Year), @Day_of_Year)

SET @FIRSTDAY =(SELECT DATEADD(dd,DATEDIFF(dd,0,@FIRSTDAY),1))
SET @ID=@ID+1;
--select * from dim_time
END

--select * from dim_emp
--select * from dim_branch
--select * from dim_designation
--select * from dim_department
--select * from dim_deduction_types
--select * from dim_earning_types
--select * from dim_time