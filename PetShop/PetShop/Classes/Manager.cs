using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PetShop.Classes
{
    public static class Manager
    {
        public static Frame MainFrame { get; set; }

        public static Data.User CurrentUser { get; set; }

        public static void GetImageData()
        {
            try
            {
                var list = Data.PetShopEntities.GetContext().Product.ToList();

                foreach (var item in list)
                {
                    string path = Directory.GetCurrentDirectory() + @"\img\" + item.PhotoName;
                    if (File.Exists(path))
                    {
                        item.ProductPhoto = File.ReadAllBytes(path);
                    }

                }
                Data.PetShopEntities.GetContext().SaveChanges();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
