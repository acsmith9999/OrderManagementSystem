using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace OrderManagementSystem.Classes
{
    public sealed class StockItems : List<StockItem>
    {
        #region Constructors
        public StockItems()
        {
            //gets all stock
            GetAllStockItems();
        }
        #endregion

        #region Public Methods
        public void Refresh()
        {
            //clear the stock item objects from this class's internal list. get all stock items from db
            this.Clear();
            GetAllStockItems();
        }
        #endregion

        #region Private Methods
        private void GetAllStockItems()
        {
            SqlDAL myDAL = new SqlDAL();
            DataTable stockItemsTable = myDAL.ExecuteStoredProc("usp_GetAllStock");

            foreach (DataRow stockRow in stockItemsTable.Rows)
            {
                StockItem stockItem = new StockItem(stockRow);
                Add(stockItem);
            }
        }
        #endregion
    }
}
