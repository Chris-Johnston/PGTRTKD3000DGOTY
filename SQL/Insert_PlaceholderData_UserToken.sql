USE [PetGame]
GO

-- Inserts sample data into the UserToken table
-- these example tokens are invalid
-- but are similar to the ones that would be generated
-- by the API

INSERT INTO [UserToken] (UserId, Token) VALUES (1, 'TokenA');
INSERT INTO [UserToken] (UserId, Token) VALUES (1, 'TokenB');
INSERT INTO [UserToken] (UserId, Token) VALUES (2, 'TokenC');
INSERT INTO [UserToken] (UserId, Token) VALUES (3, 'TokenD');
INSERT INTO [UserToken] (UserId, Token) VALUES (4, 'TokenE');
INSERT INTO [UserToken] (UserId, Token) VALUES (1, 'TokenF');
INSERT INTO [UserToken] (UserId, Token) VALUES (2, 'TokenG');
INSERT INTO [UserToken] (UserId, Token) VALUES (3, 'TokenH');
INSERT INTO [UserToken] (UserId, Token) VALUES (4, 'TokenI');
INSERT INTO [UserToken] (UserId, Token) VALUES (8, 'TokenJ');
INSERT INTO [UserToken] (UserId, Token) VALUES (4, 'TokenK');