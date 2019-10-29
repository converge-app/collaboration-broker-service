using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Database;
using Application.Models.Entities;
using MongoDB.Driver;

namespace Application.Repositories
{
    public interface IBrokerRepository
    {
        Task<List<Broker>> Get();
        Task<Broker> GetById(string id);
        Task<List<Broker>> GetByProject(string projectId);
        Task<List<Broker>> GetByFreelancerId(string freelancerId);
        Task<List<Broker>> GetByProjectAndFreelancer(string projectId, string freelancerId);
        Task<Broker> Create(Broker broker);
        Task Update(string id, Broker brokerIn);
        Task Remove(Broker brokerIn);
        Task Remove(string id);
        Task<List<Broker>> GetByProjectId(string projectId);
    }

    public class BrokerRepository : IBrokerRepository
    {
        private readonly IMongoCollection<Broker> _broker;

        public BrokerRepository(IDatabaseContext dbContext)
        {
            if (dbContext.IsConnectionOpen())
                _broker = dbContext.Broker;
        }

        public async Task<List<Broker>> Get() => await (await _broker.FindAsync(broker => true)).ToListAsync();
        public async Task<Broker> GetById(string id) => await (await _broker.FindAsync(brokerding => brokerding.Id == id)).FirstOrDefaultAsync();
        public async Task<List<Broker>> GetByProject(string projectId) => await (await _broker.FindAsync(broker => broker.ProjectId == projectId)).ToListAsync();
        public async Task<List<Broker>> GetByFreelancerId(string freelancerId) => await (await _broker.FindAsync(broker => broker.FreelancerId == freelancerId)).ToListAsync();

        public async Task<List<Broker>> GetByProjectAndFreelancer(string projectId, string freelancerId) => await (
            await _broker.FindAsync(
                broker => broker.ProjectId == projectId && broker.FreelancerId == freelancerId)
        ).ToListAsync();

        public async Task<Broker> Create(Broker broker)
            {
                await _broker.InsertOneAsync(broker);
                return broker;
            }

            <<
            <<
            <<
            < HEAD
        public async Task Update(string id, Broker brokerIn)
        {
            await _broker.ReplaceOneAsync(broker => broker.Id == id, brokerIn);
        }

        public async Task Remove(Broker brokerIn)
        {
            await _broker.DeleteOneAsync(broker => broker.Id == brokerIn.Id);
        }

        public async Task Remove(string id)
            {
                await _broker.DeleteOneAsync(broker => broker.Id == id);
            } ==
            ==
            ==
            =
            public async Task Update(string id, Broker brokerIn) => await _broker.ReplaceOneAsync(brokerding => brokerding.Id == id, brokerIn);
        public async Task Remove(Broker brokerIn) => await _broker.DeleteOneAsync(brokerding => brokerding.Id == brokerIn.Id);
        public async Task Remove(string id) => await _broker.DeleteOneAsync(brokerding => brokerding.Id == id);
        public async Task<List<Broker>> GetByProjectId(string projectId) => await (await _broker.FindAsync(broker => broker.ProjectId == projectId)).ToListAsync(); >>
        >>
        >>
        > 4838999 c2254a8a7c9ed9a387befe16e7cd460f6
    }
}