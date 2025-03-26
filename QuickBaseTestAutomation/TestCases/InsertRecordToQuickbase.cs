#region MS Directives
using AventStack.ExtentReports.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using QuickBase.TestAutomationLogicHub;
using Serilog;
using System.Net;
using Log = Serilog.Log;
#endregion 

namespace QuickBase.TestAutomationSuite.TestCases
{
    [TestFixture]
    public class InsertRecordToQuickbase
    {
        /// <summary>
        /// Configuration root object
        /// </summary>
        private IConfigurationRoot? _configuration;

        /// <summary>
        /// Headers for the request
        /// </summary>
        private Dictionary<string, string>? _headers;

        /// <summary>
        /// Rest client object
        /// </summary>
        private RestClientWrapper? client;



        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(@"TestConfiguration\\TestFrameworkSettings.json", optional: false, reloadOnChange: true);

            _configuration = builder.Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(_configuration)
                .Enrich.FromLogContext()
                .CreateLogger();

            _headers = QBUtility.ConstructHttpHeader(_configuration);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _headers?.Clear();
            _headers = null;
            Log.CloseAndFlush();
        }

        [SetUp]
        public void Setup()
        {


            client = new RestClientWrapper(_configuration);
        }

        [TearDown]
        public void Teardown()
        {
            client = null;
        }

        [Test]
        public void TC_001_Successful_request_with_valid_user_tokenand_data()
        {
            var data = JsonFileReader.GetJsonFile("TestData\\TC_001_Successful_request_with_valid_user_token_and_data.json");
            //data = data.Replace("\n", "").Replace("\r", "");
            if (_headers != null)
            {
                var response = client?.PostAsync("", data, _headers).GetAwaiter().GetResult();
                Log.Information("This is a test log message");
                Assert.That(response?.StatusCode, Is.EqualTo(HttpStatusCode.OK));

                client = new RestClientWrapper(_configuration);
                var data1 = JsonFileReader.GetJsonFile("TestData\\TC_001_Successful_request_with_valid_user_token_QueryData.json");
                var queryResponse = client.PostAsync("query", data1, _headers).GetAwaiter().GetResult();
                string responseString = QBUtility.DeserializeObject(queryResponse);
                JObject jObject = JObject.Parse(responseString);






                //Assert
                Assert.That(queryResponse?.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            }
        }

        [Test]
        public void TC_002_Unauthorized_request_due_to_invalid_user_token()
        {
            var data = JsonFileReader.GetJsonFile("TestData\\TC_DataForProjectTable.json");
            if (_headers != null)
            {
                _headers["Authorization"] = "QB-USER-TOKEN can3pf_rjp6_0_iypmqgba8xcqebh57v3wyb46t3";
                var response = client?.PostAsync("", data, _headers).GetAwaiter().GetResult();
                Assert.That(response?.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            }
        }


        [Test, Category("Sanity")]
        public void TC_002_Unauthorized_request_due_to_missing_user_token()
        {
            // Arrange 
            var data = JsonFileReader.GetJsonFile("TestData\\TC_DataForProjectTable.json");
            if (_headers != null)
            {
                _headers.Remove("Authorization");
                //Act
                var response = client?.PostAsync("", data, _headers).GetAwaiter().GetResult();
                //Assert 
                Assert.That(response?.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                //Assert.AreEqual(response.Content, "{\"message\":\"Bad Request\",\"description\":\"Required header 'authorization' not found\"}");
            }
        }


        [Test]
        public void TC_004_404_NotFound()
        {
            // Arrange 
            var data = JsonFileReader.GetJsonFile("TestData\\TC_DataForProjectTable.json");
            
            if (_headers != null)
            {
                //Act
                var response = client?.PostAsync("", data, _headers).GetAwaiter().GetResult();
                //Assert 
                Assert.That(response?.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            }

        }

        [Test]
        public void TC_010_Invalid_HTTPmethod()
        {
            var data = JsonFileReader.GetJsonFile("TestData\\TC_001_Successful_request_with_valid_user_token_and_data.json");
            if (_headers != null)
            {
                _headers.Remove("Authorization");
                var response = client?.GetAsync("", _headers).GetAwaiter().GetResult();

                JObject jObject = QBUtility.ReadJsonFileAndConvertJobject("TestData\\ErrorResponseDetails.json");
                
                string responseString = QBUtility.DeserializeObject(response);

                //Assert
                Assert.That(response?.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                Assert.That(responseString, Is.EqualTo(jObject?["Invalid_HttpMethod"].ToString()));
            }
        }

    }
} 