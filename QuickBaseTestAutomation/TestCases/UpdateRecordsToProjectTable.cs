#region MS Directives

using Microsoft.Extensions.Configuration;

#endregion

#region Third party Directives
using Allure.NUnit;
using Newtonsoft.Json.Linq;
using QuickBase.TestAutomationCore;
using Serilog;
using System.Net;
#endregion

namespace QuickBase.TestAutomationSuite.TestCases
{
    [AllureNUnit]
    [TestFixture]
    public class UpdateRecordsToProjectTable
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
                .AddJsonFile(Constants.TestFrameworkSettings, optional: false, reloadOnChange: true);

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
        public void TC_001_200_UpdateRecordsToTable()
        {
            try
            {
                //Arrange
                var data = JsonFileReader.GetJsonFile("TestData\\UpdateRecords\\TC_001_Successful_request_with_valid_user_token_and_data.json");


                if (_headers != null && _configuration != null)
                {
                    var response = client?.PostAsync("", data, _headers).GetAwaiter().GetResult();
                    string addResponseString = QBUtility.DeserializeObject(response);
                    JObject addJObect = JObject.Parse(addResponseString);
                    var addResult = addJObect?["metadata"]["createdRecordIds"];
                    Assert.That(response?.StatusCode, Is.EqualTo(HttpStatusCode.OK));

                    //Act
                    client = new RestClientWrapper(_configuration);
                    var updateData = JsonFileReader.GetJsonFile("TestData\\UpdateRecords\\TC_001_200_UpdateRecordsToTable.json");
                    JObject reqString = QBUtility.ConvertStringToJobject(updateData);

                    //Updating Record value to Update in Project Table
                    reqString["data"][0]["3"]["value"] = addResult;


                    // Update records to Db
                    var updateResponse = client.PostAsync("", QBUtility.ConvertJobjectToString(reqString), _headers).GetAwaiter().GetResult();

                    //Assert
                    Assert.That(updateResponse?.StatusCode, Is.EqualTo(HttpStatusCode.OK));


                    // Verify updated record is correct or not 

                    client = new RestClientWrapper(_configuration);
                    var queryData = JsonFileReader.GetJsonFile("TestData\\UpdateRecords\\TC_001_200_UpdateRecordsToTable_QueryData.json");

                    JObject queryReqString = QBUtility.ConvertStringToJobject(queryData);

                    var numberList = addResult.ToObject<List<string>>();

                    // Update the 'where' clause dynamically
                    string whereClause = queryReqString["where"].ToString();
                    whereClause = whereClause.Replace("dynamicValue", numberList.FirstOrDefault());
                    queryReqString["where"] = whereClause;
                    
                    // Query From Db
                    var queryResponse = client.PostAsync("query", QBUtility.ConvertJobjectToString(queryReqString), _headers).GetAwaiter().GetResult();

                    string responseString = QBUtility.DeserializeObject(queryResponse);
                    JObject jObject = JObject.Parse(responseString);

                    var result = jObject?[Constants.ResponseData]?[0];

                    JObject expectedObject = QBUtility.ReadJsonFileAndConvertJobject("TestData\\UpdateRecords\\TC_001_200_UpdateRecordsToTable_QueryData_Response.json");
                    //Assert
                    Assert.That(queryResponse?.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                    Assert.That(result?[Constants.ProjectName_DbValue]?[Constants.Value]?.ToString(), Is.EqualTo(expectedObject?[Constants.Project_Name]?.ToString()));
                    Assert.That(result?[Constants.Priority_DbValue]?[Constants.Value]?.ToString(), Is.EqualTo(expectedObject?[Constants.Priority]?.ToString()));
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in TC_001_Successful_request_with_valid_user_tokenand_data");
                Assert.Fail($"Unexpected exception occurred: {ex.Message}");
            }
        }
    }
}
