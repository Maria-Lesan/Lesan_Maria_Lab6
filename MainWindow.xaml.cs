using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AutoLotModel;
using System.Data.Entity;
using System.Data;

namespace Lesan_Maria_Lab6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    enum ActionState
    {
        New,
        Edit,
        Delete,
        Nothing
    }
    public partial class MainWindow : Window
    {
        ActionState action = ActionState.Nothing;
        AutoLotEntitiesModel ctx = new AutoLotEntitiesModel();
        CollectionViewSource customerViewSource;
        Binding txtFirstNameBinding = new Binding();
        Binding txtLastNameBinding = new Binding();

        CollectionViewSource inventoryViewSource;
        CollectionViewSource customersOrderViewSource;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
           //txtFirstNameBinding.Path = new PropertyPath("FirstName");
           // txtLastNameBinding.Path = new PropertyPath("LastName");
           // firstNameTextBox.SetBinding(TextBox.TextProperty, txtFirstNameBinding); 
           // lastNameTextBox.SetBinding(TextBox.TextProperty, txtLastNameBinding);
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            customerViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            customerViewSource.Source = ctx.Customers.Local;

            customersOrderViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customersOrderViewSource")));
            customersOrderViewSource.Source = ctx.Order.Local;
            BindDataGrid();

            ctx.Customers.Load();

            ctx.Order.Load();
            ctx.Inventories.Load();
            cmbCustomers.ItemsSource = ctx.Customers.Local;
           
            //cmbCustomers.DisplayMemberPath = "FirstName";
            cmbCustomers.SelectedValuePath = "CustId";
            cmbInventory.ItemsSource = ctx.Inventories.Local;
            //cmbInventory.DisplayMemberPath = "Make";
            cmbInventory.SelectedValuePath = "CarId";




            inventoryViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("inventoryViewSource")));
            inventoryViewSource.Source = ctx.Inventories.Local;
            ctx.Inventories.Load();
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;

            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            customerDataGrid.IsEnabled =false;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;
            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;
            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
           firstNameTextBox.Text = "";
            lastNameTextBox.Text = "";
            Keyboard.Focus(firstNameTextBox);

        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            //string tempfirst = firstNameTextBox.Text.ToString();
            //string templast = lastNameTextBox.Text.ToString();

            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            customerDataGrid.IsEnabled = false;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;
            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;
            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            SetValidationBinding();
            ////firstNameTextBox.Text = tempfirst;
            ////lastNameTextBox.Text = templast;
            Keyboard.Focus(firstNameTextBox);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            //string tempfirst = firstNameTextBox.Text.ToString();
            //string templast = lastNameTextBox.Text.ToString();

            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            customerDataGrid.IsEnabled = false;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;
            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            //firstNameTextBox.Text = tempfirst;
            //lastNameTextBox.Text = templast;
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToPrevious();

        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToNext();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
         {
            Customers customer = null;
            if (action == ActionState.New)
            {
                try
                {
                    //instantiem Customer entity
                    customer = new Customers()
                    {
                        FirstName = firstNameTextBox.Text.Trim(),
                        LastName = lastNameTextBox.Text.Trim()
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Customers.Add(customer);
                    customerViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    // phoneNumbersDataSet.RejectChanges();
                    //  MessageBox.Show(ex.Message);
                    MessageBox.Show(ex.Message);
                }
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                customerDataGrid.IsEnabled = true;
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
            }

            else if (action == ActionState.Edit)
            {
                try
                {
                    customer = (Customers)customerDataGrid.SelectedItem;
                    customer.FirstName = firstNameTextBox.Text.Trim();
                    customer.LastName = lastNameTextBox.Text.Trim();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerViewSource.View.Refresh();
                // pozitionarea pe item-ul curent
                customerViewSource.View.MoveCurrentTo(customer);


                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                customerDataGrid.IsEnabled = true;
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;
               firstNameTextBox.IsEnabled = false;
               lastNameTextBox.IsEnabled = false;
               // firstNameTextBox.SetBinding(TextBox.TextProperty, txtFirstNameBinding);
               //lastNameTextBox.SetBinding(TextBox.TextProperty, txtLastNameBinding);
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    customer = (Customers)customerDataGrid.SelectedItem;
                    ctx.Customers.Remove(customer);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerViewSource.View.Refresh();

                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                customerDataGrid.IsEnabled = true;
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                 lastNameTextBox.IsEnabled = false;
                //firstNameTextBox.SetBinding(TextBox.TextProperty, txtFirstNameBinding);
                //lastNameTextBox.SetBinding(TextBox.TextProperty, txtLastNameBinding);
            }

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnNew.IsEnabled = true;
            btnEdit.IsEnabled = true;
            btnEdit.IsEnabled = true;
            btnSave.IsEnabled = false;
            btnCancel.IsEnabled = false;
            customerDataGrid.IsEnabled = true;

            btnPrev.IsEnabled = true;
            btnNext.IsEnabled = true;
            firstNameTextBox.IsEnabled = false;
            lastNameTextBox.IsEnabled = false;
            //firstNameTextBox.SetBinding(TextBox.TextProperty, txtFirstNameBinding);
            //lastNameTextBox.SetBinding(TextBox.TextProperty, txtLastNameBinding);

        }

        private void btnNewInv_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNewInv.IsEnabled = false;
            btnEditInv.IsEnabled = false;
            btnDeleteInv.IsEnabled = false;

            btnSaveInv.IsEnabled = true;
            btnCancelInv.IsEnabled = true;
            inventoryDataGrid.IsEnabled = false;
            btnPrevInv.IsEnabled = false;
            btnNextInv.IsEnabled = false;
            colorTextBox.IsEnabled = true;
            makeTextBox.IsEnabled = true;
            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
            colorTextBox.Text = "";
            makeTextBox.Text = "";
            Keyboard.Focus(colorTextBox);

        }

        private void btnEditInv_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            //string tempfirst = colorTextBox.Text.ToString();
            //string templast = makeTextBox.Text.ToString();

            btnNewInv.IsEnabled = false;
            btnEditInv.IsEnabled = false;
            btnDeleteInv.IsEnabled = false;
            btnSaveInv.IsEnabled = true;
            btnCancelInv.IsEnabled = true;
            inventoryDataGrid.IsEnabled = false;
            btnPrevInv.IsEnabled = false;
            btnNextInv.IsEnabled = false;
            colorTextBox.IsEnabled = true;
            makeTextBox.IsEnabled = true;
            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
            //colorTextBox.Text = tempfirst;
            //makeTextBox.Text = templast;
            Keyboard.Focus(colorTextBox);

        }

        private void btnDeleteInv_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            //string tempfirst = colorTextBox.Text.ToString();
            //string templast = makeTextBox.Text.ToString();

            btnNewInv.IsEnabled = false;
            btnEditInv.IsEnabled = false;
            btnDeleteInv.IsEnabled = false;
            btnSaveInv.IsEnabled = true;
            btnCancelInv.IsEnabled = true;
            inventoryDataGrid.IsEnabled = false;
            btnPrevInv.IsEnabled = false;
            btnNextInv.IsEnabled = false;
            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
            //colorTextBox.Text = tempfirst;
            //makeTextBox.Text = templast;
        }

        private void btnSaveInv_Click(object sender, RoutedEventArgs e)
        {
            Inventory inventory = null;
            if (action == ActionState.New)
            {
                try
                {
                    //instantiem Inventory entity
                    inventory = new Inventory()
                    {
                        Color =colorTextBox.Text.Trim(),
                        Make = makeTextBox.Text.Trim()
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Inventories.Add(inventory);
                    inventoryViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    
                    MessageBox.Show(ex.Message);
                }
                btnNewInv.IsEnabled = true;
                btnEditInv.IsEnabled = true;
                btnSaveInv.IsEnabled = false;
                btnCancelInv.IsEnabled = false;
                inventoryDataGrid.IsEnabled = true;
                btnPrevInv.IsEnabled = true;
                btnNextInv.IsEnabled = true;
                colorTextBox.IsEnabled = false;
                makeTextBox.IsEnabled = false;
            }

            else if (action == ActionState.Edit)
            {
                try
                {
                    
                    inventory = (Inventory)inventoryDataGrid.SelectedItem;
                    inventory.Color = colorTextBox.Text.Trim();
                    inventory.Make = makeTextBox.Text.Trim();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
               inventoryViewSource.View.Refresh();
                // pozitionarea pe item-ul curent
                inventoryViewSource.View.MoveCurrentTo(inventory);


                btnNewInv.IsEnabled = true;
                btnEditInv.IsEnabled = true;
                btnDeleteInv.IsEnabled = true;
                btnSaveInv.IsEnabled = false;
                btnCancelInv.IsEnabled = false;
                inventoryDataGrid.IsEnabled = true;
                btnPrevInv.IsEnabled = true;
                btnNextInv.IsEnabled = true;
                colorTextBox.IsEnabled = false;
               makeTextBox.IsEnabled = false;
            
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    inventory = (Inventory)inventoryDataGrid.SelectedItem;
                    ctx.Inventories.Remove(inventory);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                inventoryViewSource.View.Refresh();

                btnNewInv.IsEnabled = true;
                btnEditInv.IsEnabled = true;
                btnDeleteInv.IsEnabled = true;
                btnSaveInv.IsEnabled = false;
                btnCancelInv.IsEnabled = false;
                inventoryDataGrid.IsEnabled = true;
                btnPrevInv.IsEnabled = true;
                btnNextInv.IsEnabled = true;
                colorTextBox.IsEnabled = false;
                makeTextBox.IsEnabled = false;
               // colorTextBox.SetBinding(TextBox.TextProperty,txtColorBinding);
               //makeTextBox.SetBinding(TextBox.TextProperty, txtMakeBinding);
            }

        }

      

        private void btnCancelInv_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnNewInv.IsEnabled = true;
            btnEditInv.IsEnabled = true;
           
            btnSaveInv.IsEnabled = false;
            btnCancelInv.IsEnabled = false;
            inventoryDataGrid.IsEnabled = true;

            btnPrevInv.IsEnabled = true;
            btnNextInv.IsEnabled = true;
           colorTextBox.IsEnabled = false;
            makeTextBox.IsEnabled = false;
            //colorTextBox.SetBinding(TextBox.TextProperty, txtColorBinding);
            //makeTextBox.SetBinding(TextBox.TextProperty, txtMakeBinding);

        }

        private void btnPrevInv_Click(object sender, RoutedEventArgs e)
        {
            inventoryViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNextInv_Click(object sender, RoutedEventArgs e)
        {
            inventoryViewSource.View.MoveCurrentToNext();

        }

        private void btnNewOrd_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNewOrd.IsEnabled = false;
            btnEditOrd.IsEnabled = false;
            btnDeleteOrd.IsEnabled = false;

            btnSaveOrd.IsEnabled = true;
            btnCancelOrd.IsEnabled = true;
            btnNextOrd.IsEnabled = false;
            btnPrevOrd.IsEnabled = false;
        }

        private void btnEditOrd_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);

            btnNewOrd.IsEnabled = false;
            btnEditOrd.IsEnabled = false;
            btnDeleteOrd.IsEnabled = false;

            btnSaveOrd.IsEnabled = true;
            btnCancelOrd.IsEnabled = true;
            btnPrevOrd.IsEnabled = false;
            btnNextOrd.IsEnabled = false;

        }

        private void btnDeleteOrd_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;

            btnNewOrd.IsEnabled = false;
            btnEditOrd.IsEnabled = false;
            btnDeleteOrd.IsEnabled = false;

            btnSaveOrd.IsEnabled = true;
            btnCancelOrd.IsEnabled = true;
            btnPrevOrd.IsEnabled = false;
            btnNextOrd.IsEnabled = false;

            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);

        }

        private void btnSaveOrd_CLick(object sender, RoutedEventArgs e)
        {
            Order order = null;
            if (action == ActionState.New)
            {
                try
                {
                    Customers customer = (Customers)cmbCustomers.SelectedItem;
                    Inventory inventory = (Inventory)cmbInventory.SelectedItem;
                    order = new Order()
                    {
                        CustId = customer.CustId,
                        CarId = inventory.CarId
                    };
                    ctx.Order.Add(order);
                    customersOrderViewSource.View.Refresh();
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNewOrd.IsEnabled = true;
                btnEditOrd.IsEnabled = true;
                btnSaveOrd.IsEnabled = false;
                btnCancelOrd.IsEnabled = false;
                btnPrevOrd.IsEnabled = true;
                btnNextOrd.IsEnabled = true;
            }
            else
                if (action == ActionState.Edit)
            {
                dynamic selectedOrder = orderDataGrid.SelectedItem;
                try
                {
                    int curr_id = selectedOrder.OrderId;
                    var editedOrder = ctx.Order.FirstOrDefault(s => s.OrderId == curr_id);
                    if (editedOrder != null)
                    {
                        editedOrder.CustId = Int32.Parse(cmbCustomers.SelectedValue.ToString());
                        editedOrder.CarId = Convert.ToInt32(cmbInventory.SelectedValue.ToString());
                        ctx.SaveChanges();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                BindDataGrid();
                customerViewSource.View.MoveCurrentTo(selectedOrder);
                SetValidationBinding();
            }
            else
                if (action == ActionState.Delete)
            {
                try
                {
                    dynamic selectedOrder = orderDataGrid.SelectedItem;
                    int curr_id = selectedOrder.OrderId;
                    var deletedOrder = ctx.Order.FirstOrDefault(s => s.OrderId == curr_id);
                    if (deletedOrder != null)
                    {
                        ctx.Order.Remove(deletedOrder);
                        ctx.SaveChanges();
                        MessageBox.Show("Order Deleted Successfully", "Message");
                        BindDataGrid();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnCancelOrd_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;

            btnNewOrd.IsEnabled = true;
            btnEditOrd.IsEnabled = true;
            btnEditOrd.IsEnabled = true;

            btnSaveOrd.IsEnabled = false;
            btnCancelOrd.IsEnabled = false;
            btnPrevOrd.IsEnabled = true;
            btnNextOrd.IsEnabled = true;


        }

        private void btnPrevOrd_Click(object sender, RoutedEventArgs e)
        {
            customersOrderViewSource.View.MoveCurrentToPrevious();

        }

        private void btnNextOrd_Click(object sender, RoutedEventArgs e)
        {
            customersOrderViewSource.View.MoveCurrentToNext();
        }

        private void BindDataGrid()
        {
            var queryOrder = from ord in ctx.Order
                             join cust in ctx.Customers on ord.CustId equals cust.CustId
                             join inv in ctx.Inventories on ord.CarId equals inv.CarId
                             select new
                             {
                                 ord.OrderId,
                                 ord.CarId,
                                 ord.CustId,
                                 cust.FirstName,
                                 cust.LastName,
                                 inv.Make,
                                 inv.Color
                             };
            customersOrderViewSource.Source = queryOrder.ToList();
        }

        private void SetValidationBinding()
        {
            Binding firstNameValidationBinding = new Binding();
            firstNameValidationBinding.Source = customerViewSource;
            firstNameValidationBinding.Path = new PropertyPath("FirstName");
            firstNameValidationBinding.NotifyOnValidationError = true;
            firstNameValidationBinding.Mode = BindingMode.TwoWay;
            firstNameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //string required
            firstNameValidationBinding.ValidationRules.Add(new StringNotEmpty());
            firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameValidationBinding);
            Binding lastNameValidationBinding = new Binding();
            lastNameValidationBinding.Source = customerViewSource;
            lastNameValidationBinding.Path = new PropertyPath("LastName");
            lastNameValidationBinding.NotifyOnValidationError = true;
            lastNameValidationBinding.Mode = BindingMode.TwoWay;
            lastNameValidationBinding.UpdateSourceTrigger =
           UpdateSourceTrigger.PropertyChanged;
            lastNameValidationBinding.ValidationRules.Add(new StringMinLengthValidator());
            lastNameTextBox.SetBinding(TextBox.TextProperty,
           lastNameValidationBinding);
        }
    }
}
