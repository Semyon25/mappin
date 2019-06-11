using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

        private void Accept_Click(object sender, RoutedEventArgs e) // ОК
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
                else
                {
                    popup2.IsOpen = true;
                }
            }
            catch { }
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Open_Click(object sender, RoutedEventArgs e) // Открыть
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.CurrentDirectory;
            if (openFileDialog.ShowDialog() == true)
            {
                using (BinaryReader reader = new BinaryReader(File.Open(openFileDialog.FileName, FileMode.Open)))
                {
                    try
                    {
                        string ap = reader.ReadString();
                        string vc = reader.ReadString();
                        string gn = reader.ReadString();
                        while (reader.PeekChar() > -1)
                        {
                            var tmp = new ClassForTable(reader.ReadString(), reader.ReadString());
                            table.Add(tmp);
                        }
                        MainWindow mainWindow = new MainWindow(ap, vc, gn, table);
                        mainWindow.Show();

                        this.Close();

                    }
                    catch
                    {
                        MessageBox.Show("Некорректный файл!");
                        return;
                    }

                }
            }
        }

        // Переменные и свойства

        private ObservableCollection<ClassForTable> table = new ObservableCollection<ClassForTable>();

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
