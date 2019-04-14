using Android.App;
using Android.Content;
using Android.Hardware.Fingerprints;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Forms.Helpers
{
    public class FingerPrintOperations
    {
        private static bool HasLockScreen()
        {
            KeyguardManager keyguardManager = (KeyguardManager)Application.Context.GetSystemService(Context.KeyguardService);

            var isSecure = keyguardManager.IsKeyguardSecure;

            return isSecure;
        }

        public static bool SupportsFingerScan()
        {
            FingerprintManager manager = Application.Context.GetSystemService(Context.FingerprintService) as FingerprintManager;

            var isHardware = manager.IsHardwareDetected;
            var hasEnrolledPrints = manager.HasEnrolledFingerprints;
            var hasLockScreen = HasLockScreen();

            return isHardware && hasEnrolledPrints && hasLockScreen;
        }

        public static async Task AddToSecureStorage(string key, string value)
        {
            await SecureStorage.SetAsync(key, value);
        }

        public static async Task<string> GetValueFromSecureStorage(string key)
        {
            var value = await SecureStorage.GetAsync(key);

            return value;
        }

        public static void RemoveFromSecureStorage(string key)
        {
            SecureStorage.Remove(key);
        }
    }
}