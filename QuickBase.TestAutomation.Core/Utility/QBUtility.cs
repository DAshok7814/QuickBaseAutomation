
#region MS Directives
using Microsoft.Extensions.Configuration;
#endregion

#region Third party Directives
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Serilog;
#endregion


namespace QuickBase.TestAutomationCore
{
    /// <summary>
    /// Utility class for QuickBase
    /// </summary>
    public class QBUtility
    {

        /// <summary>
        /// Construct http header
        /// </summary>
        /// <param name="_configuration"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Dictionary<string, string> ConstructHttpHeader(IConfigurationRoot _configuration)
        {
            var qbRealmHostname = _configuration?["apiSettings:QB_Hostname"] ??
                throw new ArgumentNullException("QB_Hostname cannot be null");
            var qbUserToken = _configuration?["apiSettings:QB_USER_TOKEN"] ?? throw new ArgumentNullException("QB_USER_TOKEN cannot be null");

            return new Dictionary<string, string>
                    {
                      { "Content-Type", "application/json"},
                      { "Authorization", qbUserToken },
                      { "QB-Realm-Hostname", qbRealmHostname},
                      { "Accept", "application/json"}
                    };
        }

        /// <summary>
        /// Method to DeserializeObject
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string? DeserializeObject(RestResponse response)
        {
            try
            {
                if (response?.Content == null)
                {
                    Log.Error("Response content cannot be null");
                    throw new ArgumentNullException(nameof(response.Content), "Response content cannot be null");
                }
                return JsonConvert.DeserializeObject(response.Content)?.ToString();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deserializing response");
                return string.Empty;
            }
        }

        /// <summary>
        /// Method to DeserializeObject
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static JObject ConvertResponseAsJobject(RestResponse response)
        {
            try
            {
                if (response?.Content == null)
                {
                    Log.Error("Response content cannot be null");
                    throw new ArgumentNullException(nameof(response.Content), "Response content cannot be null");
                }
                return JObject.Parse(DeserializeObject(response));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deserializing response");
                return new JObject();
            }
        }


        /// <summary>
        /// Method to DeserializeObject
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static JObject ReadJsonFileAndConvertJobject(string path)
        {
            try
            {
                return JObject.Parse(JsonFileReader.GetJsonFile(path));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error While Converting JObject");
                return new JObject();
            }
        }
    }
}
