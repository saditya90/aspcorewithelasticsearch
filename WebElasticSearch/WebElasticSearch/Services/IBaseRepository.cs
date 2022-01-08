using System.Threading.Tasks;

namespace WebElasticSearch.Services
{
    public interface IBaseRepository
    {
        int NumberOfShards { get; set; }
        int NumberOfReplicas { get; set; }
        Task<long> GetDocumentsCount();
        Task<bool> IsIndexExists();
        Task RefreshIndex();
        Task DeleteIndexAsync(string indexName); 
    }
}
