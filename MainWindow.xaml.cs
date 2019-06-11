using AutoIt;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace mappin
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow() { }
        public MainWindow(string amount, string vcc, string gnd)
        {
            InitializeComponent();
            DataContext = this;
            DialogShow(amount, vcc, gnd);
            paintFirst();
            CreatingTable();
        }
        public MainWindow(string amount, string vcc, string gnd, ObservableCollection<ClassForTable> t) : this(amount, vcc, gnd)
        {
            Table = t;
            if (!flag)
            {
                timer.Tick += new EventHandler(timerTick);
                timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
                timer.Start();
            }
        }
        public void init(string amount, string vcc, string gnd)
        {
            LeftBorders.Clear();
            RightBorders.Clear();
            Pairs.Clear();
            DialogShow(amount, vcc, gnd);
            paintFirst();
        }

        // Переменные
        const double radius = 12;
        const double margin = 1;
        int colorCounter = 0;
        List<int> Vcc = new List<int>();
        List<int> Gnd = new List<int>();
        private List<DockPanel> LeftBorders = new List<DockPanel>();
        private List<DockPanel> RightBorders = new List<DockPanel>();
        Node node = new Node();
        List<SolidColorBrush> brushes = new List<SolidColorBrush>() { Brushes.Magenta, Brushes.Orange, Brushes.Green, Brushes.LimeGreen, Brushes.Aqua, Brushes.DodgerBlue, Brushes.SlateBlue, Brushes.Violet, Brushes.Chocolate, Brushes.Gray };
        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
        bool flag = false;

        // Свойства

        int AmountPins { get; set; }

        private ObservableCollection<ClassForTable> table = new ObservableCollection<ClassForTable>();
        public ObservableCollection<ClassForTable> Table
        {
            get { return table; }
            set { table = value; OnPropertyChanged(); }
        }

        List<Pair> pairs = new List<Pair>();
        public List<Pair> Pairs
        {
            get { return pairs; }
        }


        #region Первоначальная отрисовка
        private void DialogShow(string amount, string vcc, string gnd) // Обработка значений, полученных из первого окна
        {
                AmountPins = Convert.ToInt32(amount);
                Vcc.Clear();
                Gnd.Clear();
                string[] VccPins = vcc.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string[] GndPins = gnd.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var c in VccPins)
                {
                    Vcc.Add(Convert.ToInt32(c));
                }
                foreach (var c in GndPins)
                {
                    Gnd.Add(Convert.ToInt32(c));
                }
        }

        private void paintFirst()
        {
            for (int i = 1; i<=16; ++i) // отрисовка левого столбца
            {
                PaintArea1.Children.Add(createLabels(i, i));
                PaintArea2.Children.Add(createLabels(i+16, i));
            }

            for(int i=1; i<= (AmountPins>16 ? 16 : AmountPins); ++i) // правый столбец
            {
                var b = createLabels(i + 32, i, true);
                PaintArea3.Children.Add(b);
            }
            for (int i = 17; i <= AmountPins; ++i)
            {
                var b = createLabels(i + 32, i, true);
                PaintArea4.Children.Add(b);
            }
        }

        void printLink()
        {
            for (int i=1; i<=Table.Count; ++i)
            {
                if ((Table[i - 1].FirstRow != "GND" && Table[i - 1].FirstRow != "VCC" && Table[i - 1].FirstRow != ""))
                {
                    char board = Table[i - 1].FirstRow[0];
                    int num = Convert.ToInt32(Table[i - 1].FirstRow.Remove(0, 1));
                    if (board == 'B') num += 16;
                    LabelEvent(LeftBorders[num - 1], null);
                    num = Convert.ToInt32(Table[i - 1].SecondRow);
                    LabelEvent(RightBorders[num - 1], null);
                }
            }
        }

        private DockPanel createLabels(int i, int textLabel, bool isRight = false)
        {
            DockPanel dock = new DockPanel();
            Border b = new Border();
            b.Height = 2 * radius;
            b.Width = 2 * radius;
            b.CornerRadius = new CornerRadius(radius);
            b.Margin = new Thickness(margin);
            b.BorderBrush = Brushes.Black;
            b.BorderThickness = new Thickness(3);
            b.Background = Brushes.Transparent;
            b.Uid = i.ToString();
            if (!(isRight && (Vcc.Find(x => x == textLabel) != 0 || Gnd.Find(x => x == textLabel) != 0)))
                dock.MouseDown += LabelEvent;
            Label l = new Label();
            l.Content = textLabel.ToString();
            l.Padding = new Thickness(0);
            l.VerticalContentAlignment = VerticalAlignment.Center;
            l.HorizontalContentAlignment = HorizontalAlignment.Center;
            b.Child = l;
            dock.Children.Add(b);
            if (!isRight)
            {
                if (i <= 16) LeftBorders.Insert(LeftBorders.Count/2, dock);
                else LeftBorders.Add(dock);
            }
            if (isRight)
            {
                RightBorders.Add(dock);

                Label l1 = new Label();
                l1.VerticalContentAlignment = VerticalAlignment.Center;
                l1.HorizontalContentAlignment = HorizontalAlignment.Center;

                if (Vcc.Find(x => x == textLabel) != 0)
                {
                    b.Background = Brushes.Red;
                    l1.Content = "VCC";
                }
                else if (Gnd.Find(x => x == textLabel) != 0)
                {
                    b.Background = Brushes.RoyalBlue;
                    l1.Content = "GND";
                }
                dock.Children.Add(l1);
            }
            return dock;
        }

        private void CreatingTable()
        {
            for (int i = 0; i < AmountPins; ++i) // для Table
            {
                string c = "";
                if (Vcc.Find(x => x == i + 1) != 0) c = "VCC";
                if (Gnd.Find(x => x == i + 1) != 0) c = "GND";
                Table.Add(new ClassForTable(c, (i + 1).ToString()));
            }
        }
        #endregion

        #region Событие при нажатии на label
        private void LabelEvent(object sender, MouseButtonEventArgs e)
        {
            DockPanel dock = sender as DockPanel;
            Border border = dock.Children[0] as Border;
            Point relativePoint = dock.TransformToVisual(Field).Transform(new Point(0, 0));
            if (SearchBorder(border)) // удаляем линию
            {
                DeleteLine(border);
            }
            else if (border == node.border) // сброс
            {
                node.border.Background = Brushes.Transparent;
                node = new Node();
                return;
            }
            else if (!node.isUsed) // добавляем первую точку
            {
                border.Background = brushes[colorCounter];
                node = new Node(border, relativePoint);
            }
            else if (node.isUsed && relativePoint.X == node.point.X) // сброс и добавление
            {
                border.Background = brushes[colorCounter];
                node.border.Background = Brushes.Transparent;
                node = new Node(border, relativePoint);
                return;
            }
            else if (node.isUsed && relativePoint.X != node.point.X) // рисуем линию
            {
                border.Background = brushes[colorCounter];
                Node node2 = new Node(border, relativePoint);
                pairs.Add(new Pair(node.isLeft ? node : node2, node.isLeft ? node2 : node, new Polyline()));
                createLine();
                node = new Node();
            }
        }

        private void createLine(bool isAdd = true)
        {
            int tmp = pairs[pairs.Count - 1].leftNode.number;
            Table[pairs[pairs.Count - 1].rightNode.number-1].FirstRow = (tmp > 16 ? "B" + (tmp-16).ToString() : "A" + (tmp).ToString());
            Point p1 = pairs[pairs.Count - 1].leftNode.point;
            Point p2 = pairs[pairs.Count - 1].rightNode.point;
            CorrectPoint(ref p1, ref p2);
            double CornerPosition = pairs[pairs.Count - 1].leftNode.number * (p2.X - p1.X) / 33 + p1.X;
            pairs[pairs.Count - 1].polyline.Points.Add(p1);
            pairs[pairs.Count - 1].polyline.Points.Add(new Point(CornerPosition, p1.Y));
            pairs[pairs.Count - 1].polyline.Points.Add(new Point(CornerPosition, p2.Y));
            pairs[pairs.Count - 1].polyline.Points.Add(p2);
            pairs[pairs.Count - 1].polyline.Stroke = isAdd ? brushes[colorCounter] : Brushes.Transparent;
            colorCounter = ++colorCounter % (brushes.Count);
            pairs[pairs.Count - 1].polyline.StrokeThickness = 2;
            pairs[pairs.Count - 1].polyline.StrokeThickness = 2;
            Field.Children.Add(pairs[pairs.Count - 1].polyline);
        }

        private void CorrectPoint(ref Point p1, ref Point p2)
        {
            p1.X += 2 * radius;
            p1.Y += radius;
            p2.Y += radius;
        }

        private bool SearchBorder(Border border)
        {
            foreach(Pair p in pairs)
            {
                if (p.leftNode.border == border || p.rightNode.border == border) return true;
            }
            return false;
        }

        private void DeleteLine(Border border)
        {
            Pair deletePair = null;
            foreach (Pair p in pairs)
            {
                if (p.leftNode.border == border || p.rightNode.border == border)
                {
                    deletePair = p;
                }
            }
            Table[deletePair.rightNode.number - 1].FirstRow = "";
            deletePair.leftNode.border.Background = Brushes.Transparent;
            deletePair.rightNode.border.Background = Brushes.Transparent;
            Field.Children.Remove(deletePair.polyline);
            pairs.Remove(deletePair);
        }
        #endregion

        #region Конвертирование
        string result;
        string filename;
        private void ChooseFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog() { Filter = "Текстовые файлы(*.txt)|*.txt" };
            if (openFileDialog1.ShowDialog() == true) filename = openFileDialog1.FileName;
            else return;
            result = string.Empty;
            bool err1 = false;
            bool err2 = false;
            using (StreamReader sr = new StreamReader(filename, System.Text.Encoding.Default))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    result += ConvertString(line, ref err1, ref err2);
                }
            }
            filename = filename.Remove(filename.Length-4);
            filename += "-Convert.pat";
            using (StreamWriter sw = new StreamWriter(filename, false, System.Text.Encoding.Default))
            {
                sw.WriteLine(result);
            }
            string msg = string.Empty;
            CreatePatFile(ref msg);
            ErrorMessages(err1, err2, msg);
        }

        private string ConvertString(string line, ref bool moreSymbolPattern, ref bool err)
        {
            const int firstWords = 3;
            line = line.Trim(';');
            List<string> words = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Cast<string>().ToList();
            List<int> specialPins = new List<int>();
            specialPins.AddRange(Gnd);
            specialPins.AddRange(Vcc);
            specialPins.Sort();
            foreach (var c in specialPins)
            {
                if (words.Count - firstWords >= c) words.Insert(c + firstWords - 1, "SPEC");
            }
            List<bool> rightList = new List<bool>(words.Count - firstWords);
            for (int i = 0; i < rightList.Capacity; ++i) rightList.Add(false);
            string result = string.Empty;
            for (int i = 0; i < firstWords; ++i) result += words[i] + " ";

            for (int i = 1; i <= 32; ++i)
            {
                bool f = false;
                foreach (var p in pairs)
                {
                    if (p.leftNode.number == i)
                    {
                        if (p.rightNode.number + firstWords - 1 < words.Count && words[p.rightNode.number + firstWords - 1] != "SPEC")
                        {
                            rightList[p.rightNode.number - 1] = true;
                            result += words[p.rightNode.number + firstWords - 1];
                            f = true;
                            break;
                        }
                        if (p.rightNode.number + firstWords - 1 >= words.Count) { moreSymbolPattern = true; break; }
                        if (words[p.rightNode.number + firstWords - 1] == "SPEC") { break; }
                    }
                }
                if (!f) result += "X";
            }
            result += ";" + Environment.NewLine;

            for (int i = 0; i < rightList.Count; i++)
            {
                if (rightList[i] == false && (words[i + firstWords] != "X" && words[i + firstWords] != "SPEC")) { err = true; break; }
            }
            return result;
        }

        private void ErrorMessages(bool err1, bool err2, string msg)
        {
            string message = string.Empty;
            if (err1) message += "В шаблоне меньше символов, чем подключенных контактов!\n";
            if (err2) message += "Есть не подключенные контакты, которые прописаны в шаблоне!\n";
            if (!err1 && !err2) message += "Конвертирование в .pat файл прошло успешно!\n";
            message += msg;
            MessageBox.Show(message);
        }
        #endregion



        // События, DllImport

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool BlockInput([In, MarshalAs(UnmanagedType.Bool)] bool fBlockIt);

    }
}
