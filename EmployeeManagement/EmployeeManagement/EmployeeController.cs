using Application.Commands;
using Application.Interfaces;
using Application.Queries;
using Application.Validators;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<EmployeeController> _logger;
        private readonly IEmployeeService _employeeService;
        public EmployeeController(IMediator mediator, ILogger<EmployeeController> logger , IEmployeeService employeeService)
        {
            _mediator = mediator;
            _logger = logger;
            _employeeService = employeeService;
        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            _logger.LogInformation($"Retrieving Employee List");
            var result = await _mediator.Send(new GetAllEmployees());
            return Ok(result);
        }
        
        // POST api/items
        [HttpPost("direct approach")]
        public async Task<IActionResult> AddEmployee([FromBody] AddEmployeeCommand command)
        {
            _logger.LogInformation($"Creating Employee {command.firstName} {command.lastName}");
            var validate = new EmployeeEmailVaidator();
            var validationResult = validate.Validate(command);
            if (validationResult.IsValid)
            {
                try
                {
                    if (command.company != null
                    && command.email != null)
                    {
                        var address = new System.Net.Mail.MailAddress(command.email);
                        var emailDomain = address.Host;

                        var isDomainExists = emailDomain.Equals(command.company + ".com", StringComparison.OrdinalIgnoreCase);
                        if (!isDomainExists)
                        {
                            ModelState.AddModelError(nameof(Employee.email), "please provide email provided by company domain!");
                        }

                    }
                    if (!ModelState.IsValid)
                    {
                        return UnprocessableEntity(ModelState);
                    }
                    var result = await _mediator.Send(command);
                    return new OkObjectResult(result);
                }
                catch (Exception ex)
                {
                    return new BadRequestResult();
                }
            }
            else
            {
                return new BadRequestObjectResult(validationResult.Errors.Select(x => x.ErrorMessage));
            }
        }

        [HttpPost("service bus approach")]
        public async Task<IActionResult> AddEmployeeThroughServiceBus([FromBody] AddEmployeeCommand command)
        {
            _logger.LogInformation($"Creating Employee {command.firstName} {command.lastName}");
            var validate = new EmployeeEmailVaidator();
            var validationResult = validate.Validate(command);
            if (validationResult.IsValid)
            {
                try
                {
                    if (command.company != null
                    && command.email != null)
                    {
                        var address = new System.Net.Mail.MailAddress(command.email);
                        var emailDomain = address.Host;

                        var isDomainExists = emailDomain.Equals(command.company + ".com", StringComparison.OrdinalIgnoreCase);
                        if (!isDomainExists)
                        {
                            ModelState.AddModelError(nameof(Employee.email), "please provide email provided by company domain!");
                        }

                    }
                    if (!ModelState.IsValid)
                    {
                        return UnprocessableEntity(ModelState);
                    }
                    var result = await _mediator.Send(command);
                    return new OkObjectResult(result);
                }
                catch (Exception ex)
                {
                    return new BadRequestResult();
                }
            }
            else
            {
                return new BadRequestObjectResult(validationResult.Errors.Select(x => x.ErrorMessage));
            }
        }
    }
}

