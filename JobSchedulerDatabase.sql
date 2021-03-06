USE [master]
GO

/****** Object:  Database [JobScheduler]    Script Date: 5/17/2017 9:15:41 AM ******/
CREATE DATABASE [JobScheduler]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'JobScheduler', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\JobScheduler.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'JobScheduler_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\JobScheduler_log.ldf' , SIZE = 2048KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO

ALTER DATABASE [JobScheduler] SET COMPATIBILITY_LEVEL = 130
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [JobScheduler].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [JobScheduler] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [JobScheduler] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [JobScheduler] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [JobScheduler] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [JobScheduler] SET ARITHABORT OFF 
GO
ALTER DATABASE [JobScheduler] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [JobScheduler] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [JobScheduler] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [JobScheduler] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [JobScheduler] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [JobScheduler] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [JobScheduler] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [JobScheduler] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [JobScheduler] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [JobScheduler] SET  DISABLE_BROKER 
GO
ALTER DATABASE [JobScheduler] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [JobScheduler] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [JobScheduler] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [JobScheduler] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [JobScheduler] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [JobScheduler] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [JobScheduler] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [JobScheduler] SET RECOVERY FULL 
GO
ALTER DATABASE [JobScheduler] SET  MULTI_USER 
GO
ALTER DATABASE [JobScheduler] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [JobScheduler] SET DB_CHAINING OFF 
GO
ALTER DATABASE [JobScheduler] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [JobScheduler] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [JobScheduler] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [JobScheduler] SET QUERY_STORE = OFF
GO

USE [JobScheduler]
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET MAXDOP = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET LEGACY_CARDINALITY_ESTIMATION = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET PARAMETER_SNIFFING = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET QUERY_OPTIMIZER_HOTFIXES = PRIMARY;
GO
ALTER DATABASE [JobScheduler] SET  READ_WRITE 
GO

USE [JobScheduler]
GO
/****** Object:  Table [dbo].[SchedulerAgent]    Script Date: 5/17/2017 9:12:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SchedulerAgent](
	[SchedulerAgentID] [int] IDENTITY(1,1) NOT NULL,
	[SchedulerServerID] [int] NOT NULL,
	[SchedulerQueueID] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [SchedulerAgent_PK] PRIMARY KEY CLUSTERED 
(
	[SchedulerAgentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SchedulerExclusion]    Script Date: 5/17/2017 9:12:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SchedulerExclusion](
	[SchedulerExclusionID] [int] IDENTITY(1,1) NOT NULL,
	[SchedulerAgentID] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[Sunday] [bit] NOT NULL,
	[Monday] [bit] NOT NULL,
	[Tuesday] [bit] NOT NULL,
	[Wednesday] [bit] NOT NULL,
	[Thursday] [bit] NOT NULL,
	[Friday] [bit] NOT NULL,
	[Saturday] [bit] NOT NULL,
	[StartTime] [time](7) NULL,
	[EndTime] [time](7) NULL,
	[SpecificDate] [date] NULL,
 CONSTRAINT [SchedulerExclusion_PK] PRIMARY KEY CLUSTERED 
(
	[SchedulerExclusionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SchedulerFolder]    Script Date: 5/17/2017 9:12:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SchedulerFolder](
	[SchedulerFolderID] [int] IDENTITY(1,1) NOT NULL,
	[SchedulerFolderName] [varchar](100) NOT NULL,
	[SchedulerQueueID] [int] NOT NULL,
	[ParentSchedulerFolderID] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[SortOrder] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreatedBy] [int] NOT NULL,
 CONSTRAINT [SchedulerFolder_PK] PRIMARY KEY CLUSTERED 
(
	[SchedulerFolderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SchedulerInterval]    Script Date: 5/17/2017 9:12:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SchedulerInterval](
	[SchedulerIntervalID] [int] IDENTITY(1,1) NOT NULL,
	[IntervalName] [varchar](50) NOT NULL,
	[Interval] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[Occurrences] [varchar](12) NOT NULL,
	[IntervalType] [varchar](10) NOT NULL,
	[StartTime] [time](7) NULL,
	[EndTime] [time](7) NULL,
	[ExclusionStart] [time](7) NULL,
	[ExclusionEnd] [time](7) NULL,
	[RepeatMinutes] [int] NULL,
	[SchedulerIntervalDetails] [varchar](max) NULL,
 CONSTRAINT [SchedulerInterval_PK] PRIMARY KEY CLUSTERED 
(
	[SchedulerIntervalID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SchedulerJob]    Script Date: 5/17/2017 9:12:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SchedulerJob](
	[SchedulerJobID] [int] IDENTITY(1,1) NOT NULL,
	[SchedulerFolderID] [int] NOT NULL,
	[JobName] [varchar](100) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[SortOrder] [int] NOT NULL,
	[SchedulerIntervalID] [int] NULL,
	[SchedulerAgentID] [int] NULL,
	[SchedulerOperatorId] [int] NULL,
	[LastRunTime] [datetime] NULL,
	[NextRunTime] [datetime] NULL,
	[MaxInstances] [int] NULL,
	[IsScheduled] [bit] NOT NULL,
 CONSTRAINT [SchedulerJob_PK] PRIMARY KEY CLUSTERED 
(
	[SchedulerJobID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SchedulerJobLog]    Script Date: 5/17/2017 9:12:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SchedulerJobLog](
	[SchedulerJobLogId] [int] IDENTITY(1,1) NOT NULL,
	[SchedulerJobId] [int] NOT NULL,
	[ExecutionScheduled] [datetime] NOT NULL,
	[ExecutionStart] [datetime2](7) NULL,
	[ExecutionEnd] [datetime2](7) NULL,
	[SchedulerUserId] [int] NULL,
 CONSTRAINT [SchedulerJobLog_PK] PRIMARY KEY CLUSTERED 
(
	[SchedulerJobLogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SchedulerOperator]    Script Date: 5/17/2017 9:12:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SchedulerOperator](
	[SchedulerOperatorId] [int] IDENTITY(1,1) NOT NULL,
	[SchedulerOperatorName] [varchar](50) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [SchedulerOperator_PK] PRIMARY KEY CLUSTERED 
(
	[SchedulerOperatorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SchedulerOperatorUser]    Script Date: 5/17/2017 9:12:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SchedulerOperatorUser](
	[SchedulerOperatorUserId] [int] IDENTITY(1,1) NOT NULL,
	[SchedulerOperatorId] [int] NOT NULL,
	[SchedulerUserId] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [SchedulerOperatorUser_PK] PRIMARY KEY CLUSTERED 
(
	[SchedulerOperatorUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SchedulerQueue]    Script Date: 5/17/2017 9:12:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SchedulerQueue](
	[SchedulerQueueID] [int] IDENTITY(1,1) NOT NULL,
	[QueueName] [varchar](100) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsRunning] [bit] NOT NULL,
	[MaxThreads] [int] NOT NULL,
	[MaxMinutes] [int] NULL,
 CONSTRAINT [SchedulerQueue_PK] PRIMARY KEY CLUSTERED 
(
	[SchedulerQueueID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SchedulerServer]    Script Date: 5/17/2017 9:12:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SchedulerServer](
	[SchedulerServerID] [int] IDENTITY(1,1) NOT NULL,
	[ServerName] [varchar](50) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[AgentEnabled] [bit] NOT NULL,
	[MaxThreads] [int] NOT NULL,
	[MailServer] [varchar](50) NULL,
	[MailID] [varchar](100) NULL,
	[MailPassword] [varchar](100) NULL,
	[MachineIP4Address] [varchar](15) NULL,
	[MachineIP6Address] [varchar](50) NULL,
 CONSTRAINT [PK_Machine] PRIMARY KEY CLUSTERED 
(
	[SchedulerServerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SchedulerTask]    Script Date: 5/17/2017 9:12:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SchedulerTask](
	[SchedulerTaskID] [int] IDENTITY(1,1) NOT NULL,
	[SchedulerJobID] [int] NOT NULL,
	[TaskName] [varchar](100) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[SortOrder] [int] NOT NULL,
	[SchedulerTaskTypeID] [int] NOT NULL,
	[TaskInformation] [varchar](max) NOT NULL,
 CONSTRAINT [SchedulerTask_PK] PRIMARY KEY CLUSTERED 
(
	[SchedulerTaskID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SchedulerTaskLog]    Script Date: 5/17/2017 9:12:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SchedulerTaskLog](
	[SchedulerTaskLogId] [int] IDENTITY(1,1) NOT NULL,
	[SchedulerTaskID] [int] NOT NULL,
	[ExecutionScheduled] [datetime] NOT NULL,
	[SchedulerJobLogId] [int] NOT NULL,
	[SchedulerServerID] [int] NOT NULL,
	[StandardOutput] [varchar](max) NOT NULL,
	[StandardError] [varchar](max) NULL,
	[ExecutionStart] [datetime2](7) NULL,
	[ExecutionEnd] [datetime2](7) NULL,
	[ExecutionCommandLine] [varchar](max) NULL,
	[ScheduledStart] [datetime] NULL,
 CONSTRAINT [PK_SchedulerTaskLog] PRIMARY KEY CLUSTERED 
(
	[SchedulerTaskLogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SchedulerTaskType]    Script Date: 5/17/2017 9:12:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SchedulerTaskType](
	[SchedulerTaskTypeID] [int] IDENTITY(1,1) NOT NULL,
	[TaskTypeName] [varchar](100) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[TaskTypeCode] [varchar](20) NOT NULL,
 CONSTRAINT [SchedulerTaskType_PK] PRIMARY KEY CLUSTERED 
(
	[SchedulerTaskTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SchedulerUser]    Script Date: 5/17/2017 9:12:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SchedulerUser](
	[SchedulerUserId] [int] IDENTITY(1,1) NOT NULL,
	[UserLogin] [varchar](50) NOT NULL,
	[UserEmail] [varchar](50) NOT NULL,
	[UserPassword] [varchar](100) NOT NULL,
	[UserFirstName] [varchar](20) NOT NULL,
	[UserLastName] [varchar](20) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[AdminAccess] [bit] NOT NULL,
	[ViewAccess] [bit] NOT NULL,
	[ReportAccess] [bit] NOT NULL,
	[OperatorAccess] [bit] NOT NULL,
 CONSTRAINT [SchedulerUser_PK] PRIMARY KEY CLUSTERED 
(
	[SchedulerUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SchedulerVariable]    Script Date: 5/17/2017 9:12:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SchedulerVariable](
	[SchedulerVariableID] [int] IDENTITY(1,1) NOT NULL,
	[SchedulerFolderID] [int] NOT NULL,
	[VariableName] [varchar](100) NOT NULL,
	[VariableValue] [varchar](1000) NOT NULL,
	[VariableDescription] [varchar](1000) NULL,
	[IsActive] [bit] NOT NULL,
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [SchedulerVariable_PK] PRIMARY KEY CLUSTERED 
(
	[SchedulerVariableID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[SchedulerJob] ADD  CONSTRAINT [DF_SchedulerJob_IsScheduled]  DEFAULT ((0)) FOR [IsScheduled]
GO
ALTER TABLE [dbo].[SchedulerAgent]  WITH CHECK ADD  CONSTRAINT [SchedulerQueue_SchedulerAgent_FK1] FOREIGN KEY([SchedulerQueueID])
REFERENCES [dbo].[SchedulerQueue] ([SchedulerQueueID])
GO
ALTER TABLE [dbo].[SchedulerAgent] CHECK CONSTRAINT [SchedulerQueue_SchedulerAgent_FK1]
GO
ALTER TABLE [dbo].[SchedulerAgent]  WITH CHECK ADD  CONSTRAINT [SchedulerServer_SchedulerAgent_FK1] FOREIGN KEY([SchedulerServerID])
REFERENCES [dbo].[SchedulerServer] ([SchedulerServerID])
GO
ALTER TABLE [dbo].[SchedulerAgent] CHECK CONSTRAINT [SchedulerServer_SchedulerAgent_FK1]
GO
ALTER TABLE [dbo].[SchedulerExclusion]  WITH CHECK ADD  CONSTRAINT [SchedulerAgent_SchedulerExclusion_FK1] FOREIGN KEY([SchedulerAgentID])
REFERENCES [dbo].[SchedulerAgent] ([SchedulerAgentID])
GO
ALTER TABLE [dbo].[SchedulerExclusion] CHECK CONSTRAINT [SchedulerAgent_SchedulerExclusion_FK1]
GO
ALTER TABLE [dbo].[SchedulerFolder]  WITH CHECK ADD  CONSTRAINT [SchedulerFolder_SchedulerFolder_FK1] FOREIGN KEY([ParentSchedulerFolderID])
REFERENCES [dbo].[SchedulerFolder] ([SchedulerFolderID])
GO
ALTER TABLE [dbo].[SchedulerFolder] CHECK CONSTRAINT [SchedulerFolder_SchedulerFolder_FK1]
GO
ALTER TABLE [dbo].[SchedulerFolder]  WITH CHECK ADD  CONSTRAINT [SchedulerQueue_SchedulerFolder_FK1] FOREIGN KEY([SchedulerQueueID])
REFERENCES [dbo].[SchedulerQueue] ([SchedulerQueueID])
GO
ALTER TABLE [dbo].[SchedulerFolder] CHECK CONSTRAINT [SchedulerQueue_SchedulerFolder_FK1]
GO
ALTER TABLE [dbo].[SchedulerJob]  WITH CHECK ADD  CONSTRAINT [SchedulerAgent_SchedulerJob_FK1] FOREIGN KEY([SchedulerAgentID])
REFERENCES [dbo].[SchedulerAgent] ([SchedulerAgentID])
GO
ALTER TABLE [dbo].[SchedulerJob] CHECK CONSTRAINT [SchedulerAgent_SchedulerJob_FK1]
GO
ALTER TABLE [dbo].[SchedulerJob]  WITH CHECK ADD  CONSTRAINT [SchedulerFolder_SchedulerJob_FK1] FOREIGN KEY([SchedulerFolderID])
REFERENCES [dbo].[SchedulerFolder] ([SchedulerFolderID])
GO
ALTER TABLE [dbo].[SchedulerJob] CHECK CONSTRAINT [SchedulerFolder_SchedulerJob_FK1]
GO
ALTER TABLE [dbo].[SchedulerJob]  WITH CHECK ADD  CONSTRAINT [SchedulerInterval_SchedulerJob_FK1] FOREIGN KEY([SchedulerIntervalID])
REFERENCES [dbo].[SchedulerInterval] ([SchedulerIntervalID])
GO
ALTER TABLE [dbo].[SchedulerJob] CHECK CONSTRAINT [SchedulerInterval_SchedulerJob_FK1]
GO
ALTER TABLE [dbo].[SchedulerJob]  WITH CHECK ADD  CONSTRAINT [SchedulerOperator_SchedulerJob_FK1] FOREIGN KEY([SchedulerOperatorId])
REFERENCES [dbo].[SchedulerOperator] ([SchedulerOperatorId])
GO
ALTER TABLE [dbo].[SchedulerJob] CHECK CONSTRAINT [SchedulerOperator_SchedulerJob_FK1]
GO
ALTER TABLE [dbo].[SchedulerJobLog]  WITH CHECK ADD  CONSTRAINT [SchedulerJob_SchedulerJobLog_FK1] FOREIGN KEY([SchedulerJobId])
REFERENCES [dbo].[SchedulerJob] ([SchedulerJobID])
GO
ALTER TABLE [dbo].[SchedulerJobLog] CHECK CONSTRAINT [SchedulerJob_SchedulerJobLog_FK1]
GO
ALTER TABLE [dbo].[SchedulerJobLog]  WITH CHECK ADD  CONSTRAINT [SchedulerUser_SchedulerJobLog_FK1] FOREIGN KEY([SchedulerUserId])
REFERENCES [dbo].[SchedulerUser] ([SchedulerUserId])
GO
ALTER TABLE [dbo].[SchedulerJobLog] CHECK CONSTRAINT [SchedulerUser_SchedulerJobLog_FK1]
GO
ALTER TABLE [dbo].[SchedulerOperatorUser]  WITH CHECK ADD  CONSTRAINT [SchedulerOperator_SchedulerOperatorUser_FK1] FOREIGN KEY([SchedulerOperatorId])
REFERENCES [dbo].[SchedulerOperator] ([SchedulerOperatorId])
GO
ALTER TABLE [dbo].[SchedulerOperatorUser] CHECK CONSTRAINT [SchedulerOperator_SchedulerOperatorUser_FK1]
GO
ALTER TABLE [dbo].[SchedulerOperatorUser]  WITH CHECK ADD  CONSTRAINT [SchedulerUser_SchedulerOperatorUser_FK1] FOREIGN KEY([SchedulerUserId])
REFERENCES [dbo].[SchedulerUser] ([SchedulerUserId])
GO
ALTER TABLE [dbo].[SchedulerOperatorUser] CHECK CONSTRAINT [SchedulerUser_SchedulerOperatorUser_FK1]
GO
ALTER TABLE [dbo].[SchedulerTask]  WITH CHECK ADD  CONSTRAINT [SchedulerJob_SchedulerTask_FK1] FOREIGN KEY([SchedulerJobID])
REFERENCES [dbo].[SchedulerJob] ([SchedulerJobID])
GO
ALTER TABLE [dbo].[SchedulerTask] CHECK CONSTRAINT [SchedulerJob_SchedulerTask_FK1]
GO
ALTER TABLE [dbo].[SchedulerTask]  WITH CHECK ADD  CONSTRAINT [SchedulerTaskType_SchedulerTask_FK1] FOREIGN KEY([SchedulerTaskTypeID])
REFERENCES [dbo].[SchedulerTaskType] ([SchedulerTaskTypeID])
GO
ALTER TABLE [dbo].[SchedulerTask] CHECK CONSTRAINT [SchedulerTaskType_SchedulerTask_FK1]
GO
ALTER TABLE [dbo].[SchedulerTaskLog]  WITH CHECK ADD  CONSTRAINT [SchedulerJobLog_SchedulerTaskLog_FK1] FOREIGN KEY([SchedulerJobLogId])
REFERENCES [dbo].[SchedulerJobLog] ([SchedulerJobLogId])
GO
ALTER TABLE [dbo].[SchedulerTaskLog] CHECK CONSTRAINT [SchedulerJobLog_SchedulerTaskLog_FK1]
GO
ALTER TABLE [dbo].[SchedulerTaskLog]  WITH CHECK ADD  CONSTRAINT [SchedulerServer_SchedulerTaskLog_FK1] FOREIGN KEY([SchedulerServerID])
REFERENCES [dbo].[SchedulerServer] ([SchedulerServerID])
GO
ALTER TABLE [dbo].[SchedulerTaskLog] CHECK CONSTRAINT [SchedulerServer_SchedulerTaskLog_FK1]
GO
ALTER TABLE [dbo].[SchedulerTaskLog]  WITH CHECK ADD  CONSTRAINT [SchedulerTask_SchedulerTaskLog_FK1] FOREIGN KEY([SchedulerTaskID])
REFERENCES [dbo].[SchedulerTask] ([SchedulerTaskID])
GO
ALTER TABLE [dbo].[SchedulerTaskLog] CHECK CONSTRAINT [SchedulerTask_SchedulerTaskLog_FK1]
GO
ALTER TABLE [dbo].[SchedulerVariable]  WITH CHECK ADD  CONSTRAINT [SchedulerFolder_SchedulerVariable_FK1] FOREIGN KEY([SchedulerFolderID])
REFERENCES [dbo].[SchedulerFolder] ([SchedulerFolderID])
GO
ALTER TABLE [dbo].[SchedulerVariable] CHECK CONSTRAINT [SchedulerFolder_SchedulerVariable_FK1]
GO


-- Data Inserts
SET IDENTITY_INSERT [dbo].[SchedulerTaskType] ON 
GO
INSERT [dbo].[SchedulerTaskType] ([SchedulerTaskTypeID], [TaskTypeName], [IsActive], [TaskTypeCode]) VALUES (1, N'SQL Task', 1, N'SQL')
GO
INSERT [dbo].[SchedulerTaskType] ([SchedulerTaskTypeID], [TaskTypeName], [IsActive], [TaskTypeCode]) VALUES (2, N'Program Execute', 1, N'EXECUTE')
GO
INSERT [dbo].[SchedulerTaskType] ([SchedulerTaskTypeID], [TaskTypeName], [IsActive], [TaskTypeCode]) VALUES (3, N'File Copy', 1, N'FILECOPY')
GO
INSERT [dbo].[SchedulerTaskType] ([SchedulerTaskTypeID], [TaskTypeName], [IsActive], [TaskTypeCode]) VALUES (4, N'Stored Procedure', 1, N'SP')
GO
INSERT [dbo].[SchedulerTaskType] ([SchedulerTaskTypeID], [TaskTypeName], [IsActive], [TaskTypeCode]) VALUES (5, N'Email', 1, N'EMAIL')
GO
INSERT [dbo].[SchedulerTaskType] ([SchedulerTaskTypeID], [TaskTypeName], [IsActive], [TaskTypeCode]) VALUES (6, N'Script', 1, N'SCRIPT')
GO
INSERT [dbo].[SchedulerTaskType] ([SchedulerTaskTypeID], [TaskTypeName], [IsActive], [TaskTypeCode]) VALUES (7, N'SQL DTS', 1, N'DTS')
GO
INSERT [dbo].[SchedulerTaskType] ([SchedulerTaskTypeID], [TaskTypeName], [IsActive], [TaskTypeCode]) VALUES (8, N'DLL Execute', 1, N'DLL')
GO
SET IDENTITY_INSERT [dbo].[SchedulerTaskType] OFF
GO
