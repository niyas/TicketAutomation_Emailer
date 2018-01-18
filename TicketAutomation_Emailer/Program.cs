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
            if (args[0].ToString() == "report")
                getReport();
            else if (args[0].ToString() == "reminder")
                Notification("ReminderNotice");
            else if (args[0].ToString() == "reminder")
                Notification("FinalNotice");
        }

        private static void Notification(string noticeType)
        {
            DAL objDAL = new DAL();
            DataTable dt = new DataTable();
            DataTable dtConfig = getConfig("updateIMS");
            string applicationUrl = ConfigurationManager.AppSettings["automationToolUrl"].ToString();
            var query = from d in dtConfig.AsEnumerable()
                        where d.Field<string>("Key").Contains(noticeType)
                        select d;
            DataTable dtConfigReminder = query.CopyToDataTable<DataRow>();
            objDAL.CommandText = "usp_GetAssigneeWithPendingTickets";
            dt = objDAL.ExecuteDataSet().Tables[0];
            foreach(DataRow dr in dt.Rows)
            {
                string assignee = dr[0].ToString().ToLower().Replace(" ", "");
                sendEmail(applicationUrl + assignee, dtConfigReminder, dr[1].ToString(), "");
            }
        }

        private static void getReport()
        {
            DAL objDAL = new DAL();
            DataTable dt = new DataTable();
            DataTable dtConfigReport = getConfig("Report");
            string filePath = ConfigurationManager.AppSettings["folderPath"].ToString() + "WeeklyReport.xlsx";
            string Query = "usp_get_WeeklyReportData";
            objDAL.CommandText = Query;
            dt = objDAL.ExecuteDataSet().Tables[0];
            XLWorkbook wb = new ClosedXML.Excel.XLWorkbook();
            wb.Worksheets.Add(dt);
            wb.SaveAs(filePath);
            sendEmail("", dtConfigReport, "", filePath);
         }

        private static DataTable getConfig(string type)
        {
            DAL objDal = new DAL();
            objDal.AddParameter("@type", type);
            DataTable dt = objDal.ExecuteDataSet("usp_getConfigDetails").Tables[0];
            return dt;
        }

        private static void sendEmail(string url, DataTable config, string to, string attachmentPath)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient();
                string[] cclist = {},
                         tolist = { to };

                string subject = "",
                        body = "";

                foreach(DataRow dr in config.Rows)
                {
                    if (dr["Key"].ToString().ToLower().Contains("to"))
                        tolist = dr["Value"].ToString().Split(',');

                    else if(dr["Key"].ToString().ToLower().Contains("cc"))
                        cclist = dr["Value"].ToString().Split(',');

                    else if (dr["Key"].ToString().ToLower().Contains("body"))
                        body = dr["Value"].ToString();

                    else if (dr["Key"].ToString().ToLower().Contains("subject"))
                        subject = dr["Value"].ToString();
                }

                if(tolist.Count() > 0)
                {
                    foreach(string tos in tolist)
                    {
                        mail.To.Add(tos);
                    }
                }

                if (cclist.Count() > 0)
                {
                    foreach (string cc in cclist)
                    {
                        mail.CC.Add(cc);
                    }
                }

                if (attachmentPath.Length > 0)
                {
                    System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(attachmentPath);
                    mail.Attachments.Add(attachment);
                }

                body = url.Length > 0 ? body + "<br/>" + url : body;

                mail.From = new MailAddress("niyasatwork@gmail.com");
                
                mail.Subject = subject;
                mail.Body =  body;
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
