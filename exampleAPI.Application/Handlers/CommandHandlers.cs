using AutoMapper;
using MediatR;
using MiApi.Application.Commands;
using MiApi.Application.DTOs;
using MiApi.Domain.Entities;
using MiApi.Domain.Exceptions;
using MiApi.Domain.Interfaces;

namespace MiApi.Application.Handlers;

public class CreateEmployeeCommandHandler(IEmployeeRepository repository, IMapper mapper) 
    : IRequestHandler<CreateEmployeeCommand, EmployeeDto>
{
    public async Task<EmployeeDto> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var existingEmployee = await repository.GetByUsernameAsync(request.Username, cancellationToken);
        if (existingEmployee is not null)
            throw new EmployeeAlreadyExistsException(request.Username);

        var employee = mapper.Map<Employee>(request);
        var createdEmployee = await repository.AddAsync(employee, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return mapper.Map<EmployeeDto>(createdEmployee);
    }
}

public class UpdateEmployeeCommandHandler(IEmployeeRepository repository, IMapper mapper) 
    : IRequestHandler<UpdateEmployeeCommand, EmployeeDto>
{
    public async Task<EmployeeDto> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (employee is null)
            throw new EmployeeNotFoundException(request.Id);

        employee.Email = request.Email;
        employee.RoleId = request.RoleId;
        employee.StatusId = request.StatusId;
        employee.Name = request.Name;
        employee.Telephone = request.Telephone;
        employee.Fax = request.Fax;
        employee.UpdatedOn = DateTime.UtcNow;

        var updatedEmployee = await repository.UpdateAsync(employee);
        await repository.SaveChangesAsync(cancellationToken);

        return mapper.Map<EmployeeDto>(updatedEmployee);
    }
}

public class DeleteEmployeeCommandHandler(IEmployeeRepository repository) 
    : IRequestHandler<DeleteEmployeeCommand, bool>
{
    public async Task<bool> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (employee is null)
            throw new EmployeeNotFoundException(request.Id);

        var result = await repository.DeleteAsync(request.Id, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return result;
    }
}

