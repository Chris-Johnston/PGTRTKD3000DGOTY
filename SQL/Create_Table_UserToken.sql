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