using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.BidTrainer
{
    public interface ICosmosDBHelper
    {
        Task<IEnumerable<Account>> GetAllAccounts();

        Task<Account?> GetAccount(string username);

        Task InsertAccount(Account account);

        Task UpdateAccount(Account account);
    }
}