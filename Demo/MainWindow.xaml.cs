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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Common;

namespace Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Closed += MainWindow_Closed;
            CheckBoxFullScreen.Checked += CheckBoxFullScreen_CheckedOrUnchecked;
            CheckBoxFullScreen.Unchecked += CheckBoxFullScreen_CheckedOrUnchecked;

            ButtonApplyFontSize.Click += ButtonApplyFontSize_Click;

            //Set the path to your XML configuration file
            MyXmlConfigurationHandler = new XmlConfigurationHandler(System.IO.Path.Combine(Environment.CurrentDirectory, "Config.xml"));

            //Add your settings here
            MyXmlConfigurationHandler.AddSetting("FullScreen", "0");
            MyXmlConfigurationHandler.AddSetting("FontSize", "14");

            //Load the XML configuration file
            MyXmlConfigurationHandler.LoadFile();

            //Read the settings
            bool isFullScreen = MyXmlConfigurationHandler.ReadBoolean("FullScreen");
            if (isFullScreen)
            {
                CheckBoxFullScreen.IsChecked = true;
            }
            else
            {
                CheckBoxFullScreen.IsChecked = false;
            }

            ApplyFontSize();

            TextBoxFontSizeSetting.Text = MyXmlConfigurationHandler.ReadString("FontSize");

        }

        XmlConfigurationHandler MyXmlConfigurationHandler = null;

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            //Save the XML configuration file
            MyXmlConfigurationHandler.SaveFile();
        }

        private void CheckBoxFullScreen_CheckedOrUnchecked(object sender, RoutedEventArgs e)
        {
            if (CheckBoxFullScreen.IsChecked == true)
            {
                MyXmlConfigurationHandler.WriteBoolean("FullScreen", true);
            }
            else
            {
                MyXmlConfigurationHandler.WriteBoolean("FullScreen", false);
            }

            bool isFullScreen = MyXmlConfigurationHandler.ReadBoolean("FullScreen");
            if (isFullScreen)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }

        private void ButtonApplyFontSize_Click(object sender, RoutedEventArgs e)
        {
            double d = 0;

            if (!double.TryParse(TextBoxFontSizeSetting.Text, out d))
            {
                return;
            }

            if (d < 0)
            {
                return;
            }

            MyXmlConfigurationHandler.WriteDouble("FontSize", d);

            ApplyFontSize();
        }

        private void ApplyFontSize()
        {
            double fontSize = MyXmlConfigurationHandler.ReadDouble("FontSize");

            if (fontSize > 0)
            {
                LabelFontSizeSetting.FontSize = fontSize;
                TextBoxFontSizeSetting.FontSize = fontSize;
                CheckBoxFullScreen.FontSize = fontSize;
            }
        }

    }
}
