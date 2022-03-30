
using System;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MyFreeCatWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private BitmapSource bitmapSource;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Task task = doAsyncHttpAsync();
           
        }

        private async Task doAsyncHttpAsync()
        {
            // Get Reqeust
            HttpClient req = new HttpClient();
            var content = await req.GetAsync("https://api.thecatapi.com/v1/images/search");
            if (content.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var response = await content.Content.ReadAsStringAsync();
                Debug.WriteLine(response);

                // [{ "breeds":[],"id":"MTkyOTcwNQ","url":"https://cdn2.thecatapi.com/images/MTkyOTcwNQ.jpg","width":1024,"height":1024}]
                // [{ "breeds":[],"id":"agp","url":"https://cdn2.thecatapi.com/images/agp.jpg","width":560,"height":242}]
                // [{ "breeds":[],"id":"3fs","url":"https://cdn2.thecatapi.com/images/3fs.jpg","width":500,"height":333}]
                // [{ "breeds":[],"id":"af6","url":"https://cdn2.thecatapi.com/images/af6.jpg","width":1280,"height":960}]

                var arrayCatImageData = JsonConvert.DeserializeObject<CatImageData[]>(response) ;
                
                var requestImg = WebRequest.Create(arrayCatImageData[0].url);

                using (var responseImg = requestImg.GetResponse())
                using (var streamImg = responseImg.GetResponseStream())
                {
                    Bitmap bitmap = (Bitmap)Bitmap.FromStream(streamImg);
                    //BitmapImage bitmapImage = Convert(B);
                    this.imgDynamic.Source = ImageSourceFromBitmap(bitmap);
                }

            }
        }

        public class CatImageData
        {
            public string id;
            public string url;
            public string width;
            public string height;
        }

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        public ImageSource ImageSourceFromBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }

    }

}
