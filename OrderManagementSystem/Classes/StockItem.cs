using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Classes
{
    public sealed class StockItem
    {
        #region Private Field Variables
        private DataTable _dtStock;
        #endregion

        #region Public Properties
        public int Item_ID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int InStock { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor: Instantiates a new empty StockItem object
        /// </summary>
        public StockItem()
        {

        }
        public StockItem(int stockItemId)
        {
            //Get the stock's details from the db
            SqlDAL myDal = new SqlDAL();
            try
            {
                //set up the parameter array for the stored procedure to accept the stockId
                SqlParameter[] parameters = { new SqlParameter("@Id", stockItemId) };

                //call the method on the DAL that reads the db
                this._dtStock = myDal.ExecuteStoredProc("usp_GetStockById", parameters);

                //check if dt has rows
                if (_dtStock != null && _dtStock.Rows.Count > 0)
                {
                    //map order's details to this class's properties by passing the first row of the table 
                    LoadStockItemProperties(_dtStock.Rows[0]);
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Unable to retrieve item details", ex);
            }
            finally
            {
                this._dtStock.Dispose();
                this._dtStock = null;
                myDal = null;
            }
        }
        /// <summary>
        /// Instantiates a stock item object from a datarow, used by the StockItems collection class
        /// </summary>
        /// <param name="stockItemRow"></param>
        public StockItem(DataRow stockItemRow)
        {
            LoadStockItemProperties(stockItemRow);
        }
        #endregion

        #region Private Methods
        private void LoadStockItemProperties(DataRow dataRow)
        {
            this.Item_ID = (int)dataRow["Id"];
            this.Name = dataRow["Name"].ToString();
            this.Price = (decimal)dataRow["Price"];
            this.InStock = (int)dataRow["InStock"];
        }
        #endregion



        #region Public Data Methods
        public int Insert()
        {
            try
            {
                SqlDAL myDAL = new SqlDAL();
                SqlParameter[] parameters = {
                    new SqlParameter("@Name", Name),
                    new SqlParameter("@Price", Price),
                    new SqlParameter("@InStock", InStock)
                };

                parameters[1].SqlDbType = SqlDbType.Decimal;

                //define variable to return
                int rowsAffected = myDAL.ExecuteNonQuerySP("usp_AddStock", parameters);
                return rowsAffected;
            }
            catch (Exception ex)
            {
                throw new Exception("The Patient could not be added! ", ex);
            }
        }

        public int Update(StockItem selectedStock)
        {
            try
            {
                SqlDAL myDAL = new SqlDAL();
                SqlParameter[] parameters = {
                    new SqlParameter("@Name", selectedStock.Name),
                    new SqlParameter("@Price", selectedStock.Price),
                    new SqlParameter("@InStock", selectedStock.InStock),
                    new SqlParameter("@Id", selectedStock.Item_ID)
                };

                parameters[1].SqlDbType = SqlDbType.Decimal;

                //define variable to return
                int rowsAffected = myDAL.ExecuteNonQuerySP("usp_UpdateStock", parameters);
                return rowsAffected;
            }
            catch (Exception ex)
            {
                throw new Exception("The stock could not be updated! ", ex);
            }
        }

        public int Delete()
        {
            try
            {
                SqlDAL myDAL = new SqlDAL();
                SqlParameter[] parameters = { new SqlParameter("Id", Item_ID) };
                int rowsAffected = myDAL.ExecuteNonQuerySP("usp_DeleteStock", parameters);
                return rowsAffected;
            }
            catch(Exception ex)
            {
                throw new Exception("The stock could not be deleted! ", ex);
            }
        }
        #endregion



        #region Public Methods
        public static int CompareStockName(StockItem s1, StockItem s2)
        {
            return s1.Name.CompareTo(s2.Name);
        }
        #endregion
    }
}
