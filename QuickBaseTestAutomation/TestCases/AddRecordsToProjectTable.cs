// Class is used for Inserting the record to the Quickbase table and validating the response

#region MS Directives
using Microsoft.Extensions.Configuration;
using System.Net;
#endregion 


#region Third party Directives
using Log = Serilog.Log;
using Allure.NUnit;
using Newtonsoft.Json.Linq;
using QuickBase.TestAutomationCore;
#endregion

namespace QuickBase.TestAutomationSuite.TestCases
{
    [AllureNUnit]
    [TestFixture]
    public class InsertRecordToProjectDataToQuickbase
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
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _configuration = null;
        }

        [SetUp]
        public void Setup()
        {
            if (_configuration == null)
            {
                Log.Error("Configuration is not initialized");
                throw new InvalidOperationException("Configuration is not initialized.");
            }
            _headers = QBUtility.ConstructHttpHeader(_configuration);
            client = new RestClientWrapper(_configuration);
        }

        [TearDown]
        public void Teardown()
        {
            _headers?.Clear();
            _headers = null;
            client = null;
        }

        [Test]
        public void TC_001_200_Successful_request_with_valid_user_tokenand_data()
        {
            try
            {
                //Arrange
                var data = JsonFileReader.GetJsonFile("TestData\\TC_001_Successful_request_with_valid_user_token_and_data.json");

                JObject expectedObject = QBUtility.ReadJsonFileAndConvertJobject("TestData\\TC_001_Successful_Response.json");

                if (_headers != null && _configuration != null)
                {
                    var response = client?.PostAsync("", data, _headers).GetAwaiter().GetResult();
                    Assert.That(response?.StatusCode, Is.EqualTo(HttpStatusCode.OK));

                    //Act
                    client = new RestClientWrapper(_configuration);
                    var queryData = JsonFileReader.GetJsonFile("TestData\\TC_001_Successful_request_with_valid_user_token_QueryData.json");

                    // Query From Db
                    var queryResponse = client.PostAsync("query", queryData, _headers).GetAwaiter().GetResult();

                    string responseString = QBUtility.DeserializeObject(queryResponse);
                    JObject jObject = JObject.Parse(responseString);

                    var result = jObject?[Constants.ResponseData]?[0];
                    //Assert
                    Assert.That(queryResponse?.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                    Assert.That(result?[Constants.ProjectName_DbValue]?[Constants.Value]?.ToString(), Is.EqualTo(expectedObject?[Constants.Project_Name]?.ToString()));
                    Assert.That(result?[Constants.Priority_DbValue]?[Constants.Value]?.ToString(), Is.EqualTo(expectedObject?[Constants.Priority]?.ToString()));
                    Log.Information("TC_001: Request executed successfully with valid user token and data.");
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in TC_001_Successful_request_with_valid_user_tokenand_data");
                Assert.Fail($"Unexpected exception occurred: {ex.Message}");
            }
        }

        [Test]
        [Repeat(2)]
        public void TC_002_401_Unauthorized_request_due_to_invalid_user_token()
        {
            try
            {
                //Arrange
                var result = TestCaseUtility.Unauthorized_Request_DuetoInvalid_Usertoken(_headers, client, HttpMethod.Post, Constants.DataForProjectTableJson);

                //Assert
                Assert.That(result.Response?.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
                Assert.That(result.ResponseString, Is.EqualTo(result.ErrorResponseDetailsJson?[Constants.Access_Denied]?.ToString()));

            }
            catch (Exception ex)
            {
                Log.Error("Error in TC_002_Unauthorized_request_due_to_invalid_user_token");
                Assert.Fail($"Unexpected exception occurred: {ex.Message}");
            }
        }


        [Test, Category("Sanity")]
        public void TC_002_400_Unauthorized_request_due_to_missing_user_token()
        {
            try
            {
                //Arrange & Act
                var result = TestCaseUtility.Unauthorized_Request_DuetoMissing_Usertoken(_headers, client, HttpMethod.Post, Constants.DataForProjectTableJson);

                //Assert 
                Assert.That(result.Response?.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(result.ResponseString, Is.EqualTo(result.ErrorResponseDetailsJson?[Constants.Authorization_header_Missed]?.ToString()));

            }
            catch (Exception ex)
            {
                Log.Error("Error in TC_002_Unauthorized_request_due_to_missing_user_token");
                Assert.Fail($"Unexpected exception occurred: {ex.Message}");
            }
        }


        [Test]
        public void TC_003_404_NotFound_Wrong_MethodType()
        {
            try
            {
                // Arrange & Act
                var result = TestCaseUtility.NotFound_Wrong_MethodType(_headers, client);

                //Assert
                Assert.That(result.Response?.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                Assert.That(result.ResponseString, Is.EqualTo(result.ErrorResponseDetailsJson?[Constants.Invalid_HttpMethod]?.ToString()));

            }
            catch (Exception ex)
            {
                Log.Error("Error in TC_003_404_NotFound_Wrong_MethodType");
                Assert.Fail($"Unexpected exception occurred: {ex.Message}");
            }
        }

        [Test]
        public void TC_003_404_NotFound_WrongURL()
        {
            try
            {
                // Arrange
                var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile(@"TestData\TC_003_404_NotFound_WrongURL.json", optional: false, reloadOnChange: true);

                var result = TestCaseUtility.NotFound_WrongURL(_headers, client, builder, HttpMethod.Post);

                //Assert
                Assert.That(result.Response?.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                Assert.That(result.ResponseString, Is.EqualTo(result.ErrorResponseDetailsJson?[Constants.Invalid_HttpMethod]?.ToString()));

            }
            catch (Exception ex)
            {
                Log.Error("Error in TC_003_404_NotFound_WrongURL");
                Assert.Fail($"Unexpected exception occurred: {ex.Message}");
            }
        }

        [Test]
        public void TC_004_403_Forbidden()
        {
            try
            {
                // Arrange
                var result = TestCaseUtility.Forbidden(_headers, client, HttpMethod.Post);
                //Assert
                Assert.That(result.Response?.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
                Assert.That(result.Response?.Content, Is.EqualTo(result.ErrorResponseDetailsJson?[Constants.Forbidden_Error]?.ToString()));

            }
            catch (Exception ex)
            {
                Log.Error("Error in TC_004_403_Forbidden");
                Assert.Fail($"Unexpected exception occurred: {ex.Message}");
            }
        }


        [TestCase("TestData\\TC_005_400_BadRequest_Without_Required_DataKey.json", "Required_Datakey_Missed")]
        [TestCase("TestData\\TC_005_400_BadRequest_Without_Required_TableKey.json", "Required_Tokey_Missed")]
        public void TC_005_400_BadRequest(string path, string key)
        {
            try
            {
                // Arrange 
                var data = JsonFileReader.GetJsonFile(path);

                if (_headers != null && _configuration != null)
                {
                    JObject jObject = QBUtility.ReadJsonFileAndConvertJobject(Constants.ErrorResponseDetailsJson);

                    client = new RestClientWrapper(_configuration);
                    //Act
                    var response = client?.PostAsync("", data, _headers).GetAwaiter().GetResult();
                    string responseString = QBUtility.DeserializeObject(response);

                    //Assert 
                    Assert.That(response?.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                    Assert.That(responseString, Is.EqualTo(jObject?[key].ToString()));
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in TC_005_400_BadRequest");
                Assert.Fail($"Unexpected exception occurred: {ex.Message}");
            }
        }

        [Test]
        public void TC_006_408_RequestTimeOut()
        {
            try
            {

                //Arrange
                var builder = new ConfigurationBuilder()
                     .SetBasePath(Directory.GetCurrentDirectory())
                     .AddJsonFile(@"TestData\TC_006_408_RequestTimeout.json", optional: false, reloadOnChange: true);
                client = new RestClientWrapper(builder.Build());

                var result = TestCaseUtility.RequestTimeOut(_headers, client, builder, HttpMethod.Post);
                if (result?.ErrorMessage == "A task was canceled.") throw new TaskCanceledException();
                //Assert 
                Assert.Fail("Expected a timeout exception, but the request succeeded.");

            }
            catch (TaskCanceledException)
            {
                Assert.Pass("The request timed out as expected.");
            }
            catch (Exception ex)
            {
                Log.Error("Error in TC_006_408_RequestTimeOut");
                Assert.Fail($"Unexpected exception occurred: {ex.Message}");
            }
        }

        [Test]
        [Ignore("This is not handled in QB")]
        public void TC_007_405_Method_NotAllowed()
        {
            try
            {
                //Arrange 
                var result = TestCaseUtility.Method_NotAllowed(_headers, client, HttpMethod.Post);

                //Assert 
                Assert.That(result?.StatusCode, Is.EqualTo(HttpStatusCode.MethodNotAllowed));

            }
            catch (Exception ex)
            {
                Log.Error("Error in TC_007_405_Method_NotAllowed");
                Assert.Fail($"Unexpected exception occurred: {ex.Message}");
            }
        }

        [TestCase("TestData\\TC_008_DataValidation_Data_CaseSesitive.json", "Required_Datakey_Missed")]
        [TestCase("TestData\\TC_008_DataValidation_To_CaseSesitive.json", "Required_Tokey_Missed")]
        [TestCase("TestData\\TC_008_DataValidation_Data_String.json", "Data_ShouldBeArray")]
        public void TC_008_DataValidation(string path, string key)
        {
            try
            {
                // Arrange 
                var data = JsonFileReader.GetJsonFile(path);

                if (_headers != null && _configuration != null)
                {
                    JObject jObject = QBUtility.ReadJsonFileAndConvertJobject(Constants.ErrorResponseDetailsJson);

                    client = new RestClientWrapper(_configuration);
                    //Act
                    var response = client?.PostAsync("", data, _headers).GetAwaiter().GetResult();
                    string responseString = QBUtility.DeserializeObject(response);

                    //Assert 
                    Assert.That(response?.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                    Assert.That(responseString, Is.EqualTo(jObject?[key].ToString()));
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in TC_005_400_BadRequest");
                Assert.Fail($"Unexpected exception occurred: {ex.Message}");
            }
        }

        [Test]
        [Repeat(2)]
        public void TC_008_401_Unauthorized_WrongTableName()
        {
            try
            {
                //Arrange
                var result = TestCaseUtility.Unauthorized_Request_DuetoInvalid_Usertoken(_headers, client, HttpMethod.Post, Constants.TC_008_401_Unauthorized_WrongTableName);

                //Assert
                Assert.That(result.Response?.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
                Assert.That(result.ResponseString, Is.EqualTo(result.ErrorResponseDetailsJson?[Constants.Access_Denied]?.ToString()));

            }
            catch (Exception ex)
            {
                Log.Error("Error in TC_002_Unauthorized_request_due_to_invalid_user_token");
                Assert.Fail($"Unexpected exception occurred: {ex.Message}");
            }
        }
    }

}