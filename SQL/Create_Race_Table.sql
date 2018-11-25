USE [PetGame]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Race](
	--RaceId is the primary key, and for each Race, must be unique 
	[RaceId] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	--The pet's score in this race
	[Score] INT NOT NULL,
	--The timestamp is when the score is inserted into the table, after the race ends
	[Timestamp] DATETIME NOT NULL,
	--Foreign key references the PetId attribute in the Pet table
	[PetId] BIGINT NOT NULL FOREIGN KEY REFERENCES Pet(PetId)
	--Nothing else should change upon deletion or update of this Race
	ON DELETE NO ACTION
	ON UPDATE NO ACTION
) ON [PRIMARY]
GO