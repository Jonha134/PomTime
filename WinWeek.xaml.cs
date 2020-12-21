using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Globalization;

namespace Pom
{
    public partial class WinWeek : Window
    {
        int relWeek = 0;
        int week;
        public WinWeek()
        {
            InitializeComponent();
            Loaded += Window2_Loaded;
            this.KeyDown += new KeyEventHandler(WinWeek_KeyDown);
            this.ShowDialog();
        }
        private void Window2_Loaded(object sender, RoutedEventArgs e)
        {
            //Figure out current week no
            DateTime dt = DateTime.Now;
            CultureInfo myCI = new CultureInfo("sv-SE");
            System.Globalization.Calendar myCal = myCI.Calendar;
            CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;
            week = myCal.GetWeekOfYear(dt, myCWR, myFirstDOW);

            LoadData();
        }
        private void WinWeek_KeyDown(object sender, KeyEventArgs e)
        {
            //Use arrow keys to step back and forth between weeks. X to quit.
            if (e.Key == Key.Right) { relWeek++; }
            else if (e.Key == Key.Left) { relWeek--; }
            else if (e.Key.ToString() is "X") { this.Close(); }
            
            LoadData();
        }
        private void LoadData()
        {
            //Function to load data for a week relative to the current where current = 0
            DateTime dt;
            LblWeek.Content = (week+relWeek).ToString();

            dt = DateTime.Now.AddDays(DayOfWeek.Monday - DateTime.Now.DayOfWeek + 7*relWeek);
            LblMon.Content = dt.ToString("yyyy:MM:dd") + "\n" + MainWindow.GetProjectsforDate(dt);
            dt = DateTime.Now.AddDays(DayOfWeek.Tuesday - DateTime.Now.DayOfWeek + 7 * relWeek);
            LblTue.Content = dt.ToString("yyyy:MM:dd") + "\n" + MainWindow.GetProjectsforDate(dt);
            dt = DateTime.Now.AddDays(DayOfWeek.Wednesday - DateTime.Now.DayOfWeek + 7 * relWeek);
            LblWed.Content = dt.ToString("yyyy:MM:dd") + "\n" + MainWindow.GetProjectsforDate(dt);
            dt = DateTime.Now.AddDays(DayOfWeek.Thursday - DateTime.Now.DayOfWeek + 7 * relWeek);
            LblThu.Content = dt.ToString("yyyy:MM:dd") + "\n" + MainWindow.GetProjectsforDate(dt);
            dt = DateTime.Now.AddDays(DayOfWeek.Friday - DateTime.Now.DayOfWeek + 7 * relWeek);
            LblFri.Content = dt.ToString("yyyy:MM:dd") + "\n" + MainWindow.GetProjectsforDate(dt);
            dt = DateTime.Now.AddDays(DayOfWeek.Saturday - DateTime.Now.DayOfWeek + 7 * relWeek);
            LblSat.Content = dt.ToString("yyyy:MM:dd") + "\n" + MainWindow.GetProjectsforDate(dt);
            dt = DateTime.Now.AddDays(DayOfWeek.Sunday - DateTime.Now.DayOfWeek - 7 + 7 * relWeek);
            LblSun.Content = dt.ToString("yyyy:MM:dd") + "\n" + MainWindow.GetProjectsforDate(dt);
        }
    }
 }

