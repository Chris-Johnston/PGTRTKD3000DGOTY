-- Drop the table if it already exists
IF OBJECT_ID('PetGame.PetActivity', 'U') IS NOT NULL
DROP TABLE PetGame.PetActivity
GO
-- Create the table in the specified schema
CREATE TABLE PetGame.PetActivity
(
    -- primary key for for table
    ActivityId BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
    
    -- foreign key from PET.PetId
    PetId BIGINT NOT NULL FOREIGN KEY REFERENCES PET(PetId),
    
    -- timestamp of activty
    Timestamp DateTime NOT NULL,   
    
    -- type of activty done by pet
    Type CHAR(1) NOT NULL,
    -- checks if activity type is valid
    CHECK (Type in ('d', 't'))
);