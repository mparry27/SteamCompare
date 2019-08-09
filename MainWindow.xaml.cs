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
        private string steamStoreURL = "";
        private string gogStoreURL = "";
        public MainWindow()
        {
            InitializeComponent();
            ApiHelper.InitializeClient();
        }

        private async void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            btnSearch.IsEnabled = false;
            await LoadGame();
            btnSearch.IsEnabled = true;

        }

        private async Task LoadGame()
        {
            
            var steamGame = await SteamProcessor.LoadGame(txtSearch.Text);
            var gogGame = await GogProcessor.LoadGame(txtSearch.Text);
            if (steamGame != null)
            {
                lblSteamGameName.Text = steamGame.name;
                lblSteamPrice.Content = steamGame.price_overview.final_formatted;
                imgSteam.Source = new BitmapImage(new Uri(steamGame.header_image));
                steamStoreURL = steamGame.steam_appid.ToString();
                btnSteamStorePage.IsEnabled = true;
            }
            else
            {
                lblSteamGameName.Text = "Couldn't Find " + txtSearch.Text;
                lblSteamPrice.Content = "";
                imgSteam.Source = null;
                btnSteamStorePage.IsEnabled = false;
            }
            if (gogGame != null)
            {
                lblGogGameName.Text = gogGame.title;
                lblGogPrice.Content = gogGame.price.finalAmount;
                imgGog.Source = new BitmapImage(new Uri("https:" + gogGame.image + ".jpg"));
                gogStoreURL = gogGame.title.Replace(" ","_").ToLower();
                btnGogStorePage.IsEnabled = true;
            }
            else
            {
                lblGogGameName.Text = "Couldn't Find " + txtSearch.Text;
                lblGogPrice.Content = "";
                imgGog.Source = null;
                btnGogStorePage.IsEnabled = false;
            }
        }

        private void BtnGogStorePage_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.gog.com/game/"+gogStoreURL);
        }

        private void BtnSteamStorePage_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://store.steampowered.com/app/"+steamStoreURL);
        }
    }
}
