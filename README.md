___
[DataManager Project Overview](#datamanager-project-overview)
- [Goal](#goal)
- [Limitations](#limitations)

[Pitches](#pitches)
- [Backend Pitch](#backend-pitch)
	- [Goal](#goal)
 	- [Requirements](#requirements)
  		- [Parsing Input Files and Storing them into a Database](#parsing-input-files-and-storing-them-into-a-database)
    		- [ModelOne (CSV)](#modelone-(csv))
      		- [ModelTwo (XLSX)](#modeltwo-(xlsx))
        	- [Data Retrieval by Filter](#data-retrieval-by-filter)
         	- [Display and Export Retrieved Data](#display-and-export-retrieved-data)
          	- [Test](#test)
- [Web API Pitch](#web-api-pitch)
	- [Goal](#goal)
 	- [Requirements](#requirements)  	 
___
# DataManager Project Overview

## Goal

As part of the onboarding process for a Junior Backend position, I was provided with two project pitches: 

1. Backend
2. Web API

These were designed to help me achieve several key objectives:

1. Acquiring proficiency in writing C# code.
2. Utilizing .NET and Entity Framework to build web applications.
3. Learning and applying object-oriented programming (OOP) principles.
4. Gaining foundational knowledge of web development.
5. Becoming adept at using Visual Studio Code, Visual Studio 2022, the dotnet CLI, NuGet, and other essential tools relevant to the position.

## Limitations

The project needed to be completed with the following:

- Programming Language: [C#](https://dotnet.microsoft.com/en-us/languages/csharp)
- Framework: [Entity Framework](https://learn.microsoft.com/en-us/ef/)
- Database: [PostgreSQL](https://www.postgresql.org/)

No limitations were imposed on how the tasks should be solved, except for the requirement to use free software. It was clearly stated that the tasks were intended to serve as a guide rather than a strict requirement, allowing for multiple sensible approaches to their completion. There were also no restrictions on the tools, IDEs, code editors, NuGet packages, or any other tools needed to get the job done.
___
# Pitches

> [!IMPORTANT]
> This project was refactored to use generic names, models, and data. `ModelOne` and `Gain Amount One` are examples of generic names for a model and its property, respectively. This was done to avoid revealing any proprietary information. For this exact reason, data is generated using a [Python script](https://github.com/IvanPavlak/Data_Manager/blob/e334610f720c6bb6e9999b82ad9b9a48345bb313/DataManager.Core/Database/Data/generate_data.ipynb), and all the code has been refactored to operate with this generated data. Despite this refactoring, the data still closely mirrors the challenges I encountered and resolved to meet the project requirements.

___
## Backend Pitch

### Goal

The goal of this task is to familiarize yourself with most of the basic functionalities and layers of the web application through a specific complex problem (focusing solely on logic, without creating a user interface). Approach the problem analytically, understand its domain, and choose a technical solution for each part of the problem.

Upon completion, the ideal scenario is that the new functionality can be easily integrated into the existing web application.

The isolated functionalities to be implemented include:

- Parsing input files (`CSV`, `XLSX`)
- Storing a selected set of parsed data in a database
- Retrieving and displaying stored data based on a specified filter (e.g. `Date`)
- Combining and calculating data from both files 
- Writing tests
___
### Requirements

#### Parsing Input Files and Storing them into a Database

##### ModelOne (CSV)

Research the appropriate NuGet packages for reading and writing `CSV` files and choose the most suitable one (most commonly used, most popular, regularly maintained, best documentation, etc.).

Read the input file and save the data in the database. Choose `PostgreSQL` as the database and `Entity Framework` as the ORM. Create a new database, not the existing one used by the web application (all data will later be transferred to the existing database of the web application).

Input data:

- `Exit`
- `Port`
- `User Group`
- `Country`
- `Member ID`
- `Date`
- `Gain Amount One`
- `Gain Amount Two`
- `Loss Amount`
- `Total Amount`

##### ModelTwo (XLSX)

Same as `ModelOne` but for `XLSX` files.

Input data: 

- `PeriodStartDate`
- `Exit`
- `Gain Amount Three`
- `PeriodEndDate`

Use the date for matching with `ModelOne` data. Exits from both input files are the same entities, thus, they are stored in the same table in the database. Store values for all gain amounts. Calculate the ratio of `totalModelOne/totalModelTwo`.

Add meaningful validations where needed - e.g., prohibit entering negative numbers for gain amounts, empty strings for names, incorrect date formats, etc.
___
#### Data Retrieval by Filter

Retrieve stored data using arbitrary filters (entered through a query in the console application).

Examples:

- `dateFrom` (mandatory)
- `dateTo` (optional)
- `exitId` (optional
	- If not provided, retrieve for all exits in the specified period

Retrieve data for:

- `ModelOnes`
- `ModelTwos`
- `Combination` of `ModelOnes` and `ModelTwos`
    - Match by `Date` and `Exit`
    - Return fields: 
	    - `Date`
	    - `Exit`
	    - `TotalModelOne`
	    - `TotalModelTwo`
	    - `Ratio` (%)

Implement pagination (PageSize, CurrentPage, TotalItems)
___
#### Display and Export Retrieved Data

Through the console app, allow selection of:

- Type of retrieval (e.g., "Enter the number for the type of retrieval: `ModelOnes - 1`, `ModelTwos - 2`, `Combination - 3`")
- Then, "Enter parameters" (one by one, `dateFrom`, `dateTo`, and `exitId`)
- After retrieval, display e.g., the first 10 retrieved rows
- Ask "Would you like to export: `No - 0`, `XLSX - 1`, `CSV - 2`?"

Using the same NuGet packages for reading `XLSX` and `CSV`, save the retrieved data in the selected format.
___
#### Test

Create a new project for unit tests (e.g., xUnit), `DataManager.Test`

Use a mock library to mock the Entity Framework repository
- Previously everyone used Moq, but some stopped after this [incident](https://snyk.io/blog/moq-package-exfiltrates-user-emails/) - if there's nothing better, Moq is acceptable).

Create a few simple scenarios for testing:

- Correct matching by `Date` and `Exit`
- Percentage calculation for a matched pair of `ModelOne` and `ModelTwo`
___
## Web API Pitch

### Goal

After implementing all the necessary functionalities in the console application, it is required to enable them through a Web API as well. Therefore, each retrieval or saving operation must be performable via a call to the Web API.
### Requirements

It will be necessary to implement all `GET` methods for data retrieval with configured pagination and filtering and create appropriate DTOs for request and response according to the needs.

Study the other HTTP methods and implement appropriate endpoints for creating, updating and deleting entities in the database - `PUT`, `POST` and `DELETE`.

When implementing the API, use `Swagger` (configured by default when creating a new web API project).

After implementing all functionalities, add user authentication - prohibit API calls from unauthorized users. For practice purposes, there is no need to create a user in the database; it is sufficient to hardcode the username and password and then check them when verifying credentials. Use `JWT` as the authentication method.
___
