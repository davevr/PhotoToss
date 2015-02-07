using Android.App;
using Android.Content;

namespace PhotoToss
{
	[BroadcastReceiver]
	[IntentFilter(new[] { Intent.ActionBootCompleted })]
	public class MyGCMBootReceiver : BroadcastReceiver
	{
		public override void OnReceive(Context context, Intent intent)
		{
			RemoteNotificationService.RunIntentInService(context, intent);
			SetResult(Result.Ok, null, null);
		}
	}
}