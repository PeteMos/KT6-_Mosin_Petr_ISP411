using PetShop.Pages;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PetShop.Pages
{
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }
      
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder errors = new StringBuilder();
                if (string.IsNullOrEmpty(LoginTextBox.Text))
                {
                    errors.AppendLine("Заполните логин!");
                }
                if (string.IsNullOrEmpty(PasswordBox.Password))
                {
                    errors.AppendLine("Заполните пароль!");
                }

                if (errors.Length > 0)
                {
                    MessageBox.Show(errors.ToString(), "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (Data.PetShopEntities.GetContext().User
                    .Any(d => d.UserLogin == LoginTextBox.Text && d.UserPassword == PasswordBox.Password))
                {
                    var user = Data.PetShopEntities.GetContext().User
                        .FirstOrDefault(d => d.UserLogin == LoginTextBox.Text && d.UserPassword == PasswordBox.Password);

                    Classes.Manager.CurrentUser = user;

                    switch (user.Role.RoleName)
                    {
                        case "Администратор":
                            Classes.Manager.MainFrame.Navigate(new AdminPage());
                            break;
                        case "Клиент":
                            Classes.Manager.MainFrame.Navigate(new ViewProductsPage());
                            break;
                        case "Менеджер":
                            Classes.Manager.MainFrame.Navigate(new ViewProductsPage());
                            break;
                    }

                    MessageBox.Show("Добро пожаловать!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {                
                    MessageBox.Show("Некорректный логин или пароль!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GuestButton_Click(object sender, RoutedEventArgs e)
        {
            Classes.Manager.MainFrame.Navigate(new ViewProductsPage());
        }

    }
}
