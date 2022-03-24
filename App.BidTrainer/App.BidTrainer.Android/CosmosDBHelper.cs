using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using App.BidTrainer.Droid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

[assembly: Xamarin.Forms.Dependency(typeof(CosmosDBHelper))]
namespace App.BidTrainer.Droid
{
    public class CosmosDBHelper : ICosmosDBHelper
    {
        public async Task<IEnumerable<Account>> GetAllAccounts()
        {
            var accounts = new List<Account>();

            var queryDefinition = new QueryDefinition("select * from c");
            using var query = GetContainer().GetItemQueryIterator<Account>(queryDefinition);
            {
                while (query.HasMoreResults)
                {
                    accounts.AddRange(await query.ReadNextAsync());
                }
            }
            return accounts;
        }

        public async Task<Account?> GetAccount(string username)
        {
            var querydefinition = new QueryDefinition($"select * from c where c.username = '{username}'");
            using var query = GetContainer().GetItemQueryIterator<Account>(querydefinition);
            while (query.HasMoreResults)
            {
                var account = await query.ReadNextAsync();
                if (account.Count != 0)
                    return account.First();
            }

            return null;
        }

        public async Task InsertAccount(Account account)
        {
            await GetContainer().CreateItemAsync(account);
        }

        public async Task UpdateAccount(Account account)
        {
            await GetContainer().UpsertItemAsync(account);
        }

        private static Container GetContainer()
        {
            var client = new CosmosClient(Constants.EndpointUri, Constants.PrimaryKey,
                new CosmosClientOptions { ApplicationRegion = Regions.WestEurope, });
            return client.GetContainer(Constants.DatabaseName, Constants.CollectionName);
        }

    }
}