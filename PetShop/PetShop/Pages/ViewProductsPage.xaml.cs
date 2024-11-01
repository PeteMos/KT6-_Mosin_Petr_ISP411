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

namespace PetShop.Pages
{
    /// <summary>
    /// Логика взаимодействия для ViewProductsPage.xaml
    /// </summary>
    public partial class ViewProductsPage : Page
    {
        public ViewProductsPage()
        {
            InitializeComponent();
            ProductListView.ItemsSource = Data.PetShopEntities.GetContext().Product.ToList();
            Init();
        }

        public void Init()
        {
            ProductListView.ItemsSource = Data.PetShopEntities.GetContext().Product.ToList();
            var manufactList = Data.PetShopEntities.GetContext().Manufacturer.ToList();
            manufactList.Insert(0, new Data.Manufacturer { Manufacturer1 = "Все производители" });
            ManufacturerComboBox.ItemsSource = manufactList;
            ManufacturerComboBox.SelectedIndex = 0;

            if (Classes.Manager.CurrentUser != null)
            {
                FIOLabel.Visibility = Visibility.Visible;
                FIOLabel.Content = $"{Classes.Manager.CurrentUser.UserSurname} " + $"{Classes.Manager.CurrentUser.UserName} " +
                    $"{Classes.Manager.CurrentUser.UserPatronymic}";
            }
            else
            {
                FIOLabel.Visibility = Visibility.Hidden;
            }

            CountOfLabel.Content = $"{Data.PetShopEntities.GetContext().Product.Count()}/" +
                $"{Data.PetShopEntities.GetContext().Product.Count()}";
        }

        public List<Data.Product> _currentProduct = Data.PetShopEntities.GetContext().Product.ToList();
        public void Update()
        {
            try
            {
                _currentProduct = Data.PetShopEntities.GetContext().Product.ToList();
                _currentProduct = (from item in Data.PetShopEntities.GetContext().Product
                                   where item.ProductName.Name.ToLower().Contains(SearchTextBox.Text) ||
                                   item.ProductDescription.ToLower().Contains(SearchTextBox.Text) ||
                                   item.Manufacturer.Manufacturer1.ToLower().Contains(SearchTextBox.Text) ||
                                   item.ProductCost.ToString().ToLower().Contains(SearchTextBox.Text) ||
                                   item.ProductQuantityInStock.ToString().ToLower().Contains(SearchTextBox.Text)
                                   select item).ToList();

                if (SortRadioButton.IsChecked == true)
                {
                    _currentProduct = _currentProduct.OrderBy(d => d.ProductCost).ToList();
                }
                if (SortDownRadioButton.IsChecked == true)
                {
                    _currentProduct = _currentProduct.OrderByDescending(m => m.ProductCost).ToList();
                }
                var selected = ManufacturerComboBox.SelectedItem as Data.Manufacturer;
                if (selected != null && selected.Manufacturer1 != "Все производители")
                {
                    _currentProduct = _currentProduct.Where(m => m.Id == selected.Id).ToList();

                }
                CountOfLabel.Content = $"{_currentProduct.Count}/{Data.PetShopEntities.GetContext().Product.Count()}";
                ProductListView.ItemsSource = _currentProduct;
            }
            catch (Exception)
            {

            }
        }
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Update();
        }

        private void SortRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Update();
        }

        private void SortDownRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Update();
        }

        private void ManufacturerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Update();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Classes.Manager.MainFrame.Navigate(new Pages.LoginPage());
        }
    }
}
