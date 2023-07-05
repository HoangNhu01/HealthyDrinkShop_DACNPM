# stt30_HealthyDrinkShop_DACNPM
Green &amp; Clean - Web Application for healthy drink fanatic
## BE
## Technologies
- ASP.NET Core 3.1
- Entity Framework Core 3.1
## Install Tools
- .NET Core SDK 3.1
- Git client
- Visual Studio 2022
- SQL Server 2022
## How to configure and run
- Clone code from Github: git clone https://github.com/HoangNhu01/HealthyDrinkShop_DACNPM.git
- Open solution eShopSolution.sln in Visual Studio 2022
- Set startup project is eShopSolution.Data
- Change connection string in Appsetting.json in eShopSolution.Data project
- Open Tools --> Nuget Package Manager -->  Package Manager Console in Visual Studio
- Run Update-database and Enter.
- After migrate database successful, set Startup Project is eShopSolution.WebApp
- Change database connection in appsettings.Development.json in eShopSolution.WebApp project.
- Choose profile to run or press F5
## How to contribute
- Fork and create your branch
- Create Pull request to us.

##FE
- Open terminal of FE folder
- Run npm i --force
- Run npm start
