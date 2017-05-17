using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;

public static class Email
{
    public static string SendEmailAddress = string.Empty;
    public static string MailServer = string.Empty;
    public static string MailID = string.Empty;
    public static string MailPassword = string.Empty;
    public static string EmailNotify = string.Empty;

    public static void SendEmail(string Subject, string Body)
    {
        try
        {
            MailMessage email = new MailMessage(SendEmailAddress, EmailNotify.Replace(";", ","));
            email.Subject = Subject;
            email.Body = Body;
            SmtpClient client = new SmtpClient(MailServer);
            //client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Send(email);
            Console.WriteLine("Subject " + Subject + "  with Message: " + Body);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to send email with error: " + ex.ToString());
            Console.WriteLine("Subject: " + Subject + "  with Message: " + Body);
        }
    }

    public static void SendEmail(string Subject, string Body, string To, string CC, string BCC)
    {
        try
        {
            MailMessage email = new MailMessage(SendEmailAddress, To.Replace(";", ","));
            if (CC != string.Empty)
            {
                email.CC.Add(CC);
            }
            if (BCC != string.Empty)
            {
                email.Bcc.Add(BCC);
            }
            email.Subject = Subject;
            email.Body = Body.Replace("<br/>", Environment.NewLine);
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(Body, null, System.Net.Mime.MediaTypeNames.Text.Html);
            email.AlternateViews.Add(htmlView);

            SmtpClient client = new SmtpClient(MailServer);
            //client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Send(email);
            Console.WriteLine("Subject " + Subject + "  with Message: " + Body);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to send email with error: " + ex.ToString());
            Console.WriteLine("Subject: " + Subject + "  with Message: " + Body);
        }
    }

    public static void SendEmail(string Subject, string Body, string AttachmentFileName)
    {
        try
        {
            MailMessage email = new MailMessage(SendEmailAddress, EmailNotify.Replace(";", ","));
            email.Subject = Subject;
            email.Body = Body;
            email.Attachments.Add(new Attachment(AttachmentFileName));
            SmtpClient client = new SmtpClient(MailServer);
            //client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Send(email);
            Console.WriteLine("Subject: " + Subject + "  with Message: " + Body);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to send email with error: " + ex.ToString());
            Console.WriteLine("Subject: " + Subject + "  with Message: " + Body);
        }
    }

    public static void SendEmail(string Subject, string Body, string CC, string AttachmentFileName)
    {
        try
        {
            if (CC.Substring(CC.Length - 1, 1) == ";")
            {
                CC = CC.Substring(0, CC.Length - 1);
            }
            MailMessage email;
            if (CC != string.Empty)
            {
                email = new MailMessage(SendEmailAddress, (EmailNotify + "," + CC).Replace(";", ","));
            }
            else
            {
                email = new MailMessage(SendEmailAddress, (EmailNotify).Replace(";", ","));
            }
            email.Subject = Subject;
            email.Body = Body;
            if (AttachmentFileName != string.Empty)
            {
                email.Attachments.Add(new Attachment(AttachmentFileName));
            }
            SmtpClient client = new SmtpClient(MailServer);
            //client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Send(email);
            Console.WriteLine("Subject: " + Subject + "  with Message: " + Body);
        }
        catch (Exception ex)
        {
            Console.WriteLine(Environment.NewLine + "Failed to send email with error: " + Environment.NewLine + ex.Message);
            Console.WriteLine("Subject: " + Subject + Environment.NewLine + "Message: " + Body);
            throw;
        }
    }

}