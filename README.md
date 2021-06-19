# dotnet 5 restful API server

### Features
    - Swagger UI
    - JWT Authentication 
    - Identity and Entity Framework Core
    - Auto register services dependencies from the assembly while startup
    - CQRS Policy
    - API Versioning
    - Register, Login, Refresh Token endpoints
    - Employee Crud
    - Employee Repository
    - Employee Service
    
    
### Instructions
    - Clean and then build the solution
    - Run migration command. With the package manager console, the command is "Update-Database" and with .Net Core CLI the command is "dotnet ef database update"
    - Then run the project
    - Register a user to test the application

### Notes
With Swagger UI the applications all endpoints can be tested. there is an "Authorize" button at the top right corner of the swagger screen. Click the button. In the popup, there will be a value input filed. Paste your token as "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9" in this format and click authorize. Now close the popup. Now you should be able to use all protected customer endpoints from the swagger screen. If you accidentally reload the page, you have to go through this process again.

## Thanks 
Md. Monir Hossain