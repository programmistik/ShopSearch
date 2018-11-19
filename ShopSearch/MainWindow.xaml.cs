using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace ShopSearch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ShopEntities context { get; set; } = new ShopEntities();
        public CollectionViewSource productViewSource;
       

        public MainWindow()
        {
            InitializeComponent();

        }

        private void Search()
        {
            var tab = context.Products.Local.Where(p => p.ProductName.ToUpper().Contains(tbProductName.Text.ToUpper()));
            productViewSource = ((CollectionViewSource)(FindResource("productViewSource")));

            if (!String.IsNullOrEmpty(tbPrice.Text))
            {
                CultureInfo culture = new CultureInfo("en-US");
                decimal result = Convert.ToDecimal(tbPrice.Text, culture);

                if (PriceCB.SelectedIndex == 0)
                    tab = tab.Where(pr => pr.UnitPrice == result);
                else if (PriceCB.SelectedIndex == 1)
                    tab = tab.Where(pr => pr.UnitPrice > result);
                else if (PriceCB.SelectedIndex == 2)
                    tab = tab.Where(pr => pr.UnitPrice < result);
                else if (PriceCB.SelectedIndex == 3)
                    tab = tab.Where(pr => pr.UnitPrice >= result);
                else if (PriceCB.SelectedIndex == 4)
                    tab = tab.Where(pr => pr.UnitPrice <= result);
            }

            if (chbDiscount.IsChecked != null)
            {
                if (chbDiscount.IsChecked == true)
                    tab = tab.Where(p => p.IsDiscontinued == true);
                else
                    tab = tab.Where(p => p.IsDiscontinued == false);
            }
            productViewSource.Source = tab.ToList();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            Search();

            tbProductName.Text = "";
            tbPrice.Text = "";
            PriceCB.SelectedIndex = 0;
            chbDiscount.IsChecked = null;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            CollectionViewSource productViewSource = ((CollectionViewSource)(FindResource("productViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // productViewSource.Source = [generic data source]
            context.Products.Load();
            productViewSource.Source = context.Products.Local.ToBindingList();
            PriceCB.SelectedIndex = 0;
            chbDiscount.IsChecked = null;
        }

        private void tbPrice_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^-?[0-9][0-9,\.]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            context = new ShopEntities();
            productViewSource = ((CollectionViewSource)(FindResource("productViewSource")));
            context.Products.Load();
            productViewSource.Source = context.Products.Local.ToBindingList();

        }

        private void tbProductName_KeyUp(object sender, KeyEventArgs e)
        {
            Search();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                context.SaveChanges();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
    }
}
