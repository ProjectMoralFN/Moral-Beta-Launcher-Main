using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
using TiltedLauncher.Resources;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using TiltedLauncher.Resources.Launch;
using System.Runtime.InteropServices;
using DnsClient;
using System.Windows.Threading;
using System.IO.Compression;
using UserControl = System.Windows.Controls.UserControl;
using System.Drawing;
using System.Reflection;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Windows.Navigation;
using Moral_BetaLauncher;

namespace Moral
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {

        public class Vars
        {
            public static string Email = "NONE";
            public static string Password = "NONE";
            public static string Path1 = "NONE";


        }
        public Login()
        {
            InitializeComponent();
            string storedEmail = UpdateINI.ReadValue("Auth", "Email");
            string storedPassword = UpdateINI.ReadValue("Auth", "Password");

            // Set the retrieved values in the TextBox and PasswordBox
            if (storedEmail != "NONE") EmailBox.Text = storedEmail;
            if (storedPassword != "NONE") PasswordBox.Password = storedPassword;
            PasswordBox.PasswordChar = '*';
        }

        private string GetJsonBody(string url, string body)
        {
            WebRequest webRequest = WebRequest.Create(url);
            webRequest.ContentType = "application/json";
            webRequest.Method = "GET";
            object value = webRequest.GetType().GetProperty("CurrentMethod", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(webRequest);
            value.GetType().GetField("ContentBodyNotAllowed", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(value, false);
            using (StreamWriter streamWriter = new StreamWriter(webRequest.GetRequestStream()))
            {
                streamWriter.Write(body);
            }
            WebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
            string result = "";
            using (Stream responseStream = webResponse.GetResponseStream())
            {
                result = new StreamReader(responseStream, Encoding.UTF8).ReadToEnd();
            }
            return result;
        }

        private bool ValidateCredentialsAsync(string email, string password)
        {
            try
            {
                return GetJsonBody("http://ip/port/login", $"{{\"email\":\"{email}\", \"password\":\"{password}\"}}") == "true";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                return false;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string email = EmailBox.Text;
            string password = PasswordBox.Password;

            bool isValid = ValidateCredentialsAsync(email, password);

            if (isValid)
            {
                UpdateINI.WriteToConfig("Auth", "Email", email); // Save email to Update.ini
                UpdateINI.WriteToConfig("Auth", "Password", password); // Save password to Update.ini
                var mainWindow = new MainWindow();
                mainWindow.Show();
                // Close the login window
                this.Close();
            }
            else
            {
                UpdateINI.WriteToConfig("Auth", "Email", email); // Save email to Update.ini
                UpdateINI.WriteToConfig("Auth", "Password", password); // Save password to Update.ini
                MessageBox.Show("Incorrect E-Mail or Password");
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailBox.Text;
            string password = PasswordBox.Password;

            bool isValid = ValidateCredentialsAsync(email, password);

            if (isValid)
            {
                UpdateINI.WriteToConfig("Auth", "Email", email); // Save email to Update.ini
                UpdateINI.WriteToConfig("Auth", "Password", password); // Save password to Update.ini
                var mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                UpdateINI.WriteToConfig("Auth", "Email", email); // Save email to Update.ini
                UpdateINI.WriteToConfig("Auth", "Password", password); // Save password to Update.ini
                MessageBox.Show("Incorrect E-Mail or Password");
            }
        }

        private void Password_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Email_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void PasswordBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void EmailBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}