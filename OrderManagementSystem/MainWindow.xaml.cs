using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using OrderManagementSystem.Classes;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace OrderManagementSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public StockItems allStock = new StockItems();
        public OrderHeaders allOrderHeaders = new OrderHeaders();
        public int orderHeaderToLoad;
        public OrderItems itemsToDisplay;
        public OrderHeader selectedOrder;

        public MainWindow()
        {
            InitializeComponent();

            LoadStockItems();
            LoadOrderHeaders();
        }

        #region Loading
        public void LoadStockItems()
        {
            allStock.Clear();
            allStock = new StockItems();
            //sort alphabetically
            Comparison<StockItem> compareName = new Comparison<StockItem>(StockItem.CompareStockName);
            allStock.Sort(compareName);
            lvStock.ItemsSource = allStock;
        }

        public void LoadOrderHeaders()
        {
            allOrderHeaders.Clear();
            allOrderHeaders = new OrderHeaders();
            lvOrders.ItemsSource = allOrderHeaders;
        }
        #endregion

        #region All Orders
        private void btnCreateNewOrder_Click(object sender, RoutedEventArgs e)
        {
            OrderHeader newOrderHeader = new OrderHeader("New",DateTime.Now);

            if (newOrderHeader.Insert() == 1)
            {
                lvOrders.SelectedIndex = lvOrders.Items.Count - 1;
                lvOrders.ScrollIntoView(lvOrders.SelectedItem);
                LoadOrderHeaders();
            }
        }

        private void btnViewOrderDetails_Click(object sender, RoutedEventArgs e)
        {

            if (lvOrders.SelectedItem != null)
            {
                selectedOrder = (OrderHeader)lvOrders.SelectedItem;
                lvOrderDetails.ItemsSource = selectedOrder.OrderItems;

                tabControl.SelectedIndex = 1;
                txtSelectedOrderStatus.Text = selectedOrder.OrderStateId;
                txtSelectedOrderId.Text = selectedOrder.Id.ToString();
            }
            else { MessageBox.Show("Please select an order to view", "Select Order!", MessageBoxButton.OK, MessageBoxImage.Asterisk); }
        }

        private void btnEditOrder_Click(object sender, RoutedEventArgs e)
        {
            if(lvOrders.SelectedItem != null)
            {
                selectedOrder = (OrderHeader)lvOrders.SelectedItem;
                if(selectedOrder.OrderStateId != "New")
                {
                    MessageBox.Show("Only orders marked 'New' can be edited", "Invalid Selection!", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
                else
                {
                    orderHeaderToLoad = selectedOrder.Id;
                    itemsToDisplay = (OrderItems)selectedOrder.OrderItems;

                    Window2 createWindow = new Window2();
                    createWindow.Show();
                }
            }
            else { MessageBox.Show("Please select an order to edit", "Select Order!", MessageBoxButton.OK, MessageBoxImage.Asterisk); }
        }

        private void btnRefreshOrders_Click(object sender, RoutedEventArgs e)
        {
            LoadOrderHeaders();
        }


        #endregion

        #region Order Details
        private void btnCancelOrder_Click(object sender, RoutedEventArgs e)
        {
            if (selectedOrder.OrderStateId == "New")
            {
                string message = "Are you sure you want to cancel the order? \n Any items not saved will be permanently deleted!";
                string caption = "Cancel Order?";
                if (MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        if (selectedOrder.Delete() == 1)
                        {
                            MessageBox.Show("Order cancelled!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            tabControl.SelectedIndex = 0;
                            //refresh lvOrderHeaders
                            LoadOrderHeaders();
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

        private void btnProcessOrder_Click(object sender, RoutedEventArgs e)
        {
            //check status
            if (selectedOrder.OrderStateId != "Pending")
            {
                MessageBox.Show("Only pending orders can be processed\nDraft orders must be submitted before processing", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                foreach (OrderItem o in selectedOrder.OrderItems)
                {
                    StockItem stockItem = new StockItem(o.StockItemId);
                    if (o.Quantity > stockItem.InStock)
                    {
                        selectedOrder.Update("Rejected");
                        MessageBox.Show("Not enough " + o.Description + " in stock. Order rejected.", "Rejected!", MessageBoxButton.OK, MessageBoxImage.Error);
                        lvOrderDetails.ItemsSource = null;
                        tabControl.SelectedIndex = 0;
                        LoadOrderHeaders();
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Enough " + o.Description + " in stock.", "In stock!", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }

            }
            foreach (OrderItem o in selectedOrder.OrderItems)
            {
                StockItem stockItem = new StockItem(o.StockItemId);
                stockItem.InStock -= o.Quantity;
                stockItem.Update(stockItem);
                LoadStockItems();
                selectedOrder.Update("Complete");

                lvOrderDetails.ItemsSource = null;
                tabControl.SelectedIndex = 0;
                LoadOrderHeaders();
            }
            MessageBox.Show("Order complete!", "Complete!", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnEditSelectedOrder_Click(object sender, RoutedEventArgs e)
        {
            if(selectedOrder.OrderStateId != "New")
            {
                MessageBox.Show("Only orders marked 'New' can be edited", "Invalid Selection!", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            else
            {
                orderHeaderToLoad = selectedOrder.Id;
                itemsToDisplay = (OrderItems)selectedOrder.OrderItems;

                Window2 createWindow = new Window2();
                createWindow.Show();
            }
        }
        private void btnRefreshSelectedOrder_Click(object sender, RoutedEventArgs e)
        {
            lvOrderDetails.ItemsSource = selectedOrder.OrderItems;
        }

        #endregion

        #region Stock
        private void btnUpdateStock_Click(object sender, RoutedEventArgs e)
        {
            if (lvStock.SelectedItem != null)
            {
                string message = "The stocks's details will be updated! \n Do you wish to continue?";
                string caption = "Update stock?";

                if (MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    StockItem selectedStock;
                    selectedStock = (StockItem)lvStock.SelectedItem;
                    int selectedIndex = lvStock.SelectedIndex;
                    AssignPropertiesToStock(selectedStock);

                    try
                    {
                        if (selectedStock.Update(selectedStock) == 1)
                        {
                            MessageBox.Show("Stock details successfully updated!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadStockItems();
                            lvStock.SelectedIndex = selectedIndex;
                            lvStock.ScrollIntoView(lvStock.SelectedIndex);
                        }
                    }
                    catch (Exception ex)
                    {
                        message = $"Something went wrong! \n The stock's details could not be updated. \n{ex.Message}";
                        MessageBox.Show(message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a stock item to update", "Select stock", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        private void btnAddStock_Click(object sender, RoutedEventArgs e)
        {
            //toggle the button between add new and save
            if (btnAddStock.Content.ToString() == "Add")
            {
                //toggle the button to save
                btnAddStock.Content = "Save";
                //deselect the listview
                lvStock.SelectedIndex = -1;
                //disable the listview
                lvStock.IsEnabled = false;
                //clear the controls

                //toggle the other buttons
                btnUpdateStock.IsEnabled = false;
                btnDeleteStock.IsEnabled = false;
                btnCancelStock.IsEnabled = true;
            }
            else
            {
                //do the save
                if (ValidateStockControls())
                {
                    StockItem newStock = new StockItem();

                    AssignPropertiesToStock(newStock);

                    if (newStock.Insert() == 1)
                    {
                        MessageBox.Show("New stock added");
                        ClearStockControls();
                        LoadStockItems();
                        lvStock.SelectedIndex = lvStock.Items.Count - 1;
                        lvStock.ScrollIntoView(lvStock.SelectedItem);
                    }
                    //toggle the button back to add new
                    if (btnAddStock.Content.ToString() == "Save")
                    {
                        //toggle the button to save
                        btnAddStock.Content = "Add";
                        //disable the listview
                        lvStock.IsEnabled = true;
                        //toggle the other buttons
                        btnUpdateStock.IsEnabled = true;
                        btnDeleteStock.IsEnabled = true;
                        btnCancelStock.IsEnabled = false;
                    }
                }
            }
        }
        private void AssignPropertiesToStock(StockItem newStock)
        {
            newStock.Name = txtStockName.Text;
            newStock.Price = Convert.ToDecimal(txtStockPrice.Text);
            newStock.InStock = Convert.ToInt32(txtStockQuantity.Text);
        }
        private void ClearStockControls()
        {
            txtStockName.Clear();
            txtStockPrice.Clear();
            txtStockQuantity.Clear();
        }
        private bool ValidateStockControls()
        {
            if (txtStockName.Text == string.Empty || txtStockName.Text == null)
            {
                MessageBox.Show("Please enter a stock name!", "Stock Name?", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }
            else if (txtStockPrice.Text == string.Empty || txtStockPrice.Text == null)
            {
                MessageBox.Show("Please enter a price!", "Price?", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }
            else if (!float.TryParse(txtStockPrice.Text, out _)){
                MessageBox.Show("Please enter a price!", "Price?", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }
            else if (txtStockQuantity.Text == string.Empty || txtStockQuantity.Text == null)
            {
                MessageBox.Show("Please enter a quantity!", "Quantity?", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }
            else
            {
                return true;
            }
        }
        private void btnCancelStock_Click(object sender, RoutedEventArgs e)
        {
            //toggle the button to save
            btnAddStock.Content = "Add";
            //disable the listview
            lvStock.IsEnabled = true;
            //toggle the other buttons
            btnUpdateStock.IsEnabled = true;
            btnDeleteStock.IsEnabled = true;
            btnCancelStock.IsEnabled = false;
        }
        private void btnDeleteStock_Click(object sender, RoutedEventArgs e)
        {
            if (lvStock.SelectedItem != null)
            {
                string message = "Are you sure you want to delete? \n The stock's details will be permanently deleted!";
                string caption = "Delete stock?";

                if (MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    StockItem selectedStock;
                    selectedStock = (StockItem)lvStock.SelectedItem;

                    try
                    {
                        if (selectedStock.Delete() == 1)
                        {
                            MessageBox.Show("Stock's details successfully deleted!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadStockItems();
                            lvStock.SelectedIndex = 0;
                            lvStock.ScrollIntoView(lvStock.SelectedIndex);
                        }
                    }
                    catch (Exception ex)
                    {
                        message = $"Something went wrong! \n The stock's details could not be deleted. \n{ex.Message}";
                        MessageBox.Show(message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a practitioner to delete", "Select a practitioner", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        #endregion

        #region SelectionChanged
        private void lvStock_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
        }
        private void lvOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
        }
        #endregion

        #region sorting
        GridViewColumnHeader _lastHeaderClicked = null;
        ListSortDirection _lastDirection = ListSortDirection.Ascending;

        void GridViewColumnHeaderClickedHandler(object sender,
                                                RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }

                    var columnBinding = headerClicked.Column.DisplayMemberBinding as Binding;
                    var sortBy = columnBinding?.Path.Path ?? headerClicked.Column.Header as string;

                    Sort(sortBy, direction);

                    if (direction == ListSortDirection.Ascending)
                    {
                        headerClicked.Column.HeaderTemplate =
                          Resources["HeaderTemplateArrowUp"] as DataTemplate;
                    }
                    else
                    {
                        headerClicked.Column.HeaderTemplate =
                          Resources["HeaderTemplateArrowDown"] as DataTemplate;
                    }

                    // Remove arrow from previously sorted header
                    if (_lastHeaderClicked != null && _lastHeaderClicked != headerClicked)
                    {
                        _lastHeaderClicked.Column.HeaderTemplate = null;
                    }

                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }
        }
        private void Sort(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView =
              CollectionViewSource.GetDefaultView(lvOrders.ItemsSource);

            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }
        #endregion

        #region input validation
        private new void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"\d+");
            e.Handled = !regex.IsMatch(e.Text);
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsValid(((TextBox)sender).Text + e.Text);
        }
        private static bool IsValid(string str)
        {
            Decimal i;
            return Decimal.TryParse(str, out i) && i >= 0 && i <= 99999 && Decimal.Round(i, 2) == i;
        }
        #endregion
    }
}
