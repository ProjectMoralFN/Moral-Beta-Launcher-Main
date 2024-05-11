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
using System.Windows.Navigation;
using TiltedLauncher.Resources;
using WindowsAPICodePack.Dialogs;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using System.Net;
using TiltedLauncher.Resources.Launch;
using DiscordRPC;
using TiltedLauncher;
using System.Reflection;

namespace Moral_BetaLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        bool _isLaunchingTheGame = false;
        string _username = string.Empty;

        Thread launcherThread;
        bool running = false;
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);


        public class Vars
        {
            public static string Email = "NONE";
            public static string Password = "NONE";
            public static string Path1 = "NONE";



        }
        public MainWindow()
        {
            InitializeComponent();
            string storedEmail = UpdateINI.ReadValue("Auth", "Email");
            string storedPassword = UpdateINI.ReadValue("Auth", "Password");

            RPC.rpctimestamp = Timestamps.Now;
            RPC.InitializeRPC();
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

        private void PathBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateINI.WriteToConfig("Auth", "Path1", PathBox.Text); // Updates Live OMG!
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog commonOpenFileDialog = new CommonOpenFileDialog();
            commonOpenFileDialog.IsFolderPicker = true;
            commonOpenFileDialog.Title = "Select A Fortnite Build";
            commonOpenFileDialog.Multiselect = false;
            CommonFileDialogResult commonFileDialogResult = commonOpenFileDialog.ShowDialog();


            bool flag = commonFileDialogResult == CommonFileDialogResult.Ok;
            if (flag)
            {
                if (File.Exists(System.IO.Path.Combine(commonOpenFileDialog.FileName, "FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping.exe")))
                {
                    this.PathBox.Text = commonOpenFileDialog.FileName;
                }
                else
                {
                    System.Windows.MessageBox.Show("Please make sure that your the folder contains FortniteGame and Engine In");

                }
            }
        }


        public void Launch()
        {
            try
            {
                if (running) return;
                running = true;
                if (Vars.Path1 == "NONE")
                {
                    Vars.Path1 = UpdateINI.ReadValue("Auth", "Path1");
                }
                string GamePath = Vars.Path1;
                if (GamePath != "NONE") // NONE THE BEST RESPONSE!
                {
                    //MessageBox.Show(Path69);
                    if (File.Exists(System.IO.Path.Combine(GamePath, "FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping.exe")))
                    {
                        if (Vars.Email == "NONE")
                        {
                            Vars.Email = UpdateINI.ReadValue("Auth", "Email");
                        }
                        if (Vars.Password == "NONE")
                        {
                            Vars.Password = UpdateINI.ReadValue("Auth", "Password");
                        }
                        if (Vars.Email == "NONE" || Vars.Password == "NONE")
                        {
                            MessageBox.Show("Your login was incorrect, please logout!");
                        }
                        foreach (var proc in Process.GetProcessesByName("FortniteClient-Win64-Shipping"))
                        {
                            try
                            {
                                if (proc.MainModule.FileName.StartsWith(GamePath))
                                {
                                    proc.Kill();
                                    proc.WaitForExit();
                                }
                            }
                            catch
                            {

                            }
                        }
                        WebClient Client = new WebClient();
                        //Client.DownloadFile("https://cdn.discordapp.com/attachments/999340493622222999/1196948255137873942/Cobalt.dll", Path.Combine(Path69, "Engine\\Binaries\\ThirdParty\\NVIDIA\\NVaftermath\\Win64", "GFSDK_Aftermath_Lib.x64.dll"));
                        //Client.DownloadFile($"{ip}/assets/sigs.bin", Path.Combine(GamePath, "EasyAntiCheat\\Certificates", "base.bin"));
                        Client.DownloadFile($"redirect url, i use direct google drive link", Path.Combine(GamePath, "Engine\\Binaries\\ThirdParty\\NVIDIA\\NVaftermath\\Win64", "GFSDK_Aftermath_Lib.x64.dll"));
                        //AntiCheat.Start(Path69);this.Dispatcher.Invoke(() =>
                        Game.Start(GamePath, "-epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -nobe -fromfl=eac -fltoken=919348d6add4c4c7c7507e61 -skippatchcheck", Vars.Email, Vars.Password);
                        //FakeAC.Start(GamePath, "MeowsclesEAC.exe", "-plooshfn -epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -nobe -fromfl=eac -fltoken=h1cdhchd10150221h130eB56 -skippatchcheck", "t");
                        FakeAC.Start(GamePath, "FortniteClient-Win64-Shipping_BE.exe", $"-epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -noeac -fromfl=be -fltoken=h1cdhchd10150221h130eB56 -skippatchcheck", "r");
                        FakeAC.Start(GamePath, "FortniteLauncher.exe", $"-epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -noeac -fromfl=be -fltoken=h1cdhchd10150221h130eB56 -skippatchcheck", "dsf");
                        try
                        {
                            FakeAC._FNLauncherProcess.Close();
                            FakeAC._FNAntiCheatProcess.Close();
                        }
                        catch (Exception ex)
                        {
                        }

                        //this.Button.Content = "Stop Meowscles";
                        //this.Button.Click += Button_Click_Stop;
                        //this.Button.IsEnabled = true;
                        IntPtr h = Process.GetCurrentProcess().MainWindowHandle;
                        ShowWindow(h, 6);
                        try
                        {
                            Game._FortniteProcess.WaitForExit();
                        }
                        catch (Exception) { }
                        try
                        {
                            try
                            {
                                Kill();
                            }
                            catch
                            {

                            }

                        }
                        catch (Exception)
                        {
                            MessageBox.Show("An error occurred while closing Fake AC!");

                        }



                        //Injector.Start(PSBasics._FortniteProcess.Id, Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "EonCurl.dll"));
                        running = false;
                    }
                    else
                    {
                        MessageBox.Show("Selected path is not a valid fortnite installation!");

                    }
                }
                else
                {
                    MessageBox.Show("Please add your game path in settings!");

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

            }
        }

        public static void Kill()
        {
            try
            {
                if (Game._FortniteProcess != null && !Game._FortniteProcess.HasExited) Process.GetProcessById(Game._FortniteProcess.Id).Kill();
                if (FakeAC._FNLauncherProcess != null && !FakeAC._FNLauncherProcess.HasExited) FakeAC._FNLauncherProcess.Kill();
                if (FakeAC._FNAntiCheatProcess != null && !FakeAC._FNAntiCheatProcess.HasExited) FakeAC._FNAntiCheatProcess.Kill();
            }
            catch (Exception)
            {

            }
        }


        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            launcherThread = new Thread(Launch);
            launcherThread.Start();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
          
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void EmailBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
                        WindowState = WindowState.Minimized;
        }
    }
}