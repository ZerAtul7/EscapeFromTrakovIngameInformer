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
        TopWindow top { get; set; }
        string exf { get; set; }
        public MainWindow()
        {
            Topmost = true;
            InitializeComponent();
             
            var buff = "2021-07-13 16:32:31.151 +03:00|0.12.11.1.13215|Debug|exfiltration|EligiblePoints (mallse): NW Exfil|RegularMode, PP Exfil|UncompleteRequirements, Interchange Cooperation|RegularMode, Hole Exfill|RegularMode, Saferoom Exfil|UncompleteRequirements ";
            var buff2 = buff.Split(' ');
            for (int i = 0; i < buff2.Length; i++)
            {
                System.Windows.MessageBox.Show($" {i} {buff2[i] }");
            }
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
                    System.Windows.MessageBox.Show("EFT Found, you can continue", "OK", MessageBoxButton.OK , MessageBoxImage.Exclamation);
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
            var pe = exfilPathNew;
            var ap = applicationPathNew;


            // Deleting files if they exists(Which we use for parse info)
            directory.CreateSubdirectory("Utils");
            if (File.Exists(exfilPathNew))
            {
                File.Move(exfilPathNew, directory.FullName + "\\Utils\\" + "1.txt");
                
            }
            
            if(File.Exists(applicationPathNew))
            {
                File.Move(applicationPathNew, directory.FullName + "\\Utils\\" + "2.txt");
            }
            Directory.Delete(directory.FullName + "\\Utils",true);


            File.Copy(exfilPath,pe);
            File.Copy(applicationPath, ap);
            


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
            
           // Start parse app log
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
                        if (nowExf[i].Contains("EligiblePoints"))
                        {
                            switch (location)
                            {
                                case "factory4_day":
                                    exf = "Cellars - Need Key\nGate 3 - Need Key\nGate 0 - Active\nGate m - Need key";
                                    Dispatcher.Invoke(new Action(() => { DrawExf(exf); }));
                                   
                                    break;
                                case "factory4_night":
                                    exf = "Cellars - Need Key\nGate 3 - Need Key\nGate 0 - Active\nGate m - Need key";
                                    Dispatcher.Invoke(new Action(() => { DrawExf(exf); }));
                                    break;
                                case "RezervBase":

                                    break;
                                case "Shoreline":
                                    {
                                        var buff = nowExf[i].Split(' ');
                                        if(buff[3] == "(riverside):")
                                        {
                                            exf += "Tunnel - Active\n";
                                            var Boat = buff[5].Split('|');
                                            if (Boat[1].Contains("NotPresent"))
                                            {
                                                exf += "Boat - Inactive\n";
                                            }
                                            else
                                            {
                                                exf += "Boat - Active\n";
                                            }
                                            var RockPassage = buff[8].Split('|');
                                            if (RockPassage[1].Contains("NotPresent"))
                                            {
                                                exf += "Rock Passage - Inactive\n";
                                            }
                                            else
                                            {
                                                exf += "Rock Passage - Active\n";
                                            }
                                            Dispatcher.Invoke(new Action(() => { DrawExf(exf); }));

                                        }
                                        else
                                        {
                                            exf += "RoadToCustoms - Active\n";
                                            var Boat = buff[5].Split('|');
                                            if (Boat[1].Contains("NotPresent"))
                                            {
                                                exf += "Boat - Inactive\n";
                                            }
                                            else
                                            {
                                                exf += "Boat - Active\n";
                                            }
                                            var CCP = buff[10].Split('|');
                                            if (CCP[1].Contains("NotPresent"))
                                            {
                                                exf += "CPP Temporary - Inactive\n";
                                            }
                                            else
                                            {
                                                exf += "CPP Temporary - Active\n";
                                            }
                                            var RockPassage = buff[12].Split('|');
                                            if (RockPassage[1].Contains("NotPresent"))
                                            {
                                                exf += "Rock Passage - Inactive\n";
                                            }
                                            else
                                            {
                                                exf += "Rock Passage - Active\n";
                                            }
                                            Dispatcher.Invoke(new Action(() => { DrawExf(exf); }));

                                        }
                                       
                                    }
                                    break;
                                case "Woods":

                                    break;
                                case "Interchange":
                                    {
                                        var buff = nowExf[i].Split(' ');
                                        if (buff.Contains("mallnw"))
                                        {
                                            
                                            exf += "Emercom Checkpoint = Active\n";
                                            var PPexfil = buff[7].Split('|');
                                            if(PPexfil[1].Contains("NotPresent"))
                                            {
                                                exf += "Power Station - Inactive\n";
                                            }
                                            else
                                            {
                                                exf += "Power Station - Waiting for money\n";
                                            }
                                            exf += "Scav Camp - Active\n";
                                            exf += "Hole - Active\n";
                                            exf += "SafeRoom - Waiting for electricity\n";
                                        }
                                        else
                                        {
                                            exf += "Railway = Active\n";
                                            var PPexfil = buff[7].Split('|');
                                            if (PPexfil[1].Contains("NotPresent"))
                                            {
                                                exf += "Power Station - Inactive\n";
                                            }
                                            else
                                            {
                                                exf += "Power Station - Waiting for money\n";
                                            }
                                            exf += "Scav Camp - Active\n";
                                            exf += "Hole - Active\n";
                                            exf += "SafeRoom - Waiting for electricity\n";
                                        }
                                    }
                                    break;
                                case "laboratory":

                                    break;
                                case "Customs":
                                    {
                                        var buff = nowExf[i].Split(' ');
                                        if (buff.Contains("(customs:)"))
                                        {
                                            exf += "ZB_013 - Waiting for lever\n";
                                            var Vex = buff[6].Split('|');
                                            if(Vex[1].Contains("NotPresent"))
                                            {
                                                exf += "Dorms V-ex - Inactive\n";
                                            }
                                            else
                                            {
                                                exf += "Dorms V-ex - Waiting for money\n";
                                            }
                                            exf += "ZB_011 - Active\n";
                                            var GAS = buff[10].Split('|');
                                            if(GAS[1].Contains("NotPresent"))
                                            {
                                                exf += "Old Gas Station - Active\n";
                                            }
                                            else
                                            {
                                                exf += "Old Gas Station - Inactive\n";
                                            }
                                            var ZB_012 = buff[11].Split('|');
                                            if(ZB_012[1].Contains("NotPresent"))
                                            {
                                                exf += "ZB-012 - Inactive\n";
                                            }
                                            else
                                            {
                                                exf += "ZB-012 - Active\n";
                                            }

                                        }
                                        else
                                        {
                                            exf += "ZB_013 - Waiting for lever\n";
                                            var Vex = buff[7].Split('|');
                                            if (Vex[1].Contains("NotPresent"))
                                            {
                                                exf += "Dorms V-ex - Inactive\n";
                                            }
                                            else
                                            {
                                                exf += "Dorms V-ex - Waiting for money\n";
                                            }
                                            exf += "CrossRoads - Active\n";
                                            exf += "Trailer Park - Active\n";
                                            var RUAF = buff[12].Split('|');
                                            if(RUAF[1].Contains("NotPresent"))
                                            {
                                                exf += "RUAF - Inactive\n";
                                            }
                                            else
                                            {
                                                exf += "RUAF - Active\n";
                                            }
                                            var Boat = buff[14].Split('|');
                                            if(Boat[1].Contains("NotPresent"))
                                            {
                                                exf += "Boat - Inactive\n";
                                            }
                                            else
                                            {
                                                exf += "Boat - Active\n";
                                            }
                                        }
                                    }
                            
                                    break;
                            }

                        }
                        else if ((nowExf[i].Contains("SelectProfile ProfileId:")))
                        {
                            InRaid = false;
                            // stop checking exfils
                        }
                        else if(nowExf[i].Contains("InfiltrationMatch"))
                        {
                            //notf
                        }

                        switch (location)
                        {
                            case "factory4_day":
                              
                                break;
                            case "factory4_night":
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
                                location = buff2[2].Replace(',',' ').Trim();
                           // System.Windows.MessageBox.Show(location);
                             Dispatcher.Invoke(new Action(() => {
                                RaidStatus.Content = "Active";
                                RaidStatus.Foreground = Brushes.Green;
                                locationField.Content = location;
                                buff2 = buff[2].Split(' ');
                                IPField.Content = buff2[2].Replace(',', ' ');
                                buff2 = buff[3].Split(' ');
                                IPField.Content += ":" + buff2[2].Replace(',', ' ').Trim();
                                buff2 = buff[7].Split(' ');
                                SHID.Content = buff2[2].Replace('\'', ' ');
                            }));
                            
                            //start checking exfils
                            if(thread1.ThreadState != ThreadState.Running)
                            {
                                thread1.Start();
                            }
                            else
                            {
                                thread1.Resume();
                                
                            }

                            

                            

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

                            if(thread1.ThreadState == ThreadState.Running)
                                thread1.Suspend();

                            
                                Dispatcher.Invoke(new Action(() => { top.Close(); }));

                        }

                    }

                    prevApp.Clear();
                    prevApp.AddRange(nowApp);
                    iteratorApp = nowApp.Count;
                }






            }
           
           
        }

        private void DrawExf(string exf)
        {
            top = new TopWindow(this,exf);
            top.Show();
        }

  
        
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {   if(thread2.IsAlive)
                thread2.Abort();
            File.Delete(exfilPathNew);
            File.Delete(applicationPathNew);
        }
    }
}
