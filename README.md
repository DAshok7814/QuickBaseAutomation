# QuickBaseAutomation

# TO genrate allura file  - This Command should run Where we have .sln file

# dotnet test --logger "trx;LogFileName=TestResults.trx"

# To Generate Allura Report - This Command should run above one level of the Allura report folder
# allure generate allure-results --clean -o allure-report

# TO Start seeing Report 
# allure open allure-report

# To Run Only Category Type Test
# dotnet test --logger "trx;LogFileName=TestResults.trx" --filter "Category=Sanity"