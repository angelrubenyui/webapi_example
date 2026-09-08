using System;

namespace MiApi.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
    }

    public class EmployeeNotFoundException : DomainException
    {
        public EmployeeNotFoundException(int id) 
            : base($"Employee with ID {id} not found") { }
    }

    public class EmployeeAlreadyExistsException : DomainException
    {
        public EmployeeAlreadyExistsException(string username)
            : base($"Employee with username '{username}' already exists") { }
    }

    public class InvalidEmployeeException : DomainException
    {
        public InvalidEmployeeException(string message) : base(message) { }
    }
}
