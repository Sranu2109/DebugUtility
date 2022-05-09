using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//namespace Payroll_Utilities.Component.commonmanager

    public class Utilities
    {
    //private System.Web.HttpApplication WebCurrentApplicationContext = new System.Web.HttpApplication();
    private SqlConnection DataConn = new SqlConnection();
    public SqlCommand dataCom = new SqlCommand();
    private SqlDataAdapter DataAdtp = new SqlDataAdapter();
    private SqlTransaction dataTran;
    private string strClientIP;
    private ExceptionManager objexceptionManager = new ExceptionManager();
    private common objcommon = new common();

    //public void OpenConnection()
    //{
    //    string strDBServer = ConfigurationManager.AppSettings("DataServer").ToString;
    //    string strDBName = ConfigurationManager.AppSettings("DataBaseName").ToString;
    //    string strDBUserID = ConfigurationManager.AppSettings("DBUserID").ToString;
    //    string strDBPWD = ConfigurationManager.AppSettings("DBPassword").ToString;

    //    try
    //    {
    //        if (HttpContext.Current.Session("ConnString").ToString == "")
    //            CreateConnectionString(strDBServer, strDBName, strDBUserID, strDBPWD);
    //        if (DataConn.State == ConnectionState.Open)
    //        {
    //            CloseConnection();
    //            DataConn.ConnectionString = "";
    //        }
    //        if (DataConn.State == ConnectionState.Closed)
    //        {
    //            DataConn.ConnectionString = HttpContext.Current.Session("ConnString").ToString;
    //            DataConn.Open();
    //        }

    //        dataCom.Connection = DataConn;
    //        DataAdtp.SelectCommand = dataCom;
    //        dataCom.CommandTimeout = 5000000;
    //    }
    //    catch (Exception ex)
    //    {
    //        // objexceptionManager.PublishError("Open Connection(OpenConnection)", ex);
    //        Console.WriteLine("Open Connection(OpenConnection)", ex.Message);
    //    }
    //}

    //public void CloseConnection()
    //{
    //    try
    //    {
    //        if (DataConn.State == ConnectionState.Open)
    //        {
    //            DataConn.Close();
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        ex.Message.ToString();
    //    }
    //}

    //public void CreateConnectionString(string strDBServer, string strDBName, string strDBUserID, string strDBPWD)
    //{
    //    try
    //    {
    //        string strConnStringSA = "Persist Security Info=False;Data Source=" + strDBServer + ";Initial Catalog=" + strDBName + ";User ID=" + strDBUserID + ";Password=" + strDBPWD;

    //        HttpContext.Current.Session.Add("ConnString", strConnStringSA);
    //    }
    //    catch (Exception ex)
    //    {
    //        // objexceptionManager.PublishError("Create Connection String(CreateConnectionString)", ex);
    //        Console.WriteLine("Create Connection String(CreateConnectionString)", ex.Message);
    //    }
    //}
    public DataSet GetDataSetProc(string strProcName, SqlParameter[] arProcParams = null, string TableName = "")
    {
        // Will return a DataSet with the Table specified in the SQL string Based on the passing of the stored procedure
        DataSet dataSet = new DataSet(); // creates a new dataset
        string cs = "Persist Security Info=False;Data Source= tpserver123;Initial Catalog= Payrollsadmin;User ID= sa;Password= admin@123;";
        SqlConnection con = new SqlConnection(cs);
        //string Q = "strProcName";
        SqlCommand cmd = new SqlCommand(strProcName, con);
        SqlDataAdapter sda = new SqlDataAdapter(strProcName, con);
        cmd.CommandType = CommandType.StoredProcedure;

        con.Open();
        try
        {
           // SqlParameter _param;
            //dataCom.Parameters.Clear();
            //DataAdtp.SelectCommand = dataCom;
            if (arProcParams != null)
            {
                foreach (var param in arProcParams)
                    cmd.Parameters.Add(param);
            }



            if (TableName == "")
                sda.Fill(dataSet);
            else
                sda.Fill(dataSet, TableName);
        }
        catch (Exception ex)
        {
            ex.Message.ToString();
        }
        finally
        {
            // CloseConnection();
            con.Close();
        }
        return dataSet;
    }

    public int ExecuteStoredProc(string strProcName, SqlParameter[] arProcParams = null, object lblMessageControl = null)
    {
        int i;
       // string cs = "Persist Security Info=False;Data Source=" + strDBServer + ";Initial Catalog=" + strDBName + ";User ID=" + strDBUserID + ";Password=" + strDBPWD;
        string cs = "Persist Security Info=False;Data Source= tpserver123;Initial Catalog= Payrollsadmin;User ID= sa;Password= admin@123;";
        SqlConnection con = new SqlConnection(cs);
        //string Q = "strProcName";
        SqlCommand cmd = new SqlCommand(strProcName, con);
        cmd.CommandType = CommandType.StoredProcedure;

        try
        {
            con.Open();
            cmd.Parameters.Clear();
            if (arProcParams != null)
            {
                foreach (var param in arProcParams)
                    cmd.Parameters.Add(param);
            }
            con.Close();
            i = cmd.ExecuteNonQuery();
            if (i >= 0)
            {
                return i;
            }
            else
            {
                return -1;
            }
            //CloseConnection();
        }
        catch (Exception ex)
        {
            ex.Message.ToString();
        }
        finally
        {
            con.Close();
        }
        return -1;
    }
    public int ExecuteStoredProcMsg(string strProcName, SqlParameter[] arProcParams, object lblMessageControl = null)
    {
        int i;
        // string cs = "Persist Security Info=False;Data Source=" + strDBServer + ";Initial Catalog=" + strDBName + ";User ID=" + strDBUserID + ";Password=" + strDBPWD;
        string cs = "Persist Security Info=False;Data Source= tpserver123;Initial Catalog= Payrollsadmin;User ID= sa;Password= admin@123;";

        SqlConnection con = new SqlConnection(cs);
        //string Q = "strProcName";
        SqlCommand cmd = new SqlCommand(strProcName, con);
        cmd.CommandType = CommandType.StoredProcedure;
        SqlTransaction sqlTrans;
        con.Open();
        // // Create the Transaction object
        sqlTrans = con.BeginTransaction(IsolationLevel.ReadCommitted);
        try
        {
            cmd.Transaction = sqlTrans;
            cmd.Parameters.Clear();
            foreach (var param in arProcParams)
                cmd.Parameters.Add(param);
            i = cmd.ExecuteNonQuery();

            if (i >= 0)
                sqlTrans.Commit();
            else
                sqlTrans.Rollback();
            if (i >= 0)
            {
                return i;
            }
            else
            {
                return -1;
            }
        }
        catch (SqlException sqlEx)
        {
            sqlTrans.Rollback();
            //   objexceptionManager.PublishSQLError("Execute Store Procedure(ExecuteStoredProc)", sqlEx, lblMessageControl);
            Console.WriteLine("Execute Store Procedure(ExecuteStoredProc)", sqlEx.Message, lblMessageControl);
        }
        catch (Exception ex)
        {
            sqlTrans.Rollback();
            // objexceptionManager.PublishError("Execute Store Procedure(ExecuteStoredProc)", ex);
            Console.WriteLine("Execute Store Procedure(ExecuteStoredProc)", ex.ToString());
        }
        finally
        {
            sqlTrans = null        /* TODO Change to default(_) if this is not a reference type */;
            con.Close();
        }
        return -1;
    }
    public string ExecuteScalarMsg(string strProcName, SqlParameter[] arProcParams = null, object lblMessageControl = null)
    {
        int i = 0;
        // string cs = "Persist Security Info=False;Data Source=" + strDBServer + ";Initial Catalog=" + strDBName + ";User ID=" + strDBUserID + ";Password=" + strDBPWD;
        string cs = "Persist Security Info=False;Data Source= tpserver123;Initial Catalog= Payrollsadmin;User ID= sa;Password= admin@123;";

        SqlConnection con = new SqlConnection(cs);
        //string Q = strProcName;
        SqlCommand cmd = new SqlCommand(strProcName, con);
        cmd.CommandType = CommandType.StoredProcedure;
        SqlTransaction sqlTrans;
        string ResultSet = "";
        // // Create the Transaction object
        con.Open();
        sqlTrans = con.BeginTransaction(IsolationLevel.ReadCommitted);
        try
        {
            cmd.Transaction = sqlTrans;

            cmd.Parameters.Clear();
            if (arProcParams != null)
            {
                foreach (var param in arProcParams)
                    cmd.Parameters.Add(param);
            }
           // int i = (int)cmd.ExecuteScalar();  //doubt
             //ResultSet = cmd.ExecuteScalar();
            ResultSet = (string)cmd.ExecuteScalar();
            if (i >= 0)
                sqlTrans.Commit();
            else
                sqlTrans.Rollback();
        }
        catch (SqlException sqlEx)
        {
            sqlTrans.Rollback();
            // objexceptionManager.PublishSQLError("Execute Store Procedure(ExecuteStoredProc)", sqlEx, lblMessageControl);
            Console.WriteLine("Execute Store Procedure(ExecuteStoredProc)", sqlEx.Message, lblMessageControl);
        }
        catch (Exception ex)
        {
            sqlTrans.Rollback();
            //   objexceptionManager.PublishError("Execute Store Procedure(ExecuteStoredProc)", ex);
            Console.WriteLine("Execute Store Procedure(ExecuteStoredProc)", ex.Message);
        }
        finally
        {
            sqlTrans = null/* TODO Change to default(_) if this is not a reference type */;
            con.Close();
        }
        return ResultSet;
    }
    public DataSet GetDataSetProcForReportSA(string compcode, string strProcName, SqlParameter[] arProcParams = null, string TableName = "")
    {
        UserManager objusermanager = new UserManager();
        // string cs = "Persist Security Info=False;Data Source=" + strDBServer + ";Initial Catalog=" + strDBName + ";User ID=" + strDBUserID + ";Password=" + strDBPWD;
        string cs = "Persist Security Info=False;Data Source= tpserver123;Initial Catalog= Payrollsadmin;User ID= sa;Password= admin@123;";

        SqlConnection con = new SqlConnection(cs);
        con.Open(); 
        // Will return a DataSet with the Table specified in the SQL string Based on the passing of the stored procedure 
        DataSet _ds = null/* TODO Change to default(_) if this is not a reference type */;
        try
        {
            objusermanager.CreateConnStringForCompanyAdmin(compcode, false);
            _ds = GetDataSetProc(strProcName, arProcParams, TableName);
        }
        catch (Exception ex)
        {
            //objexceptionManager.PublishError("Get Dataset Procedure For Reports(GetDataSetProcForReportSA)", ex);
            Console.WriteLine("Get Dataset Procedure For Reports(GetDataSetProcForReportSA)", ex.Message);
        }
        finally
        {
            //HttpContext.Current.Session("ConnString") = "";
            con.Close();
             
        }
        return _ds;
    }
    public string FixQuotes(string Value, bool AddBracklet = true)
    {
        // Fix SQL quotes to make Global
        string MyValue;
        string Bracklet;
        if (AddBracklet == true)
            Bracklet = "'";
        else
            Bracklet = "";
        MyValue = Bracklet + Value.Replace("'", "''") + Bracklet;
        //  FixQuotes = MyValue;
        return MyValue;
    }
}

