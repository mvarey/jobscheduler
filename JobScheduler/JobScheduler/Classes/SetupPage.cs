using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;
using System.Configuration;

public class SetupPage
{
    public HomeModel hm = new HomeModel();
    DataClass db = new DataClass(ConfigurationManager.ConnectionStrings["JobScheduler"].ConnectionString);
    StringBuilder menu = new StringBuilder();
    private string RootURL = string.Empty;

    public HomeModel SetupPageBasics(HttpRequestBase Request, HomeModel hm, HttpSessionStateBase Session)
    {
        
        string UserName = string.Empty;
        string UserPassword = string.Empty;
        if (ConfigurationManager.AppSettings["LoginType"].ToString().ToLower() == "windows")
        {
            UserName = Request.LogonUserIdentity.Name;
        }
        if (Session["UserName"] != null)
        {
            if (Session["UserName"].ToString() != string.Empty)
            {
                UserName = Session["UserName"].ToString();
            }
        }
        Session["UserName"] = UserName;
        hm.UserName = UserName;

        RootURL = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, (new System.Web.Mvc.UrlHelper(Request.RequestContext)).Content("~"));
        RootURL = RootURL.Substring(0, RootURL.Length - 1);

        if (Security.GetSecurity(UserName, UserPassword, ref hm))
        {
            // Build Menus etc. for valid user
            
        }
        
        
        return hm;
    }
}