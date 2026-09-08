

using MediatR;
using MiApi.Application.DTOs;

namespace MiApi.Application.Queries;

public record GetAllEmployeesQuery : IRequest<IEnumerable<EmployeeDto>>;

public record GetEmployeeByIdQuery(int Id) : IRequest<EmployeeDto>;

public record GetEmployeeByUsernameQuery(string Username) : IRequest<EmployeeDto>;

public record GetEmployeesByCompanyQuery(int CompanyId) : IRequest<IEnumerable<EmployeeDto>>;

