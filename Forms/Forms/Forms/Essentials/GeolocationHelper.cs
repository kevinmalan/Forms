using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Forms.Essentials
{
    public class GeolocationHelper
    {
        public static async Task<Location> GetCurrentLocation()
        {
            Location location = null;

            try
            {
                location = await Geolocation.GetLastKnownLocationAsync();

                /* Following is for getting the Live location. Does not work well on emulator
                 var request = new GeolocationRequest(GeolocationAccuracy.Low);
                 location = await Geolocation.GetLocationAsync(request);
                */

            }
            catch (FeatureNotSupportedException ex)
            {
                Debug.WriteLine($"Geolocation not supported: {ex.Message}");
            }
            catch (FeatureNotEnabledException ex)
            {
                Debug.WriteLine($"Geolocation not enabled: {ex.Message}");
            }
            catch (PermissionException ex)
            {
                Debug.WriteLine($"Geolocation not authorized: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Geolocation error: {ex.Message}");
            }

            return location;
        }
    }
}
