using Application.Commands;
using Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers
{
    public class AddEmployeeHandler : IRequestHandler<AddEmployeeCommand, Unit>
    {
        private readonly IEmployeeService _employeeService;

        public AddEmployeeHandler(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public Task<Unit> Handle(AddEmployeeCommand command, CancellationToken cancellationToken)
        {
            if(command.type == null || command.type == 0)
            {
                return _employeeService.AddEmployee(command);
            }
            else
            {
                return _employeeService.AddEmployeeUsingServiceBus(command);

            }
        }
    }
}
