using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        private readonly IBrokerService _brokerService;

        public BrokerController(IBrokerService brokerervice, IBrokerRepository brokerRepository, IMapper mapper)
        {
            _brokerService = brokerervice;
            _brokerRepository = brokerRepository;
            _mapper = mapper;
        }

        [HttpPost("initialize")]
        public async Task<IActionResult> SubmitResult([FromHeader] string authorization, [FromBody] ResultCreationDto resultDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var result = _mapper.Map<Result>(resultDto);
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.Name);
                if (userId != resultDto.UserId)
                {
                    throw new InvalidResult("Incorrect userId");
                }

                var initializedResult = await _brokerService.InitializeResult(authorization.Split(' ') [1], result, userId);

                InitializedResultDto initializedResultDto = _mapper.Map<InitializedResultDto>(initializedResult);
                return Ok(initializedResultDto);
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

        [HttpPost("pay")]
        public async Task<IActionResult> Pay([FromHeader] string authorization, [FromBody] PayCreationDto payCreationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var result = _mapper.Map<Result>(payCreationDto);
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.Name);
                if (userId != payCreationDto.UserId)
                {
                    throw new InvalidResult("Incorrect userId");
                }

                var paidForResult = await _brokerService.PayForResult(authorization.Split(' ') [1], result, userId);

                PaidForResultDto paidForResultDto = _mapper.Map<PaidForResultDto>(paidForResult);
                return Ok(paidForResultDto);
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

    }
}