-- Create database
CREATE DATABASE ArisHotel;
GO

USE ArisHotel;
GO

-- Roles table
CREATE TABLE Roles (
    RoleId INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL UNIQUE
);

-- Users table (system users only)
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    UserName NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(100) NOT NULL,
    RoleId INT NOT NULL,
    FOREIGN KEY (RoleId) REFERENCES Roles(RoleId)
);

-- Rooms table
CREATE TABLE Rooms (
    RoomId INT IDENTITY(1,1) PRIMARY KEY,
    RoomNumber NVARCHAR(10) NOT NULL UNIQUE,
    Capacity INT NOT NULL,
    Price DECIMAL(10,2) NOT NULL
);

-- Bookings table
CREATE TABLE Bookings (
    BookingId INT IDENTITY(1,1) PRIMARY KEY,
    RoomId INT NOT NULL,
    CreatedByUserId INT NOT NULL,     -- system user who created the record
    GuestName NVARCHAR(100) NOT NULL, -- guest name
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    FOREIGN KEY (RoomId) REFERENCES Rooms(RoomId),
    FOREIGN KEY (CreatedByUserId) REFERENCES Users(UserId)
);

-- Insert default roles
INSERT INTO Roles (RoleName) VALUES
('Owner'),
('Sysadmin'),
('Manager');
