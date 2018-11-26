USE [PetGame]
GO

--This is garbage, made up data. It was created here without the use of an external application
--Used PetId 1-10, to correspond with the placeholder Pets in this database
INSERT INTO Race (Score, Timestamp, PetId) VALUES (10, GETDATE(), 1);
INSERT INTO Race (Score, Timestamp, PetId) VALUES (12, GETDATE(), 2);
INSERT INTO Race (Score, Timestamp, PetId) VALUES (15, GETDATE(), 3);
INSERT INTO Race (Score, Timestamp, PetId) VALUES (20, GETDATE(), 4);
INSERT INTO Race (Score, Timestamp, PetId) VALUES (60, GETDATE(), 5);
INSERT INTO Race (Score, Timestamp, PetId) VALUES (100, '02/06/2018 12:00:00 AM', 6);
INSERT INTO Race (Score, Timestamp, PetId) VALUES (250, '03/07/2018 10:50:50 PM', 7);
INSERT INTO Race (Score, Timestamp, PetId) VALUES (1, '06/10/2018 12:12:12 AM', 8);
INSERT INTO Race (Score, Timestamp, PetId) VALUES (300, '10/10/2018 01:01:01 PM', 9);
INSERT INTO Race (Score, Timestamp, PetId) VALUES (147, '12/24/2018 11:59:59 PM', 10);