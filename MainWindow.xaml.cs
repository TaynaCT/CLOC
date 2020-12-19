using CLOC.Manager.CLOLC_System;
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

namespace CLOC
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CLOLCSystemManager.Instance.OnOutputRecieved = OnOutPutRecived;
        }

        private void OnOutPutRecived()
        {
            this.OutputBody.Text = CLOLCSystemManager.Instance.output;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CLOLCSystemManager.Instance.StartRepositoryCount("D:\\Projects\\Python\\first_app");
        }
    }
}
