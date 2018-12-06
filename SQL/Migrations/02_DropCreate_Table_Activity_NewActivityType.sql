/*
Migration #02, run these in order

Drops and recreates the Activity table.
Unfortunately it's kinda annoying to update a constraint that isn't named,
so it's just easier to recreate the table.

Also, nothing is dependent on the Activity table, so it won't break everything.
*/

DROP TABLE Activity;

-- Recreate the table with the new Type constraint
CREATE TABLE Activity
(
    -- primary key for for table
    ActivityId BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
    
    -- foreign key from PET.PetId assume not null
    PetId BIGINT FOREIGN KEY REFERENCES dbo.Pet(PetId)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,

    -- timestamp of activty
    [Timestamp] DateTime NOT NULL DEFAULT GETDATE(),   
    
    -- type of activty done by pet
    [Type] CHAR(1) NOT NULL DEFAULT 'd',
    -- checks if activity type is valid
	CONSTRAINT Type_Check_Valid_Activity
    CHECK ([Type] in ('d', 't', 'f', 'u', 'r', 's', 'c'))
);
