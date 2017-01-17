using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System.Collections.ObjectModel;

namespace DemoAzureOfflineSync
{
    public class AzureClient
    {
        private IMobileServiceClient _client;
		private IMobileServiceSyncTable<Contact> _table;
		const string dbPath = "contactDb";
		private const string serviceUri= "http://mod5sample.azurewebsites.net/";
        
		public AzureClient()
        {
			_client = new MobileServiceClient(serviceUri);
			var store = new MobileServiceSQLiteStore(dbPath);
			store.DefineTable<Contact>();
			_client.SyncContext.InitializeAsync(store);
			_table = _client.GetSyncTable<Contact>();
        }

        public async Task<IEnumerable<Contact>> GetContacts()
        {
            var empty = new Contact[0];
            try
            {
				if (Plugin.Connectivity.CrossConnectivity.Current.IsConnected)
					await SyncAsync();

                return await _table.ToEnumerableAsync();
            }
            catch (Exception ex)
            {
                return empty;
            }
        }

		public async void AddContact(Contact contact) 
		{
			await _table.InsertAsync(contact);

		}

		public async Task SyncAsync() 
		{ 
			ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;
			try
			{
				await _client.SyncContext.PushAsync();

				await _table.PullAsync("allContacts", _table.CreateQuery());
			}
			catch (MobileServicePushFailedException pushEx)
			{
				if (pushEx.PushResult != null)
					syncErrors = pushEx.PushResult.Errors;
			}
		}


		public async Task CleanData() 
		{
			await _table.PurgeAsync("allContacts", _table.CreateQuery(), new System.Threading.CancellationToken());
		}

    }
}
