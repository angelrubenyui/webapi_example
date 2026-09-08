
using Xunit;
using MiApi.Application.Commands;
using MiApi.Application.Validators;

namespace MiApi.Application.Tests.Validators;
public class CreateEmployeeCommandValidatorTests
{
    private readonly CreateEmployeeCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_ShouldPass()
    {
        var command = new CreateEmployeeCommand
        {
            CompanyId = 1,
            Email = "test@test.com",
            Password = "password123",
            PortalId = 1,
            RoleId = 1,
            StatusId = 1,
            Username = "testuser",
            Name = "Test User"
        };

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithoutRequiredFields_ShouldFail()
    {
        var command = new CreateEmployeeCommand();

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Validate_WithInvalidEmail_ShouldFail()
    {
        var command = new CreateEmployeeCommand
        {
            CompanyId = 1,
            Email = "invalid-email",
            Password = "password123",
            PortalId = 1,
            RoleId = 1,
            StatusId = 1,
            Username = "testuser"
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Email");
    }

    [Fact]
    public void Validate_WithShortPassword_ShouldFail()
    {
        var command = new CreateEmployeeCommand
        {
            CompanyId = 1,
            Email = "test@test.com",
            Password = "123",
            PortalId = 1,
            RoleId = 1,
            StatusId = 1,
            Username = "testuser"
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Password");
    }

    [Fact]
    public void Validate_WithShortUsername_ShouldFail()
    {
        var command = new CreateEmployeeCommand
        {
            CompanyId = 1,
            Email = "test@test.com",
            Password = "password123",
            PortalId = 1,
            RoleId = 1,
            StatusId = 1,
            Username = "ab"
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Username");
    }

    [Fact]
    public void Validate_WithLongUsername_ShouldFail()
    {
        var command = new CreateEmployeeCommand
        {
            CompanyId = 1,
            Email = "test@test.com",
            Password = "password123",
            PortalId = 1,
            RoleId = 1,
            StatusId = 1,
            Username = new string('a', 51)
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Username");
    }

    [Fact]
    public void Validate_WithZeroCompanyId_ShouldFail()
    {
        var command = new CreateEmployeeCommand
        {
            CompanyId = 0,
            Email = "test@test.com",
            Password = "password123",
            PortalId = 1,
            RoleId = 1,
            StatusId = 1,
            Username = "testuser"
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "CompanyId");
    }
}

public class UpdateEmployeeCommandValidatorTests
{
    private readonly UpdateEmployeeCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_ShouldPass()
    {
        var command = new UpdateEmployeeCommand
        {
            Id = 1,
            Email = "test@test.com",
            RoleId = 1,
            StatusId = 1,
            Name = "Updated User"
        };

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithZeroId_ShouldFail()
    {
        var command = new UpdateEmployeeCommand
        {
            Id = 0,
            Email = "test@test.com",
            RoleId = 1,
            StatusId = 1
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Id");
    }

    [Fact]
    public void Validate_WithInvalidEmail_ShouldFail()
    {
        var command = new UpdateEmployeeCommand
        {
            Id = 1,
            Email = "invalid-email",
            RoleId = 1,
            StatusId = 1
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Email");
    }
}

