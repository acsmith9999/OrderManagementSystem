using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using OrderManagementSystem;
using OrderManagementSystem.Classes;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace OrderManagementSystemTest
{

    [TestClass]
    public class AddOrderTest
    {
        [TestMethod]
        public void AddOrderHeader()
        {
            string status = "New";
            DateTime dateTime = DateTime.Now;

            OrderHeader orderHeader = new OrderHeader(status, dateTime);

            string statusActual = orderHeader.OrderStateId;
            DateTime dateTimeActual = orderHeader.OrderDate;

            //test default values called in constructor
            Assert.AreEqual(dateTime, dateTimeActual);
            Assert.AreEqual(status, statusActual);
        }
    }
    [TestClass]
    public class AddItemTest
    {
        [TestMethod]
        public void AddItem()
        {
            OrderItem orderItem = new OrderItem(1, 1, "Chair", 25, 1);

            Assert.AreEqual(1, orderItem.OrderHeaderId);
            Assert.AreEqual(1, orderItem.StockItemId);
            Assert.AreEqual("Chair", orderItem.Description);
            Assert.AreEqual(25, orderItem.Price);
            Assert.AreEqual(1, orderItem.Quantity);
        }
    }
}
