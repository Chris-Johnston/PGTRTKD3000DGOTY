USE [PetGame]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/**
    Pet Table
    
    Stores information about a pet
**/
CREATE TABLE [dbo].[Pet](
    -- Unique identifier for pet
	[PetId] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
    -- name of the pet, uses the same regex as a username, but with spaces
    -- can only have a single space between words, cannot have leading
    -- spaces, but *can* have trailing spaces
	[Name] [varchar](50) NOT NULL
    -- Check that the pet name is valid
    CONSTRAINT CHK_PetName_Valid
	--name must be between 2 and 50 characters
    CHECK (LEN([Name]) BETWEEN 2 AND 50),
    -- when the pet was created, default to now
    [Birthday] DATETIME NOT NULL DEFAULT GETDATE(),
    -- strength is an int [0-100]
    -- randomly set the initial strength to a random value between 
    -- 0 and 10
    [Strength] INT NOT NULL DEFAULT FLOOR(RAND() * 10)
    CONSTRAINT CHK_Strength_Range
    CHECK ([Strength] BETWEEN 0 AND 100),
    -- same with the endurance
    [Endurance] INT NOT NULL DEFAULT FLOOR(RAND() * 10)
    CONSTRAINT CHK_Endurance_Range
    CHECK ([Endurance] BETWEEN 0 AND 100),
    -- is the pet dead, defaults to alive
    [IsDead] BIT NOT NULL DEFAULT 0,
    -- the owner of the pet
    [UserId] [BIGINT] FOREIGN KEY REFERENCES [dbo].[User](UserId)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
) ON [PRIMARY]
GO 