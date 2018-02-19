using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xam.Plugins.OnDeviceCustomVision;
using Xamarin.Forms;
using System.Linq;

namespace XamarinFormsCV
{
    public class CameraViewModel : ViewModelBase
    {
        private const double _confidenceLevelThreshold = 0.5;
        private string _message = "";
        private bool _canTakePhoto = true;

        public bool CanTakePhoto
        {
            get => _canTakePhoto;
            set
            {
                if (Set(ref _canTakePhoto, value))
                    RaisePropertyChanged(nameof(ShowSpinner));
            }
        }

        public bool ShowSpinner => !CanTakePhoto;

        public string Message 
        {
            get => _message;
            set => Set(ref _message, value);
        }

        public ICommand CapturePhotoCommand { get; }

        public CameraViewModel()
        {
            CapturePhotoCommand = new Command(async () => await CapturePhoto());
        }

        private async Task CapturePhoto()
        {
            CanTakePhoto = false;

            var options = new StoreCameraMediaOptions { PhotoSize = PhotoSize.Medium };
            var file = await CrossMedia.Current.TakePhotoAsync(options);
            var classificationModel = await IdentifyImage(file);
            CanTakePhoto = true;

            if(classificationModel != null && classificationModel.Value.Probability != 0 && !string.IsNullOrEmpty(classificationModel.Value.Tag))
            {
                Message = $"Confidence Level: {classificationModel.Value.Probability.ToString("0.0%")} | Name: {classificationModel.Value.Tag}";
            }
            else 
            {
                Message = "I don't know what this is";
            }

            // Delete the photo the user just taken
            DeletePhoto(file);
        }

        private async Task<ImageClassification?> IdentifyImage(MediaFile file)
        {
            try
            {
                using (var stream = file.GetStream())
                {
                    var tags = await CrossImageClassifier.Current.ClassifyImage(stream);

                    return tags.OrderByDescending(t => t.Probability).FirstOrDefault();
                }
            }
            catch(Exception)
            {
                return null;
            }
        }

        private static void DeletePhoto(MediaFile file)
        {
            var path = file?.Path;

            if (!string.IsNullOrEmpty(path) && File.Exists(path))
                File.Delete(file?.Path);
        }
    }
}
