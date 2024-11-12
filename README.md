# Ensek Tech Task 

## Project Overview

This project implements a robust C# Web API that fulfills ENSEK's requirements for handling customer meter readings, allowing for bulk data uploads, validation, and storage. The solution leverages Entity Framework and is designed to streamline the process of recording and validating customer energy data, ensuring data integrity and ease of management for ENSEK's Account Managers.

## Assumptions

1. Reading value format `NNNNN` means a reading value could be an integer from 0 to 99999
2. Multiple records in one csv with the same account number will be ignored completely
3. Datetime is a local datetime

## Unit Testing Approach

I have just tested the main two interfaces witin my application layer. I have opted for limited mocking and implented an in memory database. While coverage is not exhaustive, it provides a clear example of the testing strategy applied.

## Enitity Framework Migrations 

I am using entity framework with migrations to create the databse locally.

Command to add a new migration
```bash
dotnet ef migrations add -p DataAccess [Name]
```

Command to create the database locally (connection argument will vary depending on your setup)
```bash
dotnet ef database update -p DataAccess --connection="Server=(localdb)\\local;Database=MeterReadings;Integrated Security=true;Database=MeterReadings"
```