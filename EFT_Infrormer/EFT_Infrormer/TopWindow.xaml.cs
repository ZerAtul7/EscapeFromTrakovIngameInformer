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
using System.Windows.Shapes;

namespace EFT_Infrormer
{
    /// <summary>
    /// Логика взаимодействия для TopWindow.xaml
    /// </summary>
    public partial class TopWindow : Window
    {
        MainWindow main { get; set; }
        public TopWindow(MainWindow window,string message)
        {
            main = window;
            Topmost = true;
            InitializeComponent();
            Exfils.Content = message;
            
        }



        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
           DragMove(); 
        }
    }
}
