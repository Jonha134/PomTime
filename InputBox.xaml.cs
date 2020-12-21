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
using System.Media;

namespace Pom
{
    
    public partial class InputBox : Window
    {
        private readonly string labeltext;
        private readonly string TBtext;
        public string returnstring = "";

        
        public InputBox(string ltxt, string ttxt, int maxLength)
        {
            InitializeComponent();
            labeltext = ltxt;
            TBtext = ttxt;
            TB.MaxLength = maxLength;
            Loaded += InputBox_Loaded;
            SystemSounds.Asterisk.Play();
        }
        private void InputBox_Loaded(object sender, RoutedEventArgs e)
        {
            LblTime.Content = labeltext;
            TB.Text = TBtext;
            Keyboard.Focus(TB);
            TB.SelectAll();
        }
        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                returnstring = TB.Text.ToString();
                this.Close();
            }
        }
    }
}
