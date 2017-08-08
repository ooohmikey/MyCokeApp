using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Tabs.Model;
using Xamarin.Forms;
using Newtonsoft.Json.Linq;
using Plugin.Geolocator;
using Tabs.DataModels;
using Xamarin.Forms.Xaml;
using System.Collections.Generic;

namespace Tabs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomVision : ContentPage
    {
        public CustomVision()
        {
            InitializeComponent();
        }

        private async void loadCamera(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                PhotoSize = PhotoSize.Medium,
                Directory = "Sample",
                Name = $"{DateTime.UtcNow}.jpg"
            });

            if (file == null)
                return;

            image.Source = ImageSource.FromStream(() =>
            {
                return file.GetStream();
            });

            await MakePredictionRequest(file);
        }

        static byte[] GetImageAsByteArray(MediaFile file)
        {
            var stream = file.GetStream();
            BinaryReader binaryReader = new BinaryReader(stream);
            return binaryReader.ReadBytes((int)stream.Length);
        }

        async Task MakePredictionRequest(MediaFile file)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Prediction-Key", "2fec324ff2714dceaa16c8e2af738c65");

            string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Prediction/94496237-dc49-4022-9641-ed8944743291/image";

            HttpResponseMessage response;

            byte[] byteData = GetImageAsByteArray(file);

            using (var content = new ByteArrayContent(byteData))
            {
                TagLabel.Text = "";
                PredictionLabel.Text = "";
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);


                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    JObject rss = JObject.Parse(responseString);

                    var Probability = from p in rss["Predictions"] select (int)p["Probability"];
                    var Tag = from p in rss["Predictions"] select (string)p["Tag"];
                    List<String> list = new List<String>();

                    foreach (var item in Tag)
                    {
                        TagLabel.Text += item + ": \n";
                        list.Add(item);
                    }
                    MyCokeModel model = new MyCokeModel();
                    model.Tag = "Not recognised as this type of coke!";
                    int index = 0;
                    foreach (var item in Probability)
                    {
                        PredictionLabel.Text += item + "\n";
                        if (item == 1)
                        {
                            model.Tag = "You drank " + list[index];
                        }
                        index += 1;
                    }
                        await AzureManager.AzureManagerInstance.PostCokeInformation(model);
                }
                else
                {
                    TagLabel.Text = "Something went wrong";
                }
                file.Dispose();
            }
        }
    }
}