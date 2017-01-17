using Xamarin.Forms;

namespace DemoAzureOfflineSync
{
	public partial class DemoAzureOfflineSyncPage : ContentPage
	{
		public DemoAzureOfflineSyncPage()
		{
			InitializeComponent();
			BindingContext = new ContactsVM();
		}
	}
}
