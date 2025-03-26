#region MS Directives
using Serilog;
#endregion

namespace QuickBase.TestAutomationLogicHub
{
    /// <summary>
    /// class to read json file
    /// </summary>
    public class JsonFileReader
    {
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
