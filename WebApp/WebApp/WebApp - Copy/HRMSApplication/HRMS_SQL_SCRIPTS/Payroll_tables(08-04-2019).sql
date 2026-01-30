
// PR_MonthDetails

USE [tscab_timesheetlogs_Mar27]
GO

/****** Object:  Table [dbo].[PR_Monthdetails]    Script Date: 08-04-19 3:55:33 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PR_Monthdetails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FY] [int] NULL,
	[FM] [date] NULL,
	[Weekholidays] [int] NULL,
	[Paidholidays] [int] NULL,
	[Paymentdate] [date] NULL,
	[DAslabs] [int] NULL,
	[DApoints] [float] NULL,
	[DApercent] [float] NULL,
	[Active] [bit] NULL,
	[Trans_id] [int] NULL,
 CONSTRAINT [PK_PR_Monthdetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[PR_Monthdetails] ADD  CONSTRAINT [DF_PR_Monthdetails_Active]  DEFAULT ((1)) FOR [Active]
GO



//PR_Monthattendance

USE [tscab_timesheetlogs_Mar27]
GO

/****** Object:  Table [dbo].[PR_Monthattendance]    Script Date: 08-04-19 4:01:01 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PR_Monthattendance](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FY] [int] NULL,
	[FM] [date] NULL,
	[EmpId] [int] NULL,
	[Status] [nvarchar](50) NULL,
	[Statusdate] [date] NULL,
	[Leavesavailable] [nvarchar](200) NULL,
	[LOPdays] [float] NULL,
	[Absentdays] [float] NULL,
	[Workingdays] [float] NULL,
	[Active] [bit] NULL,
	[Trans_id] [int] NULL,
 CONSTRAINT [PK_PR_Monthattendance] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[PR_Monthattendance] ADD  CONSTRAINT [DF_PR_Monthattendance_Active]  DEFAULT ((1)) FOR [Active]
GO


//PR_Payfieldmaster

USE [tscab_timesheetlogs_Mar27]
GO

/****** Object:  Table [dbo].[PR_Payfieldmaster]    Script Date: 08-04-19 3:35:28 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[PR_Payfieldmaster](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Payfieldtype] [varchar](10) NULL,
	[Payfieldname] [nvarchar](50) NULL,
	[Active] [bit] NULL CONSTRAINT [DF_PR_Payfieldmaster_Active]  DEFAULT ((1)),
	[Trans_id] [int] NULL,
 CONSTRAINT [PK_PR_Payfieldmaster] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO



//PR_Emppayfields

USE [tscab_timesheetlogs_Mar27]
GO

/****** Object:  Table [dbo].[PR_Emppayfields]    Script Date: 08-04-19 3:44:36 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PR_Emppayfields](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FY] [int] NULL,
	[FM] [date] NULL,
	[EmpId] [int] NULL,
	[PFMId] [int] NULL,
	[PFMType] [nvarchar](10) NULL,
	[Amount] [float] NULL,
	[Active] [bit] NULL CONSTRAINT [DF_PR_Emppayfields_Active]  DEFAULT ((1)),
	[Trans_id] [int] NULL,
 CONSTRAINT [PK_PR_Emppayfields] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO



//PR_Adjustpayments

USE [tscab_timesheetlogs_Mar27]
GO

/****** Object:  Table [dbo].[PR_Adjustpayments]    Script Date: 08-04-19 3:47:34 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PR_Adjustpayments](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FY] [int] NULL,
	[FM] [date] NULL,
	[EmpId] [int] NULL,
	[PFMId] [int] NULL,
	[PFMType] [nvarchar](10) NULL,
	[Amount] [float] NULL,
	[Active] [bit] NULL,
	[Trans_id] [int] NULL,
 CONSTRAINT [PK_PR_Adjustpayments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[PR_Adjustpayments] ADD  CONSTRAINT [DF_PR_Adjustpayments_Active]  DEFAULT ((1)) FOR [Active]
GO








