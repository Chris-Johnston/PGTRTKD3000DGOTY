USE [PetGame]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--This table represents races. Each row is one Race completed by one Pet
--Each row contains the RaceId, the Score, the Timestamp (race completion), and the PetId
CREATE TABLE [dbo].[Race](
	--RaceId is the primary key, and for each Race, must be unique 
	[RaceId] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	--The pet's score in this race
	[Score] INT NOT NULL
	--Check that the score is valid
	CONSTRAINT CHK_Score_Valid
	--Score must be greater than 0
	CHECK ([Score] > 0),
	--The timestamp is when the score is inserted into the table, after the race ends
	[Timestamp] DATETIME NOT NULL DEFAULT GETDATE(),
	--Foreign key references the PetId attribute in the Pet table
	[PetId] BIGINT NOT NULL FOREIGN KEY REFERENCES Pet(PetId)
	--Nothing else should change upon deletion or update of this Race
	ON DELETE NO ACTION
	ON UPDATE NO ACTION
) ON [PRIMARY]
GO