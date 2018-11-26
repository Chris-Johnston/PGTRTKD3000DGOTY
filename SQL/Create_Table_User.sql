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