using OrderManagementSystem.Classes;
using System;
using System.Windows;
using System.Windows.Controls;


namespace OrderManagementSystem
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        OrderHeader orderHeader;
        StockItem selectedItem;
        public Window2()
        {
            InitializeComponent();
            cboStockItems.ItemsSource = (Application.Current.MainWindow as MainWindow).allStock;
            orderHeader = (Application.Current.MainWindow as MainWindow).selectedOrder;
            lvNewOrder.ItemsSource = orderHeader.OrderItems;
            txtTotalCost.Text = orderHeader.Total.ToString();
        }

        private void btnCancelOrder_Click(object sender, RoutedEventArgs e)
        {         
            if (orderHeader.OrderStateId == "New")
            {
                string message = "Are you sure you want to cancel the order? \n Any items not saved will be permanently deleted!";
                string caption = "Cancel Order?";
                if (MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        if (orderHeader.Delete() == 1)
                        {
                            MessageBox.Show("Order cancelled!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            Close();
                            //refresh lvOrderHeaders
                            (Application.Current.MainWindow as MainWindow).LoadOrderHeaders();
                        }
                    }
                    catch (Exception ex)
                    {
                        message = $"Something went wrong! \n The order could not be cancelled. \n{ex.Message}";
                        MessageBox.Show(message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Only orders with status 'new' can be cancelled", "Uh-oh!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        private void btnSubmitOrder_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("The order status will be changed to 'Pending'\nNo more changes will be allowed", "Submit Order?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                //OrderHeader.Update() {status = "Pending"}
                orderHeader.OrderStateId = "Pending";
                try
                {
                    if (orderHeader.Update(orderHeader.OrderStateId) == 1)
                    {
                        MessageBox.Show("Order successfully submitted!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        Close();
                        //refresh lvOrderHeaders
                        (Application.Current.MainWindow as MainWindow).LoadOrderHeaders();
                        (Application.Current.MainWindow as MainWindow).lvOrderDetails.ItemsSource = orderHeader.OrderItems;
                        (Application.Current.MainWindow as MainWindow).txtSelectedOrderStatus.Text = orderHeader.OrderStateId;
                    }
                    else { orderHeader.OrderStateId = "New"; }
                }
                catch(Exception ex)
                {
                    orderHeader.OrderStateId = "New";
                    string message = $"Something went wrong! \n The order could not be submitted. \n{ex.Message}";
                    MessageBox.Show(message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }

        }
        private void btnSaveDraftOrder_Click(object sender, RoutedEventArgs e)
        {
            Close();
            (Application.Current.MainWindow as MainWindow).LoadOrderHeaders();
            (Application.Current.MainWindow as MainWindow).lvOrderDetails.ItemsSource = orderHeader.OrderItems;
            (Application.Current.MainWindow as MainWindow).txtSelectedOrderStatus.Text = orderHeader.OrderStateId;
        }
        private void cboStockItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedItem = (StockItem)cboStockItems.SelectedItem;
            int quantity = Convert.ToInt32(txtQuantity.Text);
            txtCostToAdd.Text = (quantity * selectedItem.Price).ToString();
            txtInStock.Text = selectedItem.InStock.ToString();
            
        }
        private void btnAddToOrder_Click(object sender, RoutedEventArgs e)
        {
            //usp_AddItemToOrder
            if (cboStockItems.SelectedItem != null && txtQuantity.Text != null)
            {
                if (Convert.ToInt32(txtQuantity.Text) > 0)
                {
                    int numberToAdd = Convert.ToInt32(txtQuantity.Text);
                    OrderItem itemToAdd = new OrderItem(orderHeader.Id, selectedItem.Item_ID, selectedItem.Name, selectedItem.Price, numberToAdd);
                    try
                    {
                        if (itemToAdd.AddItemToOrder() == 1)
                        {
                            MessageBox.Show("Item(s) added successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                            //reset controls TODO

                            //refresh lv
                            orderHeader.OrderItems = new OrderItems(orderHeader);
                            lvNewOrder.ItemsSource = orderHeader.OrderItems;

                            //update total cost
                            orderHeader.Total += itemToAdd.Price*itemToAdd.Quantity;
                            txtTotalCost.Text = orderHeader.Total.ToString();

                        }
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("The requested item(s) could not be added to the order!" + ex, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        private void btnDeleteFromOrder_Click(object sender, RoutedEventArgs e)
        {
            //usp RemoveItemFromOrder
            if (lvNewOrder.SelectedItem != null)
            {

                OrderItem itemToRemove = (OrderItem)lvNewOrder.SelectedItem;
                itemToRemove.OrderHeaderId = orderHeader.Id;
                try
                {
                    if (itemToRemove.RemoveItemFromOrder() == 1)
                    {
                        MessageBox.Show("Item(s) removed successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        //reset controls TODO

                        //refresh lv
                        orderHeader.OrderItems = new OrderItems(orderHeader);
                        lvNewOrder.ItemsSource = orderHeader.OrderItems;

                        //update total cost
                        orderHeader.Total -= itemToRemove.Price * itemToRemove.Quantity;
                        txtTotalCost.Text = orderHeader.Total.ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("The requested item(s) could not be removed from the order!" + ex, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                

            }
        }
        private void btnLess_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(txtQuantity.Text) > 0)
            {
                int replaceWith = Convert.ToInt32(txtQuantity.Text) - 1;
                txtQuantity.Text = replaceWith.ToString();
                if(cboStockItems.SelectedItem != null)
                {
                    txtCostToAdd.Text = (replaceWith * selectedItem.Price).ToString();
                }

            }
        }
        private void btnMore_Click(object sender, RoutedEventArgs e)
        {
            int replaceWith = Convert.ToInt32(txtQuantity.Text) + 1;
            txtQuantity.Text = replaceWith.ToString();
            if (cboStockItems.SelectedItem != null)
            {
                txtCostToAdd.Text = (replaceWith * selectedItem.Price).ToString();
            }
        }


    }
}
