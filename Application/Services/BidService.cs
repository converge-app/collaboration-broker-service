using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Application.Exceptions;
using Application.Models.DataTransferObjects;
using Application.Models.Entities;
using Application.Repositories;
using Application.Utility.ClientLibrary;
using Application.Utility.ClientLibrary.Project;
using Application.Utility.Exception;
using Application.Utility.Models;
using Newtonsoft.Json;

namespace Application.Services
{
    public interface IBrokerervice
    {
        Task<Broker> Open(Broker broker);
        Task<bool> Accept(Broker broker, string authorizationToken);
    }

    public class Brokerervice : IBrokerervice
    {
        private readonly IBrokerRepository _brokerRepository;
        private readonly IClient _client;

        public Brokerervice(IBrokerRepository brokerRepository, IClient client)
        {
            _brokerRepository = brokerRepository;
            _client = client;
        }

        public async Task<Broker> Open(Broker broker)
        {
            var project = await _client.GetProjectAsync(broker.ProjectId);
            if (project == null) throw new InvalidBroker();

            var createdBroker = await _brokerRepository.Create(broker);

            return createdBroker ??
                throw new InvalidBroker();
        }

        public async Task<bool> Accept(Broker broker, string authorizationToken)
        {
            var project = await _client.GetProjectAsync(broker.ProjectId);
            if (project == null) throw new InvalidBroker("projectId invalid");

            project.FreelancerId = broker.FreelancerId;

            return await _client.UpdateProjectAsync(authorizationToken, project);
        }
    }
}