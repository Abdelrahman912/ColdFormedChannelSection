using System;
using System.Windows;
using System.Windows.Controls;

namespace ColdFormedChannelSection.App.UI.Views
{
    /// <summary>
    /// Interaction logic for ReportView.xaml
    /// </summary>
    public partial class ReportView : UserControl
    {
        public ReportView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    printDialog.PrintVisual(print, "print Report");
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
           
        }
    }
}
