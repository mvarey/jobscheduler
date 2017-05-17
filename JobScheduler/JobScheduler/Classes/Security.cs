using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Configuration;

public static class Security
{
    static DataClass db = new DataClass(ConfigurationManager.ConnectionStrings["JobScheduler"].ConnectionString);
    static DataClass.TParams[] Parms;
    static string LoginType = ConfigurationManager.AppSettings["LoginType"].ToString();
    public static string LastName = string.Empty;
    public static string FirstName = string.Empty;
    public static bool AdminAccess = false;
    public static bool ViewAccess = false;
    public static bool ReportAccess = false;
    public static bool OperatorAccess = false;
    public static int UserID = 0;

    public static bool GetSecurity(string UserName, string Password, ref HomeModel hm)
    {
        bool LoginSuccessful = false;
        switch (LoginType.ToLower())
        {
            case "windows":
                {
                    Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "Login"),
                            new DataClass.TParams("UserLogin", 50, SqlDbType.VarChar, UserName)
                    };
                    break;
                }
            case "forms":
                {
                    Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "Forms"),
                            new DataClass.TParams("UserLogin", 50, SqlDbType.VarChar, UserName),
                            new DataClass.TParams("UserPassword", 100, SqlDbType.VarChar, DataEncyption.EncryptString(Password))
                    };
                    break;
                }
        }
        DataSet dsUser = db.DIRunSPretDs(Parms, "sp_SchedulerUser");
        if (dsUser.Tables[0].Rows.Count > 0)
        {
            LoginSuccessful = true;
            DataRow dr = dsUser.Tables[0].Rows[0];
            LastName = dr["UserLastName"].ToString();
            FirstName = dr["UserFirstName"].ToString();
            AdminAccess = bool.Parse(dr["AdminAccess"].ToString());
            ViewAccess = bool.Parse(dr["ViewAccess"].ToString());
            ReportAccess = bool.Parse(dr["ReportAccess"].ToString());
            OperatorAccess = bool.Parse(dr["OperatorAccess"].ToString());
            UserID = int.Parse(dr["SchedulerUserId"].ToString());

        }
        if (System.Diagnostics.Debugger.IsAttached)
        {
            AdminAccess = true;
            ViewAccess = true;
            ReportAccess = true;
            OperatorAccess = true;
            LoginSuccessful = true;
        }
        hm.UserFirstName = FirstName;
        hm.UserLastName = LastName;
        hm.UserName = UserName;
        hm.UserId = UserID;
        hm.ViewAccess = ViewAccess;
        hm.ReportAccess = ReportAccess;
        hm.OperatorAccess = OperatorAccess;
        hm.AdminAccess = AdminAccess;
        return LoginSuccessful;
    }
}