using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace OrderManagementSystem.Classes
{
    public sealed class OrderItem
    {
        #region Public Properties
        public int OrderHeaderId { get; set; }
        public int StockItemId { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }

        #endregion

        #region Constructors
        public OrderItem()
        {

        }
        public OrderItem(int orderHeaderId, int stockItemId, string description, decimal price, int quantity)
        {
            this.OrderHeaderId = orderHeaderId;
            this.StockItemId = stockItemId;
            this.Description = description;
            this.Price = price;
            this.Quantity = quantity;
        }
        public OrderItem(DataRow orderRow)
        {
            StockItemId = (int)orderRow["StockItemId"];
            Description = orderRow["Name"].ToString();
            Price = (decimal)orderRow["Price"];
            Quantity = (int)orderRow["Quantity"];
            Total = (decimal)orderRow["Total"];
        }
        #endregion

        #region Public Data Methods
        public int AddItemToOrder()
        {
            try
            {
                SqlDAL myDAL = new SqlDAL();
                SqlParameter[] parameters =
                {
                    new SqlParameter("@OrderHeaderId", OrderHeaderId),
                    new SqlParameter("@StockItemId", StockItemId),
                    new SqlParameter("@Quantity", Quantity),
                    new SqlParameter("@Description", Description),
                    new SqlParameter("@Price", Price),
                };

                int rowsAffected = myDAL.ExecuteNonQuerySP("usp_AddItemToOrder", parameters);
                return rowsAffected;
            }
            catch (Exception ex)
            {
                throw new Exception("The item(s) could not be added to the order! ", ex);
            }
        }

        public int RemoveItemFromOrder()
        {
            try
            {
                SqlDAL myDAL = new SqlDAL();
                SqlParameter[] parameters =
                {
                    new SqlParameter("@OrderHeaderId", OrderHeaderId),
                    new SqlParameter("@StockItemId", StockItemId)
                };

                int rowsAffected = myDAL.ExecuteNonQuerySP("usp_RemoveItemFromOrder", parameters);
                return rowsAffected;
            }
            catch (Exception ex)
            {
                throw new Exception("The item(s) could not be removed from the order! ", ex);
            }
        }
        #endregion
    }
}
