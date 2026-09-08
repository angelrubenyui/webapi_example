using MediatR;
using MiApi.Application.DTOs;

namespace MiApi.Application.Commands;
public class CreateEmployeeCommand : IRequest<EmployeeDto>
{
    public int CompanyId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int PortalId { get; set; }
    public int RoleId { get; set; }
    public int StatusId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Telephone { get; set; }
    public string? Fax { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
}

public class UpdateEmployeeCommand : IRequest<EmployeeDto>
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public int StatusId { get; set; }
    public string? Name { get; set; }
    public string? Telephone { get; set; }
    public string? Fax { get; set; }
}

public record DeleteEmployeeCommand(int Id) : IRequest<bool>;

