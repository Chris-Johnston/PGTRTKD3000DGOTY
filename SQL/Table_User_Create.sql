USE [PetGame]
GO

/****** Object:  Table [dbo].[User]    Script Date: 11/07/2018 4:17:59 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[User](
	[UserId] [bigint] IDENTITY(1,1) NOT NULL,
	[Username] [varchar](50) NULL,
	[PasswordHash] [binary](64) NOT NULL,
	[HMACKey] [binary](256) NOT NULL
) ON [PRIMARY]
GO

