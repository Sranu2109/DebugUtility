using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//namespace Payroll_Utilities.Component.commonmanager

    public class UserManager
    {
    private Utilities objDataManager = new Utilities();  // datamanater
    private ExceptionManager objExceptionManager = new ExceptionManager();
    public string CreateConnStringForCompanyAdmin(string strCompanyDomain, bool checkValidity = true)
    {
        DataTable dt;
        string strDBServer;
        string strDBName;
        string strDBUserID;
        string strDBPWD;
        string _Valididy = "";
        SqlParameter[] _param = new SqlParameter[1];
        try
        {
        }
        catch (Exception ex)
        {

        }
        return _Valididy;
    }

}

