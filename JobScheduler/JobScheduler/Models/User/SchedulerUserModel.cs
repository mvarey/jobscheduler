using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class SchedulerUserModel : HomeModel
{
    public string ShowUser { get; set; }
    public string ShowPassword { get; set; }
    public string UserMessage { get; set; }
    public string UserID { get; set; }
    public string UserLogin { get; set; }
    public string UserPassword { get; set; }
    public string UserEmail { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool IsActive { get; set; }
    public bool Admin { get; set; }
    public bool View { get; set; }
    public bool Report { get; set; }
    public bool Operator { get; set; }
    public string UserList { get; set; }
}