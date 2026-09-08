
using AutoMapper;
using Moq;
using Xunit;
using MiApi.Application.DTOs;
using MiApi.Application.Handlers;
using MiApi.Application.Mappings;
using MiApi.Application.Queries;
using MiApi.Domain.Entities;
using MiApi.Domain.Exceptions;
using MiApi.Domain.Interfaces;

namespace MiApi.Application.Tests.QueryHandlerTests;
public class GetAllEmployeesQueryHandlerTests
{
    private readonly Mock<IEmployeeRepository> _mockRepository = new();
    private readonly IMapper _mapper;
    private readonly GetAllEmployeesQueryHandler _handler;

    public GetAllEmployeesQueryHandlerTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        _handler = new GetAllEmployeesQueryHandler(_mockRepository.Object, _mapper);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllEmployees()
    {
        var employees = new List<Employee>
        {
            new()
            { 
                Id = 1, 
                Email = "test1@test.com", 
                Username = "user1",
                CompanyId = 1,
                Password = "pass",
                PortalId = 1,
                RoleId = 1,
                StatusId = 1
            },
            new()
            { 
                Id = 2, 
                Email = "test2@test.com", 
                Username = "user2",
                CompanyId = 1,
                Password = "pass",
                PortalId = 1,
                RoleId = 1,
                StatusId = 1
            }
        };

        _mockRepository
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(employees);

        var result = await _handler.Handle(new GetAllEmployeesQuery(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class GetEmployeeByIdQueryHandlerTests
{
    private readonly Mock<IEmployeeRepository> _mockRepository = new();
    private readonly IMapper _mapper;
    private readonly GetEmployeeByIdQueryHandler _handler;

    public GetEmployeeByIdQueryHandlerTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        _handler = new GetEmployeeByIdQueryHandler(_mockRepository.Object, _mapper);
    }

    [Fact]
    public async Task Handle_WithValidId_ShouldReturnEmployee()
    {
        const int employeeId = 1;
        var employee = new Employee 
        { 
            Id = employeeId, 
            Email = "test@test.com", 
            Username = "testuser",
            CompanyId = 1,
            Password = "pass",
            PortalId = 1,
            RoleId = 1,
            StatusId = 1
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        var result = await _handler.Handle(new GetEmployeeByIdQuery(employeeId), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(employee.Email, result.Email);
        Assert.Equal(employee.Username, result.Username);
    }

    [Fact]
    public async Task Handle_WithNonExistentId_ShouldThrowException()
    {
        _mockRepository
            .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);

        await Assert.ThrowsAsync<EmployeeNotFoundException>(
            () => _handler.Handle(new GetEmployeeByIdQuery(999), CancellationToken.None)
        );
    }
}

public class GetEmployeeByUsernameQueryHandlerTests
{
    private readonly Mock<IEmployeeRepository> _mockRepository = new();
    private readonly IMapper _mapper;
    private readonly GetEmployeeByUsernameQueryHandler _handler;

    public GetEmployeeByUsernameQueryHandlerTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        _handler = new GetEmployeeByUsernameQueryHandler(_mockRepository.Object, _mapper);
    }

    [Fact]
    public async Task Handle_WithValidUsername_ShouldReturnEmployee()
    {
        const string username = "testuser";
        var employee = new Employee 
        { 
            Id = 1, 
            Email = "test@test.com", 
            Username = username,
            CompanyId = 1,
            Password = "pass",
            PortalId = 1,
            RoleId = 1,
            StatusId = 1
        };

        _mockRepository
            .Setup(r => r.GetByUsernameAsync(username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        var result = await _handler.Handle(new GetEmployeeByUsernameQuery(username), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(employee.Username, result.Username);
    }
}

public class GetEmployeesByCompanyQueryHandlerTests
{
    private readonly Mock<IEmployeeRepository> _mockRepository = new();
    private readonly IMapper _mapper;
    private readonly GetEmployeesByCompanyQueryHandler _handler;

    public GetEmployeesByCompanyQueryHandlerTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        _handler = new GetEmployeesByCompanyQueryHandler(_mockRepository.Object, _mapper);
    }

    [Fact]
    public async Task Handle_WithValidCompanyId_ShouldReturnEmployees()
    {
        const int companyId = 1;
        var employees = new List<Employee>
        {
            new()
            { 
                Id = 1, 
                CompanyId = companyId, 
                Email = "test1@test.com", 
                Username = "user1",
                Password = "pass",
                PortalId = 1,
                RoleId = 1,
                StatusId = 1
            },
            new()
            { 
                Id = 2, 
                CompanyId = companyId, 
                Email = "test2@test.com", 
                Username = "user2",
                Password = "pass",
                PortalId = 1,
                RoleId = 1,
                StatusId = 1
            }
        };

        _mockRepository
            .Setup(r => r.GetByCompanyIdAsync(companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employees);

        var result = await _handler.Handle(new GetEmployeesByCompanyQuery(companyId), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
}

