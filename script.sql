USE [Mediatonic]
GO
/****** Object:  Schema [Mediatonic]    Script Date: 27/10/2019 13:48:13 ******/
CREATE SCHEMA [Mediatonic]
GO
/****** Object:  Table [dbo].[Animal]    Script Date: 27/10/2019 13:48:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Animal](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[type] [text] NOT NULL,
	[happinessDefault] [int] NOT NULL,
	[happinessDecrease] [int] NOT NULL,
	[hungerDefault] [int] NOT NULL,
	[hungerIncrease] [int] NOT NULL,
	[isDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Animal] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AnimalOwnership]    Script Date: 27/10/2019 13:48:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AnimalOwnership](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[userId] [bigint] NOT NULL,
	[animalId] [bigint] NOT NULL,
	[name] [text] NOT NULL,
	[hunger] [int] NOT NULL,
	[happiness] [int] NOT NULL,
	[lastUpdated] [datetime2](7) NOT NULL,
	[isDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_AnimalOwnership] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 27/10/2019 13:48:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[username] [text] NULL,
	[isDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_isDeleted]  DEFAULT ((0)) FOR [isDeleted]
GO
ALTER TABLE [dbo].[AnimalOwnership]  WITH CHECK ADD  CONSTRAINT [FK_AnimalOwnership_Animal] FOREIGN KEY([animalId])
REFERENCES [dbo].[Animal] ([id])
GO
ALTER TABLE [dbo].[AnimalOwnership] CHECK CONSTRAINT [FK_AnimalOwnership_Animal]
GO
ALTER TABLE [dbo].[AnimalOwnership]  WITH CHECK ADD  CONSTRAINT [FK_AnimalOwnership_User] FOREIGN KEY([userId])
REFERENCES [dbo].[User] ([id])
GO
ALTER TABLE [dbo].[AnimalOwnership] CHECK CONSTRAINT [FK_AnimalOwnership_User]
GO
