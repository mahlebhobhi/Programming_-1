-- ============================================================
-- RaceDay Database Script
-- SQL Server / SSMS
-- ============================================================
-- This script:
-- 1. Creates the RaceDay database
-- 2. Creates all tables, keys and constraints
-- 3. Inserts realistic sample data
-- 4. Can be safely re-run on the same SQL Server instance
-- ============================================================

USE master;
GO

IF DB_ID('RaceDayDB') IS NULL
BEGIN
    CREATE DATABASE RaceDayDB;
END;
GO

USE RaceDayDB;
GO

-- ------------------------------------------------------------
-- Remove old tables so the script can be re-run during testing
-- ------------------------------------------------------------
IF OBJECT_ID('dbo.Result', 'U') IS NOT NULL DROP TABLE dbo.Result;
IF OBJECT_ID('dbo.Enrolment', 'U') IS NOT NULL DROP TABLE dbo.Enrolment;
IF OBJECT_ID('dbo.EventCategory', 'U') IS NOT NULL DROP TABLE dbo.EventCategory;
IF OBJECT_ID('dbo.Category', 'U') IS NOT NULL DROP TABLE dbo.Category;
IF OBJECT_ID('dbo.Event', 'U') IS NOT NULL DROP TABLE dbo.Event;
IF OBJECT_ID('dbo.Participant', 'U') IS NOT NULL DROP TABLE dbo.Participant;
IF OBJECT_ID('dbo.Organiser', 'U') IS NOT NULL DROP TABLE dbo.Organiser;
IF OBJECT_ID('dbo.UserAccount', 'U') IS NOT NULL DROP TABLE dbo.UserAccount;
GO

-- ============================================================
-- 1. USER ACCOUNT
-- Stores login and basic profile information.
-- ============================================================
CREATE TABLE dbo.UserAccount
(
    UserID INT IDENTITY(1,1) NOT NULL,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    PhoneNumber NVARCHAR(20) NULL,
    UserRole NVARCHAR(20) NOT NULL
        CONSTRAINT CK_UserAccount_UserRole
        CHECK (UserRole IN ('Organiser', 'Participant')),
    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_UserAccount_CreatedAt DEFAULT SYSDATETIME(),

    CONSTRAINT PK_UserAccount PRIMARY KEY (UserID),
    CONSTRAINT UQ_UserAccount_Email UNIQUE (Email)
);
GO

-- ============================================================
-- 2. ORGANISER
-- One organiser profile belongs to one user account.
-- ============================================================
CREATE TABLE dbo.Organiser
(
    OrganiserID INT IDENTITY(1,1) NOT NULL,
    UserID INT NOT NULL,
    OrganisationName NVARCHAR(100) NOT NULL,

    CONSTRAINT PK_Organiser PRIMARY KEY (OrganiserID),
    CONSTRAINT UQ_Organiser_UserID UNIQUE (UserID),
    CONSTRAINT FK_Organiser_UserAccount
        FOREIGN KEY (UserID) REFERENCES dbo.UserAccount(UserID)
);
GO

-- ============================================================
-- 3. PARTICIPANT
-- One participant profile belongs to one user account.
-- ============================================================
CREATE TABLE dbo.Participant
(
    ParticipantID INT IDENTITY(1,1) NOT NULL,
    UserID INT NOT NULL,
    EmergencyContactName NVARCHAR(100) NULL,
    EmergencyContactPhone NVARCHAR(20) NULL,

    CONSTRAINT PK_Participant PRIMARY KEY (ParticipantID),
    CONSTRAINT UQ_Participant_UserID UNIQUE (UserID),
    CONSTRAINT FK_Participant_UserAccount
        FOREIGN KEY (UserID) REFERENCES dbo.UserAccount(UserID)
);
GO

-- ============================================================
-- 4. EVENT
-- Each event is created by one organiser.
-- ============================================================
CREATE TABLE dbo.Event
(
    EventID INT IDENTITY(1,1) NOT NULL,
    OrganiserID INT NOT NULL,
    EventName NVARCHAR(120) NOT NULL,
    EventDate DATE NOT NULL,
    Location NVARCHAR(150) NOT NULL,
    Description NVARCHAR(500) NULL,
    EventStatus NVARCHAR(20) NOT NULL
        CONSTRAINT CK_Event_EventStatus
        CHECK (EventStatus IN ('Open', 'Closed', 'Completed')),
    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Event_CreatedAt DEFAULT SYSDATETIME(),

    CONSTRAINT PK_Event PRIMARY KEY (EventID),
    CONSTRAINT FK_Event_Organiser
        FOREIGN KEY (OrganiserID) REFERENCES dbo.Organiser(OrganiserID)
);
GO

-- ============================================================
-- 5. CATEGORY
-- Categories are reusable, e.g. 5km, 10km, Half Marathon.
-- ============================================================
CREATE TABLE dbo.Category
(
    CategoryID INT IDENTITY(1,1) NOT NULL,
    CategoryName NVARCHAR(80) NOT NULL,
    DistanceKm DECIMAL(5,2) NOT NULL,
    MinimumAge INT NOT NULL,
    MaximumAge INT NULL,

    CONSTRAINT PK_Category PRIMARY KEY (CategoryID),
    CONSTRAINT UQ_Category_Name UNIQUE (CategoryName),
    CONSTRAINT CK_Category_Distance CHECK (DistanceKm > 0),
    CONSTRAINT CK_Category_MinAge CHECK (MinimumAge >= 0),
    CONSTRAINT CK_Category_MaxAge
        CHECK (MaximumAge IS NULL OR MaximumAge >= MinimumAge)
);
GO

-- ============================================================
-- 6. EVENT CATEGORY
-- Resolves the many-to-many relationship between Event
-- and Category. An event can have many categories and a
-- category can be used by many events.
-- ============================================================
CREATE TABLE dbo.EventCategory
(
    EventID INT NOT NULL,
    CategoryID INT NOT NULL,

    CONSTRAINT PK_EventCategory PRIMARY KEY (EventID, CategoryID),
    CONSTRAINT FK_EventCategory_Event
        FOREIGN KEY (EventID) REFERENCES dbo.Event(EventID),
    CONSTRAINT FK_EventCategory_Category
        FOREIGN KEY (CategoryID) REFERENCES dbo.Category(CategoryID)
);
GO

-- ============================================================
-- 7. ENROLMENT
-- Links participants to an event category.
-- A participant can enter many event categories, but cannot
-- enter the same category twice.
-- ============================================================
CREATE TABLE dbo.Enrolment
(
    EnrolmentID INT IDENTITY(1,1) NOT NULL,
    ParticipantID INT NOT NULL,
    EventID INT NOT NULL,
    CategoryID INT NOT NULL,
    EnrolmentDate DATETIME2 NOT NULL
        CONSTRAINT DF_Enrolment_Date DEFAULT SYSDATETIME(),
    EnrolmentStatus NVARCHAR(20) NOT NULL
        CONSTRAINT CK_Enrolment_Status
        CHECK (EnrolmentStatus IN ('Registered', 'Cancelled', 'Completed')),

    CONSTRAINT PK_Enrolment PRIMARY KEY (EnrolmentID),
    CONSTRAINT UQ_Enrolment_Participant_Event_Category
        UNIQUE (ParticipantID, EventID, CategoryID),

    CONSTRAINT FK_Enrolment_Participant
        FOREIGN KEY (ParticipantID) REFERENCES dbo.Participant(ParticipantID),

    CONSTRAINT FK_Enrolment_EventCategory
        FOREIGN KEY (EventID, CategoryID)
        REFERENCES dbo.EventCategory(EventID, CategoryID)
);
GO

-- ============================================================
-- 8. RESULT
-- Stores the participant's result for an enrolment.
-- ============================================================
CREATE TABLE dbo.Result
(
    ResultID INT IDENTITY(1,1) NOT NULL,
    EnrolmentID INT NOT NULL,
    FinishPosition INT NULL,
    FinishTimeSeconds INT NULL,
    ResultStatus NVARCHAR(20) NOT NULL
        CONSTRAINT CK_Result_Status
        CHECK (ResultStatus IN ('Finished', 'DidNotFinish', 'Disqualified')),
    RecordedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Result_RecordedAt DEFAULT SYSDATETIME(),

    CONSTRAINT PK_Result PRIMARY KEY (ResultID),
    CONSTRAINT UQ_Result_Enrolment UNIQUE (EnrolmentID),
    CONSTRAINT CK_Result_Position
        CHECK (FinishPosition IS NULL OR FinishPosition > 0),
    CONSTRAINT CK_Result_Time
        CHECK (FinishTimeSeconds IS NULL OR FinishTimeSeconds > 0),
    CONSTRAINT FK_Result_Enrolment
        FOREIGN KEY (EnrolmentID) REFERENCES dbo.Enrolment(EnrolmentID)
);
GO

-- ============================================================
-- SAMPLE DATA
-- ============================================================

-- Two organisers
INSERT INTO dbo.UserAccount
    (FirstName, LastName, Email, PasswordHash, PhoneNumber, UserRole)
VALUES
    ('Thabo', 'Mokoena', 'thabo@raceday.co.za', 'HASH_demo_thabo', '0712345678', 'Organiser'),
    ('Lerato', 'Dlamini', 'lerato@raceday.co.za', 'HASH_demo_lerato', '0723456789', 'Organiser');

INSERT INTO dbo.Organiser (UserID, OrganisationName)
VALUES
    (1, 'Maluti Running Club'),
    (2, 'Free State Athletics');

-- Two participants
INSERT INTO dbo.UserAccount
    (FirstName, LastName, Email, PasswordHash, PhoneNumber, UserRole)
VALUES
    ('Kabelo', 'Molefe', 'kabelo@example.com', 'HASH_demo_kabelo', '0734567890', 'Participant'),
    ('Naledi', 'Mokoena', 'naledi@example.com', 'HASH_demo_naledi', '0745678901', 'Participant');

INSERT INTO dbo.Participant
    (UserID, EmergencyContactName, EmergencyContactPhone)
VALUES
    (3, 'Mpho Molefe', '0756789012'),
    (4, 'Palesa Mokoena', '0767890123');

-- Three events
INSERT INTO dbo.Event
    (OrganiserID, EventName, EventDate, Location, Description, EventStatus)
VALUES
    (1, 'Maluti Mountain Run 2026', '2026-10-10', 'Phuthaditjhaba', 'A scenic road and mountain running event.', 'Open'),
    (2, 'Bloemfontein Spring Race 2026', '2026-10-24', 'Bloemfontein', 'Community road race for runners of different levels.', 'Open'),
    (1, 'Golden Gate Challenge 2026', '2026-11-07', 'Golden Gate Highlands', 'A challenging trail running event.', 'Open');

-- Three categories
INSERT INTO dbo.Category
    (CategoryName, DistanceKm, MinimumAge, MaximumAge)
VALUES
    ('5km Fun Run', 5.00, 10, NULL),
    ('10km Road Race', 10.00, 16, NULL),
    ('Half Marathon', 21.10, 18, NULL);

-- Give each event all three categories
INSERT INTO dbo.EventCategory (EventID, CategoryID)
VALUES
    (1, 1), (1, 2), (1, 3),
    (2, 1), (2, 2), (2, 3),
    (3, 1), (3, 2), (3, 3);

-- Sample enrolments
INSERT INTO dbo.Enrolment
    (ParticipantID, EventID, CategoryID, EnrolmentStatus)
VALUES
    (1, 1, 2, 'Completed'),
    (1, 2, 1, 'Registered'),
    (2, 1, 3, 'Completed'),
    (2, 3, 2, 'Registered');

-- Sample results for completed enrolments
INSERT INTO dbo.Result
    (EnrolmentID, FinishPosition, FinishTimeSeconds, ResultStatus)
VALUES
    (1, 12, 3125, 'Finished'),
    (3, 7, 5210, 'Finished');
GO

-- ============================================================
-- TEST QUERIES
-- These can be run after the script to prove the database works.
-- ============================================================

SELECT * FROM dbo.UserAccount;
SELECT * FROM dbo.Organiser;
SELECT * FROM dbo.Participant;
SELECT * FROM dbo.Event;
SELECT * FROM dbo.Category;
SELECT * FROM dbo.EventCategory;
SELECT * FROM dbo.Enrolment;
SELECT * FROM dbo.Result;
GO

-- Useful joined report
SELECT
    e.EventName,
    c.CategoryName,
    u.FirstName + ' ' + u.LastName AS ParticipantName,
    en.EnrolmentStatus,
    r.FinishPosition,
    r.FinishTimeSeconds
FROM dbo.Enrolment en
INNER JOIN dbo.Participant p
    ON en.ParticipantID = p.ParticipantID
INNER JOIN dbo.UserAccount u
    ON p.UserID = u.UserID
INNER JOIN dbo.Event e
    ON en.EventID = e.EventID
INNER JOIN dbo.Category c
    ON en.CategoryID = c.CategoryID
LEFT JOIN dbo.Result r
    ON en.EnrolmentID = r.EnrolmentID
ORDER BY e.EventDate, c.DistanceKm, r.FinishPosition;
GO
