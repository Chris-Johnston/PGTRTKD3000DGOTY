/**
04

Fixes the name constraint on Pet.Name. Pet names should only be unique for a specific owner, not 
to the domain of all pets.

YOU WILL NEED TO REMOVE THE CONSTRAINT IN THE DATABASE YOURSELF
*/

-- this value may be different
ALTER TABLE [dbo].[Pet] DROP CONSTRAINT [UQ__Pet__737584F6A5E3B265]
GO


ALTER TABLE [Pet]
	ALTER COLUMN Name [varchar](50) NOT NULL;