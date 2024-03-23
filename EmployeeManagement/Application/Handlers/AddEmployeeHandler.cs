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
            return _employeeService.AddEmployee(command);
        }
    }
}
