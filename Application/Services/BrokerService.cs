using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Application.Exceptions;
using Application.Models.DataTransferObjects;
using Application.Models.Entities;
using Application.Repositories;
using Application.States;
using Application.Utility.ClientLibrary;
using Application.Utility.ClientLibrary.Collaboration;
using Application.Utility.ClientLibrary.Project;
using Application.Utility.Exception;
using Application.Utility.Models;
using Newtonsoft.Json;

namespace Application.Services
{
    public interface IBrokerService
    {
        Task<Result> InitializeResult(string authToken, Result result, string userId);
        Task<Result> PayForResult(string authToken, Result result, string userId);
        Task<Result> CompleteProject(string authToken, string projectId);
    }

    public class BrokerService : IBrokerService
    {
        private readonly IBrokerRepository _brokerRepository;
        private readonly IClient _client;

        public BrokerService(IBrokerRepository brokerRepository, IClient client)
        {
            _brokerRepository = brokerRepository;
            _client = client;
        }

        public async Task<Result> CompleteProject(string authToken, string projectId)
        {
            var project = await _client.GetProjectAsync(projectId);

            if (project == null)
                throw new InvalidResult("Couldn't fetch project for projectId: " + projectId);

            var result = await _brokerRepository.GetByProjectId(project.Id);
            if (result == null)
                throw new InvalidResult("Result doesn't exist");

            if (result.State != ResultStates.PaymentSubmittet)
                throw new InvalidResult("Couldn't be set to done, as the result hasn't processed payments yet");

            var toReplace = result;
            toReplace.State = ResultStates.Done;

            try
            {
                await _brokerRepository.Update(result.Id, toReplace);
            }
            catch (System.Exception)
            {
                throw;
            }

            try
            {
                bool response = await _client.PostEvent(authToken, new EventData
                {
                    Type = "result",
                        OwnerId = result.EmployerId,
                        ProjectId = result.ProjectId,
                        Content = JsonConvert.SerializeObject(new
                        {
                            ResultId = result.Id,
                                ProjectId = result.ProjectId,
                                Status = ResultStates.Done,
                                Files = result.FileUrl
                        })

                });
                if (!response)
                    throw new InvalidResult("Service failed");
            }
            catch (Exception e)
            {
                await _brokerRepository.Update(result.Id, result);
                throw new InvalidResult("Couldn't bind to service Collaboration: " + e.Message);
            }

            return toReplace;
        }

        public async Task<Result> InitializeResult(string authToken, Result result, string userId)
        {
            result.FreelancerId = userId;
            result.State = ResultStates.FilesUploaded;

            var exists = await _brokerRepository.GetByProjectId(result.ProjectId);
            if (exists != null)
                throw new InvalidResult("Result already exists, cannot initialize");

            var createdResult = await _brokerRepository.Create(result);

            if (createdResult != null)
            {
                try
                {
                    bool response = await _client.PostEvent(authToken, new EventData
                    {
                        Type = "result",
                            OwnerId = userId,
                            ProjectId = result.ProjectId,
                            Content = JsonConvert.SerializeObject(new
                            {
                                ResultId = createdResult.Id,
                                    ProjectId = createdResult.ProjectId,
                                    Status = ResultStates.FilesUploaded
                            })

                    });
                    if (!response)
                        throw new InvalidResult("Service failed");
                }
                catch (Exception e)
                {
                    await _brokerRepository.Remove(createdResult);
                    throw new InvalidResult("Couldn't bind to service Collaboration: " + e.Message);
                }

            }

            return createdResult ??
                throw new InvalidResult();
        }

        public async Task<Result> PayForResult(string authToken, Result result, string userId)
        {
            var originalResult = await _brokerRepository.GetByProjectId(result.ProjectId);
            if (originalResult == null)
                throw new InvalidResult("Result doesn't exist, cannot update state");

            if (string.IsNullOrEmpty(originalResult.EmployerId))
                throw new InvalidResult("Payment has already been submitted");

            var toReplace = originalResult;
            toReplace.EmployerId = userId;
            toReplace.State = ResultStates.PaymentSubmittet;

            try
            {

                await _brokerRepository.Update(originalResult.Id, toReplace);
            }
            catch (System.Exception)
            {

                throw;
            }

            try
            {
                var bids = await _client.GetBidsForProject(toReplace.ProjectId);
                var bid = bids.FirstOrDefault(b => b.FreelancerId == toReplace.FreelancerId);

                var response = await _client.Transfer(authToken, new TransferData
                {
                    SenderId = userId,
                        ReceiverId = toReplace.FreelancerId,
                        Amount = (long) bid.Amount * 100
                });

                if (!response)
                    throw new InvalidResult("Payments service failed");
            }
            catch (Exception e)
            {
                await _brokerRepository.Update(originalResult.Id, originalResult);
                throw new InvalidResult("Couldn't bind to service Payment: " + e.Message);
            }

            try
            {
                bool response = await _client.PostEvent(authToken, new EventData
                {
                    Type = "result",
                        OwnerId = userId,
                        ProjectId = result.ProjectId,
                        Content = JsonConvert.SerializeObject(new
                        {
                            ResultId = toReplace.Id,
                                ProjectId = toReplace.ProjectId,
                                Status = ResultStates.PaymentSubmittet,
                                Files = toReplace.FileUrl,
                        })

                });
                if (!response)
                    throw new InvalidResult("Service failed");
            }
            catch (Exception e)
            {
                throw new InvalidResult("Couldn't bind to service Collaboration: " + e.Message);
            }

            return toReplace;
        }
    }
}