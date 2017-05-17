using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class DashboardModel : HomeModel
{
    public string ShowDashboard { get; set; }
    public string DashboardMessage { get; set; }
    public string ScheduledJobs { get; set; }
    public string ActiveJobs { get; set; }
    public string HistoryJobs { get; set; }
    public string ErrorJobs { get; set; }
}