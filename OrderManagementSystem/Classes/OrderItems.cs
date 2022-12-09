using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace OrderManagementSystem.Classes
{
    public sealed class OrderItems : List<OrderItem>
    {
        #region Constructors
        public OrderItems()
        {

        }
        public OrderItems(OrderHeader orderHeader)
        {
            try
            {
                SqlDAL myDAL = new SqlDAL();
                SqlParameter[] parameters = { new SqlParameter("@Id", orderHeader.Id) };
                DataTable dtOrderItems = myDAL.ExecuteStoredProc("usp_GetOrderItemsByHeaderId", parameters);
                foreach (DataRow orderRow in dtOrderItems.Rows)
                {
                    OrderItem orderItem = new OrderItem(orderRow);
                    this.Add(orderItem);
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Unable to retrieve order items! ", ex);
            }
        }
        #endregion

    }
}
