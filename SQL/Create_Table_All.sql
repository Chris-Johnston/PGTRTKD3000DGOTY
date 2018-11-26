USE [PetGame]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/**
    User table

    Stores information about users, including their username, userid, 
    password hash, and HMAC key.

    Passwords are hashed using a HMAC512 hashing algorithm.

    UserIds are unsigned 64 bit integers.

    Both usernames and user ID's are unique.

    Passwords may be the same, but because of the HMAC hashing algorithm (and randomly generated HMAC
    Keys), the same password will hash to a different result for two users. This is what we want.

    Usernames must be between 2 and 50 characters. Their content is not validated,
    at the SQL level, this is handled with a regular expression at the API level.
    Usernames cannot contain whitespace
**/

CREATE TABLE [dbo].[User](
    -- UserId is an identity field, starting at 1, increments by 1
	[UserId] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Username] [varchar](50) UNIQUE NOT NULL
    -- Check that the username is valid
    CONSTRAINT CHK_Username_Valid
    -- ensure that usernames are between 2 and 50 characters
    -- and doesn't contain whitespace
    CHECK ((LEN([Username]) BETWEEN 2 AND 50) AND [Username] NOT LIKE '% %'),
    -- don't need to constrain password hashes, these are set by only the API level programatically
    -- we can assume will always be valid
	[PasswordHash] [binary](64) NOT NULL,
    -- same goes for the HMAC key
	[HMACKey] [binary](256) NOT NULL
) ON [PRIMARY]
GO

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
	[Name] [varchar](50) UNIQUE NOT NULL
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

USE[PetGame]
GO
/**
PetActivity Table keeps track of all activities related to the pet

Some activites that are included are Training and Default
along with these meta data is stored such as timestamp of activty
**/
CREATE TABLE Activity
(
    -- primary key for for table
    ActivityId BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
    
    -- foreign key from PET.PetId assume not null
    PetId BIGINT FOREIGN KEY REFERENCES dbo.Pet(PetId)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,

    -- timestamp of activty
    Timestamp DateTime NOT NULL DEFAULT GETDATE(),   
    
    -- type of activty done by pet
    Type CHAR(1) NOT NULL DEFAULT 'd',
    -- checks if activity type is valid
    CHECK (Type in ('d', 't'))
);

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


USE [PetGame]
GO

/**
    UserToken Table
    
    Stores tokens that users can use to log in.

    Tokens have a LastUsed field that is updated whenever it is used.
    Tokens that haven't been used in a while will be invalid, and the 
    user will be prompted to log in again.

    One user may have [0, inf) tokens. A user token must reference
    an existing token.
**/

CREATE TABLE [dbo].[UserToken](
    -- UserTokenId is an identity field, starting at 1, increments by 1
	[UserTokenId] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
    -- Must reference a user. This will fail if the User table
    -- is not already created first.
    [UserId] [bigint] FOREIGN KEY REFERENCES [dbo].[User](UserId)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
    -- Tokens are very long strings that are set by the API
    -- and are assumed to be valid. They will always be NOT NULL
    -- and UNIQUE.
	[Token] [varchar](1024) UNIQUE NOT NULL,
    -- when this user token was last used
    -- defaults to the current time
    [LastUsed] DATETIME NOT NULL DEFAULT GETDATE(),
    -- when this token was first created, default to the server time
    [Created] DATETIME NOT NULL DEFAULT GETDATE(),
) ON [PRIMARY]
GO