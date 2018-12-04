/*
Migration #01, run these in order

Alters the User table to add the PhoneNumber column.
This defaults to a NULL value.
Numbers are stored as strings, in the format:
+1XXXYYYZZZZ
where XXX = area code
YYYZZZZ = phone number.

We are assuming that all phone numbers are located in the US.
This is the format that is accepted by the Twilio API, which is what is planned for SMS.
*/

ALTER TABLE [User] ADD
	PhoneNumber char(12) NULL DEFAULT(NULL)
	CONSTRAINT PhoneNumber_IsNumeric
	CHECK (PhoneNumber IS NULL OR PhoneNumber LIKE '+1[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]');

-- sample usage
-- UPDATE [User] Set PhoneNumber = '+14255555555' WHERE Username LIKE 'chris%';