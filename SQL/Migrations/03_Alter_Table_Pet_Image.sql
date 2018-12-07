/**
	Alters the Pet table to include the image number of the pet.
	Because the image number is a hard-coded path that will never change, this information does not have to be stored in the database, 
	and instead can be a hard-coded dict in the API code.
*/

ALTER TABLE Pet ADD
	-- by default, all existing pets will have the default image
	PetImageId INT NOT NULL DEFAULT(0)
	CONSTRAINT PetImageId_IsNotNegative
	CHECK (PetImageId >= 0);

-- Sampe usage
-- UPDATE Pet SET PetImageId = 123 WHERE PetId = 1;