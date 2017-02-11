using Client.GoodsContract;
using Client.OrderContract;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<BusinessOrderItem> Cart;
        ObservableCollection<BusinessGood> Goods;
        ObservableCollection<BusinessOrder> Orders;
        bool textChangedLock;

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
            Initialize();  
        }

        private void Initialize()
        {
            Cart = new ObservableCollection<BusinessOrderItem>();
            Goods = new ObservableCollection<BusinessGood>();
            Orders = new ObservableCollection<BusinessOrder>();
            textChangedLock = false;

            RefreshGoods();
            lstGoods.ItemsSource = Goods;
            lstCart.ItemsSource = Cart;
            RefreshOrders();
            lstOrders.ItemsSource = Orders;


            sumLabel.Content = string.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", CartSum);
        }

        private void RefreshGoods()
        {
            Goods.Clear();
            GoodsContractClient client = new GoodsContractClient();
            try
            {
                foreach (var item in client.GetGoods())
                {
                    Goods.Add(item);
                }
            }
            catch (FaultException fexc)
            {
                MessageBox.Show(string.Format("FaultException {0} Type {1}", fexc.Message));
                return;
            }
            lstGoods.SelectedIndex = -1;
        }

        private void RefreshOrders()
        {
            Orders.Clear();
            OrderContractClient client = new OrderContractClient();
            try
            {
                foreach (var item in client.GetOrders())
                {
                    Orders.Add(item);
                }
            }
            catch (FaultException fexc)
            {
                MessageBox.Show(string.Format("FaultException {0} Type {1}", fexc.Message));
            }
        }
        
        private void RefreshOrderItemSum(BusinessOrderItem item)
        {
            item.OrderItemSum = Goods.Where(p => p.GoodId == item.GoodId).FirstOrDefault().Price * item.GoodCount;
        }

        private void AddToCart(object sender, RoutedEventArgs e)
        {
            if (lstGoods.SelectedIndex < 0)
                return;
            BusinessOrderItem orderItem = new BusinessOrderItem();
            orderItem.GoodId = (lstGoods.SelectedItem as BusinessGood).GoodId;
            orderItem.GoodName = (lstGoods.SelectedItem as BusinessGood).GoodName;
            orderItem.GoodCount = 1;
            RefreshOrderItemSum(orderItem);
            Cart.Add(orderItem);
            sumLabel.Content = string.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", CartSum);
        }

        private void IncreaseItemsCount(object sender, RoutedEventArgs e)
        {
            dynamic tmp = sender;
            if (tmp.TemplatedParent.Content.GoodCount < Goods.Where(p => p.GoodId == tmp.TemplatedParent.Content.GoodId).FirstOrDefault().GoodCount)
            {
                tmp.TemplatedParent.Content.GoodCount++;
                RefreshOrderItemSum(tmp.TemplatedParent.Content);
                sumLabel.Content = string.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", CartSum);
            }
        }

        private void DecreaseItemsCount(object sender, RoutedEventArgs e)
        {
            dynamic tmp = sender;
            if (tmp.TemplatedParent.Content.GoodCount > 1)
            {
                tmp.TemplatedParent.Content.GoodCount--;
                RefreshOrderItemSum(tmp.TemplatedParent.Content);
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
            OrderContractClient client = new OrderContractClient();
            BusinessOrderItem[] arr = Cart.ToArray();
            string result = client.AddOrder(arr);
            if (result != null)
                MessageBox.Show("Error: " + result);
            else
            {
                Cart.Clear();
                RefreshGoods();
                RefreshOrders();
            }
        }

        private void lstGoods_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstGoods.SelectedIndex == -1)
            {
                imageInfo.Source = null;
                stpInfo.Visibility = Visibility.Hidden;
                return;
            }
            ImageSource imageSource = new BitmapImage(new Uri(Environment.CurrentDirectory + @"\" + (lstGoods.SelectedItem as BusinessGood).Photo));
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
            else if (Convert.ToInt32(tmp.Text) > Goods.Where(p => p.GoodId == tmp.TemplatedParent.Content.GoodId).FirstOrDefault().GoodCount)
            {
                (sender as TextBox).Text = Convert.ToInt32(Goods.Where(p => p.GoodId == tmp.TemplatedParent.Content.GoodId).FirstOrDefault().GoodCount).ToString();
                (sender as TextBox).SelectAll();
            }
            tmp.TemplatedParent.Content.GoodCount = Convert.ToInt32(tmp.Text);
            RefreshOrderItemSum(tmp.TemplatedParent.Content);
            textChangedLock = false;
        }
    }
}
