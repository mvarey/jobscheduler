using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.IO;


public static class Utility
{
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

    public static string RemoveComma(string data)
    {
        int i = data.IndexOf(",");
        if (i > 0)
        {
            data = data.Substring(0, i);
        }
        return data;
    }

    public static string RemoveZeros(string data)
    {
        if (data.Substring(0, 1) == "0")
        {
            data = data.Substring(1);
        }
        return data;
    }

    public static List<string> GetKeys(string data)
    {
        List<string> Keys = new List<string>();
        int bar = data.IndexOf("|");
        while (bar > -1)
        {
            Keys.Add(data.Substring(0, bar));
            data = data.Substring(bar + 1);
            bar = data.IndexOf("|");
        }
        Keys.Add(data);
        return Keys;
    }

    public static bool GetBoolean(string data)
    {
        if (data.IndexOf(",") > -1)
        {
            data = data.Substring(0, data.IndexOf(","));
        }
        bool Result = false;
        if (data == "1")
        {
            Result = true;
        }
        if (data.ToLower() == "true")
        {
            Result = true;
        }
        return Result;
    }

    public static string BaseUrl(string AbsoluteUri, string AbsolutePath)
    {
        return AbsoluteUri.Substring(0, AbsoluteUri.IndexOf(AbsolutePath));
    }

    public static string RelativeFromAbsolutePath(string path)
    {
        if (HttpContext.Current != null)
        {
            var request = HttpContext.Current.Request;
            var applicationPath = request.PhysicalApplicationPath;
            var virtualDir = request.ApplicationPath;
            virtualDir = virtualDir == "/" ? virtualDir : (virtualDir + "/");
            return path.Replace(applicationPath, virtualDir).Replace(@"\", "/");
        }

        throw new InvalidOperationException("We can only map an absolute back to a relative path if an HttpContext is available.");
    }

    public static string GetParam(string Params, string key)
    {
        string result = string.Empty;
        int i = Params.IndexOf("&" + key);
        if (i > 0)
        {
            result = Params.Substring(i + 1);
            i = result.IndexOf("=");
            if (i > 0)
            {
                result = result.Substring(0, i);
            }
        }
        return result;
    }


    public static void UpdateModel(HomeModel hm, ref object mdl)
    {
        HomeModel m = (HomeModel)mdl;
        m.Menu = hm.Menu;
        m.UserName = hm.UserName;
        m.UserFirstName = hm.UserFirstName;
        m.UserLastName = hm.UserLastName;
        m.AdminAccess = hm.AdminAccess;
        m.ViewAccess = hm.ViewAccess;
        m.ReportAccess = hm.ReportAccess;
        m.OperatorAccess = hm.OperatorAccess;

        m.MainImageUrl = hm.MainImageUrl;
}

}