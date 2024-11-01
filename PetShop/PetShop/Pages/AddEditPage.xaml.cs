using PetShop.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
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
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        public string FlagAddorEdit = "default";
        public Data.Product _currentproduct = new Data.Product();
        public AddEditPage(Data.Product product)
        {
            InitializeComponent();

            if (product != null)
            {
                _currentproduct = product;
                FlagAddorEdit = "edit";
            }
            else
            {
                FlagAddorEdit = "add";
            }
            DataContext = _currentproduct;

            Init();
        }
        public void Init()
        {
            try
            {
                CategoryComboBox.ItemsSource = Data.PetShopEntities.GetContext().Category.ToList();
                if (FlagAddorEdit == "add")
                {
                    IdTextBox.Visibility = Visibility.Visible;
                    IdLabel.Visibility = Visibility.Hidden;

                    ProductImage.Source = new BitmapImage();
                    CategoryComboBox.SelectedItem = null;
                    CountTextBox.Text = string.Empty;
                    UnitTextBox.Text = string.Empty;
                    NameTextBox.Text = string.Empty;
                    CostTextBox.Text = string.Empty;
                    SupplierTextBox.Text = string.Empty;
                    DescriptionTextBox.Text = string.Empty;
                    IdTextBox.Text = Data.PetShopEntities.GetContext().Product.Max(d => d.Id + 1).ToString();
                }
                else if (FlagAddorEdit == "edit")
                {
                    IdTextBox.Visibility = Visibility.Visible;
                    IdLabel.Visibility = Visibility.Hidden;
                    CategoryComboBox.SelectedItem = null;
                    CountTextBox.Text = _currentproduct.ProductQuantityInStock.ToString();
                    UnitTextBox.Text = _currentproduct.Units.Unit;
                    NameTextBox.Text = _currentproduct.ProductName.Name;
                    CostTextBox.Text = _currentproduct.ProductCost.ToString();
                    SupplierTextBox.Text = _currentproduct.ProductName.Name;
                    DescriptionTextBox.Text = _currentproduct.ProductDescription;
                    IdTextBox.Text = Data.PetShopEntities.GetContext().Product.Max(p => p.Id + 1).ToString();
                    CategoryComboBox.SelectedItem = Data.PetShopEntities.GetContext().Category.Where(d => d.Id == _currentproduct.Id).FirstOrDefault();
                }
            }
            catch (Exception)
            {

            }
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder errors = new StringBuilder();
                if (string.IsNullOrEmpty(NameTextBox.Text))
                {
                    errors.AppendLine("Заполните наименование");
                }
                if (CategoryComboBox.SelectedItem == null)
                {
                    errors.AppendLine("Выберите категорию");
                }
                if (string.IsNullOrEmpty(CountTextBox.Text))
                {
                    errors.AppendLine("Заполните количество");
                }
                else
                {
                    var tryQuantity = Int32.TryParse(CountTextBox.Text, out var resultQuantity);
                    if (!tryQuantity)
                    {
                        errors.AppendLine("Количество - целое число");
                    }
                }
                if (string.IsNullOrEmpty(UnitTextBox.Text))
                {
                    errors.AppendLine("Заполните ед.измерения");
                }
                if (string.IsNullOrEmpty(SupplierTextBox.Text))
                {
                    errors.AppendLine("Заполните поставщика");
                }
                if (string.IsNullOrEmpty(CostTextBox.Text))
                {
                    errors.AppendLine("Заполните стоимость");
                }
                else
                {
                    var tryCost = Decimal.TryParse(CostTextBox.Text, out var resultCost);
                    if (!tryCost)
                    {
                        errors.AppendLine("Стоимость - дробное число");
                    }
                    else
                    {

                    }
                    if (tryCost && resultCost < 0)
                    {
                        errors.AppendLine("Стоимость не может быть отрицательной");
                    }
                }
                if (string.IsNullOrEmpty(DescriptionTextBox.Text))
                {
                    errors.AppendLine("Заполните описание");
                }

                if (errors.Length > 0)
                {
                    MessageBox.Show(errors.ToString(), "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var selectedCategory = CategoryComboBox.SelectedItem as Data.Category;
                _currentproduct.Id = Data.PetShopEntities.GetContext().Category.Where(p => p.Id == selectedCategory.Id).FirstOrDefault().Id;
                _currentproduct.ProductQuantityInStock = Convert.ToInt32(CountTextBox.Text);
                _currentproduct.ProductCost = Convert.ToDecimal(CostTextBox.Text);
                _currentproduct.ProductDescription = DescriptionTextBox.Text;
                
                var searchUnit = (from item in Data.PetShopEntities.GetContext().Units
                                  where item.Unit == UnitTextBox.Text
                                  select item).FirstOrDefault();
                if (searchUnit != null)
                {
                    _currentproduct.Id = searchUnit.Id;
                }
                else
                {
                    Data.Units units = new Data.Units()
                    {
                        Unit = UnitTextBox.Text
                    };
                    Data.PetShopEntities.GetContext().Units.Add(units);
                    Data.PetShopEntities.GetContext().SaveChanges();
                    _currentproduct.Id = units.Id;
                }
                
                if (errors.Length > 0)
                {
                    MessageBox.Show(errors.ToString(), "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var searchProductName = (from item in Data.PetShopEntities.GetContext().ProductName
                                         where item.Name == NameTextBox.Text
                                         select item).FirstOrDefault();
                if (searchProductName != null)
                {
                    _currentproduct.Id = searchProductName.Id;
                }
                else
                {
                    Data.ProductName productName = new Data.ProductName()
                    {
                        Name = NameTextBox.Text
                    };
                    Data.PetShopEntities.GetContext().ProductName.Add(productName);
                    Data.PetShopEntities.GetContext().SaveChanges();
                    _currentproduct.Id = productName.Id;
                }

                var searchSupplier = (from item in Data.PetShopEntities.GetContext().Supplier
                                      where item.Supplier1 == SupplierTextBox.Text
                                      select item).FirstOrDefault();

                if (searchSupplier != null)
                {
                    _currentproduct.Id = searchSupplier.Id;
                }
                else
                {
                    Data.Supplier supplier = new Data.Supplier()
                    {
                        Supplier1 = SupplierTextBox.Text
                    };
                    Data.PetShopEntities.GetContext().Supplier.Add(supplier);
                    Data.PetShopEntities.GetContext().SaveChanges();
                    _currentproduct.Id = supplier.Id;
                }
                if (FlagAddorEdit == "add")
                {
                    Data.PetShopEntities.GetContext().Product.Add(_currentproduct);
                    Data.PetShopEntities.GetContext().SaveChanges();
                    MessageBox.Show("Успешно добавлено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (FlagAddorEdit == "edit")
                {
                    Data.PetShopEntities.GetContext().SaveChanges();
                    MessageBox.Show("Успешно изменено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }


            catch (Exception)
            {
                
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Classes.Manager.MainFrame.Navigate(new AdminPage());
        }

        private void ProductImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }
    }
}
