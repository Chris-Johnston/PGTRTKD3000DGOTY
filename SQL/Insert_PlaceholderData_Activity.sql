USE [PetGame] 
GO
 -- inserts data into the Activty table
 -- resembles data associated with pets
 -- data was created randomly by hand but associated with used PetId's
 -- some GetDate()'s were used to test function
 -- ActivityId data will be auto populated and Type will be defaulted to 'd'
INSERT INTO ACTIVITY (PetId, TimeStamp) VALUES (1, '03/07/2018 10:50:50 PM');
INSERT INTO ACTIVITY (PetId, TimeStamp) VALUES (2, GetDate());
INSERT INTO ACTIVITY (PetId, TimeStamp) VALUES (3, '03/09/2018 10:50:50 PM');
INSERT INTO ACTIVITY (PetId, TimeStamp) VALUES (4, '03/10/2018 10:50:50 PM');
INSERT INTO ACTIVITY (PetId, TimeStamp) VALUES (5, '03/11/2018 10:50:50 PM');
INSERT INTO ACTIVITY (PetId, TimeStamp) VALUES (6, '03/12/2018 10:50:50 PM');
INSERT INTO ACTIVITY (PetId, TimeStamp) VALUES (7, GetDate());
INSERT INTO ACTIVITY (PetId, TimeStamp) VALUES (8, GetDate());
INSERT INTO ACTIVITY (PetId, TimeStamp) VALUES (9, '03/15/2018 10:50:50 PM');
INSERT INTO ACTIVITY (PetId, TimeStamp) VALUES (10, '03/16/2018 10:50:50 PM');