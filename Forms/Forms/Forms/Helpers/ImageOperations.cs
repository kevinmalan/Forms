using Newtonsoft.Json.Bson;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Diagnostics;
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

            var options = new StoreCameraMediaOptions
            {
                Directory = "Forms",
                Name = $"FormsProfile{DateTime.Now.ToString("yyyyMMdd:hh:mm:ss")}.jpg",
                PhotoSize = PhotoSize.Medium,
                CompressionQuality = 50
            };

            MediaFile file = null;

            try
            {
                file = await CrossMedia.Current.TakePhotoAsync(options);
            }
            catch(MediaPermissionException e)
            {
                Debug.WriteLine($"Camera Permissions Not Authorized: {e.Message}");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Camera Access Issue: {e.Message}");
            }

            return file;
        }

        private static async Task<MediaFile> GetGalleryPhoto()
        {
            await CrossMedia.Current.Initialize();

            var mediaOptions = new PickMediaOptions
            {
                PhotoSize = PhotoSize.Medium,
                CompressionQuality = 50
            };

            MediaFile file = null;

            try
            {
                file = await CrossMedia.Current.PickPhotoAsync(mediaOptions);
            }
            catch (MediaPermissionException e)
            {
                Debug.WriteLine($"Storage Permissions Not Authorized: {e.Message}");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Storage Access Issue: {e.Message}");
            }

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