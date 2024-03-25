using Application.Commands;
using Application.Interfaces;
using Azure.Messaging.ServiceBus;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ILogger<EmployeeService> _logger;
        private readonly IConfiguration _configuration;

        public EmployeeService(
            IConfiguration configuration,
            ILogger<EmployeeService> logger)
        {
            _logger = logger;
            _configuration = configuration;
        }
        public async Task<List<Employee>> GetEmployees()
        {
            try
            {
                _logger.LogInformation($"GetEmployees :Employee Service Calling Azure Func GetEmployees");
                var employees = new List<Employee>();
                using (var connection = new SqlConnection(_configuration.GetConnectionString("EmployeeDatabase")))
                {
                    var sql = "SELECT id, firstName, lastName, email, phone , company FROM Employee";
                    connection.Open();
                    using SqlCommand command = new SqlCommand(sql, connection);
                    using SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var employee = new Employee()
                        {
                            id = (int)reader["id"],
                            firstName = reader["firstName"].ToString(),
                            lastName = reader["lastName"].ToString(),
                            email = reader["email"].ToString(),
                            phone = (Int64)reader["phone"],
                            company = reader["company"].ToString()

                        };
                        employees.Add(employee);
                    }
                }
                return employees;
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR {ex.Message} Calling Get Employees");
                throw;
            }

        }

        public async Task<Unit> AddEmployee(AddEmployeeCommand employee)
        {
            try
            {
                _logger.LogInformation($"AddEmployee :Employee Service Calling Azure Func AddEmployee for employee {employee.firstName} {employee.lastName}");
                using (var connection = new SqlConnection(_configuration.GetConnectionString("EmployeeDatabase")))
                {
                    var sql = "INSERT INTO Employee (firstName, lastName, email, phone, company) VALUES (@FirstName, @LastName, @Email, @PhoneNumber , @Company)";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", employee.firstName);
                        command.Parameters.AddWithValue("@LastName", employee.lastName);
                        command.Parameters.AddWithValue("@Email", employee.email);
                        command.Parameters.AddWithValue("@PhoneNumber", employee.phone);
                        command.Parameters.AddWithValue("@Company", employee.company);
                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();
                    }
                }
                return Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"ERROR {ex.Message} Calling AddEmployee for employee {employee.firstName} {employee.lastName}");

                throw;
            }

        }

        public async Task<Unit> AddEmployeeUsingServiceBus(AddEmployeeCommand command)
        {
            try
            {
                _logger.LogInformation($"AddEmployee :Employee Service Calling Azure Func AddEmployee for employee {command.firstName} {command.lastName}");
                var messageBody = JsonConvert.SerializeObject(command);

                await using var client = new ServiceBusClient(_configuration.GetConnectionString("serviceBusConnectionString"));
                var sender = client.CreateSender(_configuration.GetConnectionString("queueName"));
                var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(messageBody));
                await sender.SendMessageAsync(message);
                return Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"ERROR {ex.Message} Calling AddEmployee for employee {command.firstName} {command.lastName}");

                throw;
            }

        }

    }
}
