using Plugin.AudioRecorder;
using Plugin.Geolocator;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Screen.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Screen
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Todo : ContentPage
	{
        ObservableCollection<Media> Photos = new ObservableCollection<Media>();
        AudioRecorderService record = new AudioRecorderService();
        AudioPlayer player = new AudioPlayer();
        public Todo ()
		{
			InitializeComponent ();
            InitializeGPS();
            cargargrabadora();
		}

        void cargargrabadora()
        {


            record = new AudioRecorderService
            {
                TotalAudioTimeout = TimeSpan.FromSeconds(15),
                StopRecordingOnSilence = true,
                FilePath = "temp"
            };

            player.FinishedPlaying += Player_FinishedPlaying;
        }

        private async void InitializeGPS()
        {
            if(!CrossGeolocator.IsSupported)
            {
                await DisplayAlert("Error", "Ha ocurrido un error al cargar el pluguin", "OK");
                return;
            }
            if(!CrossGeolocator.Current.IsGeolocationAvailable || !CrossGeolocator.Current.IsGeolocationEnabled)
            {
                await DisplayAlert("Error", "Revise la confifuracion GPS de su dispositivo", "OK");
                return;
            }
            CrossGeolocator.Current.PositionChanged += Current_PositionChanged;
            await CrossGeolocator.Current.StartListeningAsync(new TimeSpan(0,0,2),0.50);
            
        }
        double lati;
        double longi;
        private void Current_PositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
        {
            if (!CrossGeolocator.Current.IsListening)
            {
                return;
            }
            var position = CrossGeolocator.Current.GetPositionAsync();
            lat.Text = position.Result.Latitude.ToString();
            lon.Text = position.Result.Longitude.ToString();
            alt.Text = position.Result.Altitude.ToString();
            lati = Convert.ToDouble(lat.Text);
            longi = Convert.ToDouble(lon.Text);
        }

        private async void Fotobotoon_Pressed(object sender, EventArgs e)
        {
            var isInitialize = await CrossMedia.Current.Initialize();
            if(!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported ||!CrossMedia.IsSupported || !isInitialize   )
            {
                await DisplayAlert("Error", "La camara no esta disponible", "OK");
                return;
            }

            var newPhotoID = Guid.NewGuid();

            var photo = await CrossMedia.Current.TakeVideoAsync(new StoreVideoOptions()
            {
                Name = newPhotoID.ToString(),
                SaveToAlbum = true,
                DefaultCamera = CameraDevice.Rear,
                Directory = "Demo Camara"
            });

            if (string.IsNullOrWhiteSpace(photo?.Path))
            {
                return;
            }
            var newPhotoMedia = new Media()
            {
                MediaID = newPhotoID,
                path = photo.Path,
                LocalDatetime = DateTime.Now
            };
            if (!CrossGeolocator.Current.IsListening)
            {
                return;
            }

            using (var ms = new MemoryStream())
            {
                 MiImagen.Source = ImageSource.FromStream(() =>
                {
                    var stream = photo.GetStream();
                    
                    return stream;
                });
                photo.GetStream().CopyTo(ms);
                photo.Dispose();
                ms.ToArray();
                GPS gps = new GPS();
                gps.Latitud = Convert.ToDouble(lat.Text);
                gps.longitud = Convert.ToDouble(lon.Text);
                gps.Imagen = ms.ToArray();
                GpsDao db = new GpsDao();
                //db.Guardarposisicon(gps);
                Photos.Add(newPhotoMedia);
            }
               
            
            listarPhotos.ItemsSource = Photos;
        }

        private async void Posicion_Clicked(object sender, EventArgs e)
        {
            MapLaunchOptions options = new MapLaunchOptions { Name = "Mi posicion" };
            await Map.OpenAsync(lati,longi,options);
            //Navigation.PushAsync(new Mapa());
        }

        private async void PlayStop_Clicked(object sender, EventArgs e)
        {
           if(!record.IsRecording)
            {
                await record.StartRecording();
            }
            else if (record.IsRecording)
            {
                await record.StopRecording();
            }
            
        }

        private void Player_FinishedPlaying(object sender, EventArgs e)
        {
            PlayStop.IsEnabled = true;
            PlayStop.BackgroundColor = Color.FromHex("#7cbb45");
            Reproducir.IsEnabled = true;
            Reproducir.BackgroundColor = Color.FromHex("#7cbb45");
        }

        private void Reproducir_Clicked(object sender, EventArgs e)
        {
            var filepath = record.GetAudioFilePath();
            player.Play(filepath);
        }
    }
}