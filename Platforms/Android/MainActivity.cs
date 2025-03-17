using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;

namespace SobradeKontakt
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        const int RequestCameraPermissionId = 1001;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            RequestCameraPermission();
        }

        private void RequestCameraPermission()
        {
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.Camera) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.Camera }, RequestCameraPermissionId);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == RequestCameraPermissionId)
            {
                if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                {
                }
                else
                {
                    Android.Widget.Toast.MakeText(this, "Kaamera kasutamine on keelatud!", Android.Widget.ToastLength.Long)?.Show();
                }
            }
        }
    }
}
