
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Data;

namespace EBSCore.AdoClass
{
    public class DBParentStoredProcedureClass
    {

        // FieldsArrayList : Store TabelFields for all stored Procedure that will passed to SP
        // SPName : Stored Procedure Name in the DataBase
        // DBConnection : Object that will intract Directly With Database
        protected ArrayList FieldsArrayList;
        protected string SPName;
        protected DBConnectionClass DBConnection;

        // Initialize DBConnection Object 
        public DBParentStoredProcedureClass(IConfiguration configuration)
        {
            DBConnection = new DBConnectionClass(configuration);
        }

        public DBParentStoredProcedureClass(string storedProcedureName, IConfiguration configuration)
            : this(configuration)
        {
            SPName = storedProcedureName;
        }

        // SqlQueryType : is Enumeration of all availabe operation that 
        // can be Accomplished through DBConnection Object 

        // ExecuteNonQuery : Performs commands that change the database but do not return a 
        // specific value, including adding and deleting items from a database. 
        // The ExecuteNonQuery method returns the number of rows affected by the command.
        // ExecuteScalar   : Performs query commands that return a single value, such as counting the number of records in a table.
        // FillDataset     : Return the select Statment result from Databse as a Dataset
        // ExecuteReader   : Reads records sequentially from the database.
        // ExecuteXmlReader: Return the select Statment result from Databse as a XmlReader

        public enum SqlQueryType
        {
            ExecuteNonQuery,
            FillDataset,
            ExecuteReader,
            ExecuteScalar,
            ExecuteXmlReader
        }

        // QueryDatabase Function call the approbrite function from DBConnection Class
        // based on the sended QueryType Operation
        protected virtual object QueryDatabase(SqlQueryType QueryType)
        {
            switch (QueryType)
            {
                case SqlQueryType.ExecuteNonQuery:
                    {
                        return DBConnection.ExecuteNonQuery(ref FieldsArrayList, SPName);
                    }

                case SqlQueryType.ExecuteScalar:
                    {
                        return DBConnection.ExecuteScalar(ref FieldsArrayList, SPName);
                    }

                case SqlQueryType.FillDataset:
                    {
                        return DBConnection.FillDataset(ref FieldsArrayList, SPName);
                    }

                case SqlQueryType.ExecuteReader:
                    {
                        return DBConnection.ExecuteReader(ref FieldsArrayList, SPName);
                    }

                case SqlQueryType.ExecuteXmlReader:
                    {
                        return DBConnection.ExecuteXmlReader(ref FieldsArrayList, SPName);
                    }
            }
            return null;
        }

        protected virtual object QueryDatabase(SqlQueryType QueryType, object parameters)
        {
            FieldsArrayList = new ArrayList();

            if (parameters != null)
            {
                foreach (var propertyInfo in parameters.GetType().GetProperties())
                {
                    var value = propertyInfo.GetValue(parameters, null);
                    var sqlDbType = GetSqlDbType(propertyInfo.PropertyType);
                    var tableField = new TableField(propertyInfo.Name, sqlDbType, true);
                    tableField.SetValue(value?.ToString() ?? string.Empty, ref FieldsArrayList);
                }
            }

            return QueryDatabase(QueryType);
        }

        private SqlDbType GetSqlDbType(Type propertyType)
        {
            var type = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

            if (type == typeof(int))
            {
                return SqlDbType.Int;
            }

            if (type == typeof(long))
            {
                return SqlDbType.BigInt;
            }

            if (type == typeof(bool))
            {
                return SqlDbType.Bit;
            }

            if (type == typeof(DateTime))
            {
                return SqlDbType.DateTime;
            }

            return SqlDbType.NVarChar;
        }

        // Set Or Get Stored Procedure Name
        //public string StoredProcedureName { set;get; }
    }
}