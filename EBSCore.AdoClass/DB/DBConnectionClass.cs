using System.Data;
using System.Collections;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace EBSCore.AdoClass
{
    using Microsoft.Extensions.Configuration;

    public class DBConnectionClass
    {

        private SqlConnection objSqlConnection;
        private SqlCommand objSqlCommand = new SqlCommand();
        private int nCounter;
        private GlobalFunctions GFunction = new GlobalFunctions();
        string strConnection = "";
       
        private readonly IConfiguration _configuration;
        public DBConnectionClass(IConfiguration configuration)
        {
            _configuration = configuration;
            strConnection = configuration.GetConnectionString("DefaultConnection"); 
        }
        public int ExecuteNonQuery(ref ArrayList DBFieldsArrayList, string storedProcedureName)
        {
            int noOfEffectedRows;
            //builder.Sources.

            try
            {
                using (System.Data.SqlClient.SqlConnection sqlConn = new System.Data.SqlClient.SqlConnection(strConnection))
                {
                    sqlConn.Open();
                    objSqlCommand = new SqlCommand(storedProcedureName, sqlConn);
                    objSqlCommand.CommandType = CommandType.StoredProcedure;

                    fillSqlCommandParametersArray(ref DBFieldsArrayList);
                    noOfEffectedRows = objSqlCommand.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                noOfEffectedRows = default(int);
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
                noOfEffectedRows = default(int);
            }
            return noOfEffectedRows;
        }

        public object ExecuteScalar(ref ArrayList DBFieldsArrayList, string storedProcedureName)
        {
            object Scalar;
            try
            {
                using (System.Data.SqlClient.SqlConnection sqlConn = new System.Data.SqlClient.SqlConnection(strConnection))
                {
                    sqlConn.Open();
                    objSqlCommand = new SqlCommand(storedProcedureName, sqlConn);
                    objSqlCommand.CommandType = CommandType.StoredProcedure;

                    fillSqlCommandParametersArray(ref DBFieldsArrayList);
                    Scalar = objSqlCommand.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Scalar;
        }

        public SqlDataReader ExecuteReader(ref ArrayList DBFieldsArrayList, string storedProcedureName)
        {
            SqlDataReader objSqlDataReader;
            try
            {
                using (System.Data.SqlClient.SqlConnection sqlConn = new System.Data.SqlClient.SqlConnection(strConnection))
                {
                    sqlConn.Open();
                    objSqlCommand = new SqlCommand(storedProcedureName, sqlConn);
                    objSqlCommand.CommandType = CommandType.StoredProcedure;

                    fillSqlCommandParametersArray(ref DBFieldsArrayList);
                    objSqlDataReader = objSqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
                }
            }
            catch (Exception ex)
            {
                objSqlDataReader = null;
            }
            return objSqlDataReader;
        }

        public DataSet FillDataset(ref ArrayList DBFieldsArrayList, string storedProcedureName)
        {
            SqlDataAdapter objSqlDataAdapter = new SqlDataAdapter(objSqlCommand);
            DataSet selectedDatatset = new DataSet();
            try
            {
                using (System.Data.SqlClient.SqlConnection sqlConn = new System.Data.SqlClient.SqlConnection(strConnection))
                {
                    
                    sqlConn.Open();
                    objSqlCommand = new SqlCommand(storedProcedureName, sqlConn);
                    objSqlCommand.CommandTimeout = objSqlCommand.CommandTimeout * 3;
                    objSqlCommand.CommandType = CommandType.StoredProcedure;

                    fillSqlCommandParametersArray(ref DBFieldsArrayList);
                    objSqlDataAdapter.SelectCommand = objSqlCommand;
                    objSqlDataAdapter.Fill(selectedDatatset);
                }
            }
            catch (Exception ex)
            {
                selectedDatatset = null;
                throw ex;
            }
            return selectedDatatset;
        }
        public DataSet FillDataset(string Query)
        {
            SqlDataAdapter objSqlDataAdapter = new SqlDataAdapter(objSqlCommand);
            DataSet selectedDatatset = new DataSet();
            try
            {
                using (System.Data.SqlClient.SqlConnection sqlConn = new System.Data.SqlClient.SqlConnection(strConnection))
                {
                    sqlConn.Open();

                    objSqlCommand = new SqlCommand(Query, sqlConn);
                    objSqlCommand.CommandTimeout = objSqlCommand.CommandTimeout * 0;
                    objSqlCommand.CommandType = CommandType.Text;

                    objSqlDataAdapter.SelectCommand = objSqlCommand;
                    objSqlDataAdapter.Fill(selectedDatatset);
                }
            }
            catch (Exception ex)
            {
                selectedDatatset = null;
            }

            return selectedDatatset;
        }

        public System.Xml.XmlReader ExecuteXmlReader(ref ArrayList DBFieldsArrayList, string storedProcedureName)
        {
            System.Xml.XmlReader objXmlReader;
            try
            {
                using (System.Data.SqlClient.SqlConnection sqlConn = new System.Data.SqlClient.SqlConnection(strConnection))
                {
                    sqlConn.Open();

                    objSqlCommand = new SqlCommand(storedProcedureName, sqlConn);
                    objSqlCommand.CommandType = CommandType.StoredProcedure;


                    fillSqlCommandParametersArray(ref DBFieldsArrayList);
                    objXmlReader = objSqlCommand.ExecuteXmlReader();
                }
            }
            catch (Exception ex)
            {
                objXmlReader = null;
            }
            return objXmlReader;
        }

        private void fillSqlCommandParametersArray(ref ArrayList DBFieldsArrayList)
        {
            objSqlCommand.Parameters.Clear();
            for (nCounter = 0; nCounter <= DBFieldsArrayList.Count - 1; nCounter++)
            {
                SqlParameter objParameter = new SqlParameter();
                TableField DBTableField = (TableField)DBFieldsArrayList[nCounter];
                objParameter.ParameterName = "@" + DBTableField.Name;
                objParameter.Direction = ParameterDirection.Input;

                // Check if the type is date to do some sort of conversion
                if (DBTableField.Type == SqlDbType.DateTime || DBTableField.Type == SqlDbType.Date)
                {
                    if (DBTableField.Value != null)
                    {
                        objParameter.Value = GFunction.FormatDBDate((string)DBTableField.Value);
                        objParameter.SqlDbType = SqlDbType.VarChar;
                    }
                }
                else
                {
                    objParameter.Value = DBTableField.Value;
                    objParameter.SqlDbType = DBTableField.Type;
                }

                objSqlCommand.Parameters.Add(objParameter);
            }
        }

        public string ConnectionString
        {
            get
            {
                return strConnection;
            }
        }

        public SqlConnection DBSqlConnection
        {
            get
            {
                return objSqlConnection;
            }
        }
    }
}
