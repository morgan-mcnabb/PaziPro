using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;

namespace PaziPro
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            const int requestNotification = 0;
            string[] notiPermissions =
            {
                Manifest.Permission.PostNotifications
            };

            if((int)Build.VERSION.SdkInt < 33) return;

            if(CheckSelfPermission(Manifest.Permission.PostNotifications) != Permission.Granted)
                RequestPermissions(notiPermissions, requestNotification);
        }
    }
}
