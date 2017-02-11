using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using ShopClient.GoodsContract;
using ShopClient.OrderContract;

namespace Shop
{
    public partial class MainWindow : Window
    {
        //ShopContext context = new ShopContext();
        //ObservableCollection<OrderItem> Cart = new ObservableCollection<OrderItem>();
        ObservableCollection<BusinessOrderItem> Cart = new ObservableCollection<BusinessOrderItem>();

        bool textChangedLock = false;

        decimal CartSum
        {
            get
            {
                decimal sum = 0;
                foreach (var item in Cart)
                {
                    sum += item.OrderItemSum;
                }
                return sum;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            context.Goods.Include(g => g.Category).Include(g => g.Manufacturer).Load();
            lstGoods.ItemsSource = context.Goods.Local;
            lstCart.ItemsSource = Cart;
            context.Orders.Include(o => o.OrderItems).Load();
            lstOrders.ItemsSource = context.Orders.Local;
            sumLabel.Content = string.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", CartSum);
        }

        private void AddToCart(object sender, RoutedEventArgs e)
        {
            if (lstGoods.SelectedIndex < 0)
                return;
            OrderItem orderItem = new OrderItem();
            orderItem.Good = lstGoods.SelectedItem as Good;
            orderItem.GoodCount = 1;
            Cart.Add(orderItem);
            sumLabel.Content = string.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", CartSum);
        }

        private void IncreaseItemsCount(object sender, RoutedEventArgs e)
        {
            dynamic tmp = sender;
            if (tmp.TemplatedParent.Content.GoodCount < context.Goods.Local.Where(p => p.GoodId == tmp.TemplatedParent.Content.Good.GoodId).FirstOrDefault().GoodCount)
            {
                tmp.TemplatedParent.Content.GoodCount++;
                sumLabel.Content = string.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", CartSum);
            }
        }

        private void DecreaseItemsCount(object sender, RoutedEventArgs e)
        {
            dynamic tmp = sender;
            if (tmp.TemplatedParent.Content.GoodCount > 1)
            {
                tmp.TemplatedParent.Content.GoodCount--;
                sumLabel.Content = string.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", CartSum);
            }
        }

        private void DeleteFromCart(object sender, RoutedEventArgs e)
        {
            dynamic tmp = sender;
            Cart.Remove(tmp.TemplatedParent.Content);
            sumLabel.Content = string.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", CartSum);
        }

        private void BuyFromCart(object sender, RoutedEventArgs e)
        {
            Order newOrder = new Order();
            newOrder.OrderDate = DateTime.Now;
            newOrder.OrderSum = CartSum;
            var tran = context.Database.BeginTransaction();
            try
            {
                context.Orders.Add(newOrder);
                context.SaveChanges();
                foreach (var item in Cart)
                {
                    item.OrderId = Convert.ToInt32(newOrder.OrderId);
                    context.OrderItems.Add(item);
                }
                context.SaveChanges();
                Cart.Clear();
                tran.Commit();
                sumLabel.Content = string.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", CartSum);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
                tran.Rollback();
            }
        }

        private void lstGoods_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ImageSource imageSource = new BitmapImage(new Uri(Environment.CurrentDirectory + @"\" + (lstGoods.SelectedItem as Good).Photo));
            imageInfo.Source = imageSource;
            stpInfo.Visibility = Visibility.Visible;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textChangedLock == true)
                return;
            Regex reg = new Regex("^[0-9]+$");
            dynamic tmp = sender;
            textChangedLock = true;
            if (reg.IsMatch((sender as TextBox).Text) != true || Convert.ToInt32(tmp.Text) == 0)
            {
                (sender as TextBox).Text = "1";
                (sender as TextBox).SelectAll();
            }
            else if (Convert.ToInt32(tmp.Text) > context.Goods.Local.Where(p => p.GoodId == tmp.TemplatedParent.Content.Good.GoodId).FirstOrDefault().GoodCount)
            {
                (sender as TextBox).Text = Convert.ToInt32(context.Goods.Local.Where(p => p.GoodId == tmp.TemplatedParent.Content.Good.GoodId).FirstOrDefault().GoodCount).ToString();
                (sender as TextBox).SelectAll();
            }
            tmp.TemplatedParent.Content.GoodCount = Convert.ToInt32(tmp.Text);
            textChangedLock = false;
        }
    }
}
