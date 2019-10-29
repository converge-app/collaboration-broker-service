using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Exceptions;
using Application.Helpers;
using Application.Models.DataTransferObjects;
using Application.Models.Entities;
using Application.Repositories;
using Application.Services;
using Application.Utility;
using Application.Utility.Exception;
using Application.Utility.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;

namespace Application.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class BrokerController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IBrokerRepository _brokerRepository;
        private readonly IBrokerervice _brokerervice;

        public BrokerController(IBrokerervice brokerervice, IBrokerRepository brokerRepository, IMapper mapper)
        {
            _brokerervice = brokerervice;
            _brokerRepository = brokerRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> OpenBroker([FromBody] BrokerCreationDto brokerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var createBroker = _mapper.Map<Broker>(brokerDto);
            try
            {
                var createdBroker = await _brokerervice.Open(createBroker);
                return Ok(createdBroker);
            }
            catch (UserNotFound)
            {
                return NotFound(new MessageObj("User not found"));
            }
            catch (EnvironmentNotSet)
            {
                throw;
            }
            catch (Exception e)
            {
                return BadRequest(new MessageObj(e.Message));
            }
        }

        [HttpPut("{brokerId}")]
        public async Task<IActionResult> AcceptBroker([FromHeader] string authorization, [FromRoute] string brokerId, [FromBody] BrokerUpdateDto brokerDto)
        {
            if (brokerId != brokerDto.Id)
                return BadRequest(new MessageObj("Invalid id(s)"));

            if (!ModelState.IsValid)
                return BadRequest(new { message = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var updateBroker = _mapper.Map<Broker>(brokerDto);
            try
            {
                if (await _brokerervice.Accept(updateBroker, authorization.Split(' ') [1]))
                    return Ok();
                throw new InvalidBroker();
            }
            catch (Exception e)
            {
                return BadRequest(new MessageObj(e.Message));
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var broker = await _brokerRepository.Get();
            var brokerDtos = _mapper.Map<IList<BrokerDto>>(broker);
            return Ok(brokerDtos);
        }

        [HttpGet("freelancer/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByFreelancerId(string id)
        {
            var broker = await _brokerRepository.GetByFreelancerId(id);
            var brokerDto = _mapper.Map<IList<BrokerDto>>(broker);
            return Ok(brokerDto);
        }

        [HttpGet("project/{projectId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByProjectId([FromRoute] string projectId)
        {
            var broker = await _brokerRepository.GetByProjectId(projectId);
            var brokerDtos = _mapper.Map<IList<BrokerDto>>(broker);
            return Ok(brokerDtos);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(string id)
        {
            var broker = await _brokerRepository.GetById(id);
            var brokerDto = _mapper.Map<BrokerDto>(broker);
            return Ok(brokerDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _brokerRepository.Remove(id);
            }
            catch (Exception e)
            {
                return BadRequest(new MessageObj(e.Message));
            }

            return Ok();
        }
    }
}