using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace EduVerse.Models
{
    public class DBAccess
    {
            //calling of connection string that is globally set in web.config file
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);

            public int InsertUpdateDelete(string procedure, SqlParameter[] parameters)
            {
                SqlCommand command = new SqlCommand(procedure, connection);
                command.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter param in parameters)
                {
                    if (param != null)
                    {
                        command.Parameters.Add(param);
                    }

                }
                connection.Open();
                int res = command.ExecuteNonQuery();
                connection.Close();
                return res;
            }
            public DataTable SelectData(string procedure, SqlParameter[] parameters)
            {
                SqlCommand command = new SqlCommand(procedure, connection);
                command.CommandType = CommandType.StoredProcedure;

                if (parameters != null)
                {
                    foreach (SqlParameter param in parameters)
                    {
                        command.Parameters.Add(param);
                    }
                }
                
                DataTable table = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(table);
                return table;
            }

            
            public object ExecuteForSingleValue(string procedure, SqlParameter[] parameters)
            {
                SqlCommand command = new SqlCommand(procedure, connection);
                command.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter param in parameters)
                {
                    command.Parameters.Add(param);
                }
                connection.Open();
                object res = command.ExecuteScalar(); 
                connection.Close();
                return res;
            }

            public int InsertUpdateDeleteWithReturnValue(string procedure, SqlParameter[] parameters)
            {
                using (SqlCommand command = new SqlCommand(procedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                        command.Parameters.AddRange(parameters);

                    // Add return value parameter in one line
                    var returnParam = command.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParam.Direction = ParameterDirection.ReturnValue;

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();

                    return (int)returnParam.Value;
                }
            }

    }

}