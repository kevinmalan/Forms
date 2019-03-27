using Newtonsoft.Json.Bson;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Forms.Helpers
{
    public class ImageOperations
    {
        public static async Task<byte[]> GetCameraPhotoBytes()
        {
            var mediaFile = await GetCameraPhoto();

            if (mediaFile == null)
                return default(byte[]);

            var stream = mediaFile.GetStream();

            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);

                return ms.ToArray();
            }
        }

        public static async Task<byte[]> GetGalleryPhotoBytes()
        {
            var mediaFile = await GetGalleryPhoto();

            if (mediaFile == null)
                return default(byte[]);

            var stream = mediaFile.GetStream();

            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);

                return ms.ToArray();
            }
        }

        private static async Task<MediaFile> GetCameraPhoto()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await App.Current.MainPage.DisplayAlert("No Camera", ":( No camera available.", "OK");
                return null;
            }

            var options = new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Forms",
                Name = $"FormsProfile{DateTime.Now.ToString("yyyyMMdd:hh:mm:ss")}.jpg",
                PhotoSize = PhotoSize.Medium,
                CompressionQuality = 50
            };

            var file = await CrossMedia.Current.TakePhotoAsync(options);

            return file;
        }

        private static async Task<MediaFile> GetGalleryPhoto()
        {
            await CrossMedia.Current.Initialize();

            var mediaOptions = new Plugin.Media.Abstractions.PickMediaOptions
            {
                PhotoSize = PhotoSize.Medium,
                CompressionQuality = 50
            };

            var file = await CrossMedia.Current.PickPhotoAsync(mediaOptions);

            return file;
        }

        public static ImageSource StreamToImageSource(Stream stream)
        {
            var source = ImageSource.FromStream(() =>
            {
                return stream;
            });

            return source;
        }
    }
}