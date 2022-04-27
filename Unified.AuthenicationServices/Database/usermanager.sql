USE [master]
GO
/****** Object:  Database [UserManager]    Script Date: 4/27/2022 10:16:38 AM ******/
CREATE DATABASE [UserManager]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'UserManager', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\UserManager.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'UserManager_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\UserManager_log.ldf' , SIZE = 73728KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [UserManager] SET COMPATIBILITY_LEVEL = 130
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [UserManager].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [UserManager] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [UserManager] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [UserManager] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [UserManager] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [UserManager] SET ARITHABORT OFF 
GO
ALTER DATABASE [UserManager] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [UserManager] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [UserManager] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [UserManager] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [UserManager] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [UserManager] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [UserManager] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [UserManager] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [UserManager] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [UserManager] SET  ENABLE_BROKER 
GO
ALTER DATABASE [UserManager] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [UserManager] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [UserManager] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [UserManager] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [UserManager] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [UserManager] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [UserManager] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [UserManager] SET RECOVERY FULL 
GO
ALTER DATABASE [UserManager] SET  MULTI_USER 
GO
ALTER DATABASE [UserManager] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [UserManager] SET DB_CHAINING OFF 
GO
ALTER DATABASE [UserManager] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [UserManager] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [UserManager] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [UserManager] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [UserManager] SET QUERY_STORE = OFF
GO
USE [UserManager]
GO
/****** Object:  UserDefinedTableType [dbo].[IdentityID]    Script Date: 4/27/2022 10:16:38 AM ******/
CREATE TYPE [dbo].[IdentityID] AS TABLE(
	[Id] [nvarchar](425) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[IntID]    Script Date: 4/27/2022 10:16:38 AM ******/
CREATE TYPE [dbo].[IntID] AS TABLE(
	[Id] [int] NULL
)
GO
/****** Object:  Table [dbo].[ClaimType]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ClaimType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EnTitle] [nvarchar](200) NOT NULL,
	[ArTitle] [nvarchar](200) NOT NULL,
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[ModuleId] [int] NULL,
 CONSTRAINT [PK_Claim_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Claim_ArName_ApplicationId_ModuleId] UNIQUE NONCLUSTERED 
(
	[ArTitle] ASC,
	[ApplicationId] ASC,
	[ModuleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Claim_EnName_ApplicationId_ModuleId] UNIQUE NONCLUSTERED 
(
	[EnTitle] ASC,
	[ApplicationId] ASC,
	[ModuleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](450) NOT NULL,
	[UserName] [nvarchar](256) NULL,
	[NormalizedUserName] [nvarchar](256) NULL,
	[Email] [nvarchar](256) NOT NULL,
	[NormalizedEmail] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEnd] [datetimeoffset](7) NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[ispasswordreset] [bit] NOT NULL,
 CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_AspNetUsers_Email] UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoleClaims]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoleClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
	[ClaimType] [varchar](50) NOT NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_AspNetRoleClaims] UNIQUE NONCLUSTERED 
(
	[RoleId] ASC,
	[ClaimType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](450) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[V_getuserrolewithmodule]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[V_getuserrolewithmodule]
AS
SELECT        dbo.ClaimType.ModuleId, dbo.AspNetUserRoles.UserId, dbo.AspNetUserRoles.RoleId, dbo.ClaimType.EnTitle, dbo.ClaimType.ArTitle, dbo.AspNetUsers.UserName AS ToName, dbo.AspNetUsers.Email AS ToEmail
FROM            dbo.AspNetUserRoles INNER JOIN
                         dbo.AspNetRoleClaims ON dbo.AspNetUserRoles.RoleId = dbo.AspNetRoleClaims.RoleId INNER JOIN
                         dbo.ClaimType ON dbo.AspNetRoleClaims.ClaimType = dbo.ClaimType.Id INNER JOIN
                         dbo.AspNetUsers ON dbo.AspNetUserRoles.UserId = dbo.AspNetUsers.Id
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](256) NULL,
	[NormalizedName] [nvarchar](256) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[Disabled] [bit] NOT NULL,
	[ArName] [nvarchar](256) NULL,
	[ModuleId] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_AspNetRoles_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](450) NOT NULL,
	[ProviderKey] [nvarchar](450) NOT NULL,
	[ProviderDisplayName] [nvarchar](max) NULL,
	[UserId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserTokens]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserTokens](
	[UserId] [nvarchar](450) NOT NULL,
	[LoginProvider] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](450) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[LoginProvider] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HraApplications]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HraApplications](
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[ApplicationName] [varchar](45) NOT NULL,
	[ApplicationUrl] [varchar](200) NULL,
	[ApplicationIcon] [varchar](50) NULL,
	[Description] [nvarchar](4000) NULL,
	[IsInternalApplication] [bit] NOT NULL,
 CONSTRAINT [PK_HraApplications] PRIMARY KEY CLUSTERED 
(
	[ApplicationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HraEndpoint]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HraEndpoint](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EndpointUri] [varchar](400) NOT NULL,
	[ApplicationId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_HraEndpoint] PRIMARY KEY CLUSTERED 
(
	[EndpointUri] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Module]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Module](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EnName] [nvarchar](300) NOT NULL,
	[ArName] [nvarchar](300) NULL,
	[ModuleUrl] [varchar](400) NULL,
	[Description] [nvarchar](4000) NULL,
	[CategoryId] [smallint] NULL,
 CONSTRAINT [PK_Module] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Module] UNIQUE NONCLUSTERED 
(
	[ModuleUrl] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ThirdPartyLogIn]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ThirdPartyLogIn](
	[EmailAddress] [varchar](45) NOT NULL,
	[IsLoginAllowed] [bit] NOT NULL,
 CONSTRAINT [PK_ThirdPartyLogIn] PRIMARY KEY CLUSTERED 
(
	[EmailAddress] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetRoleClaims_RoleId]    Script Date: 4/27/2022 10:16:38 AM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetRoleClaims_RoleId] ON [dbo].[AspNetRoleClaims]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [RoleNameIndex]    Script Date: 4/27/2022 10:16:38 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] ON [dbo].[AspNetRoles]
(
	[NormalizedName] ASC
)
WHERE ([NormalizedName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserClaims_UserId]    Script Date: 4/27/2022 10:16:38 AM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserClaims_UserId] ON [dbo].[AspNetUserClaims]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserLogins_UserId]    Script Date: 4/27/2022 10:16:38 AM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserLogins_UserId] ON [dbo].[AspNetUserLogins]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserRoles_RoleId]    Script Date: 4/27/2022 10:16:38 AM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserRoles_RoleId] ON [dbo].[AspNetUserRoles]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [EmailIndex]    Script Date: 4/27/2022 10:16:38 AM ******/
CREATE NONCLUSTERED INDEX [EmailIndex] ON [dbo].[AspNetUsers]
(
	[NormalizedEmail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UserNameIndex]    Script Date: 4/27/2022 10:16:38 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[AspNetUsers]
(
	[NormalizedUserName] ASC
)
WHERE ([NormalizedUserName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AspNetRoles] ADD  CONSTRAINT [DF_AspNetRole_Disabled]  DEFAULT ((0)) FOR [Disabled]
GO
ALTER TABLE [dbo].[HraApplications] ADD  CONSTRAINT [DF_HraApplications_ApplicationId]  DEFAULT (newid()) FOR [ApplicationId]
GO
ALTER TABLE [dbo].[HraApplications] ADD  CONSTRAINT [DF_HraApplications_IsInternalApplication]  DEFAULT ((1)) FOR [IsInternalApplication]
GO
ALTER TABLE [dbo].[AspNetRoleClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetRoleClaims] CHECK CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserTokens]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserTokens] CHECK CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[ClaimType]  WITH CHECK ADD  CONSTRAINT [FK_ClaimType_ApplicationId] FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[HraApplications] ([ApplicationId])
GO
ALTER TABLE [dbo].[ClaimType] CHECK CONSTRAINT [FK_ClaimType_ApplicationId]
GO
ALTER TABLE [dbo].[ClaimType]  WITH CHECK ADD  CONSTRAINT [FK_ClaimType_ModuleId] FOREIGN KEY([ModuleId])
REFERENCES [dbo].[Module] ([Id])
GO
ALTER TABLE [dbo].[ClaimType] CHECK CONSTRAINT [FK_ClaimType_ModuleId]
GO
/****** Object:  StoredProcedure [dbo].[ClaimType_Select]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[ClaimType_Select]
	
	@ApplicationId uniqueidentifier = null,
	@ModuleId int = null -- if null, brings the application-related claims, or else the module-related ones
As

	Select * from ClaimType 
	where 
	(
		( (not @ApplicationId is null) and (ApplicationId = @ApplicationId) )
		or 
		(@ApplicationId is null)
	)
	And
	(
		((@ModuleId is null) and (ModuleId is null))
		Or
		((not @ModuleId is null) and (ModuleId = @ModuleId))
	)

/*	
	Update ClaimType Set ModuleId = Id where Id in (1,2)

	Select * from HraApplications
	exec ClaimType_Select '2B764E21-912A-4E57-8464-371A127F9053'
	exec ClaimType_Select 'EA5B1598-2D16-4B9A-B575-9C3E29187FAB', null	
	exec ClaimType_Select 'EA5B1598-2D16-4B9A-B575-9C3E29187FAB', 2
	exec ClaimType_Select '7a7d333d-b4e7-4c0d-b81f-840a3a0149a4', null
*/
GO
/****** Object:  StoredProcedure [dbo].[GetAllUserClaims]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create proc [dbo].[GetAllUserClaims]
	
	@UserId nvarchar(450)	
As

	Select * from AspNetRoleClaims A
		Inner Join AspNetUserRoles B on A.RoleId = B.RoleId
		Inner Join ClaimType C on A.ClaimType = C.Id
	Where
		UserId = @UserId		

/*	
	exec GetAllUserClaims 
		@UserId  = '453a896b-e41e-45dd-81a8-0de883d045f2'
			
	exec GetAllUserClaims 
		@UserId  = 'fa65669c-84bf-49f1-85a6-09e9878e8712'

	exec GetAllUserClaims 
		@UserId  = '453a896b-e41e-45dd-81a8-0de883d045f2'

	Select * from [AspNetUserClaims] 
	Where 
		UserId = @UserId 
		And 
		ClaimType in 
		(
			Select cast(Id as nvarchar(100)) from ClaimType	
			Where ApplicationId = @ApplicationId And 
			(
				((@ModuleId is null) and (ModuleId is null))
				Or
				((@ModuleId is not null) and (ModuleId = @ModuleId))
			)
		)
*/
GO
/****** Object:  StoredProcedure [dbo].[GetRoleApplications]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE proc [dbo].[GetRoleApplications]
	
	@RoleId nvarchar(425)
As

	Select distinct C.ApplicationId --, C.ApplicationName
	From AspNetRoleClaims A
		Inner Join ClaimType B on cast(B.Id as nvarchar(20)) = A.ClaimType 
		Inner Join HraApplications C on C.ApplicationId = B.ApplicationId	
	where 
		RoleId = @RoleId


/*
	Select * from AspNetRoleClaims
	Select * from AspNetRoles
	Select * from HraApplications
	
	Declare @RoleId nvarchar(425) = '2B764E21-912A-4E57-8464-371A127F9053'  -- (Select top 1 Id from AspNetRoles)	
	Select * from [dbo].[AspNetRoleClaims] Where RoleId = @RoleId
	
	exec GetRoleApplications @RoleId
*/
GO
/****** Object:  StoredProcedure [dbo].[GetRoleClaims]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[GetRoleClaims]
	
	@RoleId nvarchar(450),
	@ApplicationId uniqueidentifier = null, -- filter or not

	@ModuleId int = null -- filter even with null
As

	Select B.* from [AspNetRoleClaims] A
		Inner Join ClaimType B on cast(B.Id as nvarchar(20)) = A.ClaimType
	Where 
		RoleId = @RoleId
		And
		(
			(@ApplicationId is null)
			Or
			((not @ApplicationId is null) And (ApplicationId = @ApplicationId))
		)
		And
		(
			((@ModuleId is null) and (ModuleId is null))
			Or
			((not @ModuleId is null) and (ModuleId = @ModuleId))
		)

/*	
	exec GetRoleClaims 
		@RoleId  = '317f6220-5650-4e1a-b21c-cb072368ed9d', 
		@ApplicationId = 'ea5b1598-2d16-4b9a-b575-9c3e29187fab',
		@ModuleId = 2
			
	exec GetRoleClaims 
		@RoleId  = 'fa65669c-84bf-49f1-85a6-09e9878e8712', 
		@ApplicationId = 'EA5B1598-2D16-4B9A-B575-9C3E29187FAB',
		@ModuleId = 2

	exec GetRoleClaims 
		@RoleId  = 'fa65669c-84bf-49f1-85a6-09e9878e8712', 
		@ApplicationId = 'EA5B1598-2D16-4B9A-B575-9C3E29187FAB',
		@ModuleId = null

	Select * from [AspNetRoleClaims] 
	Where 
		RoleId = @RoleId 
		And 
		ClaimType in 
		(
			Select cast(Id as nvarchar(100)) from ClaimType	
			Where ApplicationId = @ApplicationId And 
			(
				((@ModuleId is null) and (ModuleId is null))
				Or
				((@ModuleId is not null) and (ModuleId = @ModuleId))
			)
		)
*/
GO
/****** Object:  StoredProcedure [dbo].[GetRoleClaimsTester]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[GetRoleClaimsTester]
As

	Select B.Id as RoleId, B.Name as RoleName, 
			D.ApplicationId, D.ApplicationName, 		
			C.Id as ClaimTypeId, C.EnTitle as ClaimType, 
			E.Id as ModuleId, E.EnName as Module
	from AspNetRoleClaims A
		Inner Join AspNetRoles B on A.RoleId = B.Id
		Inner Join ClaimType C on A.ClaimType = C.Id
		Inner Join HraApplications D on C.ApplicationId = D.ApplicationId
		Left Join Module E on C.ModuleId = E.Id
	/*
	Where
		B.Id = '317f6220-5650-4e1a-b21c-cb072368ed9d' And
		D.ApplicationId = 'ea5b1598-2d16-4b9a-b575-9c3e29187fab' And
		E.Id = 2
	*/
GO
/****** Object:  StoredProcedure [dbo].[GetRoleIDsByApplicationUser]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

------------------------------------------------------------------------
-- This procedure is used after user login to a system to retrieve all his roles.
-- e.g. In JobApplication, after user login, the system will use this SP to get all roles the user has in the system,
-- so it can know the user's transition from table Role_Transition.
------------------------------------------------------------------------

CREATE proc [dbo].[GetRoleIDsByApplicationUser]

	@ApplicationId UniqueIdentifier,
	@UserId nvarchar(256)
As

	Select distinct A.RoleId
	From AspNetUserRoles A
		Inner Join AspNetRoleClaims B on A.RoleId = B.RoleId
		Inner Join ClaimType C on cast(C.Id as nvarchar(20)) = B.ClaimType 
		Inner Join HraApplications D on D.ApplicationId = C.ApplicationId	
	where 
		D.ApplicationId = @ApplicationId
		And
		A.UserId = @UserId
			
/*

Select * from HraApplications where ApplicationId = '0327886E-E81B-49E4-B3A2-D7436858BA2B' -- HRA OPS
Select * from AspNetUserRoles where userId = '2508a0f4-c5a8-4299-b026-1d25f6982038' -- t0144
exec GetRoleIDsByApplicationUser @ApplicationId = '0327886E-E81B-49E4-B3A2-D7436858BA2B', @UserId = '2508a0f4-c5a8-4299-b026-1d25f6982038'

--exec [GetUsersByApplications] @ApplicationId = '0327886E-E81B-49E4-B3A2-D7436858BA2B' -- HRA OPS

*/
GO
/****** Object:  StoredProcedure [dbo].[GetRoleIdsForUser]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[GetRoleIdsForUser]

	@UserId nvarchar(256) = null
As
	
	Select A.Id From AspNetRoles A	
		Inner Join AspNetUserRoles B on A.Id = B.RoleId
	Where
	(
		((not @UserId is null) and (B.UserId = @UserId))
		or
		(@UserId is null)
	)
	Order by A.Id

/*	
	Select * from AspNetUsers
		
	exec GetRoleIdsForUser '5dce4a88-65cd-4e20-b634-54ab6b0eccfd'
	exec GetRoleIdsForUser '453a896b-e41e-45dd-81a8-0de883d045f2'	
*/
GO
/****** Object:  StoredProcedure [dbo].[GetRoleModules]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create proc [dbo].[GetRoleModules]
	
	@RoleId nvarchar(256),
	@ApplicationId uniqueidentifier = null -- filter or not
As

	Select distinct D.* 
	From AspNetRoleClaims A		
		Inner Join ClaimType C on C.Id = A.ClaimType 
		Inner Join Module D on D.Id = C.ModuleId
	Where 
		A.RoleId = @RoleId
		And
		( 
			(@ApplicationId is null) 
			or
			((not @ApplicationId is null) and (C.ApplicationId = @ApplicationId))
		)

/*
	--Select * from AspNetUsers 
	Declare @UserId nvarchar(425) = '7f027ea4-db3e-4b83-90b6-4f331736729c'	
	exec GetUserModules @UserId, @ApplicationId = ''

*/
GO
/****** Object:  StoredProcedure [dbo].[GetRolesByUserId]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create proc [dbo].[GetRolesByUserId]

	@UserId nvarchar(256)
As

	Select * from AspNetRoles Where Id in (Select RoleId from AspNetUserRoles where UserId = @UserId)
	
/*
Select * from AspNetUserRoles
exec GetRolesByUserId @UserId = 'c7e3bd08-0613-4f5d-9d08-eb82b4077b3c'
*/
GO
/****** Object:  StoredProcedure [dbo].[GetRoleUsers]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[GetRoleUsers]
	@RoleId nvarchar(256)
As

	Select * from AspNetUsers 
		Where Id in (Select UserId from AspNetUserRoles where RoleId = @RoleId)
	
	Select * from AspNetUsers 
		Where Id not in (Select UserId from AspNetUserRoles where RoleId = @RoleId)
	
/*
Select * from AspNetUserRoles
exec GetRoleUsers @RoleId = '32010113-c2b3-4616-846e-a3ed776a89e3'
*/
GO
/****** Object:  StoredProcedure [dbo].[GetUserApplications]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[GetUserApplications]
	
	@UserId nvarchar(256)
As

	Select * from HraApplications 
	Where
	ApplicationId in 
	(
		Select ApplicationId from ClaimType A
			Inner Join AspNetRoleClaims B on A.Id = cast(B.ClaimType as int)
			Inner Join AspNetUserRoles C on B.RoleId = C.RoleId		
		where
			C.UserId = @UserId
	)

/*
	Select distinct D.*	
	From AspNetRoleClaims A
		Inner Join AspNetUserRoles B on A.RoleId = B.RoleId
		Inner Join ClaimType C on cast(C.Id as nvarchar(20)) = A.ClaimType 
		Inner Join HraApplications D on D.ApplicationId = C.ApplicationId	
	where
		B.UserId = @UserId
*/
GO
/****** Object:  StoredProcedure [dbo].[GetUserAppsAndModules]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[GetUserAppsAndModules]
	
	@UserId nvarchar(256),
	@ApplicationId uniqueidentifier = null -- filter or not
As


	Select distinct D.Id as ModuleId, D.ModuleUrl, D.EnName as EnModuleTitle, D.ArName as ArModuleTitle, D.[Description], 
		E.ApplicationId as AppId, E.ApplicationName as AppName, E.IsInternalApplication as AppIsInternal, D.CategoryId as CategoryId
	From AspNetRoleClaims A
		Inner Join AspNetUserRoles B on B.RoleId = A.RoleId
		Inner Join ClaimType C on C.Id = A.ClaimType 		
		Inner Join Module D on D.Id = C.ModuleId
		Inner Join HraApplications E on E.ApplicationId = C.ApplicationId
	Where 
		B.UserId = @UserId
		And
		( 
			(@ApplicationId is null) 
			or
			((not @ApplicationId is null) and (C.ApplicationId = @ApplicationId))
		)
/*
Select * from AspNetUsers
exec GetUserAppsAndModules @UserId = '453a896b-e41e-45dd-81a8-0de883d045f2'

*/
GO
/****** Object:  StoredProcedure [dbo].[GetUserClaims]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[GetUserClaims]
	
	@UserId nvarchar(450),
	@ApplicationId uniqueidentifier = null, -- filter or not
	@ModuleId int = null -- filter even with null
As

	Select distinct C.* from AspNetRoleClaims A
		Inner Join AspNetUserRoles B on A.RoleId = B.RoleId
		Inner Join ClaimType C on A.ClaimType = C.Id
	Where
		UserId = @UserId
		And
		(
			(@ApplicationId is null)
			Or
			((not @ApplicationId is null) And (C.ApplicationId = @ApplicationId))			
		)
		And
		(
			--((@ModuleId is null) and (ModuleId is null))
			(@ModuleId is null)
			Or
			((not @ModuleId is null) and (ModuleId = @ModuleId))
		)

/*	

	Declare @UserId nvarchar(425) = '2588a119-d815-49ea-9361-32347c4585cb',
		@ApplicationId uniqueidentifier = 'EA5B1598-2D16-4B9A-B575-9C3E29187FAB'

	exec GetUserClaims 
		@UserId, 
		@ApplicationId
		--,@ModuleId = 2
			
	exec GetUserClaims 
		@UserId  = 'fa65669c-84bf-49f1-85a6-09e9878e8712', 
		@ApplicationId = 'EA5B1598-2D16-4B9A-B575-9C3E29187FAB',
		@ModuleId = 2

	exec GetUserClaims 
		@UserId  = '453a896b-e41e-45dd-81a8-0de883d045f2', 
		@ApplicationId = 'EA5B1598-2D16-4B9A-B575-9C3E29187FAB',
		@ModuleId = null

	Select * from [AspNetUserClaims] 
	Where 
		UserId = @UserId 
		And 
		ClaimType in 
		(
			Select cast(Id as nvarchar(100)) from ClaimType	
			Where ApplicationId = @ApplicationId And 
			(
				((@ModuleId is null) and (ModuleId is null))
				Or
				((@ModuleId is not null) and (ModuleId = @ModuleId))
			)
		)
*/
GO
/****** Object:  StoredProcedure [dbo].[GetUserModules]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[GetUserModules]
	
	@UserId nvarchar(256),
	@ApplicationId uniqueidentifier = null -- filter or not
As

	Select distinct D.*, C.ApplicationId 
	From AspNetRoleClaims A
		Inner Join AspNetUserRoles B on B.RoleId = A.RoleId
		Inner Join ClaimType C on C.Id = A.ClaimType 
		Inner Join Module D on D.Id = C.ModuleId
	Where 
		B.UserId = @UserId
		And
		( 
			(@ApplicationId is null) 
			or
			((not @ApplicationId is null) and (C.ApplicationId = @ApplicationId))
		)

GO
/****** Object:  StoredProcedure [dbo].[GetUserModulesByModuleId]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create proc [dbo].[GetUserModulesByModuleId]
	
	@UserId nvarchar(256),
	@ModuleId int
As

	Select distinct D.*, C.ApplicationId 
	From AspNetRoleClaims A
		Inner Join AspNetUserRoles B on B.RoleId = A.RoleId
		Inner Join ClaimType C on C.Id = A.ClaimType 
		Inner Join Module D on D.Id = C.ModuleId
	Where 
		B.UserId = @UserId
		and
		D.Id = @ModuleId

GO
/****** Object:  StoredProcedure [dbo].[GetUserRoles]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[GetUserRoles]
	@UserId nvarchar(256)
As

	Select * from AspNetRoles 
		Where Id in (Select RoleId from AspNetUserRoles where UserId = @UserId)
	
	Select * from AspNetRoles 
		Where Id not in (Select RoleId from AspNetUserRoles where UserId = @UserId)
	
/*
Select * from AspNetUserRoles
exec GetUserRoles @UserId = '2588a119-d815-49ea-9361-32347c4585cb'
*/
GO
/****** Object:  StoredProcedure [dbo].[GetUserRolesAndClaims]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[GetUserRolesAndClaims]

	@UserId nvarchar(256),
	@ApplicationId uniqueidentifier = null,
	@ModuleId int = null

As
	--------------------------------
	-- #T1: Roles, #T2: Role Claims 
	--------------------------------

	-- Get user roles
	Select Id as RoleId, Name as RoleTitleEn, ArName as RoleTitleAr into #T1 
	From AspNetRoles
	Where 
		Id in (Select RoleId from AspNetUserRoles where UserId = @UserId)
		And
		Disabled = 0
	
	--Select * from #T1

	Select C.RoleId, C.RoleTitleEn, C.RoleTitleAr, 		
		A.Id as ClaimTypeId, A.EnTitle as ClaimTypeEn, A.ArTitle as ClaimTypeAr, A.ApplicationId, 
		D.Id as ModuleId, D.EnName as ModuleEn, D.ArName as ModuleAr, D.ModuleUrl, D.Description as ModuleDescription
	From ClaimType A
		Inner Join [dbo].[AspNetRoleClaims] B on A.Id = cast(B.ClaimType as int)
		Inner Join #T1 C on C.RoleId = B.RoleId
		Inner Join Module D on D.Id = A.ModuleId
	where 
		(
			( (not @ApplicationId is null) and (ApplicationId = @ApplicationId) )
			or 
			(@ApplicationId is null)
		)
		And
		(
			((@ModuleId is null))
			Or
			((not @ModuleId is null) and (ModuleId = @ModuleId))
		)

	/*

	Declare @C Cursor, @RoleId nvarchar(450)
	Set @C = Cursor FAST_FORWARD for Select RoleId from #T1
	Open @C
	Fetch Next from @C into @RoleId

	While @@FETCH_STATUS = 0
	begin
		
		if(object_ID('tempdb..#T2') = 1)

			Insert #T2(RoleId, ClaimTypeId, ClaimTypeEn, ClaimTypeAr)
				Select @RoleId as RoleId, A.Id as ClaimTypeId, A.EnTitle as ClaimTypeEn, A.ArTitle as ClaimTypeAr 
				From ClaimType A
					Inner Join [dbo].[AspNetRoleClaims] B on A.Id = cast(B.ClaimType as int)
					where 
					(
						( (not @ApplicationId is null) and (ApplicationId = @ApplicationId) )
						or 
						(@ApplicationId is null)
					)
					And
					(
						((@ModuleId is null) and (ModuleId is null))
						Or
						((not @ModuleId is null) and (ModuleId = @ModuleId))
					)
		else
			Select @RoleId as RoleId, A.Id as ClaimTypeId, A.EnTitle as ClaimTypeEn, A.ArTitle as ClaimTypeAr
				into #T2 From ClaimType A
					Inner Join [dbo].[AspNetRoleClaims] B on A.Id = cast(B.ClaimType as int)
					where 
					(
						( (not @ApplicationId is null) and (ApplicationId = @ApplicationId) )
						or 
						(@ApplicationId is null)
					)
					And
					(
						((@ModuleId is null) and (ModuleId is null))
						Or
						((not @ModuleId is null) and (ModuleId = @ModuleId))
					)
		
		Fetch Next from @C into @RoleId
	end

	Close @C
	DeAllocate @C

	Select A.*, B.* from #T1 A inner join #T2 B on A.RoleId = B.RoleId
*/
GO
/****** Object:  StoredProcedure [dbo].[GetUsersByApplications]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create proc [dbo].[GetUsersByApplications]

	@ApplicationId uniqueidentifier
As
	Select Id, UserName, Email from AspNetUsers
	Where Id in 
	(
		Select UserId from AspNetUserRoles A
			Inner Join AspNetRoleClaims B on A.RoleId = B.RoleId
			Inner Join ClaimType C on C.Id = cast(B.ClaimType as int)
		Where 			
			C.ApplicationId = @ApplicationId
	)

/*
	Select * from [dbo].[HraApplications]
	exec GetUsersByApplications
		@ApplicationId = '015660E5-C164-4022-BBCB-D794330B29AE'
	
*/

GO
/****** Object:  StoredProcedure [dbo].[GetUsersByClaimTypes]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[GetUsersByClaimTypes]

	@ApplicationId uniqueidentifier,
	@ClaimTypes dbo.IntID readonly
As

	Select Id, UserName, Email From AspNetUsers
	Where Id in 
	(
		Select UserId from AspNetUserRoles A
			Inner Join AspNetRoles B on B.Id = A.RoleId
			Inner Join AspNetRoleClaims C on B.Id = C.RoleId
			Inner Join ClaimType D on D.Id = cast(C.ClaimType as int)
		Where 
			D.Id in (Select Id from @ClaimTypes)
			and
			D.ApplicationId = @ApplicationId
	)
	And
	LockoutEnabled = 0

/*	
	Declare @T dbo.IntID
	Insert @T values(8), (9)

	exec GetUsersByClaimTypes 
		@ApplicationId = '7A7D333D-B4E7-4C0D-B81F-840A3A0149A4', 
		@ClaimTypes = @T
*/
GO
/****** Object:  StoredProcedure [dbo].[GetUsersByModules]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[GetUsersByModules]

	@ApplicationId uniqueidentifier,
	@Modules dbo.IntID readonly

As
	Select Id, UserName, Email from AspNetUsers
	Where Id in 
	(
		Select UserId from AspNetUserRoles A
			Inner Join AspNetRoleClaims B on A.RoleId = B.RoleId
			Inner Join ClaimType C on C.Id = cast(B.ClaimType as int)
		Where 
			C.ModuleId in (Select Id from @Modules)
			and
			C.ApplicationId = @ApplicationId
	)

GO
/****** Object:  StoredProcedure [dbo].[GetUsersByRoleId]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- This procedure is for all systems
Create proc [dbo].[GetUsersByRoleId]

	@RoleId nvarchar(256)
As

	Select * from AspNetUsers Where Id in (Select UserId from AspNetUserRoles where RoleId = @RoleId)
		
/*
Select * from AspNetUserRoles
exec [GetUsersByRoleId] @RoleId = '32010113-c2b3-4616-846e-a3ed776a89e3'
*/
GO
/****** Object:  StoredProcedure [dbo].[Module_Select]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- This should have paging I think
CREATE proc [dbo].[Module_Select]
	
	-- when passed, only modules used in the claim types will be retrieved
	@ApplicationId uniqueidentifier = null

As

	if(@ApplicationId is null)
		Select * from Module
	else
		Select * from Module A where 
			A.Id in (Select ModuleId from ClaimType Where ApplicationId = @ApplicationId)

/*
	Select * from HraApplications
	Select * from ClaimType

	Module_Select '7A7D333D-B4E7-4C0D-B81F-840A3A0149A4'

	Select * from Module A
		Inner Join ClaimType B on A.Id = B.ModuleId
		Inner Join HraApplications C on C.ApplicationId = B.ApplicationId	
*/
GO
/****** Object:  StoredProcedure [dbo].[Role_Select]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[Role_Select]

	@Page int = 1,
	@Count int = 10,
	@Disabled bit = null,	
	@TotalCount int = null output 

As
	
	select @TotalCount = count(*) from AspNetRoles
	Where 
	(
		((not @Disabled is null) and ([Disabled] = @Disabled))
		or
		(@Disabled is null) 
	)

	Select A.*, UserCount = Isnull(B.UserCount, 0) From AspNetRoles A
	left Join 
		(Select count(UserId) as UserCount, RoleId from AspNetUserRoles Group by RoleId) B on A.Id = B.RoleId		
	Where 
	(
		((not @Disabled is null) and ([Disabled] = @Disabled))
		or
		(@Disabled is null) 
	)
	Order by A.Id 
	Offset ((@Page - 1) * @Count) Rows Fetch next @Count rows only
/*	
	exec Role_Select 1, 5

	Declare @TotalCount int
	exec Role_Select 1, 10, @TotalCount out	
	print(@TotalCount)
*/
GO
/****** Object:  StoredProcedure [dbo].[UpdateRole_Claims]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- AppId and ModuleId should be passed to remove the relevant claims 
CREATE proc [dbo].[UpdateRole_Claims]

	@RoleId nvarchar(425),
	@ApplicationId uniqueidentifier,
	@ModuleId int, 
	@TVP_Grant dbo.IntID readonly
As

	/*
	Select cast(Id as nvarchar(100)) From ClaimType 
	Where ApplicationId = @ApplicationId 
	and				
	(
		((not @ModuleId is null) and (ModuleId = @ModuleId)) -- Module level rights
		or 
		((@ModuleId is null) and (ModuleId is null)) -- Application level rights
	)
	*/

	------------------------------------------------------------------------------------------
	-- Clear all the claims - that belong to (RoleId, ApplicationId, ModuleId) 
	
	Delete [AspNetRoleClaims] 
	Where 
		ClaimType in 
		(
			Select cast(Id as nvarchar(100)) 
			From ClaimType 
			Where ApplicationId = @ApplicationId 
			and				
			(
				((not @ModuleId is null) and (ModuleId = @ModuleId)) -- Module level rights
				or 
				((@ModuleId is null) and (ModuleId is null)) -- Application level rights
			)				
		)
		And
		RoleId = @RoleId

	/*
	Select @RoleId as RoleId, Id as ClaimTypeId From @TVP_Grant
	Select @RoleId as RoleId, Id From @TVP_Grant A
		where not exists(Select * from [AspNetRoleClaims] where RoleId = @RoleId and ClaimType = cast(A.Id as nvarchar(100)) ) 
	*/
	
	----------------------------
	-- Insert the coming claims
	
	Insert [AspNetRoleClaims] (RoleId, ClaimType) 
		Select @RoleId as RoleId, Id as ClaimTypeId From @TVP_Grant A
			where not exists(Select * from [AspNetRoleClaims] where RoleId = @RoleId and ClaimType = cast(A.Id as nvarchar(100))) 
	
/*

Declare 
	@RoleId nvarchar(256) = '317f6220-5650-4e1a-b21c-cb072368ed9d',
	@AppId uniqueidentifier = 'ea5b1598-2d16-4b9a-b575-9c3e29187fab',
	@ModuleId int = 2

Declare @T dbo.IntID
Insert @T values(1),(2)

--Select @RoleId as RoleId, Id From @T A where not exists(Select * from [AspNetRoleClaims] where RoleId = @RoleId and ClaimType = cast(A.Id as nvarchar(100)) ) 
--Select * from [AspNetRoleClaims] where RoleId = @RoleId 

------------------

exec UpdateRole_Claims @RoleId = @RoleId, @ApplicationId = @AppId, 
	@ModuleId = @ModuleId, @TVP_Grant = @T

Select * from AspNetRoleClaims where RoleId = @RoleId
Select * from ClaimType where ModuleId = @ModuleId
------------------
exec GetRoleClaims 
	@RoleId  = @RoleId, 
	@ApplicationId = @AppId,
	@ModuleId = @ModuleId
	
Go



*/
GO
/****** Object:  StoredProcedure [dbo].[UpdateUser_Roles]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[UpdateUser_Roles]
	@UserId nvarchar(256), 
	@TVP_GivenRoleIds dbo.IdentityID readonly
As

	-- Clearing @UserId roles 
	Delete AspNetUserRoles where UserId = @UserId 

	-- Assigning him the given roles
	Insert AspNetUserRoles(UserId, RoleId)
		Select @UserId, Id from @TVP_GivenRoleIds

/*
Select * from AspNetUserRoles where userId = '5dce4a88-65cd-4e20-b634-54ab6b0eccfd'
Select * from AspNetUserRoles where userId = '453a896b-e41e-45dd-81a8-0de883d045f2'
*/
GO
/****** Object:  StoredProcedure [dbo].[User_Select]    Script Date: 4/27/2022 10:16:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[User_Select]

	@Page int = 1,
	@Count int = 10,
	@Text nvarchar(100) = null,
	@Locked bit = null,
	@TotalCount int = null output 

As

	Select * into #T From AspNetUsers
	Where 
	(
		((not @Locked is null) and ([LockoutEnabled] = @Locked))
		or
		(@Locked is null) 
	)
	And
	(
		(
			(not @Text is null) 
			And 
			(
				lower(UserName) like '%' + lower(@Text) + '%'  
				or 
				lower(Email) like '%' + lower(@Text) + '%'  
			)
		)
		or
		(@Text is null) 
	)

	select @TotalCount = count(*) from #T 

	Select * From #T
	Order by Id 
	Offset ((@Page - 1) * @Count) Rows Fetch next @Count rows only

/*	
	exec User_Select 1, 5

	Declare @TotalCount int
	exec User_Select 1, 10, 'min', null, @TotalCount out
	print(@TotalCount)
*/
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[41] 4[20] 2[21] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "AspNetUserRoles"
            Begin Extent = 
               Top = 26
               Left = 364
               Bottom = 122
               Right = 534
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AspNetRoleClaims"
            Begin Extent = 
               Top = 7
               Left = 630
               Bottom = 164
               Right = 800
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ClaimType"
            Begin Extent = 
               Top = 6
               Left = 891
               Bottom = 188
               Right = 1061
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AspNetUsers"
            Begin Extent = 
               Top = 43
               Left = 36
               Bottom = 173
               Right = 260
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 4140
         Width = 3405
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         O' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_getuserrolewithmodule'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'r = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_getuserrolewithmodule'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_getuserrolewithmodule'
GO
USE [master]
GO
ALTER DATABASE [UserManager] SET  READ_WRITE 
GO
