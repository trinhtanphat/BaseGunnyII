using Android.App;
using Android.Content;
using Android.Content.PM;
using GunnyLauncher.Core;

namespace GunnyMobile.Android;

public static class RuffleIntentLauncher
{
    public static bool IsRuntimeInstalled(Context context)
    {
        ArgumentNullException.ThrowIfNull(context);
        try
        {
            _ = context.PackageManager?.GetPackageInfo(
                RuffleAndroidContract.PackageName,
                PackageInfoFlags.Activities);
            return true;
        }
        catch (PackageManager.NameNotFoundException)
        {
            return false;
        }
    }

    public static void Launch(Context context, GameLaunchInfo launch, System.Uri gameBase)
    {
        ArgumentNullException.ThrowIfNull(context);
        var descriptor = RuffleAndroidContract.CreateLaunchDescriptor(launch, gameBase);
        var intent = new Intent(Intent.ActionView, global::Android.Net.Uri.Parse(descriptor.GameUri.AbsoluteUri));
        intent.SetType(descriptor.MimeType);
        intent.SetPackage(descriptor.PackageName);
        if (context is not Activity)
            intent.AddFlags(ActivityFlags.NewTask);
        context.StartActivity(intent);
    }
}
