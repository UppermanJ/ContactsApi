# Pre Build Requirements
As it is, this application expects a folder destination for Logs and Databases (Production, Development and Automated Tests).

If you wish to use the destinations already configured, just run the following commands in powershell:
```ps1
New-Item -Path "c:\" -Name "Coding_Challenge_Upperman" -ItemType "directory"
New-Item -Path "c:\Coding_Challenge_Upperman" -Name "Prod-DB" -ItemType "directory"
New-Item -Path "c:\Coding_Challenge_Upperman" -Name "Dev-DB" -ItemType "directory"
New-Item -Path "c:\Coding_Challenge_Upperman" -Name "Test-DB" -ItemType "directory"
New-Item -Path "c:\Coding_Challenge_Upperman" -Name "Log" -ItemType "directory"
$env:Coding_Challenge_Upperman = 'c:\Coding_Challenge_Upperman\Test-DB\'
```
> **Note that this expects you to have a C drive.**

Alternatively you can create your own folders, just know that you will need to update the following locations:
* appsettings.Development.Json line 8
* appsettings.Json line 8
* nlog.config line 6

> **Note that the integration tests will require you to have an environment variable with the desired directory for the integration database location.  That environment variable is named `Coding_Challenge_Upperman` nad is referenced in the file LiteDbTestBase on line 15.**

#### Required for build

You will need .NET Core 3.1 SDK and Runtime which can be found here: https://dotnet.microsoft.com/download/visual-studio-sdks

# Building

Once you have pulled, navigate in powershell to the contacts folder and run the command: `dotnet build Contacts.sln -o "./build" -c release `

Then, simply run the generated `API.exe` in the `build` folder.  The API will then be reachable at `localhost:5000`.

# Architectural decisions

I try to maintain unidirectional flow.  For context, here's the VS generated dependency graph: 
![Architecture view for Contacts With Tests](https://user-images.githubusercontent.com/56522001/134828137-3c470bdc-57ce-4a48-bd11-597e1cb55ddf.png)
And here it is without the tests (Which are excluded from production builds)
![Architecture view for Contacts_4](https://user-images.githubusercontent.com/56522001/134828238-ab2dd195-d7b1-4403-8c71-d8141f8df536.png)

