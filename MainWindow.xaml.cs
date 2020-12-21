using CLOC.Manager.CLOLC_System;
using CLOC.Manager.Email;
using System.Windows;

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

            this.EmailText.Visibility = Visibility.Hidden;
            this.SendEmailBttn.Visibility = Visibility.Hidden;
            this.SendToLabel.Visibility = Visibility.Hidden;            

            CLOCSystemManager.Instance.ShowOutput += OnOutPutRecived;
        }

        private void OnOutPutRecived()
        {
            this.EmailText.Visibility = Visibility.Visible;
            this.SendEmailBttn.Visibility = Visibility.Visible;
            this.SendToLabel.Visibility = Visibility.Visible;

            this.OutputBody.Text = $"REPORT RECIVED";
            this.OutputBody.Text += $"\nREPOSITORY: {this.PathText.Text}\n";
            this.OutputBody.Text += $"\n{ CLOCSystemManager.Instance.output}";
        }
                
        private void SendEmailBttn_Click(object sender, RoutedEventArgs e)
        {
            EmailManager.Instance.SendEmail( reciverEmail: this.EmailText.Text,
                                             subject: "CLOC Report",
                                             body: this.OutputBody.Text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.OutputBody.Text = $"Recieving data from repository: {this.PathText.Text} ...";
            CLOCSystemManager.Instance.StartRepositoryCount(this.PathText.Text);
        }

    }
}
