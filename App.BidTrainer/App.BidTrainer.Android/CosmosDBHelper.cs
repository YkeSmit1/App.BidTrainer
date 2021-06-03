using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using App.BidTrainer.Droid;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.BidTrainer;

[assembly: Xamarin.Forms.Dependency(typeof(CosmosDBHelper))]
namespace App.BidTrainer.Droid
{
    public class CosmosDBHelper : ICosmosDBHelper
    {
        private static readonly DocumentClient client = new DocumentClient(new Uri(Constants.EndpointUri), Constants.PrimaryKey);
        private static readonly Uri collectionLink = UriFactory.CreateDocumentCollectionUri(Constants.DatabaseName, Constants.CollectionName);
        private static readonly FeedOptions feedOptions = new FeedOptions() { EnableCrossPartitionQuery = true };

        public async Task<IEnumerable<Account>> GetAllAccounts()
        {
            var accounts = new List<Account>();
            var query = client.CreateDocumentQuery<Account>(collectionLink, feedOptions).AsDocumentQuery();
            while (query.HasMoreResults)
            {
                accounts.AddRange(await query.ExecuteNextAsync<Account>());
            }
            return accounts;
        }

        public async Task<Account?> GetAccount(string username)
        {
            var account = await client.CreateDocumentQuery<Account>(collectionLink, feedOptions).
                Where(x => x.username == username).AsDocumentQuery().ExecuteNextAsync<Account>();
            return account.FirstOrDefault();
        }

        public async Task InsertAccount(Account account)
        {
            await client.CreateDocumentAsync(collectionLink, account);
        }

        public async Task UpdateAccount(Account account)
        {
            await client.UpsertDocumentAsync(collectionLink, account);
        }
    }
}