using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class DataHelper
{
    public const int ERRNUM_CONN = 50101;
    public static string RetErrMsg(string ErrSource, string ErrMsg, string stack)
    {
        return string.Format("Source={0}\nMessage={1}\nStack={2}", ErrSource, ErrMsg, stack);
    }
    public class dbExceptionConn : ApplicationException
    {
        public new string Message;//replace base.Message
        public dbExceptionConn(string sMessage)
        {
            Message = sMessage;
        }

    }
}