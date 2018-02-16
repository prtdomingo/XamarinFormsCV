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
        public ICommand CapturePhotoCommand { get; }
        private const double _confidenceLevelThreshold = 0.5;
        private string _tagName = "";

        public string TagName 
        {
            get => _tagName;
            set => Set(ref _tagName, value);
        }

        public CameraViewModel()
        {
            CapturePhotoCommand = new Command(async () => await CapturePhoto());
        }

        private async Task CapturePhoto()
        {
            var options = new StoreCameraMediaOptions { PhotoSize = PhotoSize.Medium };
            var file = await CrossMedia.Current.TakePhotoAsync(options);
            var classificationModel = await GetBestTag(file);
            TagName = classificationModel != null ? $"Confidence Level: {classificationModel.Value.Probability} | Name: {classificationModel.Value.Tag}" : "I don't know what this is";

            // Delete the photo the user just taken
            DeletePhoto(file);
        }



        private async Task<ImageClassification?> GetBestTag(MediaFile file)
        {
            try
            {
                using (var stream = file.GetStream())
                {
                    var tags = await CrossImageClassifier.Current.ClassifyImage(stream);
                    return tags.OrderByDescending(t => t.Probability).FirstOrDefault(t => t.Probability >= _confidenceLevelThreshold);
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
