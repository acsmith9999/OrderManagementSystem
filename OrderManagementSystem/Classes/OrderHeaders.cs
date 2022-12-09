using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace OrderManagementSystem.Classes
{
    public sealed class OrderHeaders:List<OrderHeader>
    {
        #region Constructors
        public OrderHeaders()
        {
            GetAllOrderHeaders();
        }
        #endregion

        #region Public Methods
        public void Refresh()
        {
            this.Clear();
            GetAllOrderHeaders();
        }

        #endregion

        #region Private Methods
        private void GetAllOrderHeaders()
        {
            SqlDAL myDAL = new SqlDAL();
            DataTable orderHeadersTable = myDAL.ExecuteStoredProc("usp_GetAllOrderHeaders");

            foreach (DataRow orderRow in orderHeadersTable.Rows)
            {
                OrderHeader orderHeader = new OrderHeader(orderRow);
                Add(orderHeader);
            }
        }

        #endregion
    }
}
