using System.ComponentModel.DataAnnotations;

namespace TalentoPlus.Application.DTOs.Employee
{
    public class CreateEmployeeDto
    {
        public string Document { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }
        public int DepartmentId { get; set; }
        public int JobTitleId { get; set; }
        public int EducationLevelId { get; set; }
        public string ProfessionalProfile { get; set; } = string.Empty;
    }
    
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string Document { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ProfessionalProfile { get; set; } = string.Empty;
    
        // Relaciones
        public string DepartmentName { get; set; } = string.Empty;
        public string JobTitleName { get; set; } = string.Empty;
        public string EducationLevelName { get; set; } = string.Empty;
    
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}