using AutoIt;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;

namespace mappin
{
    public partial class MainWindow
    {


        private void CreatePatFile(ref string clipboard) // Конвертирование
        {
            Process x;
            try
            {
                x = System.Diagnostics.Process.Start(@"C:\OpenATE\MTS3\ut\PATEDIT.exe");
            }
            catch { MessageBox.Show("Файл PATEDIT.exe не найден!"); return; }
            try
            {
                Clipboard.SetText(result); // копируем в буфер обмена
                BlockInput(true); // блокировка мыши
                x.WaitForExit(3000); // ждем пока откроется приложение
                System.Drawing.Point pointMouse = AutoItX.MouseGetPos(); // запоминает позицию курсора
                AutoItX.AutoItSetOption("MouseCoordMode", 2); // Координаты мыши будут рассчитываться от рабочей области окна, а не всего экрана
                AutoItX.MouseClick("left", 55, 40, 1, 0); // open
                Thread.Sleep(2000); // ждем пока откроется окно
                AutoItX.Send(filename); // Пишем имя pat файла
                AutoItX.Send("{ENTER}"); // Нажимаем enter, чтобы открыть
                Thread.Sleep(1000); // ждем пока закроется окно
                AutoItX.MouseClick("left", 320, 40, 1, 0); // compile
                Thread.Sleep(1000); // ждем пока скомпилируется
                AutoItX.MouseClick("left", 200, 400, 1, 0); // ставим курсор
                AutoItX.Send("{CTRLDOWN}"); // удерживаем кнопку ctrl
                AutoItX.Send("A"); // выделяем информацию о компиляции
                AutoItX.Send("C"); // копируем информацию о компиляции
                AutoItX.Send("{CTRLUP}"); // отпускаем кнопку ctrl
                clipboard = Clipboard.GetText();
                AutoItX.MouseMove(pointMouse.X, pointMouse.Y, 0); // возвращает курсор обратно
                x.Kill();  // close
                BlockInput(false); // разблокировка мыши
            }
            catch
            {
                MessageBox.Show("Произошел сбой!");
                BlockInput(false); // разблокировка мыши
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e) // Сброс
        {
            FirstWindow firstWindow = new FirstWindow();
            firstWindow.Show();
            this.Close();
        }

        private void Clear_Click(object sender, RoutedEventArgs e) // Очистить
        {
            foreach (Pair p in pairs)
            {
                p.leftNode.border.Background = Brushes.Transparent;
                p.rightNode.border.Background = Brushes.Transparent;
                Field.Children.Remove(p.polyline);
            }
            pairs.Clear();
            foreach (var t in table)
            {
                if (t.FirstRow != "VCC" && t.FirstRow != "GND")
                    t.FirstRow = "";
            }
        }

        private void SaveTable_Click(object sender, RoutedEventArgs e) // Сохранить
        {
            FileStream myStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == true)
            {
                myStream = new FileStream(saveFileDialog1.FileName, FileMode.Create);
                BinaryWriter writer = new BinaryWriter(myStream);
                writer.Write(AmountPins.ToString());
                string s = "";
                foreach (var p in Vcc)
                {
                    s += p.ToString() + " ";
                }
                writer.Write(s);
                s = "";
                foreach (var p in Gnd)
                {
                    s += p.ToString() + " ";
                }
                writer.Write(s);
                foreach (var t in Table)
                {
                    writer.Write(t.FirstRow);
                    writer.Write(t.SecondRow);
                }
                myStream.Close();
            }
        }

        
        private void OpenTable_Click(object sender, RoutedEventArgs e) // Открыть
        {
            flag = false;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.CurrentDirectory;
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (BinaryReader reader = new BinaryReader(File.Open(openFileDialog.FileName, FileMode.Open)))
                    {
                        // Чтение данных из файла
                        ObservableCollection<ClassForTable> tablebuf = new ObservableCollection<ClassForTable>();
                        string ap;
                        string vc;
                        string gn;
                        try
                        {
                            ap = reader.ReadString();
                            vc = reader.ReadString();
                            gn = reader.ReadString();
                            while (reader.PeekChar() > -1)
                            {
                                var tmp = new ClassForTable(reader.ReadString(), reader.ReadString());
                                tablebuf.Add(tmp);
                            }
                        }
                        catch
                        {
                            MessageBox.Show("Некорректный файл!");
                            return;
                        }
                        // Стирание прошлых данных
                        Clear_Click(null, null);
                        for (int i = 0; i < 16; ++i)
                        {
                            PaintArea1.Children.Remove(LeftBorders[i]);
                            PaintArea2.Children.Remove(LeftBorders[i + 16]);
                        }
                        LeftBorders.Clear();
                        for (int i = 0; i < AmountPins; ++i)
                        {
                            if (i < 16) PaintArea3.Children.Remove(RightBorders[i]);
                            else PaintArea4.Children.Remove(RightBorders[i]);
                        }
                        table.Clear();
                        Table = tablebuf;
                        init(ap, vc, gn);
                    }
                }
                catch
                {
                    MessageBox.Show("Ошибка открытия файла!");
                    return;
                }
                if (!flag)
                {
                    timer.Tick += new EventHandler(timerTick);
                    timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
                    timer.Start();
                }
            }
        }
        private void timerTick(object sender, EventArgs e)
        {
            if (!flag)
            {
                printLink();
                timer.Stop();
                flag = true;
            }
        }

    }
}
