using System;

namespace MiApi.Domain.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        
        // Campos obligatorios
        public int CompanyId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int PortalId { get; set; }
        public int RoleId { get; set; }
        public int StatusId { get; set; }
        public string Username { get; set; } = string.Empty;
        
        // Campos opcionales
        public string? Name { get; set; }
        public string? Telephone { get; set; }
        public string? Fax { get; set; }
        public DateTime? LastLogin { get; set; }
        
        // Auditoría
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        
        // Soft delete
        public bool IsDeleted { get; set; }
    }
}
