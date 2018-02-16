using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace XamarinFormsCV
{
    public class CameraViewModel : ViewModelBase
    {
        public ICommand CapturePhotoCommand { get; }

        public CameraViewModel()
        {
            CapturePhotoCommand = new Command(async () => await CapturePhoto());
        }

        private async Task CapturePhoto()
        {
            var options = new StoreCameraMediaOptions { PhotoSize = PhotoSize.Medium };
            var file = await CrossMedia.Current.TakePhotoAsync(options);
        }

        private static void DeletePhoto(MediaFile file)
        {
            var path = file?.Path;

            if (!string.IsNullOrEmpty(path) && File.Exists(path))
                File.Delete(file?.Path);
        }
    }
}
