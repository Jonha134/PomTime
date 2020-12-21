using System;
using System.Timers;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using System.Diagnostics;


namespace Pom
{
    public partial class MainWindow : Window
    {
        private System.Timers.Timer aTimer;
        private static readonly Random random = new Random();
        private readonly int interval = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["interval"]);
        private string password = "";
        private readonly string passLength = System.Configuration.ConfigurationManager.AppSettings["passLength"];
        private int i; //Current timer value in seconds
        private int weekPoms;
        private static readonly string logpath = "log.txt";
        private static readonly string todopath = "todo.txt";
        private static readonly string activityLogpath = "activityLog.txt";
        private string currActivity;
        private bool mouseDisabled = false;
        private bool tHelp = false;
        public static RoutedCommand ctrlACmd = new RoutedCommand();
        public static RoutedCommand ctrlOCmd = new RoutedCommand();
        public static RoutedCommand ctrlXCmd = new RoutedCommand();
        public static RoutedCommand ctrlLCmd = new RoutedCommand();
        public static RoutedCommand ctrlTCmd = new RoutedCommand();
        public static RoutedCommand ctrlHCmd = new RoutedCommand();
        public static RoutedCommand ctrlMCmd = new RoutedCommand();
        public static RoutedCommand ctrlWCmd = new RoutedCommand();
        public static RoutedCommand ctrlPCmd = new RoutedCommand();
        public static RoutedCommand ctrlDCmd = new RoutedCommand();
        public static RoutedCommand ctrlQCmd = new RoutedCommand();
        public static RoutedCommand ctrlNCmd = new RoutedCommand();
        public static RoutedCommand ctrlICmd = new RoutedCommand();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            this.Closed += new EventHandler(MainWindow_Closed);

            //Shortcuts for the commands
            ctrlACmd.InputGestures.Add(new KeyGesture(Key.A, ModifierKeys.Control));
            ctrlOCmd.InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Control));
            ctrlXCmd.InputGestures.Add(new KeyGesture(Key.X, ModifierKeys.Control));
            ctrlLCmd.InputGestures.Add(new KeyGesture(Key.L, ModifierKeys.Control));
            ctrlTCmd.InputGestures.Add(new KeyGesture(Key.T, ModifierKeys.Control));
            ctrlHCmd.InputGestures.Add(new KeyGesture(Key.H, ModifierKeys.Control));
            ctrlMCmd.InputGestures.Add(new KeyGesture(Key.M, ModifierKeys.Control));
            ctrlWCmd.InputGestures.Add(new KeyGesture(Key.W, ModifierKeys.Control));
            ctrlPCmd.InputGestures.Add(new KeyGesture(Key.P, ModifierKeys.Control));
            ctrlDCmd.InputGestures.Add(new KeyGesture(Key.D, ModifierKeys.Control));
            ctrlQCmd.InputGestures.Add(new KeyGesture(Key.Q, ModifierKeys.Control));
            ctrlNCmd.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control));
            ctrlICmd.InputGestures.Add(new KeyGesture(Key.I, ModifierKeys.Control));
        }
        void MainWindow_Closed(object sender, EventArgs e)
        {
            DisableChrome(false, false);
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TBDayProjects.Text = GetProjectsforDate(DateTime.Now);
            weekPoms = GetWeekHours();

            TBmessage.Text = "This week's poms: " + weekPoms.ToString();

            //Load first activity from todo.txt
            FinishActivity(2);

            //Generate password
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            password = new string(Enumerable.Repeat(chars, Int32.Parse(passLength)).Select(s => s[random.Next(s.Length)]).ToArray());
        }
        
        //Shortcut commands
        private void CtrlACmd(object sender, ExecutedRoutedEventArgs e) { StartPom(); }
        private void CtrlOCmd(object sender, ExecutedRoutedEventArgs e) { aTimer.Stop(); }
        private void CtrlXCmd(object sender, ExecutedRoutedEventArgs e) { i = 0; }
        private void CtrlLCmd(object sender, ExecutedRoutedEventArgs e) { System.Diagnostics.Process.Start("notepad.exe", logpath); }
        private void CtrlTCmd(object sender, ExecutedRoutedEventArgs e) { System.Diagnostics.Process.Start("notepad.exe", todopath); }
        private void CtrlHCmd(object sender, ExecutedRoutedEventArgs e) { ToggleHelp(); }
        private void CtrlMCmd(object sender, ExecutedRoutedEventArgs e) { ToggleMouse(); }
        private void CtrlWCmd(object sender, ExecutedRoutedEventArgs e) { WinWeek winWeek = new WinWeek(); }
        private void CtrlPCmd(object sender, ExecutedRoutedEventArgs e) { FinishActivity(3); }
        private void CtrlDCmd(object sender, ExecutedRoutedEventArgs e) { FinishActivity(1); }
        private void CtrlQCmd(object sender, ExecutedRoutedEventArgs e) { FinishActivity(2); }
        private void CtrlNCmd(object sender, ExecutedRoutedEventArgs e) { AddActivity(); }
        private void CtrlICmd(object sender, ExecutedRoutedEventArgs e) { DisableChrome(false, true); }

        private void StartPom()
        {
            if (i <= 0) { i = interval; }
            SetTimer();
            DisableChrome(true, false);
        }
        private void AddActivity()
        {
            InputBox ib = new InputBox("Add activity:", "Activity", 50);
            ib.ShowDialog();
            AppendFile(todopath, ib.returnstring);
        }
        private void ToggleHelp()
        {
            string str = "A Start\nO Stop\nX End\nL Log.txt\nT Todo.txt\nH Help\nM Mouse\nW Week\nP Postpone\nD Done\nQ Delete\nN New Act.";

            if (tHelp)
            {
                TBDayProjects.TextAlignment = TextAlignment.Left;
                TBDayProjects.Text = GetProjectsforDate(DateTime.Now);
                tHelp = false;
            }
            else
            {
                TBDayProjects.TextAlignment = TextAlignment.Left;
                TBDayProjects.Text = str;
                tHelp = true;
            }
        }
        private void FinishActivity (int x)
        {
            //x=1 Activity is done
            //x=2 Delete activitity
            //x=3 Postpone activity

            //If activity should be postponed
            if (x == 3) { AppendFile(todopath, "\n" + TBactivity.Text); }

            //If activity is done and not empty, append to activityLog
            if (TBactivity.Text != "" && x==1) { AppendFile(activityLogpath, DateTime.Now.ToString("yy-MM-dd") + ";" + TBactivity.Text); }

            //Remove activity from log.txt, unless todo.txt have been updated through other means
            if (new FileInfo(todopath).Length > 6 && currActivity == File.ReadLines(todopath).First() )
            {
                //Remove first line in todo.txt
                var lines = File.ReadAllLines(todopath);
                File.WriteAllLines(todopath, lines.Skip(1).ToArray());
            }

            TBmessage.Text = "";
            //check if todo contains more activities
            try { TBactivity.Text = currActivity = File.ReadLines(todopath).First(); }
            catch
            {
                TBactivity.Text = "";
                TBmessage.Text = "No more activities";
            }
        }
        private void AppendFile(string path, string line)
        {
            if (File.Exists(path))
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(line);
                    sw.Close();
                }
            }
            else { TBmessage.Text = "Error: File " + path + " does not exist."; }
        }
        private bool ReqPassword()
        {
            InputBox ib = new InputBox("Provide password: " + password, "", 30);
            ib.ShowDialog();
            if (ib.returnstring == password)
            {
                TBmessage.Text = "Correct pass!";
                return true;
            }
            else 
            { 
                TBmessage.Text = "Wrong pass!";
                return false;
            }
        }
        private void ToggleMouse()
        {
            string message ="";
            var psi = new ProcessStartInfo();
            psi.FileName = "powershell.exe";
            psi.Arguments = "";

            if (mouseDisabled)
            {
                if (ReqPassword())
                {
                    psi.Arguments = "Enable-PnpDevice -InstanceId 'HID\\VID_046D&PID_C526&MI_00\\8&39bfd600&0&0000' -confirm:$false";
                    message = "Mouse enabled";
                    mouseDisabled = false;
                }
            }
            else if (mouseDisabled == false) //Logic to disable mouse
            {
                psi.Arguments = "Disable-PnpDevice -InstanceId 'HID\\VID_046D&PID_C526&MI_00\\8&39bfd600&0&0000' -confirm:$false";
                message = "Mouse disabled";
                mouseDisabled = true;
            }
            if (psi.Arguments != "") //This should not happen unless psi.arg has been set. 
            {
                psi.UseShellExecute = true;
                psi.Verb = "runas";

                var process = new Process();
                try
                {
                    process.StartInfo = psi;
                    process.Start();
                    process.WaitForExit();
                    TBmessage.Text = message;
                }
                catch { TBmessage.Text = "Adm.rights req to disable mouse"; } //User not admin
            }
        }
        private void DisableChrome(bool b, bool reqPass)
        {
            RegistryKey myKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\chrome.exe", true);

            if (b && myKey != null) //User wants to disable Chrome 
            {
                myKey.SetValue("Debugger", "1", RegistryValueKind.String); //random invalid value
            }
            else if (myKey != null) //User wants to enable chrome,
            {
                if (reqPass) //In case password was required when calling the method
                {
                    if (ReqPassword()) { myKey.SetValue("Debugger", "", RegistryValueKind.String); }
                }
                else { myKey.SetValue("Debugger", "", RegistryValueKind.String); } //password was not required
                
                TBmessage.Text = "Chrome enabled!";
            }
            myKey.Close();
        }
        private void SetTimer()
        {
            aTimer = new System.Timers.Timer(1000);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        public static string GetProjectsforDate(DateTime dt)
        {
            List<string> list = new List<string>();
            
            foreach (string line in File.ReadLines(logpath))
            {
                if (line.StartsWith(dt.ToString("yy-MM-dd")))
                {
                    list.Add(line.Substring(9));
                }
            }
            String[] strDay = list.ToArray();
            Dictionary<string, int> dicDay = strDay
                    .GroupBy(item => item)
                    .ToDictionary(item => item.Key, item => item.Count());

            StringBuilder sb = new StringBuilder();

            foreach (var item in dicDay)
            {
                sb.AppendFormat("{0}: {1}{2}", item.Key, item.Value, Environment.NewLine);
            }
            return sb.ToString();
        }
        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                TBtimer.Text = (new DateTime()).AddSeconds(i).ToString("mm:ss");
            });
            
            if (i==0)
            {
                aTimer.Stop();
                
                Application.Current.Dispatcher.Invoke((Action)delegate {
                    this.Activate();
                    EndPom();
                });
            }
            i--;
        }
        void EndPom()
        {
            //Logic to happen when a pom comes to an end
            InputBox ib = new InputBox("Pom ended: " + (DateTime.Now).ToString("HH:mm"),"Projekt", 7);
            ib.ShowDialog();

            AppendFile(logpath, DateTime.Now.ToString("yy-MM-dd") + ";" + ib.returnstring);

            TBDayProjects.Text = GetProjectsforDate(DateTime.Now);
            weekPoms++;
            DisableChrome(false, false);
            TBmessage.Text = "This week's poms: " + weekPoms.ToString();
        }
        private int GetWeekHours()
        {
            int weekHours = 0;
            DateTime dt;

            for (int i = 0; i < (int)DateTime.Now.DayOfWeek; i++) //Start with todays date and then iterate backwards one day at a time
            {
                dt = DateTime.Now.AddDays( - (long)i);
                weekHours += GetHoursForDate(dt);
            }
            return weekHours;
        }
        private int GetHoursForDate (DateTime dt)
        {
            List<string> list = new List<string>();
            int i=0;

            foreach (string line in File.ReadLines(logpath))
            {
                if (line.StartsWith(dt.ToString("yy-MM-dd"))) { i++; }
            }
            return i;
        }
    }
}
