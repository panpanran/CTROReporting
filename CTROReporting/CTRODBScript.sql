USE [master]
GO
/****** Object:  Database [CTROReporting]    Script Date: 3/29/2019 7:50:32 AM ******/
CREATE DATABASE [CTROReporting]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'AttitudeLoose', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\AttitudeLoose.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'AttitudeLoose_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\AttitudeLoose_log.ldf' , SIZE = 26816KB , MAXSIZE = UNLIMITED, FILEGROWTH = 10%)
GO
ALTER DATABASE [CTROReporting] SET COMPATIBILITY_LEVEL = 130
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [CTROReporting].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [CTROReporting] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [CTROReporting] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [CTROReporting] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [CTROReporting] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [CTROReporting] SET ARITHABORT OFF 
GO
ALTER DATABASE [CTROReporting] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [CTROReporting] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [CTROReporting] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [CTROReporting] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [CTROReporting] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [CTROReporting] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [CTROReporting] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [CTROReporting] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [CTROReporting] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [CTROReporting] SET  DISABLE_BROKER 
GO
ALTER DATABASE [CTROReporting] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [CTROReporting] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [CTROReporting] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [CTROReporting] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [CTROReporting] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [CTROReporting] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [CTROReporting] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [CTROReporting] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [CTROReporting] SET  MULTI_USER 
GO
ALTER DATABASE [CTROReporting] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [CTROReporting] SET DB_CHAINING OFF 
GO
ALTER DATABASE [CTROReporting] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [CTROReporting] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [CTROReporting] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [CTROReporting] SET QUERY_STORE = OFF
GO
USE [CTROReporting]
GO
/****** Object:  Schema [HangFire]    Script Date: 3/29/2019 7:50:32 AM ******/
CREATE SCHEMA [HangFire]
GO
/****** Object:  Table [dbo].[__MigrationHistory]    Script Date: 3/29/2019 7:50:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__MigrationHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ContextKey] [nvarchar](300) NOT NULL,
	[Model] [varbinary](max) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK_dbo.__MigrationHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC,
	[ContextKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 3/29/2019 7:50:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 3/29/2019 7:50:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 3/29/2019 7:50:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](128) NOT NULL,
	[ProviderKey] [nvarchar](128) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 3/29/2019 7:50:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](128) NOT NULL,
	[RoleId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 3/29/2019 7:50:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](128) NOT NULL,
	[LastLoginTime] [datetime] NULL,
	[Activated] [bit] NOT NULL,
	[RoleId] [int] NOT NULL,
	[Email] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEndDateUtc] [datetime] NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[UserName] [nvarchar](256) NOT NULL,
	[DepartmentId] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Charts]    Script Date: 3/29/2019 7:50:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Charts](
	[ChartId] [int] IDENTITY(1,1) NOT NULL,
	[DepartmentId] [int] NOT NULL,
	[ChartName] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Charts] PRIMARY KEY CLUSTERED 
(
	[ChartId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ChartSettings]    Script Date: 3/29/2019 7:50:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChartSettings](
	[ChartSettingId] [int] IDENTITY(1,1) NOT NULL,
	[ChartId] [int] NOT NULL,
	[Code] [nvarchar](max) NULL,
	[Category] [nvarchar](max) NULL,
	[ChartType] [nvarchar](max) NULL,
	[XLabel] [nvarchar](max) NULL,
	[YLabel] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.ChartSettings] PRIMARY KEY CLUSTERED 
(
	[ChartSettingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Departments]    Script Date: 3/29/2019 7:50:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Departments](
	[DepartmentId] [int] IDENTITY(1,1) NOT NULL,
	[DepartmentName] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Departments] PRIMARY KEY CLUSTERED 
(
	[DepartmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Emails]    Script Date: 3/29/2019 7:50:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Emails](
	[EmailId] [int] IDENTITY(1,1) NOT NULL,
	[Template] [nvarchar](max) NULL,
	[Name] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Emails] PRIMARY KEY CLUSTERED 
(
	[EmailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Fiscalyear]    Script Date: 3/29/2019 7:50:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Fiscalyear](
	[Fid] [int] IDENTITY(1,1) NOT NULL,
	[FiscalyearFromDate] [datetime] NULL,
	[FiscalyearToDate] [datetime] NULL,
	[Year] [varchar](4) NULL,
 CONSTRAINT [PK_Fiscalyear] PRIMARY KEY CLUSTERED 
(
	[Fid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Loggers]    Script Date: 3/29/2019 7:50:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Loggers](
	[LogId] [int] IDENTITY(1,1) NOT NULL,
	[Level] [int] NOT NULL,
	[ClassName] [nvarchar](max) NULL,
	[MethodName] [nvarchar](max) NULL,
	[Message] [nvarchar](max) NULL,
	[UserId] [nvarchar](128) NULL,
	[CreatedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.Loggers] PRIMARY KEY CLUSTERED 
(
	[LogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MemberRegistration]    Script Date: 3/29/2019 7:50:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MemberRegistration](
	[MemberId] [bigint] IDENTITY(1,1) NOT NULL,
	[MemberNo] [nvarchar](20) NULL,
	[MemberFName] [nvarchar](100) NULL,
	[MemberLName] [nvarchar](100) NULL,
	[MemberMName] [nvarchar](100) NULL,
	[DOB] [datetime] NULL,
	[Age] [nvarchar](10) NULL,
	[Contactno] [nvarchar](10) NULL,
	[EmailID] [nvarchar](30) NULL,
	[Gender] [nvarchar](30) NULL,
	[PlanID] [int] NULL,
	[SchemeID] [int] NULL,
	[Createdby] [bigint] NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [bigint] NULL,
	[MemImagename] [nvarchar](500) NULL,
	[MemImagePath] [nvarchar](500) NULL,
	[JoiningDate] [datetime] NULL,
	[Address] [nvarchar](500) NULL,
	[MainMemberID] [bigint] NULL,
PRIMARY KEY CLUSTERED 
(
	[MemberId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PaymentDetails]    Script Date: 3/29/2019 7:50:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PaymentDetails](
	[PaymentID] [bigint] IDENTITY(1,1) NOT NULL,
	[PlanID] [int] NULL,
	[WorkouttypeID] [int] NULL,
	[Paymenttype] [nvarchar](50) NULL,
	[PaymentFromdt] [datetime] NULL,
	[PaymentTodt] [datetime] NULL,
	[PaymentAmount] [numeric](18, 0) NULL,
	[NextRenwalDate] [datetime] NULL,
	[CreateDate] [datetime] NULL,
	[Createdby] [int] NULL,
	[ModifyDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[RecStatus] [char](1) NULL,
	[MemberID] [bigint] NULL,
	[MemberNo] [nvarchar](30) NULL,
 CONSTRAINT [PK_PaymentDetails] PRIMARY KEY CLUSTERED 
(
	[PaymentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PeriodTB]    Script Date: 3/29/2019 7:50:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PeriodTB](
	[PeriodID] [int] IDENTITY(1,1) NOT NULL,
	[Text] [varchar](50) NULL,
	[Value] [varchar](50) NULL,
 CONSTRAINT [PK_PeriodTB] PRIMARY KEY CLUSTERED 
(
	[PeriodID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PlanMaster]    Script Date: 3/29/2019 7:50:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PlanMaster](
	[PlanID] [int] IDENTITY(1,1) NOT NULL,
	[PlanName] [varchar](50) NULL,
	[PlanAmount] [decimal](18, 0) NULL,
	[ServicetaxAmount] [decimal](18, 0) NULL,
	[ServiceTax] [varchar](50) NULL,
	[CreateDate] [datetime] NULL,
	[CreateUserID] [int] NULL,
	[ModifyDate] [datetime] NULL,
	[ModifyUserID] [int] NULL,
	[RecStatus] [bit] NULL,
	[SchemeID] [int] NULL,
	[PeriodID] [int] NULL,
	[TotalAmount] [decimal](18, 0) NULL,
	[ServicetaxNo] [varchar](50) NULL,
 CONSTRAINT [PK_PlanMaster] PRIMARY KEY CLUSTERED 
(
	[PlanID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Records]    Script Date: 3/29/2019 7:50:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Records](
	[RecordId] [int] IDENTITY(1,1) NOT NULL,
	[ReportId] [int] NOT NULL,
	[UserId] [nvarchar](max) NULL,
	[StartDate] [nvarchar](max) NULL,
	[EndDate] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[FilePath] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Records] PRIMARY KEY CLUSTERED 
(
	[RecordId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Reports]    Script Date: 3/29/2019 7:50:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Reports](
	[ReportId] [int] IDENTITY(1,1) NOT NULL,
	[DepartmentId] [int] NOT NULL,
	[ReportName] [nvarchar](max) NULL,
	[Template] [nvarchar](max) NULL,
	[Savepath] [nvarchar](max) NULL,
	[Active] [bit] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[EmailId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.Reports] PRIMARY KEY CLUSTERED 
(
	[ReportId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReportSettings]    Script Date: 3/29/2019 7:50:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportSettings](
	[ReportSettingId] [int] IDENTITY(1,1) NOT NULL,
	[ReportId] [int] NOT NULL,
	[Code] [nvarchar](max) NULL,
	[Category] [nvarchar](max) NULL,
	[Startrow] [int] NOT NULL,
	[Startcolumn] [int] NOT NULL,
	[AdditionStartrow] [int] NOT NULL,
	[AdditionStartcolumn] [int] NOT NULL,
	[ReportType] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.ReportSettings] PRIMARY KEY CLUSTERED 
(
	[ReportSettingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 3/29/2019 7:50:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[RoleId] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](256) NOT NULL,
	[Status] [bit] NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Schedules]    Script Date: 3/29/2019 7:50:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Schedules](
	[ScheduleId] [int] IDENTITY(1,1) NOT NULL,
	[StartTime] [datetime] NOT NULL,
	[IntervalDays] [int] NOT NULL,
	[ReportId] [int] NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.Schedules] PRIMARY KEY CLUSTERED 
(
	[ScheduleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SchemeMaster]    Script Date: 3/29/2019 7:50:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SchemeMaster](
	[SchemeID] [int] IDENTITY(1,1) NOT NULL,
	[SchemeName] [nvarchar](50) NULL,
	[Createdby] [int] NULL,
	[Createddate] [datetime] NULL,
	[Status] [bit] NULL,
 CONSTRAINT [PK_SchemeMaster] PRIMARY KEY CLUSTERED 
(
	[SchemeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserProfiles]    Script Date: 3/29/2019 7:50:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserProfiles](
	[UserProfileId] [int] IDENTITY(1,1) NOT NULL,
	[DateEdited] [datetime] NOT NULL,
	[Email] [nvarchar](max) NULL,
	[FirstName] [nvarchar](100) NULL,
	[LastName] [nvarchar](max) NULL,
	[ProfilePicUrl] [nvarchar](max) NULL,
	[DateOfBirth] [datetime] NULL,
	[Gender] [bit] NULL,
	[Address] [nvarchar](max) NULL,
	[City] [nvarchar](100) NULL,
	[State] [nvarchar](50) NULL,
	[Country] [nvarchar](50) NULL,
	[ZipCode] [float] NULL,
	[ContactNo] [float] NULL,
	[Id] [nvarchar](50) NULL,
	[ApplicationUser_Id] [nvarchar](128) NULL,
 CONSTRAINT [PK_dbo.UserProfiles] PRIMARY KEY CLUSTERED 
(
	[UserProfileId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 3/29/2019 7:50:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](56) NOT NULL,
	[FullName] [nvarchar](200) NULL,
	[EmailId] [nvarchar](200) NULL,
	[Contactno] [nvarchar](10) NULL,
	[Password] [nvarchar](200) NULL,
	[Createdby] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[Status] [bit] NULL,
 CONSTRAINT [PK__Users__3214EC070F975522] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UsersInRoles]    Script Date: 3/29/2019 7:50:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UsersInRoles](
	[UserRolesId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[RoleId] [int] NULL,
 CONSTRAINT [PK_UsersInRoles] PRIMARY KEY CLUSTERED 
(
	[UserRolesId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [HangFire].[AggregatedCounter]    Script Date: 3/29/2019 7:50:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [HangFire].[AggregatedCounter](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Key] [nvarchar](100) NOT NULL,
	[Value] [bigint] NOT NULL,
	[ExpireAt] [datetime] NULL,
 CONSTRAINT [PK_HangFire_CounterAggregated] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [HangFire].[Counter]    Script Date: 3/29/2019 7:50:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [HangFire].[Counter](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Key] [nvarchar](100) NOT NULL,
	[Value] [smallint] NOT NULL,
	[ExpireAt] [datetime] NULL,
 CONSTRAINT [PK_HangFire_Counter] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [HangFire].[Hash]    Script Date: 3/29/2019 7:50:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [HangFire].[Hash](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Key] [nvarchar](100) NOT NULL,
	[Field] [nvarchar](100) NOT NULL,
	[Value] [nvarchar](max) NULL,
	[ExpireAt] [datetime2](7) NULL,
 CONSTRAINT [PK_HangFire_Hash] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [HangFire].[Job]    Script Date: 3/29/2019 7:50:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [HangFire].[Job](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StateId] [int] NULL,
	[StateName] [nvarchar](20) NULL,
	[InvocationData] [nvarchar](max) NOT NULL,
	[Arguments] [nvarchar](max) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[ExpireAt] [datetime] NULL,
 CONSTRAINT [PK_HangFire_Job] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [HangFire].[JobParameter]    Script Date: 3/29/2019 7:50:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [HangFire].[JobParameter](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[JobId] [int] NOT NULL,
	[Name] [nvarchar](40) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_HangFire_JobParameter] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [HangFire].[JobQueue]    Script Date: 3/29/2019 7:50:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [HangFire].[JobQueue](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[JobId] [int] NOT NULL,
	[Queue] [nvarchar](50) NOT NULL,
	[FetchedAt] [datetime] NULL,
 CONSTRAINT [PK_HangFire_JobQueue] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [HangFire].[List]    Script Date: 3/29/2019 7:50:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [HangFire].[List](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Key] [nvarchar](100) NOT NULL,
	[Value] [nvarchar](max) NULL,
	[ExpireAt] [datetime] NULL,
 CONSTRAINT [PK_HangFire_List] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [HangFire].[Schema]    Script Date: 3/29/2019 7:50:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [HangFire].[Schema](
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_HangFire_Schema] PRIMARY KEY CLUSTERED 
(
	[Version] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [HangFire].[Server]    Script Date: 3/29/2019 7:50:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [HangFire].[Server](
	[Id] [nvarchar](100) NOT NULL,
	[Data] [nvarchar](max) NULL,
	[LastHeartbeat] [datetime] NOT NULL,
 CONSTRAINT [PK_HangFire_Server] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [HangFire].[Set]    Script Date: 3/29/2019 7:50:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [HangFire].[Set](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Key] [nvarchar](100) NOT NULL,
	[Score] [float] NOT NULL,
	[Value] [nvarchar](256) NOT NULL,
	[ExpireAt] [datetime] NULL,
 CONSTRAINT [PK_HangFire_Set] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [HangFire].[State]    Script Date: 3/29/2019 7:50:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [HangFire].[State](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[JobId] [int] NOT NULL,
	[Name] [nvarchar](20) NOT NULL,
	[Reason] [nvarchar](100) NULL,
	[CreatedAt] [datetime] NOT NULL,
	[Data] [nvarchar](max) NULL,
 CONSTRAINT [PK_HangFire_State] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [RoleNameIndex]    Script Date: 3/29/2019 7:50:35 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] ON [dbo].[AspNetRoles]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_UserId]    Script Date: 3/29/2019 7:50:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserClaims]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_UserId]    Script Date: 3/29/2019 7:50:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserLogins]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_RoleId]    Script Date: 3/29/2019 7:50:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_RoleId] ON [dbo].[AspNetUserRoles]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_UserId]    Script Date: 3/29/2019 7:50:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserRoles]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_DepartmentId]    Script Date: 3/29/2019 7:50:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_DepartmentId] ON [dbo].[AspNetUsers]
(
	[DepartmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UserNameIndex]    Script Date: 3/29/2019 7:50:36 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[AspNetUsers]
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_DepartmentId]    Script Date: 3/29/2019 7:50:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_DepartmentId] ON [dbo].[Charts]
(
	[DepartmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ChartId]    Script Date: 3/29/2019 7:50:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_ChartId] ON [dbo].[ChartSettings]
(
	[ChartId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_UserId]    Script Date: 3/29/2019 7:50:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[Loggers]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ReportId]    Script Date: 3/29/2019 7:50:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_ReportId] ON [dbo].[Records]
(
	[ReportId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_EmailId]    Script Date: 3/29/2019 7:50:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_EmailId] ON [dbo].[Reports]
(
	[EmailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ReportId]    Script Date: 3/29/2019 7:50:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_ReportId] ON [dbo].[ReportSettings]
(
	[ReportId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ReportId]    Script Date: 3/29/2019 7:50:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_ReportId] ON [dbo].[Schedules]
(
	[ReportId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_User_Id]    Script Date: 3/29/2019 7:50:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_User_Id] ON [dbo].[Schedules]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ApplicationUser_Id]    Script Date: 3/29/2019 7:50:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_ApplicationUser_Id] ON [dbo].[UserProfiles]
(
	[ApplicationUser_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UX_HangFire_CounterAggregated_Key]    Script Date: 3/29/2019 7:50:36 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UX_HangFire_CounterAggregated_Key] ON [HangFire].[AggregatedCounter]
(
	[Key] ASC
)
INCLUDE ( 	[Value]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_HangFire_Counter_Key]    Script Date: 3/29/2019 7:50:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_HangFire_Counter_Key] ON [HangFire].[Counter]
(
	[Key] ASC
)
INCLUDE ( 	[Value]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_HangFire_Hash_ExpireAt]    Script Date: 3/29/2019 7:50:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_HangFire_Hash_ExpireAt] ON [HangFire].[Hash]
(
	[ExpireAt] ASC
)
INCLUDE ( 	[Id]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_HangFire_Hash_Key]    Script Date: 3/29/2019 7:50:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_HangFire_Hash_Key] ON [HangFire].[Hash]
(
	[Key] ASC
)
INCLUDE ( 	[ExpireAt]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UX_HangFire_Hash_Key_Field]    Script Date: 3/29/2019 7:50:36 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UX_HangFire_Hash_Key_Field] ON [HangFire].[Hash]
(
	[Key] ASC,
	[Field] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_HangFire_Job_ExpireAt]    Script Date: 3/29/2019 7:50:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_HangFire_Job_ExpireAt] ON [HangFire].[Job]
(
	[ExpireAt] ASC
)
INCLUDE ( 	[Id]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_HangFire_Job_StateName]    Script Date: 3/29/2019 7:50:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_HangFire_Job_StateName] ON [HangFire].[Job]
(
	[StateName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_HangFire_JobParameter_JobIdAndName]    Script Date: 3/29/2019 7:50:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_HangFire_JobParameter_JobIdAndName] ON [HangFire].[JobParameter]
(
	[JobId] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_HangFire_JobQueue_QueueAndFetchedAt]    Script Date: 3/29/2019 7:50:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_HangFire_JobQueue_QueueAndFetchedAt] ON [HangFire].[JobQueue]
(
	[Queue] ASC,
	[FetchedAt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_HangFire_List_ExpireAt]    Script Date: 3/29/2019 7:50:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_HangFire_List_ExpireAt] ON [HangFire].[List]
(
	[ExpireAt] ASC
)
INCLUDE ( 	[Id]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_HangFire_List_Key]    Script Date: 3/29/2019 7:50:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_HangFire_List_Key] ON [HangFire].[List]
(
	[Key] ASC
)
INCLUDE ( 	[ExpireAt],
	[Value]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_HangFire_Set_ExpireAt]    Script Date: 3/29/2019 7:50:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_HangFire_Set_ExpireAt] ON [HangFire].[Set]
(
	[ExpireAt] ASC
)
INCLUDE ( 	[Id]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_HangFire_Set_Key]    Script Date: 3/29/2019 7:50:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_HangFire_Set_Key] ON [HangFire].[Set]
(
	[Key] ASC
)
INCLUDE ( 	[ExpireAt],
	[Value]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UX_HangFire_Set_KeyAndValue]    Script Date: 3/29/2019 7:50:36 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UX_HangFire_Set_KeyAndValue] ON [HangFire].[Set]
(
	[Key] ASC,
	[Value] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_HangFire_State_JobId]    Script Date: 3/29/2019 7:50:36 AM ******/
CREATE NONCLUSTERED INDEX [IX_HangFire_State_JobId] ON [HangFire].[State]
(
	[JobId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AspNetUsers] ADD  DEFAULT ('1900-01-01T00:00:00.000') FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Fiscalyear] ADD  CONSTRAINT [DF_Fiscalyear_Year]  DEFAULT (NULL) FOR [Year]
GO
ALTER TABLE [dbo].[Records] ADD  DEFAULT ((0)) FOR [ReportId]
GO
ALTER TABLE [dbo].[Reports] ADD  DEFAULT ((0)) FOR [DepartmentId]
GO
ALTER TABLE [dbo].[Reports] ADD  DEFAULT ((0)) FOR [Active]
GO
ALTER TABLE [dbo].[ReportSettings] ADD  DEFAULT ((0)) FOR [Startrow]
GO
ALTER TABLE [dbo].[ReportSettings] ADD  DEFAULT ((0)) FOR [Startcolumn]
GO
ALTER TABLE [dbo].[ReportSettings] ADD  DEFAULT ((0)) FOR [AdditionStartrow]
GO
ALTER TABLE [dbo].[ReportSettings] ADD  DEFAULT ((0)) FOR [AdditionStartcolumn]
GO
ALTER TABLE [dbo].[Schedules] ADD  DEFAULT ('1900-01-01T00:00:00.000') FOR [CreatedDate]
GO
ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUsers]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUsers_dbo.Departments_DepartmentId] FOREIGN KEY([DepartmentId])
REFERENCES [dbo].[Departments] ([DepartmentId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUsers] CHECK CONSTRAINT [FK_dbo.AspNetUsers_dbo.Departments_DepartmentId]
GO
ALTER TABLE [dbo].[Charts]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Charts_dbo.Departments_DepartmentId] FOREIGN KEY([DepartmentId])
REFERENCES [dbo].[Departments] ([DepartmentId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Charts] CHECK CONSTRAINT [FK_dbo.Charts_dbo.Departments_DepartmentId]
GO
ALTER TABLE [dbo].[ChartSettings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ChartSettings_dbo.Charts_ChartId] FOREIGN KEY([ChartId])
REFERENCES [dbo].[Charts] ([ChartId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ChartSettings] CHECK CONSTRAINT [FK_dbo.ChartSettings_dbo.Charts_ChartId]
GO
ALTER TABLE [dbo].[Loggers]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Loggers_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[Loggers] CHECK CONSTRAINT [FK_dbo.Loggers_dbo.AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[Records]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Records_dbo.Reports_ReportId] FOREIGN KEY([ReportId])
REFERENCES [dbo].[Reports] ([ReportId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Records] CHECK CONSTRAINT [FK_dbo.Records_dbo.Reports_ReportId]
GO
ALTER TABLE [dbo].[Reports]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Reports_dbo.Emails_EmailId] FOREIGN KEY([EmailId])
REFERENCES [dbo].[Emails] ([EmailId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Reports] CHECK CONSTRAINT [FK_dbo.Reports_dbo.Emails_EmailId]
GO
ALTER TABLE [dbo].[ReportSettings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ReportSettings_dbo.Reports_ReportId] FOREIGN KEY([ReportId])
REFERENCES [dbo].[Reports] ([ReportId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ReportSettings] CHECK CONSTRAINT [FK_dbo.ReportSettings_dbo.Reports_ReportId]
GO
ALTER TABLE [dbo].[Schedules]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Schedules_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[Schedules] CHECK CONSTRAINT [FK_dbo.Schedules_dbo.AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[Schedules]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Schedules_dbo.Reports_ReportId] FOREIGN KEY([ReportId])
REFERENCES [dbo].[Reports] ([ReportId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Schedules] CHECK CONSTRAINT [FK_dbo.Schedules_dbo.Reports_ReportId]
GO
ALTER TABLE [dbo].[UserProfiles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UserProfiles_dbo.AspNetUsers_ApplicationUser_Id] FOREIGN KEY([ApplicationUser_Id])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[UserProfiles] CHECK CONSTRAINT [FK_dbo.UserProfiles_dbo.AspNetUsers_ApplicationUser_Id]
GO
ALTER TABLE [HangFire].[JobParameter]  WITH CHECK ADD  CONSTRAINT [FK_HangFire_JobParameter_Job] FOREIGN KEY([JobId])
REFERENCES [HangFire].[Job] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [HangFire].[JobParameter] CHECK CONSTRAINT [FK_HangFire_JobParameter_Job]
GO
ALTER TABLE [HangFire].[State]  WITH CHECK ADD  CONSTRAINT [FK_HangFire_State_Job] FOREIGN KEY([JobId])
REFERENCES [HangFire].[Job] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [HangFire].[State] CHECK CONSTRAINT [FK_HangFire_State_Job]
GO
/****** Object:  StoredProcedure [dbo].[GetRecepit]    Script Date: 3/29/2019 7:50:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Proc [dbo].[GetRecepit]

@PaymentID int

as

begin

select 

MR.MemberNo,

MR.MemberFName+' '+ MR.MemberMName +' '+MR.MemberLName  as MemberName,

SM.SchemeName,

PM.PlanName,

CONVERT(varchar(10), PD.PaymentFromdt, 105) AS PaymentFromdt,

CONVERT(varchar(10), PD.PaymentTodt, 105) AS PaymentTodt,

CONVERT(varchar(10), PD.NextRenwalDate, 105) AS NextRenwalDate,

PM.ServiceTax,

PD.PaymentAmount,

PM.PlanAmount,

PM.ServicetaxAmount,


CONVERT(varchar(10), PD.CreateDate, 105) AS CreateDate

from MemberRegistration MR

inner join PaymentDetails PD on MR.MemberId = PD.MemberID

INNER JOIN SchemeMaster SM ON MR.SchemeID = sm.SchemeID

INNER JOIN PlanMaster PM ON MR.PlanID = PM.PlanID

where PD.PaymentID =@PaymentID

end




/****** Object:  StoredProcedure [dbo].[sprocMemberRegistrationSelectSingleItem]    Script Date: 12/28/2018 AM 09:25:38 ******/
SET ANSI_NULLS ON

GO
/****** Object:  StoredProcedure [dbo].[sprocMemberRegistrationDeleteSingleItem]    Script Date: 3/29/2019 7:50:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sprocMemberRegistrationDeleteSingleItem]      

(      

 @MemberId bigint      

)      

AS      

BEGIN



DELETE FROM [MemberRegistration]

WHERE MemberID = @MemberId



DELETE FROM PaymentDetails

WHERE MemberID = @MemberId

    

END





GO
/****** Object:  StoredProcedure [dbo].[sprocMemberRegistrationInsertUpdateSingleItem]    Script Date: 3/29/2019 7:50:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create    

 PROCEDURE [dbo].[sprocMemberRegistrationInsertUpdateSingleItem] (    

  @MemberId BIGINT = 0    

 ,@MemberFName NVARCHAR(100) = NULL    

 ,@MemberLName NVARCHAR(100) = NULL    

 ,@MemberMName NVARCHAR(100) = NULL    

 ,@Address NVARCHAR(500) = NULL    

 ,@DOB DATETIME    

 ,@Age NVARCHAR(10) = NULL    

 ,@Contactno NVARCHAR(10) = NULL    

 ,@EmailID NVARCHAR(30) = NULL    

 ,@Gender NVARCHAR(30) = NULL    

 ,@PlanID INT = 0    

 ,@SchemeID INT = 0    

 ,@Createdby BIGINT = 0    

 ,@ModifiedBy BIGINT = 0    

 ,@JoiningDate DATETIME    

 ,@MemIDOUT int out    

 )    

AS    

DECLARE @ReturnValue INT    

    

IF (@MemberId = 0) -- New Item                    

BEGIN    

 DECLARE @MemberNo VARCHAR(20)    

 DECLARE @MainMemberID VARCHAR(20)    

 DECLARE @tables TABLE (    

  MemberNo VARCHAR(20)    

  ,Memnumber VARCHAR(20)    

  )    

    

 INSERT INTO @tables    

 EXEC Usp_Generatenumber    

    

 SELECT @MemberNo = MemberNo    

  ,@MainMemberID = Memnumber    

 FROM @tables    

    

 INSERT INTO [MemberRegistration] (    

  MemberFName    

  ,MemberLName    

  ,MemberMName    

  ,DOB    

  ,Age    

  ,Contactno    

  ,EmailID    

  ,Gender    

  ,PlanID    

  ,SchemeID    

  ,Createdby    

  ,CreatedDate    

  ,ModifiedBy    

  ,JoiningDate    

  ,Address    

  ,MainMemberID    

  ,MemberNo    

  )    

 VALUES (    

  @MemberFName    

  ,@MemberLName    

  ,@MemberMName    

  ,@DOB    

  ,@Age    

  ,@Contactno    

  ,@EmailID    

  ,@Gender    

  ,@PlanID

  ,@SchemeID    

  ,@Createdby    

  ,GETDATE()    

  ,@ModifiedBy    

  ,CONVERT(date,@JoiningDate,103)    

  ,@Address    

  ,@MainMemberID    

  ,@MemberNo    

  )    

    

 SELECT @ReturnValue = SCOPE_IDENTITY()    

 set @MemIDOUT =SCOPE_IDENTITY()    

END    

ELSE    

BEGIN    

 UPDATE [MemberRegistration]    

 SET MemberFName = @MemberFName    

  ,MemberLName = @MemberLName    

  ,MemberMName = @MemberMName    

  ,DOB = @DOB    

  ,Age = @Age    

  ,Contactno = @Contactno    

  ,EmailID = @EmailID    

  ,Gender = @Gender    

  ,Createdby = @Createdby    

  ,ModifiedBy = @ModifiedBy    

  ,Address = @Address    

 WHERE MemberRegistration.MemberId = @MemberId    

   
 SELECT @ReturnValue = @MemberId    
  set @MemIDOUT =@MemberId    

  

END    

    

IF (@@ERROR != 0)    

BEGIN    

 RETURN - 1    

END    

ELSE    

BEGIN    

 RETURN @ReturnValue    

     

END




GO
/****** Object:  StoredProcedure [dbo].[sprocMemberRegistrationSelectList]    Script Date: 3/29/2019 7:50:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create PROCEDURE [dbo].[sprocMemberRegistrationSelectList]    



AS    



BEGIN    



    



 SET NOCOUNT ON    



 DECLARE @Err int    



    



 SELECT    



  MemberRegistration.MemberId    



,MemberRegistration.MemberNo    



,MemberFName    



,MemberLName    



,MemberMName    



,Upper(MemberLName) +' '+Upper(MemberFName) +' '+Upper(MemberMName)  as  MemberName  



,DOB    



,Age    



,Contactno    



,EmailID    



,Gender    



,PM.PlanName



,SM.SchemeName  

 

,ModifiedDate    



,ModifiedBy    



,MemImagename    



,MemImagePath    



,Convert(varchar(10),JoiningDate,103)  as JoiningDate



 FROM [MemberRegistration]  



 Inner Join PlanMaster PM on PM.PlanID = MemberRegistration.PlanID 

 Inner Join SchemeMaster SM on SM.SchemeID =MemberRegistration.SchemeID 

    



 SET @Err = @@Error    



    



 RETURN @Err    



END






GO
/****** Object:  StoredProcedure [dbo].[sprocMemberRegistrationSelectSingleItem]    Script Date: 3/29/2019 7:50:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create PROCEDURE [dbo].[sprocMemberRegistrationSelectSingleItem] (@MemberId BIGINT)
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @Err INT

	SELECT TOP 1 MR.MemberId
		,MR.MemberNo s
		,MR.MemberFName
		,MR.MemberLName
		,MR.MemberMName
		,CONVERT(DATE, DOB, 103) AS DOB
		,Age
		,Contactno
		,EmailID
		,COnvert(INT, Gender) AS Gender
		,MR.PlanID
		,MR.SchemeID
		,MR.Createdby
		,MR.CreatedDate
		,ModifiedDate
		,MR.ModifiedBy
		,MemImagename
		,MemImagePath
		,CONVERT(DATE, JoiningDate, 103) AS JoiningDate
		,pd.PaymentAmount AS Amount
		,Address
		,pd.PaymentID
		,SM.SchemeName
		,PM.PlanName
	FROM [MemberRegistration] MR
	INNER JOIN PaymentDetails pd ON MR.MemberId = pd.MemberID
	INNER JOIN SchemeMaster SM ON MR.SchemeID = sm.SchemeID
	INNER JOIN PlanMaster PM ON MR.PlanID = PM.PlanID
	WHERE (MR.MemberId = @MemberId)
	ORDER BY pd.PaymentID DESC

	SET @Err = @@Error

	RETURN @Err
END



GO
/****** Object:  StoredProcedure [dbo].[sprocPaymentDetailsInsertUpdateSingleItem]    Script Date: 3/29/2019 7:50:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create PROCEDURE [dbo].[sprocPaymentDetailsInsertUpdateSingleItem] (
	@PaymentID BIGINT = 0
	,@PlanID INT = NULL
	,@WorkouttypeID INT = NULL
	,@Paymenttype NVARCHAR(50) = NULL
	,@PaymentFromdt DATETIME = NULL
	,@PaymentAmount NUMERIC(18, 0) = NULL
	,@CreateUserID INT = NULL
	,@ModifyUserID INT = NULL
	,@RecStatus CHAR(1) = NULL
	,@MemberID BIGINT = NULL
	,@PaymentIDOUT INT OUTPUT
	)
AS
DECLARE @ReturnValue INT
DECLARE @period INT
DECLARE @PaymentTodt DATETIME
DECLARE @tempdate DATETIME
DECLARE @NextRenwalDate DATETIME

IF (@PaymentID = 0) -- New Item    
BEGIN
	SET @period = (
			SELECT PeriodID
			FROM PlanMaster
			WHERE PlanID = @PlanID
			)
	SET @tempdate = @PaymentFromdt
	SET @PaymentTodt = DATEADD(mm, @period, @PaymentFromdt)
	SET @NextRenwalDate = DATEADD(mm, @period, @PaymentFromdt)

	INSERT INTO [PaymentDetails] (
		PlanID
		,WorkouttypeID
		,Paymenttype
		,PaymentFromdt
		,PaymentTodt
		,PaymentAmount
		,NextRenwalDate
		,CreateDate
		,Createdby
		,ModifyDate
		,ModifiedBy
		,RecStatus
		,MemberID
		,MemberNo
		)
	VALUES (
		@PlanID
		,@WorkouttypeID
		,@Paymenttype
		,@PaymentFromdt
		,@PaymentTodt
		,@PaymentAmount
		,@NextRenwalDate
		,GETDATE()
		,@CreateUserID
		,NULL
		,@ModifyUserID
		,@RecStatus
		,@MemberID
		,NULL
		)

	SELECT @ReturnValue = SCOPE_IDENTITY()

	SET @PaymentIDOUT = SCOPE_IDENTITY()
END

IF (@@ERROR != 0)
BEGIN
	RETURN - 1
END
ELSE
BEGIN
	RETURN @ReturnValue
END



GO
/****** Object:  StoredProcedure [dbo].[sprocPlanMasterInsertUpdateSingleItem]    Script Date: 3/29/2019 7:50:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create PROCEDURE [dbo].[sprocPlanMasterInsertUpdateSingleItem] (
	@PlanID INT = 0
	,@SchemeID INT = 0
	,@PeriodID INT = 0
	,@PlanName VARCHAR(50) = NULL
	,@PlanAmount DECIMAL(18, 2) = NULL
	,@ServiceTax DECIMAL(18, 2) = NULL
	,@CreateUserID INT = 0
	,@ModifyUserID INT = 0
	,@RecStatus CHAR(1) = NULL
	)
AS
DECLARE @ReturnValue INT
DECLARE @servicetaxAM DECIMAL(18, 2) = 12.36

IF (@PlanID = 0) -- New Item          
BEGIN
	DECLARE @tax DECIMAL(18, 0)

	SET @tax = @PlanAmount * @ServiceTax
	SET @servicetaxAM = @tax / 100

	DECLARE @totalamt DECIMAL(18, 0)

	SET @totalamt = @PlanAmount + @servicetaxAM

	INSERT INTO [PlanMaster] (
		PlanName
		,PlanAmount
		,ServiceTax
		,CreateDate
		,CreateUserID
		,ModifyDate
		,ModifyUserID
		,RecStatus
		,SchemeID
		,PeriodID
		,TotalAmount
		,ServicetaxAmount
		)
	VALUES (
		@PlanName
		,@PlanAmount
		,'14.00'
		,GETDATE()
		,@CreateUserID
		,GETDATE()
		,@ModifyUserID
		,@RecStatus
		,@SchemeID
		,@PeriodID
		,@totalamt
		,@servicetaxAM
		)

	SELECT @ReturnValue = SCOPE_IDENTITY()
END
ELSE
BEGIN
	UPDATE [PlanMaster]
	SET PlanName = @PlanName
		,PlanAmount = @PlanAmount
		,ServiceTax = @ServiceTax
		,CreateDate = GETDATE()
		,CreateUserID = @CreateUserID
		,ModifyDate = GETDATE()
		,ModifyUserID = @ModifyUserID
		,RecStatus = @RecStatus
		,SchemeID = @SchemeID
		,PeriodID = @PeriodID
	WHERE [PlanID] = @PlanID

	SELECT @ReturnValue = @PlanID
END

IF (@@ERROR != 0)
BEGIN
	RETURN - 1
END
ELSE
BEGIN
	RETURN @ReturnValue
END





/****** Object:  StoredProcedure [dbo].[Usp_GetAllRenwalrecords]    Script Date: 12/28/2018 AM 09:30:01 ******/
SET ANSI_NULLS ON

GO
/****** Object:  StoredProcedure [dbo].[Usp_Generatenumber]    Script Date: 3/29/2019 7:50:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[Usp_Generatenumber]        

        

as        

        

begin        

        

BEGIN TRANSACTION        

        

Declare @Memnum nvarchar(4)        

Declare @Name nvarchar(6)        

Declare @MemberNo nvarchar(20)        

Declare @Year nvarchar(4)        

        

set @Year = (select Year FROM Fiscalyear where GETDATE() BETWEEN FiscalyearFromDate AND FiscalyearToDate)        

        

set @Name = 'GYMONE'        

        

set @Memnum = (SELECT Isnull(max(MainMemberID),0) FROM MemberRegistration WITH(HOLDLOCK))        

        

if(@Memnum = 0)        

begin             

set @MemberNo = @Name+'/'+'1'+'/'+@Year   

set @Memnum = 1     

end        

else        

begin        

set @Memnum = (SELECT (max(MainMemberID )+ 1) FROM MemberRegistration WITH(HOLDLOCK))        

set @MemberNo = @Name+'/'+@Memnum+'/'+@Year        

end        

COMMIT TRANSACTION        

        

select @MemberNo as MemberNo  ,@Memnum as Memnumber      

        

end

GO
/****** Object:  StoredProcedure [dbo].[Usp_GetAllRenwalrecords]    Script Date: 3/29/2019 7:50:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create proc [dbo].[Usp_GetAllRenwalrecords]            

         

as            

            

begin            

            

SELECT             

 m.MemberFName + ' | ' + m.MemberLName  AS Name  ,       

 m.Address,      

 m.Contactno,      

 m.EmailID,           

 m.MemberNo,            

 PM.PlanName,            

 SM.SchemeName,            

 CONVERT(varchar, PD.PaymentFromdt, 103) AS JoiningDate,            

 CONVERT(varchar, PD.PaymentTodt, 103) AS RenwalDate,            

 PD.PaymentAmount      

FROM PaymentDetails PD            

INNER JOIN PlanMaster PM            

 ON PD.PlanID = PM.PlanID            

 INNER JOIN SchemeMaster SM on  PD.WorkouttypeID = SM.SchemeID            

 INNER JOIN MemberRegistration m  ON PD.MemberID = m.MainMemberID       

 where PD.PaymentTodt in (SELECT MAX(PaymentTodt) from PaymentDetails group BY MemberID)      

 order BY PD.PaymentID desc        

            

end




	

GO
/****** Object:  StoredProcedure [dbo].[Usp_GetAllRenwalrecordsFromBetweenDate]    Script Date: 3/29/2019 7:50:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create proc [dbo].[Usp_GetAllRenwalrecordsFromBetweenDate]              

     @Paymentfromdt datetime = null,      

     @Paymenttodt datetime = null,

     @exactdate  datetime = null    

as              

              

begin

 

     

  IF(@Paymentfromdt is not null and @Paymenttodt is not null)

  begin



SELECT

	m.MemberID,

	PaymentID,

	m.MemberFName + ' | ' + m.MemberLName AS Name,

	m.Address,

	m.Contactno,

	m.EmailID,

	m.MemberNo,

	PM.PlanName,

	SM.SchemeName,

	CONVERT(varchar, PD.PaymentFromdt, 103) AS JoiningDate,

	CONVERT(varchar, PD.PaymentTodt, 103) AS RenwalDate,

	PD.PaymentAmount,

	PD.WorkouttypeID,

	PD.PlanID AS PlantypeID

FROM PaymentDetails PD

INNER JOIN PlanMaster PM

	ON PD.PlanID = PM.PlanID

INNER JOIN SchemeMaster SM

	ON PD.WorkouttypeID = SM.SchemeID

INNER JOIN MemberRegistration m

	ON PD.MemberID = m.MemberID

WHERE CONVERT(varchar(10), PaymentTodt, 126) IN (SELECT

	CONVERT(varchar(10), MAX(PaymentTodt), 126)

FROM PaymentDetails



WHERE CONVERT(varchar(10), PaymentTodt, 126) BETWEEN CONVERT(varchar(10), @Paymentfromdt, 126) AND

CASE

	WHEN @Paymenttodt = '1990-01-01 00:00:00.000' THEN CONVERT(varchar(10), GETDATE(), 126)

	 ELSE CONVERT(varchar(10), @Paymenttodt, 126) 

END

GROUP BY MemberID)

ORDER BY PD.PaymentID DESC

      

      end

      

      else

      

      begin

SELECT

	M.MemberID,

	PaymentID,

	m.MemberFName + ' | ' + m.MemberLName AS Name,

	m.Address,

	m.Contactno,

	m.EmailID,

	PD.MemberNo,

	PM.PlanName,

	SM.SchemeName,

	CONVERT(varchar, PD.PaymentFromdt, 103) AS JoiningDate,

	CONVERT(varchar, PD.PaymentTodt, 103) AS RenwalDate,

	PD.PaymentAmount,

	PD.WorkouttypeID,

	PD.PlanID AS PlantypeID

FROM PaymentDetails PD

INNER JOIN PlanMaster PM

	ON PD.PlanID = PM.PlanID

INNER JOIN SchemeMaster SM

	ON PD.WorkouttypeID = SM.SchemeID

INNER JOIN MemberRegistration m

	ON PD.MemberID = m.MemberID

WHERE CONVERT(varchar(10), PaymentTodt, 126) IN (SELECT

	CONVERT(varchar(10), MAX(PaymentTodt), 126)

FROM PaymentDetails

WHERE CONVERT(varchar(10), PaymentTodt, 126) = @exactdate



GROUP BY MemberID)

ORDER BY PD.PaymentID DESC

      

      end

          

              

end




GO
/****** Object:  StoredProcedure [dbo].[Usp_GetAmount_reg]    Script Date: 3/29/2019 7:50:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create proc [dbo].[Usp_GetAmount_reg]

@PlanID int,

@SchemeID int

as

begin

SELECT TotalAmount FROM PlanMaster where PlanID =@PlanID and SchemeID=@SchemeID

end




GO
/****** Object:  StoredProcedure [dbo].[Usp_GetMonthwisepaymentdetails]    Script Date: 3/29/2019 7:50:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create PROCEDURE [dbo].[Usp_GetMonthwisepaymentdetails]       
	@month BIGINT
AS
BEGIN
	DECLARE @Year NVARCHAR(4)

	SET @Year = DATEPART(YEAR, GETDATE())

	DECLARE @total BIGINT

	SET @total = (
			SELECT SUM(PaymentAmount) AS Total
			FROM PaymentDetails p
			WHERE DATEPART(MM, CreateDate) = @month
				AND DATEPART(YEAR, CreateDate) = @Year
			)

	SELECT m.MemberFName
		,m.MemberNo
		,m.MemberLName
		,m.MemberMName
		,CONVERT(VARCHAR, p.CreateDate, 103) AS CreateDate
		,@total AS Total
		,DATENAME(MM, p.CreateDate) AS Paymentmonth
		,p.PaymentAmount
		,AL.Username
	FROM PaymentDetails p
	INNER JOIN MemberRegistration m ON m.MemberId = p.MemberID
	INNER JOIN Users AL ON AL.UserId = p.Createdby
	WHERE DATEPART(MM, p.CreateDate) = @month
		AND DATEPART(YEAR, p.CreateDate) = @Year
END



GO
/****** Object:  StoredProcedure [dbo].[Usp_GetYearwisepaymentdetails]    Script Date: 3/29/2019 7:50:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create proc [dbo].[Usp_GetYearwisepaymentdetails]         
--exec Usp_GetYearwisepaymentdetails 2014        
@year bigint        
as        
        
begin  
        
        
declare @leap char(3)  
declare @totalsum numeric(18,2)  
  
declare @mm bigint  
declare @year1 bigint  
  
SET @mm = (SELECT  
 DATEPART(MM, GETDATE()))  
  
if(@mm = 1)  
begin  
SET @year1 = @year + 1  
  
  
end  
  
  
  
SET @leap = (SELECT  
 CASE  
  WHEN (@YEAR % 4 = 0 AND @YEAR % 100 <> 0) OR @YEAR % 400 = 0 THEN 'YES' ELSE 'NO'  
 END AS LeapYear)  
  
declare @mainsum4 numeric(19,2)  
declare @mainsum5 numeric(19,2)  
declare @mainsum6 numeric(19,2)  
declare @mainsum7 numeric(19,2)  
declare @mainsum8 numeric(19,2)  
declare @mainsum9 numeric(19,2)  
declare @mainsum10 numeric(19,2)  
declare @mainsum11 numeric(19,2)  
declare @mainsum12 numeric(19,2)  
declare @mainsum3 numeric(19,2)  
declare @mainsum2 numeric(19,2)  
declare @mainsum1 numeric(19,2)  
       
Declare @total bigint  
  
SET @mainsum4 = (SELECT  
 SUM(PaymentAmount) AS Total  
FROM PaymentDetails M  
WHERE CONVERT(varchar(10), M.CreateDate, 126)  
BETWEEN (CONVERT(varchar, @year) + '-04-01') AND (CONVERT(varchar, @year) + '-04-30')  
AND DATEPART(yyyy, M.CreateDate) = @year)  
  
  
SET @mainsum5 = (SELECT  
 SUM(PaymentAmount) AS Total  
FROM PaymentDetails M  
WHERE CONVERT(varchar(10), M.CreateDate, 126) BETWEEN (CONVERT(varchar, @year) + '-05-01') AND (CONVERT(varchar, @year) + '-05-31')  
AND DATEPART(yyyy, M.CreateDate) = @year) ---may  
  
  
SET @mainsum6 = (SELECT  
 SUM(PaymentAmount) AS Total  
FROM PaymentDetails M  
WHERE CONVERT(varchar(10), M.CreateDate, 126) BETWEEN (CONVERT(varchar, @year) + '-06-01') AND (CONVERT(varchar, @year) + '-06-30')  
AND DATEPART(yyyy, M.CreateDate) = @year) ---may  
  
SET @mainsum7 = (SELECT  
 SUM(PaymentAmount) AS Total  
FROM PaymentDetails M  
WHERE CONVERT(varchar(10), M.CreateDate, 126) BETWEEN (CONVERT(varchar, @year) + '-07-01') AND (CONVERT(varchar, @year) + '-07-31')  
AND DATEPART(yyyy, M.CreateDate) = @year) ---may  
  
SET @mainsum8 = (SELECT  
 SUM(PaymentAmount) AS Total  
FROM PaymentDetails M  
WHERE CONVERT(varchar(10), M.CreateDate, 126) BETWEEN (CONVERT(varchar, @year) + '-08-01') AND (CONVERT(varchar, @year) + '-08-31')  
AND DATEPART(yyyy, M.CreateDate) = @year) ---may  
  
SET @mainsum9 = (SELECT  
 SUM(PaymentAmount) AS Total  
FROM PaymentDetails M  
WHERE CONVERT(varchar(10), M.CreateDate, 126) BETWEEN (CONVERT(varchar, @year) + '-09-01') AND (CONVERT(varchar, @year) + '-09-30')  
AND DATEPART(yyyy, M.CreateDate) = @year) ---may  
  
SET @mainsum10 = (SELECT  
 SUM(PaymentAmount) AS Total  
FROM PaymentDetails M  
WHERE CONVERT(varchar(10), M.CreateDate, 126) BETWEEN (CONVERT(varchar, @year) + '-10-01') AND (CONVERT(varchar, @year) + '-10-31')  
AND DATEPART(yyyy, M.CreateDate) = @year) ---may  
  
SET @mainsum11 = (SELECT  
 SUM(PaymentAmount) AS Total  
FROM PaymentDetails M  
WHERE CONVERT(varchar(10), M.CreateDate, 126) BETWEEN (CONVERT(varchar, @year) + '-11-01') AND (CONVERT(varchar, @year) + '-11-30')  
AND DATEPART(yyyy, M.CreateDate) = @year) ---may  
  
SET @mainsum12 = (SELECT  
 SUM(PaymentAmount) AS Total  
FROM PaymentDetails M  
WHERE CONVERT(varchar(10), M.CreateDate, 126) BETWEEN (CONVERT(varchar, @year) + '-12-01') AND (CONVERT(varchar, @year) + '-12-31')  
AND DATEPART(yyyy, M.CreateDate) = @year) ---may  
  
SET @mainsum3 = (SELECT  
 SUM(PaymentAmount) AS Total  
FROM PaymentDetails M  
WHERE CONVERT(varchar(10), M.CreateDate, 126) BETWEEN (CONVERT(varchar, @year1) + '-01-01') AND (CONVERT(varchar, @year1) + '-01-31')  
AND DATEPART(yyyy, M.CreateDate) = @year1)  
 ---may  
  
   
   
  
if(@leap ='NO')  
begin  
SET @mainsum2 = (SELECT  
 SUM(PaymentAmount) AS Total  
FROM PaymentDetails M  
WHERE CONVERT(varchar(10), M.CreateDate, 126) BETWEEN (CONVERT(varchar, @year1) + '-02-01') AND (CONVERT(varchar, @year1) + '-02-28')  
AND DATEPART(yyyy, M.CreateDate) = @year1)  
 ---may  
end  
  
if(@leap ='YES')  
begin  
SET @mainsum2 = (SELECT  
 SUM(PaymentAmount) AS Total  
FROM PaymentDetails M  
WHERE CONVERT(varchar(10), M.CreateDate, 126) BETWEEN (CONVERT(varchar, @year1) + '-02-01') AND (CONVERT(varchar, @year1) + '-02-29')  
AND DATEPART(yyyy, M.CreateDate) = @year1)  
 ---may  
end  
  
  
SET @mainsum1 = (SELECT  
 SUM(PaymentAmount) AS Total  
FROM PaymentDetails M  
WHERE CONVERT(varchar(10), M.CreateDate, 126) BETWEEN (CONVERT(varchar, @year1) + '-03-01') AND (CONVERT(varchar, @year1) + '-03-31')  
AND DATEPART(yyyy, M.CreateDate) = @year1) ---may  
  
  
SET @totalsum = (ISNULL(@mainsum4, 0) +  
ISNULL(@mainsum5, 0) +  
ISNULL(@mainsum6, 0) +  
ISNULL(@mainsum7, 0) +  
ISNULL(@mainsum8, 0) +  
ISNULL(@mainsum9, 0) +  
ISNULL(@mainsum10, 0) +  
ISNULL(@mainsum11, 0) +  
ISNULL(@mainsum12, 0) +  
ISNULL(@mainsum1, 0) +  
ISNULL(@mainsum2, 0) +  
ISNULL(@mainsum3, 0))  
  
SELECT  
 CONVERT(varchar, @year + 1) AS CurrentYear,  
 @mainsum4 AS april,  
 @mainsum5 AS may,  
 @mainsum6 AS june,  
 @mainsum7 AS july,  
 @mainsum8 AS august,  
 @mainsum9 AS sept,  
 @mainsum10 AS oct,  
 @mainsum11 AS nov,  
 @mainsum12 AS Decm,  
 @mainsum3 AS jan,  
 @mainsum2 AS feb,  
 @mainsum1 AS march,  
 @totalsum AS Total  
  
end



GO
/****** Object:  StoredProcedure [dbo].[Usp_UpdateMemberDetails]    Script Date: 3/29/2019 7:50:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create proc [dbo].[Usp_UpdateMemberDetails]

  @MemberId BIGINT = 0    

 ,@MemberFName NVARCHAR(100) = NULL    

 ,@MemberLName NVARCHAR(100) = NULL    

 ,@MemberMName NVARCHAR(100) = NULL    

 ,@Address NVARCHAR(500) = NULL    

 ,@DOB DATETIME    

 ,@Age NVARCHAR(10) = NULL    

 ,@Contactno NVARCHAR(10) = NULL    

 ,@EmailID NVARCHAR(30) = NULL    

 ,@Gender NVARCHAR(30) = NULL    

 ,@ModifiedBy BIGINT = 0    

as
begin

 UPDATE [MemberRegistration]    

 SET MemberFName = @MemberFName    

  ,MemberLName = @MemberLName    

  ,MemberMName = @MemberMName    

  ,DOB = @DOB    

  ,Age = @Age    

  ,Contactno = @Contactno    

  ,EmailID = @EmailID    

  ,Gender = @Gender    

  ,ModifiedBy = @ModifiedBy    

  ,Address = @Address    

 WHERE MemberId = @MemberId    

end



GO
USE [master]
GO
ALTER DATABASE [CTROReporting] SET  READ_WRITE 
GO
