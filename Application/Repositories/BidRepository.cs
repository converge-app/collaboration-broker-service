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
    }

    public class BrokerRepository : IBrokerRepository
    {
        private readonly IMongoCollection<Broker> _broker;

        public BrokerRepository(IDatabaseContext dbContext)
        {
            if (dbContext.IsConnectionOpen())
                _broker = dbContext.Broker;
        }

        public async Task<List<Broker>> Get()
        {
            return await (await _broker.FindAsync(broker => true)).ToListAsync();
        }

        public async Task<Broker> GetById(string id)
        {
            return await (await _broker.FindAsync(broker => broker.Id == id)).FirstOrDefaultAsync();
        }

        public async Task<List<Broker>> GetByProject(string projectId)
        {
            return await (await _broker.FindAsync(broker => broker.ProjectId == projectId)).ToListAsync();
        }

        public async Task<List<Broker>> GetByFreelancerId(string freelancerId)
        {
            return await (await _broker.FindAsync(broker => broker.FreelancerId == freelancerId)).ToListAsync();
        }

        public async Task<List<Broker>> GetByProjectAndFreelancer(string projectId, string freelancerId)
        {
            return await (
                await _broker.FindAsync(
                    broker => broker.ProjectId == projectId && broker.FreelancerId == freelancerId)
            ).ToListAsync();
        }

        public async Task<Broker> Create(Broker broker)
        {
            await _broker.InsertOneAsync(broker);
            return broker;
        }

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
        }
    }
}