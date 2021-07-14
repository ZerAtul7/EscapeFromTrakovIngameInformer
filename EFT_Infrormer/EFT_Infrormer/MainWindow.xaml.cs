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
using System.IO;
using System.Windows.Forms;
using System.Threading;
namespace EFT_Infrormer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> prevExf { get; set; } = new List<string>();
        List<string> nowExf { get; set; } = new List<string>();

        List<string> prevApp { get; set; } = new List<string>();
        List<string> nowApp { get; set; } = new List<string>();
        int iteratorExf { get; set; }
        int iteratorApp { get; set; }

        bool InRaid { get; set; } = false;

        string exfilPath { get; set; } = "";
        string applicationPath { get; set; } = "";

        string exfilPathNew { get; set; } = "";

        string applicationPathNew { get; set; } = "";

        string location { get; set; } = "";

        Thread thread2 { get; set; }

        public MainWindow()
        {
            Topmost = true;
            InitializeComponent();
            TopWindow top = new TopWindow(this);
            top.Show();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();

            if(result != 0)
            {
                
                
                DirectoryInfo Di = new DirectoryInfo(fbd.SelectedPath);

                var dl = Di.GetFiles();
                bool found = false;
                foreach(var item in dl)
                {
                    if (item.Name == "EscapeFromTarkov.exe")
                        found = true;

                }

                if (found)
                {
                    System.Windows.MessageBox.Show("EFT Found, you can continue");
                    GamePath.Text = fbd.SelectedPath;

                    
                        
                    
                }
                else
                {
                    System.Windows.MessageBox.Show("EFT Not Found! Select EFT root folder!");

                }
            }
           
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string newpath = GamePath.Text + "\\Logs\\";
          
            DirectoryInfo DI =  new DirectoryInfo(newpath);
            var directory = DI.GetDirectories().OrderByDescending(d => d.LastWriteTimeUtc).First();
            var LogsList =  directory.GetFiles();
            
            foreach(var file in LogsList)
            {
             
                

                if (file.Name.Contains("exfiltration"))
                {
                    exfilPath = file.FullName;
                }
                else if(file.Name.Contains("application"))
                {
                    applicationPath = file.FullName;
                }
            }

            // Trash with copying
            var buff1 = exfilPath.Split(' ');
            var buff2 = applicationPath.Split(' ');
            var buff3 = buff1[1].Split('.');
            var buff4 = buff2[1].Split('.');
            System.Windows.MessageBox.Show(buff1[0] + " " + buff3[0] + ".txt");
            System.Windows.MessageBox.Show(buff2[0] + " " + buff4[0] + ".txt");
            exfilPathNew = buff1[0] + " " + buff3[0] + ".txt";
            applicationPathNew = buff2[0] + " " + buff4[0] + ".txt";
            if(!File.Exists(exfilPathNew))
                File.Copy(exfilPath,exfilPathNew);
            if(!File.Exists(applicationPathNew))
                File.Copy(applicationPath, applicationPathNew);
            


            using (StreamReader sr = new StreamReader(applicationPathNew))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    prevApp.Add(line);
                    iteratorApp = prevApp.Count;
                }
            }

            using (StreamReader sr = new StreamReader(exfilPathNew))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    prevExf.Add(line);
                    iteratorExf = prevExf.Count;
                }
            }
            
           
            thread2 = new Thread(CheckApp);
            thread2.Start();
            thread2.IsBackground = true;

        }


        private void CheckExf()
        {
            
            using (StreamReader sr = new StreamReader(exfilPathNew))
            {
                nowExf.Clear();
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    nowExf.Add(line);
                    
                }

            }
            while(InRaid == true && File.Exists(exfilPathNew))
            {
                Thread.Sleep(1000);
                File.Delete(exfilPathNew);
                File.Copy(exfilPath, exfilPathNew);
                
                if (prevExf.Count == nowExf.Count)
                {
                    using (StreamReader sr = new StreamReader(exfilPathNew))
                    {
                        nowExf.Clear();
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            nowExf.Add(line);

                        }

                    }
                }
                else
                {
                    for (int i = iteratorExf; i < nowExf.Count; i++)
                    {
                        //Dispatcher.Invoke(new Action(() => { DebugBox.Text += $"{nowApp[i]}\n"; }));
                        if (nowExf[i].Contains("TRACE-NetworkGameCreate profileStatus:"))
                        {
                            switch (location)
                            {
                                case "factory4_day":
                                    break;
                                case "factory4_night":
                                    break;
                                case "RezervBase":
                                    break;
                                case "Shoreline":
                                    break;
                                case "Woods":
                                    break;
                                case "Interchange":
                                    break;
                                case "laboratory":
                                    break;
                                case "Customs":
                                    break;
                            }

                        }
                        else if ((nowExf[i].Contains("SelectProfile ProfileId:")))
                        {
                            InRaid = false;
                            // stop checking exfils
                        }

                    }
                    prevExf.Clear();
                    prevExf.AddRange(nowExf);
                    iteratorExf = nowExf.Count;
                }
                
            }

           
        }
                    
           


        private void CheckApp()
        {
            Thread thread1 = new Thread(CheckExf);
            thread1.IsBackground = true;

            using (StreamReader sr = new StreamReader(applicationPathNew))
            {
                nowApp.Clear();
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    nowApp.Add(line);


                }

            }

            while (File.Exists(applicationPathNew))
            {
               
                Thread.Sleep(1000);
                File.Delete(applicationPathNew);
                File.Copy(applicationPath, applicationPathNew);
                if(prevApp.Count == nowApp.Count)
                {

                    using (StreamReader sr = new StreamReader(applicationPathNew))
                    {
                        nowApp.Clear();
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            nowApp.Add(line);

                        
                        }
                        
                    }

                }
                else
                {
                    

                    for (int i = iteratorApp; i < nowApp.Count; i++)
                    {
                        //Dispatcher.Invoke(new Action(() => { DebugBox.Text += $"{nowApp[i]}\n"; }));
                        if (nowApp[i].Contains("TRACE-NetworkGameCreate profileStatus:"))
                        {
                            InRaid = true;
                            var buff = nowApp[i].Split(',');
                            Dispatcher.Invoke(new Action(() => { DebugBox.Text += $"{nowApp[i]}\n"; }));
                            var buff2 = buff[4].Split(' ');
                            if (buff2[1] == "bigmap")
                                location = "Customs";
                            else
                                location = buff2[2].Replace(',',' ');
                             Dispatcher.Invoke(new Action(() => {
                                RaidStatus.Content = "Active";
                                RaidStatus.Foreground = Brushes.Green;
                                locationField.Content = location;
                                buff2 = buff[2].Split(' ');
                                IPField.Content = buff2[2].Replace(',', ' ');
                                buff2 = buff[3].Split(' ');
                                IPField.Content += ":" + buff2[2].Replace(',', ' ');
                                buff2 = buff[7].Split(' ');
                                SHID.Content = buff2[2].Replace('\'', ' ');
                            }));
                            
                            //start checking exfils
                            
                            //thread1.Start();
                            

                            break;

                        }
                        else if(nowApp[i].Contains("SelectProfile ProfileId:"))
                        {
                            InRaid = false;
                            Dispatcher.Invoke(new Action(() => {
                                RaidStatus.Content = "Inactive";
                                RaidStatus.Foreground = Brushes.Red;
                                locationField.Content = "None";
                                IPField.Content = "None";
                                SHID.Content = "None";
                            }));
                            // stop checking exfils


                        }

                    }

                    prevApp.Clear();
                    prevApp.AddRange(nowApp);
                    iteratorApp = nowApp.Count;
                }






            }
           
           
        }

        
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            thread2.Abort();
            File.Delete(exfilPathNew);
            File.Delete(applicationPathNew);
        }
    }
}
