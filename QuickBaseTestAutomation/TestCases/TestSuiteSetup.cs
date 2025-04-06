

#region MS Directives
using Microsoft.Extensions.Configuration;
using QuickBase.TestAutomationCore;
using Serilog;
#endregion


namespace QuickBase.TestAutomationSuite.TestCases
{
    [SetUpFixture]
    public class TestSuiteSetup
    {
        [OneTimeSetUp]
        public void Setup()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(Constants.TestFrameworkSettings, optional: false, reloadOnChange: true);


            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                .CreateLogger();

        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Log.CloseAndFlush();
        }
    }
}
