USE master;
IF DB_ID('ShopWCF') IS NOT NULL
   DROP DATABASE ShopWCF;
CREATE DATABASE ShopWCF collate Ukrainian_CI_AS;
GO

USE ShopWCF ;
IF OBJECT_ID('Manufacturer', 'U') IS NOT NULL
   DROP TABLE Manufacturer;
CREATE TABLE dbo.Manufacturer
(
   ManufacturerId int not null IDENTITY(1,1) primary key,
   ManufacturerName NVARCHAR(20) NOT NULL
);
GO
INSERT INTO dbo.Manufacturer (ManufacturerName) 
VALUES ('Samsung'),('Lenovo'),('Nokia'),('Huawei'),('Sony'),('LG'),('Acer'),('HP'),('Canon'),('Asus');

IF OBJECT_ID('Category', 'U') IS NOT NULL
   DROP TABLE Category;
CREATE TABLE dbo.Category
(
   CategoryId int not null IDENTITY(1,1) primary key,
   CategoryName NVARCHAR(20) NOT NULL
);
GO
INSERT INTO dbo.Category (CategoryName) 
VALUES ('Smartphone'),('Notebook'),('Printer'),('Telephon');

IF OBJECT_ID('Good', 'U') IS NOT NULL
   DROP TABLE Good;
CREATE TABLE dbo.Good
(
   GoodId int not null IDENTITY(1,1) primary key,
   GoodName VARCHAR(100) NOT NULL,
   ManufacturerId int foreign key REFERENCES dbo.Manufacturer(ManufacturerId), 
   CategoryId int foreign key REFERENCES dbo.Category(CategoryId), 
   Price money not null default 0,
   GoodCount numeric(18,3) not null default 0,
   Photo NVARCHAR(100)
);
GO

INSERT INTO dbo.Good (GoodName, ManufacturerId, CategoryId, Price, GoodCount, Photo) 
VALUES 
		('HP LaserJet P2035 (CE461A)', 8,3, 5445, 12, 'images/photo1.jpg'),
		('Canon i-SENSYS MF212W with Wi-F' ,9,3,3999, 7, 'images/photo2.jpg'),
		('Samsung SCX-4650N ', 1,3,3999, 15, 'images/photo3.jpg'),
		('HP DJ1510 (B2L56C) ', 8,3,1199, 2, 'images/photo4.jpg'),
		('Asus Transformer Book T100TAF 32GB',10,2, 4999, 11, 'images/photo5.jpg'),
		('Lenovo Flex 10 (59427902)', 2, 2, 5555, 11, 'images/photo6.jpg'),
		('Acer Aspire ES1-411-C1XZ', 7,2,6399, 7, 'images/photo7.jpg'),
		('HP 255 G4 (N0Y69ES)', null, 2, 6199, 5, 'images/photo8.jpg'),
		('HP ProBook 430 (N0Y70ES)', 8,2,12400, 3, 'images/photo9.jpg'),
		('Ultrabook Samsung 535U3', 1, null, 8000,10, 'images/photo10.jpg'),
		('Samsung Galaxy S3 Neo Duos I9300i Black', 1,1,3999, 7, 'images/photo11.jpg'),
		('Lenovo A5000 Black', 2,1,3299, 5, 'images/photo12.jpg'),
		('Microsoft Lumia 640 XL (Nokia)', 3,1, 4888, 5, 'images/photo13.jpg'),
		('LG G3s Dual D724 Titan', 6, 1,3999, 0, 'images/photo14.jpg');

IF OBJECT_ID('Order', 'U') IS NOT NULL
   DROP TABLE [Order];
CREATE TABLE [dbo].[Order]
(
   OrderId int not null IDENTITY(1,1) primary key,
   OrderDate date not null default GetDate(),
   OrderSum money not null
);
GO

IF OBJECT_ID('OrderItem', 'U') IS NOT NULL
   DROP TABLE OrderItem;
CREATE TABLE dbo.OrderItem
(
   OrderItemId int not null IDENTITY(1,1) primary key,
   OrderId int foreign key REFERENCES [dbo].[Order](OrderId), 
   GoodId int foreign key REFERENCES [dbo].[Good](GoodId), 
   GoodCount numeric(18,3) not null,
   OrderItemSum money not null
);
GO