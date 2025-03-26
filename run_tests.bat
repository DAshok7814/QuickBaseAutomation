@echo off


REM Run NUnit tests with specific category and generate .trx file
@echo "Running Testcases"
dotnet test --logger "trx;LogFileName=TestResults.trx"

@echo "Tests run successfully"

REM Mentiond testResult directory - To get the report of all test cases
set "test_results_directory=D:\Learning\DotNet\QB\QuickBaseAutomation\QuickBaseTestAutomation\bin\Debug\net8.0"

@echo "Generate Allura report"

@echo "Changing directory to generate Allure report"

pushd "%test_results_directory%"

IF ERRORLEVEL 1 (
    @echo Failed to change directory
	popd
    pause
    exit /b
)

@echo The current directory is: %CD%

@echo "Generating Allure report"
allure generate allure-results --clean -o allure-report
set "errorlevel=%ERRORLEVEL%"

IF ERRORLEVEL 1 (
    @echo Failed to generate Allure report
    popd
    pause
    exit /b %errorlevel%
)

@echo "Report Generated Successfully..."

@echo Opening Allure report
allure open allure-report
set "errorlevel=%ERRORLEVEL%"
IF ERRORLEVEL 1 (
    @echo Failed to open Allure report
    popd
    pause
    exit /b %errorlevel%
)


@echo "Removing temporary directory change"
popd
REM Optionally, generate report as static files and serve from additional tools
REM allure generate allure-results --clean -o allure-report

pause