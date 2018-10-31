USE [PetGame]
GO

/****** Object:  Table [dbo].[User]    Script Date: 10/31/2018 2:23:05 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- TODO: Add primary key constraints to the User table

CREATE TABLE [dbo].[User](
	[UserId] [bigint] NOT NULL,
	[Username] [varchar](50) NOT NULL,
	[PasswordHash] [binary](256) NOT NULL,
	[HMACKey] [binary](256) NOT NULL
) ON [PRIMARY]
GO

