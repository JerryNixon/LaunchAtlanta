using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace LaunchFaceSwapTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var foregroundBitmap = await GetAsset("Foreground.png");

            var backgroundBitmap = await GetAsset("Background.png");

            var service = new LaunchFaceSwapLibrary.LaunchFaceSwapService();

            var bitmap = await service.SwapFacesAsync(backgroundBitmap, foregroundBitmap);

            var source = await LaunchFaceSwapLibrary.ConvertTo.SoftwareBitmapSource.FromSoftwareBitmap(bitmap);

            Background = new ImageBrush
            {
                ImageSource = source,
                AlignmentX = AlignmentX.Center,
                AlignmentY = AlignmentY.Center,
                Stretch = Stretch.UniformToFill,
            };
        }


        private async Task<Windows.Graphics.Imaging.SoftwareBitmap> GetAsset(string name)
        {
            var packageFolder = Package.Current.InstalledLocation;

            var assetsFolder = await packageFolder.GetFolderAsync("Assets");

            var file = await assetsFolder.GetFileAsync(name);

            var path = file.Path;

            var service = new LaunchFaceSwapLibrary.LaunchFaceSwapService();

            return await LaunchFaceSwapLibrary.ConvertTo.SoftwareBitmap.FromFilePathAsync(path);
        }
    }
}
