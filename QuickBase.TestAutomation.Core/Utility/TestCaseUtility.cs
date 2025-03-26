#region Third party Directives
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using RestSharp;
#endregion

namespace QuickBase.TestAutomationCore
{
    public class TestCaseUtility
    {
        /// <summary>
        /// Method to get the response for the unauthorized request due to invalid user token
        /// </summary>
        /// <param name="_headers"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static (RestResponse Response, string ResponseString, JObject ErrorResponseDetailsJson) Unauthorized_Request_DuetoInvalid_Usertoken(Dictionary<string, string>? _headers
            , RestClientWrapper? client, HttpMethod httpMethod,string inputFileName)
        {
            //Arrange
            var data = JsonFileReader.GetJsonFile(inputFileName);
            if (_headers != null && client != null)
            {
                JObject jObject = QBUtility.ReadJsonFileAndConvertJobject(Constants.ErrorResponseDetailsJson);
                _headers[Constants.Authorization] = "QB-USER-TOKEN can3pf_rjp6_0_iypmqgba8xcqebh57v3wyb46t3";

                var response = new RestResponse();
                //Act
                if (httpMethod == HttpMethod.Post)
                {
                    response = client.PostAsync("", data, _headers).GetAwaiter().GetResult();
                }
                else if (httpMethod == HttpMethod.Delete)
                {
                    response = client.DeleteAsync("", data, _headers).GetAwaiter().GetResult();
                }
                if (response != null)
                {
                    return (Response: response, ResponseString: QBUtility.DeserializeObject(response) ?? string.Empty, ErrorResponseDetailsJson: jObject);
                }
            }
            return (Response: null, ResponseString: string.Empty, ErrorResponseDetailsJson: null);
        }

        /// <summary>
        /// Method to get the response for the unauthorized request due to missing user token
        /// </summary>
        /// <param name="_headers"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static (RestResponse Response, string ResponseString, JObject ErrorResponseDetailsJson) Unauthorized_Request_DuetoMissing_Usertoken(Dictionary<string, string>? _headers,
            RestClientWrapper? client, HttpMethod httpMethod, string inputFileName)
        {
            //Arrange
            // Arrange 
            var data = JsonFileReader.GetJsonFile(inputFileName);
            if (_headers != null)
            {
                JObject jObject = QBUtility.ReadJsonFileAndConvertJobject(Constants.ErrorResponseDetailsJson);
                _headers.Remove(Constants.Authorization);
                //Act
                var response = new RestResponse();
                //Act
                if (httpMethod == HttpMethod.Post)
                {
                    response = client.PostAsync("", data, _headers).GetAwaiter().GetResult();
                }
                else if (httpMethod == HttpMethod.Delete)
                {
                    response = client.DeleteAsync("", data, _headers).GetAwaiter().GetResult();
                }

                string responseString = QBUtility.DeserializeObject(response);
                if (response != null)
                {
                    return (Response: response, ResponseString: QBUtility.DeserializeObject(response) ?? string.Empty, ErrorResponseDetailsJson: jObject);
                }
            }
            return (Response: null, ResponseString: string.Empty, ErrorResponseDetailsJson: null);
        }

        /// <summary>
        /// Method to get the response for the NotFound_Wrong_MethodType request
        /// </summary>
        /// <param name="_headers"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static (RestResponse Response, string ResponseString, JObject ErrorResponseDetailsJson) NotFound_Wrong_MethodType(Dictionary<string, string>? _headers,
            RestClientWrapper? client)
        {
            var data = JsonFileReader.GetJsonFile(Constants.DataForProjectTableJson);
            if (_headers != null)
            {

                //Act

                var response = client.GetAsync("", _headers).GetAwaiter().GetResult();

                JObject jObject = QBUtility.ReadJsonFileAndConvertJobject(Constants.ErrorResponseDetailsJson);

                string responseString = QBUtility.DeserializeObject(response);
                if (response != null)
                {
                    return (Response: response, ResponseString: QBUtility.DeserializeObject(response) ?? string.Empty, ErrorResponseDetailsJson: jObject);
                }
            }
            return (Response: null, ResponseString: string.Empty, ErrorResponseDetailsJson: null);
        }

        /// <summary>
        /// Method to get the response for the NotFound_WrongURL request 
        /// </summary>
        /// <param name="_headers"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static (RestResponse Response, string ResponseString, JObject ErrorResponseDetailsJson) NotFound_WrongURL(Dictionary<string, string>? _headers, RestClientWrapper? client,
            IConfigurationBuilder builder, HttpMethod httpMethod)
        {
            var data = JsonFileReader.GetJsonFile(Constants.DataForProjectTableJson);
            if (_headers != null)
            {
                client = new RestClientWrapper(builder.Build());

                //Act
                var response = new RestResponse();
                //Act
                if (httpMethod == HttpMethod.Post)
                {
                    response = client.PostAsync("", data, _headers).GetAwaiter().GetResult();
                }
                else if (httpMethod == HttpMethod.Delete)
                {
                    response = client.DeleteAsync("", data, _headers).GetAwaiter().GetResult();
                }

                JObject jObject = QBUtility.ReadJsonFileAndConvertJobject(Constants.ErrorResponseDetailsJson);

                string responseString = QBUtility.DeserializeObject(response);
                if (response != null)
                {
                    return (Response: response, ResponseString: QBUtility.DeserializeObject(response) ?? string.Empty, ErrorResponseDetailsJson: jObject);
                }
            }
            return (Response: null, ResponseString: string.Empty, ErrorResponseDetailsJson: null);
        }

        /// <summary>
        /// Method to get the response for the Forbidden request 
        /// </summary>
        /// <param name="_headers"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static (RestResponse Response, JObject ErrorResponseDetailsJson) Forbidden(Dictionary<string, string>? _headers, RestClientWrapper? client
            , HttpMethod httpMethod)
        {
            var data = JsonFileReader.GetJsonFile(Constants.DataForProjectTableJson);
            if (_headers != null)
            {
                _headers.Remove("QB-Realm-Hostname");

                //Act
                var response = new RestResponse();
                //Act
                if (httpMethod == HttpMethod.Post)
                {
                    response = client.PostAsync("", data, _headers).GetAwaiter().GetResult();
                }
                else if (httpMethod == HttpMethod.Delete)
                {
                    response = client.DeleteAsync("", data, _headers).GetAwaiter().GetResult();
                }

                JObject jObject = QBUtility.ReadJsonFileAndConvertJobject(Constants.ErrorResponseDetailsJson);
                if (response != null)
                {
                    return (Response: response, ErrorResponseDetailsJson: jObject);
                }
            }
            return (Response: null, ErrorResponseDetailsJson: null);
        }

        /// <summary>
        /// Method to get the response for the RequestTimeOut request 
        /// </summary>
        /// <param name="_headers"></param>
        /// <param name="client"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static RestResponse? RequestTimeOut(Dictionary<string, string>? _headers, RestClientWrapper? client
            , IConfigurationBuilder builder, HttpMethod httpMethod)
        {
            var data = JsonFileReader.GetJsonFile(Constants.DataForProjectTableJson);

            if (_headers != null)
            {
                JObject jObject = QBUtility.ReadJsonFileAndConvertJobject(Constants.ErrorResponseDetailsJson);

                //Act
                if (httpMethod == HttpMethod.Post)
                {
                    return client.PostAsync("", data, _headers).GetAwaiter().GetResult();
                }
                else if (httpMethod == HttpMethod.Delete)
                {
                    return client.DeleteAsync("", data, _headers).GetAwaiter().GetResult();
                }
            }
            return null;
        }

        /// <summary>
        /// Method to get the response for the Method_NotAllowed request
        /// </summary>
        /// <param name="_headers"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static RestResponse? Method_NotAllowed(Dictionary<string, string>? _headers, RestClientWrapper? client, HttpMethod httpMethod)
        {
            var data = JsonFileReader.GetJsonFile(Constants.DataForProjectTableJson);

            if (_headers != null)
            {
                JObject jObject = QBUtility.ReadJsonFileAndConvertJobject(Constants.ErrorResponseDetailsJson);

                //Act
                var response = new RestResponse();
                //Act
                if (httpMethod == HttpMethod.Post)
                {
                    response = client.PostAsync("", data, _headers).GetAwaiter().GetResult();
                }
                else if (httpMethod == HttpMethod.Delete)
                {
                    response = client.DeleteAsync("", data, _headers).GetAwaiter().GetResult();
                }
            }
            return null;
        }
    }
}
