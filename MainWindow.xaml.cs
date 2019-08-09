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

namespace SteamCompare
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            ApiHelper.InitializeClient();
        }

        private async void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            await LoadGame();

        }

        private async Task LoadGame()
        {
            var steamGame = await SteamProcessor.LoadGame(txtSearch.Text);
            var gogGame = await GogProcessor.LoadGame(txtSearch.Text);
            if (steamGame != null)
            {
                lblSteamGameName.Content = steamGame.name;
                lblSteamPrice.Content = steamGame.price_overview.final_formatted;
            }
            if (gogGame != null)
            {
                lblGogGameName.Content = gogGame.title;
                lblGogPrice.Content = "$"+gogGame.price.finalAmount;
            }
        }


    }
}
