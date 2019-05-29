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
using System.Windows.Shapes;

namespace mappin
{
    /// <summary>
    /// Логика взаимодействия для FirstWindow.xaml
    /// </summary>
    public partial class FirstWindow : Window
    {
        public FirstWindow()
        {
            InitializeComponent();
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int x = Convert.ToInt32(Pins.Text);
                if (x > 0 && x <= 32)
                {
                    MainWindow mainWindow = new MainWindow(AmountPins, VCCPins, GNDPins);

                    mainWindow.Show();

                    this.Close();
                }
            }
            catch { }
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public string AmountPins
        {
            get { return Pins.Text; }
        }

        public string VCCPins
        {
            get { return VccPins.Text; }
        }

        public string GNDPins
        {
            get { return GndPins.Text; }
        }

    }
}
