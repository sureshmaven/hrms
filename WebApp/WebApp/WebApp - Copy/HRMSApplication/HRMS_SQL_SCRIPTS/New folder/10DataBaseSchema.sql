USE [HRMSPK]
GO
/****** Object:  Table [dbo].[Banks]    Script Date: 1/9/2018 12:49:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Banks](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](250) NULL,
	[AddressLine1] [varchar](250) NULL,
	[AddressLine2] [varchar](250) NULL,
	[City] [varchar](100) NULL,
	[PhoneNo1] [varchar](50) NULL,
	[PhoneNo2] [varchar](50) NULL,
	[PhoneNo3] [varchar](50) NULL,
	[UpdatedBy] [varchar](150) NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK__Banks__3214EC07E9EFC7F1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Branch_Designation_Mapping]    Script Date: 1/9/2018 12:49:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Branch_Designation_Mapping](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BranchId] [int] NULL,
	[DesignationId] [int] NULL,
	[UpdatedBy] [varchar](150) NULL
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Branches]    Script Date: 1/9/2018 12:49:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Branches](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BankName] [int] NULL,
	[Name] [varchar](250) NULL,
	[BranchCode] [varchar](250) NULL,
	[IFSCCode] [varchar](50) NULL,
	[AddressLine1] [varchar](250) NULL,
	[AddressLine2] [varchar](250) NULL,
	[City] [varchar](100) NULL,
	[PhoneNo1] [varchar](50) NULL,
	[PhoneNo2] [varchar](50) NULL,
	[PhoneNo3] [varchar](50) NULL,
	[UpdatedBy] [varchar](150) NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK__Branchs__3214EC071079EBE0] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DeliveryDate_PTL]    Script Date: 1/9/2018 12:49:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DeliveryDate_PTL](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LeaveId] [int] NULL,
	[DeliveryDate] [datetime] NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_DeliveryDate_PTL] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Departments]    Script Date: 1/9/2018 12:49:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Departments](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](50) NULL,
	[Name] [varchar](100) NULL,
	[Description] [varchar](250) NULL,
	[UpdatedBy] [varchar](150) NULL,
	[UpdatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Designations]    Script Date: 1/9/2018 12:49:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Designations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](50) NULL,
	[Name] [varchar](100) NULL,
	[Description] [varchar](250) NULL,
	[UpdatedBy] [varchar](150) NULL,
	[UpdatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[EmpLeaveBalance]    Script Date: 1/9/2018 12:49:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmpLeaveBalance](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LeaveTypeId] [int] NULL,
	[EmpId] [int] NULL,
	[LeaveBalance] [int] NULL,
	[UpdatedBy] [varchar](150) NULL
 CONSTRAINT [PK_EmpLeaveBalances] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[EmployeeHistory]    Script Date: 1/9/2018 12:49:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EmployeeHistory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmpId] [int] NOT NULL,
	[FirstName] [varchar](150) NULL,
	[LastName] [varchar](150) NULL,
	[Password] [varchar](150) NULL,
	[MartialStatus] [varchar](10) NULL,
	[SpouseName] [varchar](150) NULL,
	[PersonalEmailId] [varchar](250) NULL,
	[MobileNumber] [varchar](20) NULL,
	[HomeNumber] [varchar](20) NULL,
	[PresentAddress] [varchar](500) NULL,
	[PermanentAddress] [varchar](500) NULL,
	[EmergencyName] [varchar](150) NULL,
	[EmergencyContactNo] [varchar](50) NULL,
	[EmpCode] [varchar](50) NULL,
	[Branch] [varchar](150) NULL,
	[JoinedDesignation] [varchar](150) NULL,
	[CurrentDesignation] [varchar](150) NULL,
	[Department] [varchar](150) NULL,
	[Role] [varchar](150) NULL,
	[OfficalEmailId] [varchar](250) NULL,
	[TotalExperience] [varchar](20) NULL,
	[DOJ] [datetime] NULL,
	[ControllingAuthority] [varchar](150) NULL,
	[SanctioningAuthority] [varchar](150) NULL,
	[BloodGroup] [varchar](50) NULL,
	[AadharCardNo] [varchar](50) NULL,
	[PanCardNo] [varchar](50) NULL,
	[ProfessionalQualifications] [varchar](250) NULL,
	[UpdatedBy] [varchar](150) NULL,
	[UpdatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Employees]    Script Date: 1/9/2018 12:49:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Employees](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [varchar](150) NULL,
	[LastName] [varchar](150) NULL,
	[Password] [varchar](150) NULL,
	[Gender] [varchar](10) NULL,
	[MartialStatus] [varchar](10) NULL,
	[DOB] [datetime] NULL,
	[PersonalEmailId] [varchar](250) NULL,
	[FatherName] [varchar](150) NULL,
	[MotherName] [varchar](150) NULL,
	[MobileNumber] [varchar](20) NULL,
	[HomeNumber] [varchar](20) NULL,
	[PresentAddress] [varchar](500) NULL,
	[PermanentAddress] [varchar](500) NULL,
	[Graduation] [varchar](150) NULL,
	[PostGraduation] [varchar](150) NULL,
	[EmergencyName] [varchar](150) NULL,
	[EmergencyContactNo] [varchar](50) NULL,
	[Category] [varchar](20) NULL,
	[EmpId] [varchar](50) NULL,
	[Branch] [int] NULL,
	[JoinedDesignation] [int] NULL,
	[CurrentDesignation] [int] NULL,
	[Department] [int] NULL,
	[Role] [int] NULL,
	[OfficalEmailId] [varchar](250) NULL,
	[TotalExperience] [varchar](20) NULL,
	[DOJ] [datetime] NULL,
	[RelievingDate] [datetime] NULL,
	[RetirementDate] [datetime] NULL,
	[ControllingAuthority] [varchar](150) NULL,
	[SanctioningAuthority] [varchar](150) NULL,
	[UploadPhoto] [varchar](500) NULL,
	[UpdatedBy] [varchar](150) NULL,
	[UpdatedDate] [datetime] NULL,
	[RelievingReason] [varchar](500) NULL,
	[SpouseName] [varchar](150) NULL,
	[BloodGroup] [varchar](50) NULL,
	[AadharCardNo] [varchar](50) NULL,
	[PanCardNo] [varchar](50) NULL,
	[ProfessionalQualifications] [varchar](250) NULL,
	[LoginMode] [varchar](100) NULL,
	[ControllingDepartment] [int] NULL,
	[ControllingBranch] [int] NULL,
	[ControllingDesignation] [int] NULL,
	[SanctioningDepartment] [int] NULL,
	[SanctioningBranch] [int] NULL,
	[SanctioningDesignation] [int] NULL,
	[Branch_Value1] [varchar](50) NULL,
	[Branch_Value_2] [int] NULL,
 CONSTRAINT [PK__Employee__3214EC0751FE41D7] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[HolidayList]    Script Date: 1/9/2018 12:49:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[HolidayList](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Occasion] [varchar](max) NULL,
	[Date] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdateDate] [datetime] NULL,
	[DeleteAt] [datetime] NULL,
 CONSTRAINT [PK_Table_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[LeaveInfo]    Script Date: 1/9/2018 12:49:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LeaveInfo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmpId] [int] NULL,
	[LeaveId] [int] NULL,
	[LeaveType] [int] NULL,
	[LeaveDays] [int] NULL,
	[FromDate] [datetime] NULL,
	[ToDate] [datetime] NULL,
	[TotalDays] [int] NULL,
	[UpdatedBy] [varchar](150) NULL
 CONSTRAINT [PK_LeaveInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Leaves]    Script Date: 1/9/2018 12:49:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Leaves](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmpId] [int] NULL,
	[ControllingAuthority] [int] NULL,
	[SanctioningAuthority] [int] NULL,
	[LeaveType] [int] NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[Subject] [varchar](250) NULL,
	[Reason] [varchar](500) NULL,
	[UpdatedBy] [varchar](150) NULL,
	[UpdatedDate] [datetime] NULL,
	[Status] [varchar](50) NULL,
 CONSTRAINT [PK__Leaves__3214EC07F61FD04D] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[LeaveTypes]    Script Date: 1/9/2018 12:49:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[LeaveTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Type] [varchar](100) NULL,
	[Description] [varchar](250) NULL,
	[UpdatedBy] [varchar](150) NULL,
	[UpdatedDate] [datetime] NULL,
	[Code] [varchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 1/9/2018 12:49:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Roles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NULL,
	[Description] [varchar](250) NULL,
	[UpdatedBy] [varchar](150) NULL,
	[UpdatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[tenp]    Script Date: 1/9/2018 12:49:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[tenp](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [varchar](150) NULL,
	[LastName] [varchar](150) NULL,
	[Password] [varchar](150) NULL,
	[Gender] [varchar](10) NULL,
	[MartialStatus] [varchar](10) NULL,
	[DOB] [datetime] NULL,
	[PersonalEmailId] [varchar](250) NULL,
	[FatherName] [varchar](150) NULL,
	[MotherName] [varchar](150) NULL,
	[MobileNumber] [varchar](20) NULL,
	[HomeNumber] [varchar](20) NULL,
	[PresentAddress] [varchar](500) NULL,
	[PermanentAddress] [varchar](500) NULL,
	[Graduation] [varchar](150) NULL,
	[PostGraduation] [varchar](150) NULL,
	[EmergencyName] [varchar](150) NULL,
	[EmergencyContactNo] [varchar](50) NULL,
	[Category] [varchar](20) NULL,
	[EmpId] [varchar](50) NULL,
	[Branch] [int] NULL,
	[JoinedDesignation] [int] NULL,
	[CurrentDesignation] [int] NULL,
	[Department] [int] NULL,
	[Role] [int] NULL,
	[OfficalEmailId] [varchar](250) NULL,
	[TotalExperience] [varchar](20) NULL,
	[DOJ] [datetime] NULL,
	[RelievingDate] [datetime] NULL,
	[RetirementDate] [datetime] NULL,
	[ControllingAuthority] [varchar](150) NULL,
	[SanctioningAuthority] [varchar](150) NULL,
	[UploadPhoto] [varchar](500) NULL,
	[UpdatedBy] [varchar](150) NULL,
	[UpdatedDate] [datetime] NULL,
	[RelievingReason] [varchar](500) NULL,
	[SpouseName] [varchar](150) NULL,
	[BloodGroup] [varchar](50) NULL,
	[AadharCardNo] [varchar](50) NULL,
	[PanCardNo] [varchar](50) NULL,
	[ProfessionalQualifications] [varchar](250) NULL,
	[LoginMode] [varchar](100) NULL,
	[ControllingDepartment] [int] NULL,
	[ControllingBranch] [int] NULL,
	[ControllingDesignation] [int] NULL,
	[SanctioningDepartment] [int] NULL,
	[SanctioningBranch] [int] NULL,
	[SanctioningDesignation] [int] NULL,
	[Branch_Value1] [varchar](50) NULL,
	[Branch_Value_2] [int] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Tx_History]    Script Date: 1/9/2018 12:49:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Tx_History](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Tx_id] [int] NULL,
	[Tx_type] [varchar](250) NULL,
	[Tx_subtype] [varchar](250) NULL,
	[Tx_by] [varchar](250) NULL,
	[Tx_on] [varchar](250) NULL,
	[Tx_date] [datetime] NULL,
	[Notes] [varchar](250) NULL,
	[Comments] [varchar](250) NULL,
	[UpdatedBy] [varchar](150) NULL
 CONSTRAINT [PK_Tx_History] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[WorkingDays]    Script Date: 1/9/2018 12:49:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WorkingDays](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmpId] [int] NULL,
	[LastCountDate] [datetime] NULL,
	[CL] [decimal](18, 2) NULL,
	[PL] [int] NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDate] [datetime] NULL
) ON [PRIMARY]

GO
/****** Object:  View [dbo].[V_LeaveBalance]    Script Date: 1/9/2018 12:49:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE  View [dbo].[V_LeaveBalance]

as
(select CAST(ROW_NUMBER() over(order by l.Id desc) AS INT) Id,
(select distinct l.EmpId) as EmpId,

(select isnull(sum(l1.LeaveBalance),0) from [EmpLeaveBalance] l1 where l1.EmpId=l.empid  and LeaveTypeId = (select Id from [LeaveTypes] where Type='Casual Leave' )) as CasualLeave,
(select isnull(sum(l2.LeaveBalance),0) from [EmpLeaveBalance] l2 where l2.EmpId=l.empid  and LeaveTypeId = (select Id from [LeaveTypes] where Type='Medical/Sick Leave' )) as MedicalSickLeave,
(select isnull(sum(l3.LeaveBalance),0) from [EmpLeaveBalance] l3 where l3.EmpId=l.empid  and LeaveTypeId = (select Id from [LeaveTypes] where Type='Privilege Leave' )) as PrivilegeLeave,
(select isnull(sum(l4.LeaveBalance),0) from [EmpLeaveBalance] l4 where l4.EmpId=l.empid  and LeaveTypeId = (select Id from [LeaveTypes] where Type='Maternity Leave' )) as MaternityLeave,
(select isnull(sum(l5.LeaveBalance),0) from [EmpLeaveBalance] l5 where l5.EmpId=l.empid  and LeaveTypeId = (select Id from [LeaveTypes] where Type='Paternity Leave' )) as PaternityLeave,
(select isnull(sum(l6.LeaveBalance),0) from [EmpLeaveBalance] l6 where l6.EmpId=l.empid  and LeaveTypeId = (select Id from [LeaveTypes] where Type='Extraordinary Leave' )) as ExtraordinaryLeave,
(select isnull(sum(l7.LeaveBalance),0) from [EmpLeaveBalance] l7 where l7.EmpId=l.empid  and LeaveTypeId = (select Id from [LeaveTypes] where Type='Special Casual Leave' )) as SpecialCasualLeave

from [EmpLeaveBalance] l)






GO
/****** Object:  View [dbo].[V_EmpLeaveBalance]    Script Date: 1/9/2018 12:49:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE view [dbo].[V_EmpLeaveBalance] as

WITH cte AS
(
   SELECT *,
        ROW_NUMBER() OVER (PARTITION BY empID ORDER BY empid DESC) as RowId
   FROM [V_LeaveBalance]
)
SELECT *
FROM cte
WHERE RowId = 1 

alter table branch_designation_mapping add UpdatedBy varchar(150) NULL;
alter table employee_circulars add UpdatedBy varchar(150) NULL;
alter table employee_meeting add UpdatedBy varchar(150) NULL;
alter table  employee_meetingschedule add UpdatedBy varchar(150) NULL;
alter table  employee_transfer add UpdatedBy varchar(150) NULL;
alter table  leaveinfo add UpdatedBy varchar(150) NULL;
alter table  tx_history add UpdatedBy varchar(150) NULL;





GO
/****** Object:  View [dbo].[V_LeaveHistory]    Script Date: 1/9/2018 12:49:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  view [dbo].[V_LeaveHistory]
as
select CAST(ROW_NUMBER() over(order by e.Id desc) AS INT) Id,e.EmpId as EmployeeCode, CONCAT(e.FirstName, ' ',e.LastName) AS EmployeeName,
l.EmpId,l.LeaveType,e.FirstName,e.Branch,
e.LastName,e.CurrentDesignation, l.StartDate,l.EndDate,lf.TotalDays,lf.LeaveDays,l.Subject
,l.Reason,l.Status from leaves l inner join leaveinfo 
lf on l.Id=lf.LeaveId  inner join Employees e on e.Id= l.EmpId
where l.EmpId=lf.EmpId or
 l.StartDate=lf.FromDate
 or l.EndDate=lf.ToDate 
or l.leaveType=
lf.LeaveType 







GO
ALTER TABLE [dbo].[Branch_Designation_Mapping]  WITH CHECK ADD  CONSTRAINT [FK_Branch_Designation_Mapping_Branches] FOREIGN KEY([BranchId])
REFERENCES [dbo].[Branches] ([Id])
GO
ALTER TABLE [dbo].[Branch_Designation_Mapping] CHECK CONSTRAINT [FK_Branch_Designation_Mapping_Branches]
GO
ALTER TABLE [dbo].[Branch_Designation_Mapping]  WITH CHECK ADD  CONSTRAINT [FK_Branch_Designation_Mapping_Designations] FOREIGN KEY([DesignationId])
REFERENCES [dbo].[Designations] ([Id])
GO
ALTER TABLE [dbo].[Branch_Designation_Mapping] CHECK CONSTRAINT [FK_Branch_Designation_Mapping_Designations]
GO
ALTER TABLE [dbo].[Branches]  WITH CHECK ADD  CONSTRAINT [FK_Branches_Banks] FOREIGN KEY([BankName])
REFERENCES [dbo].[Banks] ([Id])
GO
ALTER TABLE [dbo].[Branches] CHECK CONSTRAINT [FK_Branches_Banks]
GO
ALTER TABLE [dbo].[DeliveryDate_PTL]  WITH CHECK ADD  CONSTRAINT [FK_DeliveryDate_PTL_Employees] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[Employees] ([Id])
GO
ALTER TABLE [dbo].[DeliveryDate_PTL] CHECK CONSTRAINT [FK_DeliveryDate_PTL_Employees]
GO
ALTER TABLE [dbo].[DeliveryDate_PTL]  WITH CHECK ADD  CONSTRAINT [FK_DeliveryDate_PTL_Leaves] FOREIGN KEY([LeaveId])
REFERENCES [dbo].[Leaves] ([Id])
GO
ALTER TABLE [dbo].[DeliveryDate_PTL] CHECK CONSTRAINT [FK_DeliveryDate_PTL_Leaves]
GO
ALTER TABLE [dbo].[EmpLeaveBalance]  WITH CHECK ADD  CONSTRAINT [FK_EmpLeaveBalances_Employees] FOREIGN KEY([EmpId])
REFERENCES [dbo].[Employees] ([Id])
GO
ALTER TABLE [dbo].[EmpLeaveBalance] CHECK CONSTRAINT [FK_EmpLeaveBalances_Employees]
GO
ALTER TABLE [dbo].[EmpLeaveBalance]  WITH CHECK ADD  CONSTRAINT [FK_EmpLeaveBalances_LeaveTypes1] FOREIGN KEY([LeaveTypeId])
REFERENCES [dbo].[LeaveTypes] ([Id])
GO
ALTER TABLE [dbo].[EmpLeaveBalance] CHECK CONSTRAINT [FK_EmpLeaveBalances_LeaveTypes1]
GO
ALTER TABLE [dbo].[EmployeeHistory]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeHistory_Employees] FOREIGN KEY([EmpId])
REFERENCES [dbo].[Employees] ([Id])
GO
ALTER TABLE [dbo].[EmployeeHistory] CHECK CONSTRAINT [FK_EmployeeHistory_Employees]
GO
ALTER TABLE [dbo].[Employees]  WITH CHECK ADD  CONSTRAINT [FK_Employees_Branches] FOREIGN KEY([Branch])
REFERENCES [dbo].[Branches] ([Id])
GO
ALTER TABLE [dbo].[Employees] CHECK CONSTRAINT [FK_Employees_Branches]
GO
ALTER TABLE [dbo].[Employees]  WITH CHECK ADD  CONSTRAINT [FK_Employees_Departments] FOREIGN KEY([Department])
REFERENCES [dbo].[Departments] ([Id])
GO
ALTER TABLE [dbo].[Employees] CHECK CONSTRAINT [FK_Employees_Departments]
GO
ALTER TABLE [dbo].[Employees]  WITH CHECK ADD  CONSTRAINT [FK_Employees_Designations] FOREIGN KEY([JoinedDesignation])
REFERENCES [dbo].[Designations] ([Id])
GO
ALTER TABLE [dbo].[Employees] CHECK CONSTRAINT [FK_Employees_Designations]
GO
ALTER TABLE [dbo].[Employees]  WITH CHECK ADD  CONSTRAINT [FK_Employees_Roles] FOREIGN KEY([Role])
REFERENCES [dbo].[Roles] ([Id])
GO
ALTER TABLE [dbo].[Employees] CHECK CONSTRAINT [FK_Employees_Roles]
GO
ALTER TABLE [dbo].[LeaveInfo]  WITH CHECK ADD  CONSTRAINT [FK_LeaveInfo_Employees] FOREIGN KEY([EmpId])
REFERENCES [dbo].[Employees] ([Id])
GO
ALTER TABLE [dbo].[LeaveInfo] CHECK CONSTRAINT [FK_LeaveInfo_Employees]
GO
ALTER TABLE [dbo].[LeaveInfo]  WITH CHECK ADD  CONSTRAINT [FK_LeaveInfo_Leaves] FOREIGN KEY([LeaveId])
REFERENCES [dbo].[Leaves] ([Id])
GO
ALTER TABLE [dbo].[LeaveInfo] CHECK CONSTRAINT [FK_LeaveInfo_Leaves]
GO
ALTER TABLE [dbo].[LeaveInfo]  WITH CHECK ADD  CONSTRAINT [FK_LeaveInfo_LeaveTypes] FOREIGN KEY([LeaveType])
REFERENCES [dbo].[LeaveTypes] ([Id])
GO
ALTER TABLE [dbo].[LeaveInfo] CHECK CONSTRAINT [FK_LeaveInfo_LeaveTypes]
GO
ALTER TABLE [dbo].[Leaves]  WITH CHECK ADD  CONSTRAINT [FK_Leaves_Employees] FOREIGN KEY([EmpId])
REFERENCES [dbo].[Employees] ([Id])
GO
ALTER TABLE [dbo].[Leaves] CHECK CONSTRAINT [FK_Leaves_Employees]
GO
ALTER TABLE [dbo].[Leaves]  WITH CHECK ADD  CONSTRAINT [FK_Leaves_Employees1] FOREIGN KEY([SanctioningAuthority])
REFERENCES [dbo].[Employees] ([Id])
GO
ALTER TABLE [dbo].[Leaves] CHECK CONSTRAINT [FK_Leaves_Employees1]
GO
ALTER TABLE [dbo].[Leaves]  WITH CHECK ADD  CONSTRAINT [FK_Leaves_Employees2] FOREIGN KEY([ControllingAuthority])
REFERENCES [dbo].[Employees] ([Id])
GO
ALTER TABLE [dbo].[Leaves] CHECK CONSTRAINT [FK_Leaves_Employees2]
GO
ALTER TABLE [dbo].[WorkingDays]  WITH CHECK ADD  CONSTRAINT [FK_WorkingDays_Employees] FOREIGN KEY([EmpId])
REFERENCES [dbo].[Employees] ([Id])
GO
ALTER TABLE [dbo].[WorkingDays] CHECK CONSTRAINT [FK_WorkingDays_Employees]
GO
ALTER TABLE [dbo].[WorkingDays]  WITH CHECK ADD  CONSTRAINT [FK_WorkingDays_Employees1] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[Employees] ([Id])
GO
ALTER TABLE [dbo].[WorkingDays] CHECK CONSTRAINT [FK_WorkingDays_Employees1]
GO
/****** Object:  StoredProcedure [dbo].[GetLeavesHistory]    Script Date: 1/9/2018 12:49:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[GetLeavesHistory]
(
@startDate datetime,
@endDate datetime,
@leaveType varchar(50))
as begin
select l.StartDate,l.EndDate,lf.TotalDays,l.leaveType,l.Reason,l.Status from leaves l INNER JOIN LeaveInfo lf on l.Id=lf.LeaveId
 where  
 l.StartDate= @startDate 
 or l.EndDate= @endDate 
 or l.leaveType=@leaveType
 or l.EmpID=1
end



GO
/****** Object:  StoredProcedure [dbo].[HolidayCount]    Script Date: 1/9/2018 12:49:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[HolidayCount]
(@fromdate datetime, @todate datetime)
as
begin

declare @date int
select @date = count(date) from HolidayList where 
date >= @fromdate and date<=@todate
print @date

end



GO
