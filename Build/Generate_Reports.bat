@echo off

REM Mentiond testResult directory - To get the report of all test cases
set "TestResults_Directory=D:\Learning\DotNet\QB\QuickBaseAutomation\QuickBaseTestAutomation\bin\Debug\net8.0"

pushd "%TestResults_Directory%"

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