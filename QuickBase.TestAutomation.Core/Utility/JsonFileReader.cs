#region MS Directives
using Serilog;
#endregion

namespace QuickBase.TestAutomationCore
{
    /// <summary>
    /// class to read json file
    /// </summary>
    public class JsonFileReader
    {
        /// <summary>
        /// Method to read json file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetJsonFile(string filePath)
        {
            string json = string.Empty;
            try
            {
                using (StreamReader r = new StreamReader(filePath))
                {
                    json = r.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Log.Information($"Error reading json file: {ex.Message}");
            }
            return json;
        }
    }
}
