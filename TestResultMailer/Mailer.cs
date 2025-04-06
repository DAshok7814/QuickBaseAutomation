using QuickBase.TestAutomationCore;
using QuickBase.TestAutomationCore.Utility;

namespace TestResultMailer
{
    public class Mailer
    {
        static void Main(string[] args)
        {
            string testResultFile = Constants.TrxFilePath;
            HTMLReportGenerator.ConvertTrxToHtml(testResultFile, Constants.HtmlReportPath);
            EmailHelper.SendEmailWithReport();
        }
    }
}
