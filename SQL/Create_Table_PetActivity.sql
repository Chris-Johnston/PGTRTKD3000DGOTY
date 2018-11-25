GO
USE PetGame

/**
PetActivity Table keeps track of all activites PET's do in the PetGame

Some activites that are included are Training and Default
along with these meta data is stored such as timestamp of activty
**/
CREATE TABLE PetActivity
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