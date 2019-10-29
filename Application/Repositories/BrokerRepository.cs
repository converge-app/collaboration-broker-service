using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Database;
using Application.Models.Entities;
using MongoDB.Driver;

namespace Application.Repositories
{
    public interface IBrokerRepository
    {
        Task<List<Result>> Get();
        Task<Result> GetById(string id);
        Task<List<Result>> GetByProject(string projectId);
        Task<Result> Create(Result broker);
        Task Update(string id, Result brokerIn);
        Task Remove(Result brokerIn);
        Task Remove(string id);
        Task<Result> GetByProjectId(string projectId);
    }

    public class BrokerRepository : IBrokerRepository
    {
        private readonly IMongoCollection<Result> _broker;

        public BrokerRepository(IDatabaseContext dbContext)
        {
            if (dbContext.IsConnectionOpen())
                _broker = dbContext.Broker;
        }

        public async Task<List<Result>> Get() => await (await _broker.FindAsync(broker => true)).ToListAsync();
        public async Task<Result> GetById(string id) => await (await _broker.FindAsync(brokerding => brokerding.Id == id)).FirstOrDefaultAsync();
        public async Task<List<Result>> GetByProject(string projectId) => await (await _broker.FindAsync(broker => broker.ProjectId == projectId)).ToListAsync();

        public async Task<Result> Create(Result broker)
        {
            await _broker.InsertOneAsync(broker);
            return broker;
        }

        public async Task Update(string id, Result brokerIn)
        {
            await _broker.ReplaceOneAsync(broker => broker.Id == id, brokerIn);
        }

        public async Task Remove(Result brokerIn)
        {
            await _broker.DeleteOneAsync(broker => broker.Id == brokerIn.Id);
        }

        public async Task Remove(string id)
        {
            await _broker.DeleteOneAsync(broker => broker.Id == id);
        }

        public async Task<Result> GetByProjectId(string projectId) => await (await _broker.FindAsync(broker => broker.ProjectId == projectId)).FirstOrDefaultAsync();
    }
}