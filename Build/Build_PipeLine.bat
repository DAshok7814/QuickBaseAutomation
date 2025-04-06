@echo off
REM Set the path to the .NET SDK (If not in PATH environment variable)
set DOTNET_PATH="C:\Program Files\dotnet"

REM Path to the solutions/projects
set SOLUTION_PATH="D:\Learning\DotNet\QB\QuickBaseAutomation\QuickBaseTestAutomation.sln"
set EXE_PATH="D:\Learning\DotNet\QB\QuickBaseAutomation\TestResultMailer\bin\Release\net8.0\TestResultMailer.exe"
set "TestResults_Directory=D:\Learning\DotNet\QB\QuickBaseAutomation\QuickBaseTestAutomation\bin\Debug\net8.0"

REM Clean the output directories
echo Cleaning previous build outputs...
%DOTNET_PATH%\dotnet clean %SOLUTION_PATH%
@echo "Cleaning Solution successfully"


REM Build the solutions/projects
echo Building Solution...
%DOTNET_PATH%\dotnet build %SOLUTION_PATH% --configuration Release
@echo "Building Solution successfully"

REM Optional: Run tests
echo Running Tests for Solution...
%DOTNET_PATH%\dotnet test --logger "trx;LogFileName=TestResults.trx" %SOLUTION_PATH%
@echo "Tests run successfully"

REM Call the Report_Generate script 
call Generate_Reports.bat 

popd

REM Check the errorlevel to see if the child script failed
if ERRORLEVEL 1 (
    echo Generate_Reports script failed.
    pause
    exit /b 1
)

if not exist %EXE_PATH% (
    echo Error: %EXE_PATH% does not exist.
    exit /b 1
)

@echo "Sending E-mail"
start "" %EXE_PATH%


pushd "%TestResults_Directory%"

@echo Opening Allure report
allure open allure-report
set "errorlevel=%ERRORLEVEL%"
IF ERRORLEVEL 1 (
    @echo Failed to open Allure report
    popd
    popd
    pause
    exit /b %errorlevel%
)

@echo "Removing temporary directory change"

popd

REM If you need to publish the built projects
REM echo Publishing Solution1...
REM %DOTNET_PATH%\dotnet publish %SOLUTION_PATH% --configuration Release --output "C:\Path\To\Publish\Output1"


echo Build and test process completed!
pause