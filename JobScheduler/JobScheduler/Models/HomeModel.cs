using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class HomeModel
{
    public string UserName { get; set; }
    public string UserFirstName { get; set; }
    public string UserLastName { get; set; }
    public int UserId { get; set; }
    public bool AdminAccess { get; set; }
    public bool ViewAccess { get; set; }
    public bool ReportAccess { get; set; }
    public bool OperatorAccess { get; set; }

    public string Menu { get; set; }
    public string MainImageUrl { get; set; }

}