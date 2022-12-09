using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;

namespace OrderManagementSystem.Classes
{
    class SqlDAL
    {
        #region Private Properties
        private string _connString;
        #endregion

        #region Constructors
        public SqlDAL()
        {
            //get the connection string from app.config
            _connString = ConfigurationManager.ConnectionStrings["cnnStrOMS"].ConnectionString;
        }
        #endregion

        #region Methods
        public DataTable ExecuteStoredProc(string SPName)
        {
            SqlConnection conn = new SqlConnection(_connString);

            //create command object
            SqlCommand cmd = new SqlCommand(SPName, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Connection.Open();

            SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            DataTable dataTable = new DataTable();
            dataTable.Load(dataReader);
            return dataTable;
        }


        public DataTable ExecuteStoredProc(string SPName, SqlParameter[] parameters)
        {
            SqlConnection conn = new SqlConnection(_connString);

            //create command object
            SqlCommand cmd = new SqlCommand(SPName, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            FillParameter(cmd, parameters);

            cmd.Connection.Open();

            SqlDataReader dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            DataTable dataTable = new DataTable();
            dataTable.Load(dataReader);
            return dataTable;

        }

        private void FillParameter(SqlCommand cmd, SqlParameter[] parameters)
        {
            foreach (SqlParameter parameter in parameters)
            {
                cmd.Parameters.Add(parameter);
            }
        }

        internal int ExecuteNonQuerySP(string SPName, SqlParameter[] parameters)
        {
            SqlConnection conn = new SqlConnection(_connString);

            //create cmd obj
            SqlCommand cmd = new SqlCommand(SPName, conn);
            cmd.CommandType = CommandType.StoredProcedure;


            FillParameter(cmd, parameters);

            cmd.Connection.Open();

            //execute the sp
            int _ = cmd.ExecuteNonQuery();

            Debug.Print($"The db connection is {cmd.Connection.State.ToString()}");
            if (cmd.Connection.State == ConnectionState.Open)
            {
                cmd.Connection.Close();
            }

            //return the result to the calling code
            return _;
        }
        #endregion
    }
}
