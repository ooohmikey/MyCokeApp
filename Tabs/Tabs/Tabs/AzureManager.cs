using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabs.DataModels;

namespace Tabs
{
    public class AzureManager
    {

        private static AzureManager instance;
        private MobileServiceClient client;
        private IMobileServiceTable<MyCokeModel> myCokeTable;

        private AzureManager()
        {
            this.client = new MobileServiceClient("http://mycoke.azurewebsites.net");
            this.myCokeTable = this.client.GetTable<MyCokeModel>();
        }

        public MobileServiceClient AzureClient
        {
            get { return client; }
        }

        public static AzureManager AzureManagerInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AzureManager();
                }

                return instance;
            }
        }
        public async Task<List<MyCokeModel>> GetCokeInformation()
        {
            return await this.myCokeTable.ToListAsync();
        }
        public async Task PostCokeInformation(MyCokeModel myCokeModel)
        {
            await this.myCokeTable.InsertAsync(myCokeModel);
        }

    }
}
