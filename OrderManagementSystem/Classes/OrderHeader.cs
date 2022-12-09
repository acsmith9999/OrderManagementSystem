using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Classes
{
    public sealed class OrderHeader
    {
        #region Private Field Variables
        private DataTable _dtOrderHeader;
        #endregion

        #region Public Properties
        public int Id { get; set; }
        public string OrderStateId { get; set; }
        public DateTime OrderDate { get; set; }
        public int NumberOfLineItems { get; set; }
        public decimal Total { get; set; }
        public IEnumerable<OrderItem> OrderItems { get; set; }

        #endregion

        #region Constructors
        public OrderHeader(string orderState, DateTime dateTime)
        {
            this.OrderStateId = orderState;
            this.OrderDate = dateTime;
        }
        public OrderHeader(int Id)
        {
            //Get the orders's details from the db
            SqlDAL myDal = new SqlDAL();
            try
            {
                //set up the parameter array for the stored procedure to accept the orderId
                SqlParameter[] parameters = { new SqlParameter("@Id", Id) };

                //call the method on the DAL that reads the db
                this._dtOrderHeader = myDal.ExecuteStoredProc("usp_GetOrderHeader", parameters);

                //check if dt has rows
                if (_dtOrderHeader != null && _dtOrderHeader.Rows.Count > 0)
                {
                    //map order's details to this class's properties by passing the first row of the table 
                    LoadOrderHeaderProperties(_dtOrderHeader.Rows[0]);
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Unable to retrieve order details", ex);
            }
            finally
            {
                this._dtOrderHeader.Dispose();
                this._dtOrderHeader = null;
                myDal = null;
            }
        }
        public OrderHeader(DataRow orderRow)
        {
            LoadOrderHeaderProperties(orderRow);
        }

        #endregion

        #region
        private void LoadOrderHeaderProperties(DataRow dataRow)
        {
            this.Id = (int)dataRow["Id"];
            this.OrderStateId = dataRow["Name"].ToString();
            this.OrderDate = (DateTime)dataRow["OrderDate"];
            OrderItems orderItems = new OrderItems(this);
            this.OrderItems = orderItems;
            this.NumberOfLineItems = (int)dataRow["NumberOfOrderItems"];
            if(NumberOfLineItems != 0)
            {
                this.Total = (decimal)dataRow["TotalCost"];
            }

        }

        #endregion

        #region Publc Data Methods
        public int Insert()
        {
            try
            {
                SqlDAL myDAL = new SqlDAL();
                SqlParameter[] parameters =
                {
                    new SqlParameter("@orderStateId", 1),
                    new SqlParameter("@date", OrderDate)
                };

                int rowsAffected = myDAL.ExecuteNonQuerySP("usp_AddOrderHeader", parameters);
                return rowsAffected;
            }
            catch (Exception ex)
            {
                throw new Exception("The order could not be created! ", ex);
            }
        }

        public int Delete()
        {
            try
            {
                SqlDAL myDAL = new SqlDAL();
                SqlParameter[] parameters = { new SqlParameter("Id", Id) };
                int rowsAffected = myDAL.ExecuteNonQuerySP("usp_DeleteOrderHeader", parameters);
                return rowsAffected;
            }
            catch (Exception ex)
            {
                throw new Exception("The order could not be cancelled! ", ex);
            }
        }

        public int Update(string state)
        {
            int stateId = 1;
            switch (state)
            {
                case "New": stateId = 1;
                    break;
                case "Pending": stateId = 2;
                    break;
                case "Rejected": stateId = 3;
                    break;
                case "Complete": stateId = 4;
                    break;
            }
            try
            {
                SqlDAL myDAL = new SqlDAL();
                SqlParameter[] parameters = {
                    new SqlParameter("@Id", Id),
                    new SqlParameter("@OrderStateId", stateId),
                    new SqlParameter("@OrderDate", OrderDate)
                };
                //convert date value for DOB
                //parameters[2].SqlDbType = SqlDbType.DateTime;

                //define variable to return
                int rowsAffected = myDAL.ExecuteNonQuerySP("usp_UpdateOrderHeader", parameters);
                return rowsAffected;
            }
            catch (Exception ex)
            {
                throw new Exception("The order could not be submitted! ", ex);
            }
        }
        #endregion
    }
}
