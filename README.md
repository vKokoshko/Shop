# Shop-WCF

This is the recent WCF final course project.

The goal was to make a shopping program with following features:
1. Get list of goods from database using WCF service;
2. Show goods info;
3. Make shopping cart with ability to add/delete goods and to change it quantity;
4. Create a new order from cart and save it to database using another WCF service (using transactions);
5. Show information about orders;

In this project I used WCF service library to host required services,
ADO.NET Entity Framework (code first from database) to work with MSSQL Server
and WPF application as a client

To run it you'll need MSSQL Server running. You should use ShopWCF.sql script to create DB to work with. And maybe you'll need to change connection string in ServiceLibrary App.config file (don't forget to change it's name in ShopContext.cs file if needed)
