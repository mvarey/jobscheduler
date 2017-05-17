using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

public class DataClass
{
    private SqlConnection oConn;
    private string sConn = "";//ConnString property state
    public string ConversionError = string.Empty;

    public DataClass(string ConnStr)
    {
        sConn = ConnStr;
    }
    /// <summary>
    ///  object representation of Command.Parameter
    ///  usage: new TParams( "@country",100,System.Data.SqlDbType.VarChar,"Germany")
    /// </summary>
    public struct TParams
    {
        public TParams(string inName, int inSize, System.Data.SqlDbType inDataType, object inValue)
        {
            this.dtDataType = inDataType;
            this.Name = inName;
            this.oValue = inValue;
            this.Size = inSize;

        }
        public string Name;
        public int Size;
        public System.Data.SqlDbType dtDataType;
        public object oValue;

    }
    // Function to convert passed XML data to dataset
    public DataSet ConvertXMLToDataSet(string xmlData)
    {
        StringReader stream = null;
        XmlTextReader reader = null;
        try
        {
            DataSet xmlDS = new DataSet();
            stream = new StringReader(xmlData);
            // Load the XmlTextReader from the stream
            reader = new XmlTextReader(stream);
            xmlDS.ReadXml(reader);
            return xmlDS;
        }
        catch (Exception ex)
        {
            ConversionError = ex.Message;
            return null;
        }
        finally
        {
            if (reader != null) reader.Close();
        }
    }

    public string ConvertDataSetToXML(DataSet dsData)
    {
        using (var memoryStream = new MemoryStream())
        {
            using (TextWriter streamWriter = new StreamWriter(memoryStream))
            {
                var xmlSerializer = new XmlSerializer(typeof(DataSet));
                xmlSerializer.Serialize(streamWriter, dsData);
                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }
    }

    # region private utils
    private string ConnString
    {
        get
        {
            return sConn;
        }
        set
        {
            sConn = value;
        }
    }
    /// <summary>
    ///  populates Command obj's parameters collection based on TParams 
    /// </summary>
    /// <param name="inComm">&CommandObj</param>
    /// <param name="inParam">struct</param>
    private void PrepCommParams(ref SqlCommand inComm, ref TParams[] inParam)
    {
        string sFunctionErr = "\nFailed to Prepare parameter obj.\n";
        try
        {
            SqlParameter Prm;//temp
            for (int i = 0; i < inParam.Length; i++)
            {
                //TParams t = ((TParams)aParam[i]);
                Prm = inComm.Parameters.Add(inParam[i].Name, inParam[i].dtDataType, inParam[i].Size);
                Prm.Value = inParam[i].oValue;
            }
        }
        catch (Exception e)
        {
            throw new Exception(sFunctionErr + e.Message);
        }
    }
    #endregion
    #region public data methods
    /// <summary>
    ///  exec T-SQL against dat source
    /// </summary>
    /// <param name="SQL">valid T-SQL</param>
    public void DIRunSQL(string SQL)
    {

        string sErr = "";
        oConn = new SqlConnection();
        SqlCommand Cm1;

        try
        {
            sErr = "\nFailed to connect to database.\n";
            oConn.ConnectionString = ConnString;
            oConn.Open();
            Cm1 = new SqlCommand();
            Cm1.Connection = oConn;
        }
        catch (SqlException e)
        {
            throw (e);
        }
        catch (Exception e)
        {//database connection error
            throw new Exception(DataHelper.RetErrMsg(e.Source, sErr + e.Message, e.StackTrace));
        }
        try
        {
            sErr = "\nFailed to execute SQL.\n";
            Cm1.CommandType = System.Data.CommandType.Text;
            Cm1.CommandText = SQL;
            int t = Cm1.ExecuteNonQuery();
            oConn.Close();
            oConn.Dispose();
            Cm1.Dispose();
        }
        catch (SqlException e)
        {//param build error

            if (e.Number == DataHelper.ERRNUM_CONN)//concurrency error
                throw new DataHelper.dbExceptionConn(e.Message);

            throw (e);

        }
        catch (Exception e)
        {//data exec. failed
            throw new Exception(DataHelper.RetErrMsg(e.Source, sErr + e.Message, e.StackTrace));
        }

    }
    /// <summary>
    ///  same as DIRunSQL, but returns DataSet
    /// </summary>
    /// <param name="SQL">valid T-SQL</param>
    /// <returns>populated DataSet</returns>
    public System.Data.DataSet DIRunSQLretDs(string SQL)
    {
        string sErr = "";
        string sSQL = SQL;
        oConn = new SqlConnection();
        System.Data.DataSet tempdS;
        try
        {
            sErr = "\nFailed to connect to database.\n";
            oConn.ConnectionString = ConnString;
            oConn.Open();
        }
        catch (Exception e)
        {//data connection error
            throw new Exception(DataHelper.RetErrMsg(e.Source, sErr + e.Message, e.StackTrace));
        }
        try
        {
            sErr = "\nFailed to retrieve data.\n";
            SqlDataAdapter dA = new SqlDataAdapter(sSQL, oConn);
            dA.SelectCommand.CommandTimeout = oConn.ConnectionTimeout;
            tempdS = new System.Data.DataSet();
            sErr = "\nFailed to fill dataset.\n";
            dA.Fill(tempdS);
            dA.Dispose();
            oConn.Close();
            oConn.Dispose();
        }
        catch (SqlException e)
        {//param build error

            if (e.Number == DataHelper.ERRNUM_CONN)//concurrency error
                throw new DataHelper.dbExceptionConn(e.Message);

            throw (e);

        }
        catch (Exception e)
        {//data retrieval error
            throw new Exception(DataHelper.RetErrMsg(e.Source, sErr + e.Message, e.StackTrace));
        }

        return tempdS;
    }
    /// <summary>
    ///  exec stored procedure
    /// </summary>
    /// <param name="param"></param>
    /// <param name="sSPName"></param>
    /// <returns>populated DataSet</returns>
    public int DIRunSP(TParams[] param, string sSPName)
    {  //new TParams( "@country",100,System.Data.SqlDbType.VarChar,"Germany")

        string sErr = "";
        SqlCommand Cm1;

        try
        {
            sErr = "\nFailed to connect to database.\n";
            oConn = new SqlConnection();
            oConn.ConnectionString = ConnString;
            oConn.Open();
            Cm1 = new SqlCommand();
            Cm1.CommandText = sSPName;
            Cm1.CommandType = System.Data.CommandType.StoredProcedure;
            Cm1.Connection = oConn;
        }
        catch (Exception e)
        {//data connection error
            throw new Exception(DataHelper.RetErrMsg(e.Source, sErr + e.Message, e.StackTrace));
        }
        try
        {
            sErr = "\nFailed to build parameters.\n";
            PrepCommParams(ref Cm1, ref param);
            Cm1.Connection = oConn;
            sErr = "\nFailed to executenonquery.\n";
            int i = Cm1.ExecuteNonQuery();
            oConn.Close();
            oConn.Dispose();
            Cm1.Dispose();
            return i;
        }
        catch (SqlException e)
        {//param build error

            if (e.Number == DataHelper.ERRNUM_CONN)//concurrency error
                throw new DataHelper.dbExceptionConn(e.Message);

            throw (e);

        }
        catch (Exception e)
        {//param build error
            throw new Exception(DataHelper.RetErrMsg(e.Source, sErr + e.Message, e.StackTrace));
        }

    }
    /// <summary>
    ///  same as DIRunSQLretDs, but with params
    /// </summary>
    /// <param name="param">TParams struct</param>
    /// <param name="sSPName">name of stored procedure</param>
    /// <returns>populated dataset</returns>
    public System.Data.DataSet DIRunSPretDs(TParams[] param, string sSPName)
    {

        string sErr = "\nFailed to setup connection & command.\n";
        oConn = new SqlConnection();
        SqlCommand Cm1 = new SqlCommand();
        System.Data.DataSet tempdS;

        try
        {
            sErr = "\nFailed to connect to database.\n";
            oConn.ConnectionString = ConnString;
            oConn.Open();

            Cm1.CommandType = System.Data.CommandType.StoredProcedure;
            Cm1.CommandText = sSPName;
            Cm1.Connection = oConn;
        }
        catch (Exception e)
        {//data connection error
            throw new Exception(DataHelper.RetErrMsg(e.Source, sErr + e.Message, e.StackTrace));
        }
        try
        {
            sErr = "\nFailed to build parameters.\n";
            if (param != null)
                PrepCommParams(ref Cm1, ref param);
        }

        catch (Exception e)
        {//param build error
            throw new Exception(DataHelper.RetErrMsg(e.Source, sErr + e.Message, e.StackTrace));
        }
        try
        {
            sErr = "\nFailed to retrieve data.\n";
            SqlDataAdapter dA = new SqlDataAdapter(Cm1);
            dA.SelectCommand.CommandTimeout = oConn.ConnectionTimeout;
            tempdS = new System.Data.DataSet();
            sErr = "\nFailed to fill dataset.\n";
            dA.Fill(tempdS);

            dA.Dispose();

            oConn.Close();
            oConn.Dispose();
        }
        catch (SqlException e)
        {//param build error

            if (e.Number == DataHelper.ERRNUM_CONN)//concurrency error
                throw new DataHelper.dbExceptionConn(e.Message);

            throw (e);

        }
        catch (Exception e)
        {//data retrieval error
            throw new Exception(DataHelper.RetErrMsg(e.Source, sErr + e.Message, e.StackTrace));
        }
        return tempdS;
    }


    /// <summary>
    ///  same as DIRunSQLretDs, but returns XML
    /// </summary>
    /// <param name="param">TParams struct</param>
    /// <param name="sSPName">name of stored procedure</param>
    /// <returns>populated dataset</returns>
    public System.Xml.XmlDocument DIRunSPretXML(TParams[] param, string sSPName)
    {

        string sErr = "\nFailed to setup connection & command.\n";
        oConn = new SqlConnection();
        SqlCommand Cm1 = new SqlCommand();
        System.Data.DataSet tempdS;
        System.Xml.XmlDocument tempXML;

        try
        {
            sErr = "\nFailed to connect to database.\n";
            oConn.ConnectionString = ConnString;
            oConn.Open();

            Cm1.CommandType = System.Data.CommandType.StoredProcedure;
            Cm1.CommandText = sSPName;
            Cm1.Connection = oConn;
        }
        catch (Exception e)
        {//data connection error
            throw new Exception(DataHelper.RetErrMsg(e.Source, sErr + e.Message, e.StackTrace));
        }
        try
        {
            sErr = "\nFailed to build parameters.\n";
            if (param != null)
                PrepCommParams(ref Cm1, ref param);
        }

        catch (Exception e)
        {//param build error
            throw new Exception(DataHelper.RetErrMsg(e.Source, sErr + e.Message, e.StackTrace));
        }
        try
        {
            sErr = "\nFailed to retrieve data.\n";
            SqlDataAdapter dA = new SqlDataAdapter(Cm1);
            tempdS = new System.Data.DataSet();
            sErr = "\nFailed to fill xml.\n";
            dA.Fill(tempdS);
            tempXML = new System.Xml.XmlDocument();
            tempXML.Load(tempdS.GetXml());

            dA.Dispose();

            oConn.Close();
            oConn.Dispose();
        }
        catch (SqlException e)
        {//param build error

            if (e.Number == DataHelper.ERRNUM_CONN)//concurrency error
                throw new DataHelper.dbExceptionConn(e.Message);

            throw (e);

        }
        catch (Exception e)
        {//data retrieval error
            throw new Exception(DataHelper.RetErrMsg(e.Source, sErr + e.Message, e.StackTrace));
        }
        return tempXML;
    }

    /// <summary>
    ///  same as DIRunSP, but with params AND returns (int) scalar for op.
    /// </summary>
    /// <param name="param">TParams struct</param>
    /// <param name="sSPName">name of stored procedure</param>
    /// <returns>int scalart representation</returns>
    public int DIRunSPretScalar(TParams[] param, string sSPName)
    {   //new TParams( "@country",100,System.Data.SqlDbType.VarChar,"Germany")

        string sErr = "";
        oConn = new SqlConnection();
        SqlCommand Cm1;
        try
        {
            sErr = "\nFailed to connect to database.\n";
            oConn.ConnectionString = ConnString;
            oConn.Open();
            Cm1 = new SqlCommand();
            Cm1.CommandType = System.Data.CommandType.StoredProcedure;
            Cm1.CommandText = sSPName;
            Cm1.Connection = oConn;
        }
        catch (Exception e)
        {//data connection error
            throw new Exception(DataHelper.RetErrMsg(e.Source, sErr + e.Message, e.StackTrace));
        }

        try
        {//build params error
            sErr = "\nFailed to build parameters.\n";
            if (param != null)
                PrepCommParams(ref Cm1, ref param);
        }
        catch (Exception e)
        {//build params error
            throw new Exception(DataHelper.RetErrMsg(e.Source, sErr + e.Message, e.StackTrace));
        }
        try
        {
            sErr = "\nFailed to retrieve scalar.\n";

            int i = Convert.ToInt32(Cm1.ExecuteScalar().ToString());
            oConn.Close();
            oConn.Dispose();
            return i;
        }
        catch (SqlException e)
        {//param build error

            if (e.Number == DataHelper.ERRNUM_CONN)//concurrency error
                throw new DataHelper.dbExceptionConn(e.Message);

            throw (e);

        }
        catch (Exception e)
        {//data read error
            throw new Exception(DataHelper.RetErrMsg(e.Source, sErr + e.Message, e.StackTrace));
        }
        //return -1;//some error condition
    }
    #endregion
}