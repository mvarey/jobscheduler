using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Data;
using System.Security.Cryptography;

public static class Utilities
{
    public static string databaseConnection = string.Empty;
    public static string EmailNotify = string.Empty;
    public static string EnvironmentConfiguration = "PROD";
    private static DataClass.TParams[] Parms;
    public static string[] MonthNames = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
    public static string[] DayNames = new string[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
    public static string SendEmailAddress = string.Empty;
    public static string SendEmailServer = string.Empty;
    public static string SendEmailPassword = string.Empty;
    public static int SendEmailPort = 25;
    public static DataClass db { get; set; }
    /// <summary>
    /// Encryption Key.
    /// </summary>
    static byte[] key = { 55, 66, 77, 88, 99, 88, 99, 88, 99, 88, 99, 88, 99, 88, 99, 55 };

    public static string ErrorMessage = string.Empty;

    public static void SendEmail(string Subject, string Body)
    {
        try
        {
            MailMessage email = new MailMessage(SendEmailAddress, EmailNotify.Replace(";", ","));
            email.Subject = Subject;
            email.Body = Body;
            SmtpClient client = new SmtpClient();
            client.Port = SendEmailPort;
            client.Host = SendEmailServer;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(SendEmailAddress, SendEmailPassword);
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

            SmtpClient client = new SmtpClient();
            client.Port = SendEmailPort;
            client.Host = SendEmailServer;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(SendEmailAddress, SendEmailPassword);
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

    public static void SendEmail(string Subject, string Body, string To, string CC, string BCC, string EmailFrom)
    {
        try
        {
            MailMessage email = new MailMessage(EmailFrom, To.Replace(";", ","));
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

            SmtpClient client = new SmtpClient();
            client.Port = SendEmailPort;
            client.Host = SendEmailServer;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(SendEmailAddress, SendEmailPassword);
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
            SmtpClient client = new SmtpClient();
            client.Port = SendEmailPort;
            client.Host = SendEmailServer;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(SendEmailAddress, SendEmailPassword);
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
            SmtpClient client = new SmtpClient();
            client.Port = SendEmailPort;
            client.Host = SendEmailServer;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(SendEmailAddress, SendEmailPassword);
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

    public static bool IsNumeric(string value)
    {
        bool bReturn = false;
        double f = -9999999.9;
        if (double.TryParse(value, out f))
        {
            bReturn = true;
        }
        return bReturn;
    }

    public static List<string> ExtractValues(string ddlValues)
    {
        List<string> values = new List<string>();
        while (ddlValues != string.Empty)
        {
            int newline = ddlValues.IndexOf(Environment.NewLine);
            if (newline == 0)
            {
                values.Add("");
            }
            else
            {
                if (newline > 0)
                {
                    values.Add(ddlValues.Substring(0, newline));
                }
                else
                {
                    values.Add(ddlValues);
                }
            }
            if (newline > -1)
            {
                ddlValues = ddlValues.Substring(newline + 2);
            }
            else
            {
                ddlValues = string.Empty;
            }
        }
        return values;
    }

    public static List<string> ExtractValuesFromCSV(string CSV)
    {
        List<string> values = new List<string>();
        while (CSV != string.Empty)
        {
            int newline = CSV.IndexOf(",");
            if (newline == 0)
            {
                values.Add("");
            }
            else
            {
                if (newline > 0)
                {
                    values.Add(CSV.Substring(0, newline));
                }
                else
                {
                    values.Add(CSV);
                }
            }
            if (newline > -1)
            {
                CSV = CSV.Substring(newline + 1);
            }
            else
            {
                CSV = string.Empty;
            }
        }
        return values;
    }

    public static string ParseDate(string date)
    {
        if (date == "&nbsp;")
        {
            return string.Empty;
        }
        return date.Substring(0, date.IndexOf(" "));
    }

    public static string ParseTime(string date)
    {
        if (date == "&nbsp;")
        {
            return string.Empty;
        }
        string result = DateTime.Parse(date).ToShortTimeString(); //.Substring(date.IndexOf(" "));
        return result;
    }

    public static int GetDayOfWeek(DayOfWeek dayofweek)
    {
        int day = 1;
        foreach (string d in DayNames)
        {
            if (d.ToLower() == dayofweek.ToString().ToLower())
            {
                break;
            }
            day++;
        }
        return day;
    }

    public static int GetMonthOfYear(string MonthName)
    {
        int month = 1;
        foreach (string m in MonthNames)
        {
            if (m.ToLower() == MonthName.ToLower())
            {
                break;
            }
            month++;
        }
        return month;
    }

    public static string Encode64(string value)
    {
        string results = string.Empty;
        var bytes = Encoding.UTF8.GetBytes(value);
        var base64 = Convert.ToBase64String(bytes);
        results = base64.ToString();
        return results;
    }

    public static string Decode64(string value)
    {
        string results = string.Empty;
        var data = Convert.FromBase64String(value);
        results = Encoding.UTF8.GetString(data);
        return results;
    }


    public static DataSet ConvertXmlToDataSet(string xmlData)
    {
        DataSet ds = new DataSet();
        if (xmlData != string.Empty && xmlData != "Error")
        {
            StringReader reader = new StringReader(xmlData);
            ds.ReadXml(reader);
        }
        return ds;
    }

    public static DateTime GetStartOfMonth(DateTime date)
    {
        return DateTime.Parse(date.Year.ToString() + "-" + date.Month.ToString() + "-1");
    }

    public static int GetConfigurationID(string TableName, int OrganizationID)
    {
        int ConfigurationID = 0;
        Parms = new DataClass.TParams[] {new DataClass.TParams("Command", 10, SqlDbType.VarChar, "ListItem"),
                        new DataClass.TParams("OrganizationID", 4, SqlDbType.Int, OrganizationID),
                        new DataClass.TParams("TableName", 50, SqlDbType.VarChar, TableName)
                };
        DataSet dsConfig = db.DIRunSPretDs(Parms, "cm_Configuration");
        if (dsConfig.Tables[0].Rows.Count > 0)
        {
            ConfigurationID = int.Parse(dsConfig.Tables[0].Rows[0]["ConfigurationID"].ToString());
        }
        return ConfigurationID;
    }

    public static int GetConfigurationID(string TableName, int OrganizationID, string ConfigurationName)
    {
        int ConfigurationID = 0;
        Parms = new DataClass.TParams[] {new DataClass.TParams("Command", 10, SqlDbType.VarChar, "GetByConf"),
                        new DataClass.TParams("TableName", 50, SqlDbType.VarChar, TableName),
                        new DataClass.TParams("OrganizationID", 4, SqlDbType.Int, OrganizationID),
                        new DataClass.TParams("ConfigurationName", 50, SqlDbType.VarChar, ConfigurationName)
                };
        DataSet dsConfig = db.DIRunSPretDs(Parms, "cm_Configuration");
        if (dsConfig.Tables[0].Rows.Count > 0)
        {
            ConfigurationID = int.Parse(dsConfig.Tables[0].Rows[0]["ConfigurationID"].ToString());
        }
        return ConfigurationID;
    }

    public static string ConvertBitToString(bool bit)
    {
        string Result = "";
        if (bit)
        {
            Result = "Y";
        }
        else
        {
            Result = "N";
        }
        return Result;
    }

    public static bool ConvertStringToBit(string value)
    {
        bool Result = false;
        if (value == "Y")
        {
            Result = true;
        }
        return Result;
    }



    /// <summary>
    /// Encrypts data before passing it to the server.
    /// </summary>
    /// <param name="TextField">String to be encrypted.</param>
    /// <returns>Encrypted value of the string.</returns>
    public static string EncryptString(string TextField)
    {
        AesManaged encryptor = new AesManaged();
        encryptor.Key = key;
        encryptor.IV = key;
        using (MemoryStream encryptionStream = new MemoryStream())
        {
            using (CryptoStream encrypt = new CryptoStream(encryptionStream, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
            {
                byte[] utfD1 = UTF8Encoding.UTF8.GetBytes(TextField);
                encrypt.Write(utfD1, 0, utfD1.Length);
                encrypt.FlushFinalBlock();
                encrypt.Close();
                return Convert.ToBase64String(encryptionStream.ToArray());
            }
        }
    }

    public static string EncryptString(string TextField, string CryptoKey)
    {
        AesManaged encryptor = new AesManaged();
        encryptor.Key = GetBytes(CryptoKey);
        encryptor.IV = GetBytes(CryptoKey);
        using (MemoryStream encryptionStream = new MemoryStream())
        {
            using (CryptoStream encrypt = new CryptoStream(encryptionStream, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
            {
                byte[] utfD1 = UTF8Encoding.UTF8.GetBytes(TextField);
                encrypt.Write(utfD1, 0, utfD1.Length);
                encrypt.FlushFinalBlock();
                encrypt.Close();
                return Convert.ToBase64String(encryptionStream.ToArray());
            }
        }
    }

    public static string DecryptString(string TextField)
    {
        if (TextField == string.Empty)
        {
            return string.Empty;
        }
        if (TextField == "Error")
        {
            return TextField;
        }
        AesManaged decryptor = new AesManaged();
        byte[] encryptedData = Convert.FromBase64String(TextField);
        decryptor.Key = key;
        decryptor.IV = key;
        using (MemoryStream decryptionStream = new MemoryStream())
        {
            using (CryptoStream decrypt = new CryptoStream(decryptionStream, decryptor.CreateDecryptor(), CryptoStreamMode.Write))
            {
                decrypt.Write(encryptedData, 0, encryptedData.Length);
                decrypt.Flush();
                decrypt.Close();
                byte[] decryptedData = decryptionStream.ToArray();
                return UTF8Encoding.UTF8.GetString(decryptedData, 0, decryptedData.Length);
            }
        }
    }

    public static string DecryptString(string TextField, string Cryptokey)
    {
        AesManaged decryptor = new AesManaged();
        byte[] encryptedData = Convert.FromBase64String(TextField);
        decryptor.Key = GetBytes(Cryptokey);
        decryptor.IV = GetBytes(Cryptokey);
        using (MemoryStream decryptionStream = new MemoryStream())
        {
            using (CryptoStream decrypt = new CryptoStream(decryptionStream, decryptor.CreateDecryptor(), CryptoStreamMode.Write))
            {
                decrypt.Write(encryptedData, 0, encryptedData.Length);
                decrypt.Flush();
                decrypt.Close();
                byte[] decryptedData = decryptionStream.ToArray();
                return UTF8Encoding.UTF8.GetString(decryptedData, 0, decryptedData.Length);
            }
        }
    }

    public static byte[] GetBytes(string str)
    {
        byte[] bytes = new byte[str.Length * sizeof(char)];
        System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
        return bytes;
    }

    public static string GetString(byte[] bytes)
    {
        char[] chars = new char[bytes.Length / sizeof(char)];
        System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
        return new string(chars);
    }

    // Function to convert passed dataset to XML data
    public static string ConvertDataSetToXML(DataSet xmlDS)
    {
        MemoryStream stream = null;
        XmlTextWriter writer = null;
        try
        {
            stream = new MemoryStream();
            // Load the XmlTextReader from the stream
            writer = new XmlTextWriter(stream, Encoding.Unicode);
            // Write to the file with the WriteXml method.
            xmlDS.WriteXml(writer);
            int count = (int)stream.Length;
            byte[] arr = new byte[count];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(arr, 0, count);
            UnicodeEncoding utf = new UnicodeEncoding();
            return utf.GetString(arr).Trim();
        }
        catch
        {
            return String.Empty;
        }
        finally
        {
            if (writer != null) writer.Close();
        }
    }

    public static string Format2(int a)
    {
        string Result = a.ToString();
        if (Result.Length == 1)
        {
            Result = "0" + Result;
        }
        return Result;
    }

    public static string FormatMoney(decimal value)
    {
        return value.ToString("###,###,##0.00");
    }

    public static string RemoveTrailingZeros(string value)
    {
        int iDot = value.IndexOf(".");
        if (iDot > -1)
        {
            if (value.Substring(value.Length - 1, 1) == "0")
            {
                value = value.Substring(0, value.Length - 1);
            }
        }
        return value;
    }

    public static bool TimeLessThan(DateTime SourceTime, DateTime CompareTime)
    {
        bool Result = false;
        if (SourceTime.Hour > CompareTime.Hour || (SourceTime.Hour == CompareTime.Hour && SourceTime.Minute > CompareTime.Minute ||
            SourceTime.Hour == CompareTime.Hour && SourceTime.Minute == CompareTime.Minute && SourceTime.Second > CompareTime.Second))
        {
            Result = true;
        }
        return Result;
    }

    public static bool TimeGreaterThan(DateTime SourceTime, DateTime CompareTime)
    {
        bool Result = false;
        if (SourceTime.Hour < CompareTime.Hour || (SourceTime.Hour == CompareTime.Hour && SourceTime.Minute < CompareTime.Minute ||
            SourceTime.Hour == CompareTime.Hour && SourceTime.Minute == CompareTime.Minute && SourceTime.Second < CompareTime.Second))
        {
            Result = true;
        }
        return Result;
    }

    public static int GetNextDayOfWeek(DayOfWeek dow, string Occurrences, out int DaysOffset)
    {
        int d = 0;
        DaysOffset = 1;
        switch (dow)
        {
            case DayOfWeek.Sunday:
                {
                    break;
                }
            case DayOfWeek.Monday:
                {
                    d = 1;
                    break;
                }
            case DayOfWeek.Tuesday:
                {
                    d = 2;
                    break;
                }
            case DayOfWeek.Wednesday:
                {
                    d = 3;
                    break;
                }
            case DayOfWeek.Thursday:
                {
                    d = 4;
                    break;
                }
            case DayOfWeek.Friday:
                {
                    d = 5;
                    break;
                }
            case DayOfWeek.Saturday:
                {
                    d = 6;
                    break;
                }
        }
        // Loop through each day from current day to find next running day
        int WorkingDay = d + 1;
        if (d == 6)
        {
            WorkingDay = 0;
        }
        while (WorkingDay != d)
        {
            if (Occurrences.Substring(WorkingDay, 1) == "Y")
            {
                break;
            }
            WorkingDay++;
            DaysOffset++;
        }
        return WorkingDay;
    }

    public static int GetNextMonth(DateTime StartDate, string Occurrences, out int MonthsOffset)
    {
        int d = 0;
        MonthsOffset = 1;
        d = StartDate.Month;
        // Loop through each day from current day to find next running day
        int WorkingMonth = d + 1;
        if (d == 12)
        {
            WorkingMonth = 1;
        }
        while (WorkingMonth != d)
        {
            if (Occurrences.Substring(WorkingMonth, 1) == "Y")
            {
                break;
            }
            WorkingMonth++;
            if (WorkingMonth == 13)
            {
                WorkingMonth = 1;
            }
            MonthsOffset++;
        }
        return WorkingMonth;
    }

}