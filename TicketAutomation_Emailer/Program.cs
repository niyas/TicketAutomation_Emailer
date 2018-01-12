using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using System.Net;
using System.Net.Mail;
using System.Data;
using System.Configuration;


namespace TicketAutomation_Emailer
{
    class Program
    {
        static void Main(string[] args)
        {
            getReport();
        }

        private static void getReport()
        {
            DAL objDAL = new DAL();
            DataTable dt = new DataTable();
            string filePath = ConfigurationManager.AppSettings["folderPath"].ToString() + "WeeklyReport.xlsx";
            string Query = "usp_get_WeeklyReportData";
            objDAL.CommandText = Query;
            dt = objDAL.ExecuteDataSet().Tables[0];
            XLWorkbook wb = new ClosedXML.Excel.XLWorkbook();
            wb.Worksheets.Add(dt);
            wb.SaveAs(filePath);
         }

        private static void sendEmail(string url, string to, string cc, string name, string emailBody, string attachmentPath)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient();
                if (attachmentPath.Length > 0)
                {
                    System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(attachmentPath);
                    mail.Attachments.Add(attachment);
                }
                mail.From = new MailAddress("niyasatwork@gmail.com");
                mail.To.Add(to);
                mail.Subject = "Ticket Automation Email";
                mail.Body = "Hi " + name + ",<br/>" + emailBody;
                mail.IsBodyHtml = true;
                SmtpServer.Host = "smtp.gmail.com";
                SmtpServer.Port = 587;
                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                SmtpServer.Credentials = new System.Net.NetworkCredential("niyasatwork@gmail.com", "yourpassword");
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);
            }
            catch(Exception ex)
            {

            }
            
        }
    }
}
