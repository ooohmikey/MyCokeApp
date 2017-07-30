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

            //await postLocationAsync();

            await MakePredictionRequest(file);
            file.Dispose();
        }

        //async Task postLocationAsync()
        //{
        //    var locator = CrossGeolocator.Current;
        //    locator.DesiredAccuracy = 50;
        //    var position = await locator.GetPositionAsync();
        //    MyCokeModel model = new MyCokeModel()
        //    {
        //        Longitude = (float)position.Longitude,
        //        Latitude = (float)position.Latitude
        //    };
        //    //await AzureManager.AzureManagerInstance.PostCokeInformation(model);

        //}

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

                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);


                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    ///*
                    JObject rss = JObject.Parse(responseString);

                    var Probability = from p in rss["Predictions"] select (int)p["Probability"];
                    var Tag = from p in rss["Predictions"] select (string)p["Tag"];

                    foreach (var item in Tag)
                    {
                        TagLabel.Text += item + ": \n";
                    }

                    foreach (var item in Probability)
                    {

                        PredictionLabel.Text += item + "\n";
                        if (item == 1)
                        {
                            MyCokeModel model = new MyCokeModel()
                            {
                                Tag = "You have a coke"
                            };
                            await AzureManager.AzureManagerInstance.PostCokeInformation(model);
                        }
                    }
                    //*/
                    /*

                    EvaluationModel responseModel = JsonConvert.DeserializeObject<EvaluationModel>(responseString);

                    double max = responseModel.Predictions.Max(m => m.Probability);

                    TagLabel.Text = (max >= 0.5) ? "Beagle" : "Border Collie";
                    */
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