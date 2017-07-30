using Microsoft.WindowsAzure.MobileServices;
using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabs.DataModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Tabs
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AzureTable : ContentPage
	{
        //MobileServiceClient client = AzureManager.AzureManagerInstance.AzureClient;
        public AzureTable ()
		{
			InitializeComponent ();
		}

        //async void Handle_ClickedAsync(object sender, System.EventArgs e)
        //{
        //    List<MyCokeModel> myCokeInformation = await AzureManager.AzureManagerInstance.GetCokeInformation();

        //    MyCokeList.ItemsSource = myCokeInformation;

        //    await postLocationAsync();
        //}

        /*async Task postLocationAsync()
        {

            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;

            var position = await locator.GetPositionAsync();

            MyCokeModel model = new MyCokeModel()
            {
                Longitude = (float)position.Longitude,
                Latitude = (float)position.Latitude

            };

            await AzureManager.AzureManagerInstance.PostCokeInformation(model);
        }*/
        async void Handle_ClickedAsync(object sender, System.EventArgs e)
        {
            List<MyCokeModel> cokeInformation = await AzureManager.AzureManagerInstance.GetCokeInformation();

            MyCokeList.ItemsSource = cokeInformation;
        }
    }
}