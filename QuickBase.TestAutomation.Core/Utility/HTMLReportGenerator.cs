using System.Text;
using System.Xml;


namespace QuickBase.TestAutomationCore.Utility
{
    public class HTMLReportGenerator
    {
        public static void ConvertTrxToHtml(string trxFilePath, string htmlFilePath)
        {
            if (!File.Exists(trxFilePath))
            {
                Console.WriteLine("TRX file not found!");
                return;
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(trxFilePath);

            StringBuilder htmlContent = new StringBuilder();
            htmlContent.Append("<html><head><title>Test Report</title></head><body>");
            htmlContent.Append("<h2>Test Results</h2>");
            htmlContent.Append("<table border='1'><tr><th>Test Name</th><th>Outcome</th></tr>");

            XmlNodeList testResults = doc.GetElementsByTagName("UnitTestResult");

            foreach (XmlNode test in testResults)
            {
                string testName = test.Attributes["testName"].Value;
                string outcome = test.Attributes["outcome"].Value;

                htmlContent.Append($"<tr><td>{testName}</td><td>{outcome}</td></tr>");
            }

            htmlContent.Append("</table></body></html>");

            File.WriteAllText(htmlFilePath, htmlContent.ToString());
        }
    }
}