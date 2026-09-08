

using AutoMapper;
using Moq;
using Xunit;
using MiApi.Application.Commands;
using MiApi.Application.Handlers;
using MiApi.Application.Mappings;
using MiApi.Domain.Entities;
using MiApi.Domain.Exceptions;
using MiApi.Domain.Interfaces;

namespace MiApi.Application.Tests.CommandHandlersTests;

public class CreateEmployeeCommandHandlerTests
{
    private readonly Mock<IEmployeeRepository> _repositoryMock = new();
    private readonly IMapper _mapper;
    private readonly CreateEmployeeCommandHandler _handler;

    public CreateEmployeeCommandHandlerTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        _handler = new CreateEmployeeCommandHandler(_repositoryMock.Object, _mapper);
    }

    [Fact]
    public async Task Handle_WhenUsernameIsUnique_ShouldCreateEmployeeAndReturnDto()
    {
        var command = new CreateEmployeeCommand
        {
            CompanyId = 1,
            Email = "carlos@empresa.com",
            Password = "securePassword123",
            PortalId = 1,
            RoleId = 2,
            StatusId = 1,
            Username = "carlosg",
            Name = "Carlos Gómez",
            Telephone = "+34600112233"
        };

        _repositoryMock
            .Setup(r => r.GetByUsernameAsync(command.Username, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee e, CancellationToken _) => 
            { 
                e.Id = 10; 
                return e; 
            });

        _repositoryMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(10, result.Id);
        Assert.Equal("carlosg", result.Username);
        Assert.Equal("carlos@empresa.com", result.Email);
        Assert.Equal("Carlos Gómez", result.Name);

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUsernameAlreadyExists_ShouldThrowEmployeeAlreadyExistsException()
    {
        var command = new CreateEmployeeCommand { Username = "carlosg" };
        var existingEmployee = new Employee { Id = 5, Username = "carlosg" };

        _repositoryMock
            .Setup(r => r.GetByUsernameAsync(command.Username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingEmployee);

        await Assert.ThrowsAsync<EmployeeAlreadyExistsException>(
            () => _handler.Handle(command, CancellationToken.None)
        );

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}

public class UpdateEmployeeCommandHandlerTests
{
    private readonly Mock<IEmployeeRepository> _repositoryMock = new();
    private readonly IMapper _mapper;
    private readonly UpdateEmployeeCommandHandler _handler;

    public UpdateEmployeeCommandHandlerTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        _handler = new UpdateEmployeeCommandHandler(_repositoryMock.Object, _mapper);
    }

    [Fact]
    public async Task Handle_WhenEmployeeExists_ShouldUpdateAndReturnDto()
    {
        var command = new UpdateEmployeeCommand
        {
            Id = 1,
            Email = "carlos.nuevo@empresa.com",
            Name = "Carlos Gómez Actualizado",
            RoleId = 3,
            StatusId = 1,
            Telephone = "+34699887766"
        };

        var existingEmployee = new Employee
        {
            Id = 1,
            Username = "carlosg",
            Email = "carlos@empresa.com",
            Name = "Carlos Gómez",
            RoleId = 2,
            StatusId = 1
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingEmployee);

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Employee>()))
            .ReturnsAsync((Employee e) => e);

        _repositoryMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Equal("carlos.nuevo@empresa.com", result.Email);
        Assert.Equal("Carlos Gómez Actualizado", result.Name);

        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Employee>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenEmployeeNotFound_ShouldThrowEmployeeNotFoundException()
    {
        var command = new UpdateEmployeeCommand { Id = 999 };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);

        await Assert.ThrowsAsync<EmployeeNotFoundException>(
            () => _handler.Handle(command, CancellationToken.None)
        );

        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Employee>()), Times.Never);
    }
}

public class DeleteEmployeeCommandHandlerTests
{
    private readonly Mock<IEmployeeRepository> _repositoryMock = new();
    private readonly DeleteEmployeeCommandHandler _handler;

    public DeleteEmployeeCommandHandlerTests()
    {
        _handler = new DeleteEmployeeCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenEmployeeExists_ShouldDeleteAndReturnTrue()
    {
        const int employeeId = 1;
        var existingEmployee = new Employee { Id = employeeId, Username = "carlosg" };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingEmployee);

        _repositoryMock
            .Setup(r => r.DeleteAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _repositoryMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new DeleteEmployeeCommand(employeeId), CancellationToken.None);

        Assert.True(result);
        _repositoryMock.Verify(r => r.DeleteAsync(employeeId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenEmployeeNotFound_ShouldThrowEmployeeNotFoundException()
    {
        const int nonExistentId = 999;

        _repositoryMock
            .Setup(r => r.GetByIdAsync(nonExistentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);

        await Assert.ThrowsAsync<EmployeeNotFoundException>(
            () => _handler.Handle(new DeleteEmployeeCommand(nonExistentId), CancellationToken.None)
        );

        _repositoryMock.Verify(r => r.DeleteAsync(nonExistentId, It.IsAny<CancellationToken>()), Times.Never);
    }
}