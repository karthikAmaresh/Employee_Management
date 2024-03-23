using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands
{
    public class AddEmployeeCommand : IRequest<Unit>
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string company { get; set; }
        public string email { get; set; }
        public long phone { get; set; }
    }
}
