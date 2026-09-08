
using AutoMapper;
using MediatR;
using MiApi.Application.DTOs;
using MiApi.Application.Queries;
using MiApi.Domain.Exceptions;
using MiApi.Domain.Interfaces;

namespace MiApi.Application.Handlers;
public class GetAllEmployeesQueryHandler(IEmployeeRepository repository, IMapper mapper) 
    : IRequestHandler<GetAllEmployeesQuery, IEnumerable<EmployeeDto>>
{
    public async Task<IEnumerable<EmployeeDto>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
    {
        var employees = await repository.GetAllAsync(cancellationToken);
        return mapper.Map<IEnumerable<EmployeeDto>>(employees);
    }
}

public class GetEmployeeByIdQueryHandler(IEmployeeRepository repository, IMapper mapper) 
    : IRequestHandler<GetEmployeeByIdQuery, EmployeeDto>
{
    public async Task<EmployeeDto> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (employee is null)
            throw new EmployeeNotFoundException(request.Id);

        return mapper.Map<EmployeeDto>(employee);
    }
}

public class GetEmployeeByUsernameQueryHandler(IEmployeeRepository repository, IMapper mapper) 
    : IRequestHandler<GetEmployeeByUsernameQuery, EmployeeDto>
{
    public async Task<EmployeeDto> Handle(GetEmployeeByUsernameQuery request, CancellationToken cancellationToken)
    {
        var employee = await repository.GetByUsernameAsync(request.Username, cancellationToken);
        if (employee is null)
            throw new EmployeeNotFoundException(0);

        return mapper.Map<EmployeeDto>(employee);
    }
}

public class GetEmployeesByCompanyQueryHandler(IEmployeeRepository repository, IMapper mapper) 
    : IRequestHandler<GetEmployeesByCompanyQuery, IEnumerable<EmployeeDto>>
{
    public async Task<IEnumerable<EmployeeDto>> Handle(GetEmployeesByCompanyQuery request, CancellationToken cancellationToken)
    {
        var employees = await repository.GetByCompanyIdAsync(request.CompanyId, cancellationToken);
        return mapper.Map<IEnumerable<EmployeeDto>>(employees);
    }
}

