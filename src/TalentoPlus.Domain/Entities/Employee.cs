using TalentoPlus.Domain.Models;

namespace TalentoPlus.Domain.Entities;

public class Employee : BaseEntity
{
    public string Document { get; private set; }
    public string Name { get; private set; }
    public string Surname { get; private set; }
    public DateTime BirthDate { get; private set; }
    public string Address { get; private set; }
    public string Phone { get; private set; }
    public string Email { get; private set; }
    public decimal Salary { get; private set; }
    public DateTime HireDate { get; private set; }
    public EmployeeStatus Status { get; private set; }
    public string ProfessionalProfile { get; private set; }

    // Relaciones
    public int DepartmentId { get; private set; }
    public Department Department { get; private set; }

    public int JobTitleId { get; private set; }
    public JobTitle JobTitle { get; private set; }

    public int EducationLevelId { get; private set; }
    public EducationLevel EducationLevel { get; private set; }

    protected Employee()
    {
    }
    
    public Employee(
        string document,
        string name,
        string surname,
        DateTime birthDate,
        string address,
        string phone,
        string email)
    {
        Document = document;
        Name = name;
        Surname = surname;
        BirthDate = birthDate;
        Address = address;
        Phone = phone;
        Email = email;

        Status = EmployeeStatus.Active;
        HireDate = DateTime.UtcNow;

        ProfessionalProfile = string.Empty;

        Validate();
    }



    public Employee(
        string document,
        string name,
        string surname,
        DateTime birthDate,
        string address,
        string phone,
        string email,
        decimal salary,
        DateTime hireDate,
        int departmentId,
        int jobTitleId,
        int educationLevelId,
        string professionalProfile)
    {
        Document = document ?? throw new ArgumentNullException(nameof(document));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Surname = surname ?? throw new ArgumentNullException(nameof(surname));
        BirthDate = birthDate;
        Address = address ?? throw new ArgumentNullException(nameof(address));
        Phone = phone ?? throw new ArgumentNullException(nameof(phone));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Salary = salary;
        HireDate = hireDate;
        Status = EmployeeStatus.Active;
        DepartmentId = departmentId;
        JobTitleId = jobTitleId;
        EducationLevelId = educationLevelId;
        ProfessionalProfile = professionalProfile ?? string.Empty;
        
        Validate();
    }

    public void UpdatePersonalInfo(
        string name,
        string surname,
        DateTime birthDate,
        string address,
        string phone,
        string email)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Surname = surname ?? throw new ArgumentNullException(nameof(surname));
        BirthDate = birthDate;
        Address = address ?? throw new ArgumentNullException(nameof(address));
        Phone = phone ?? throw new ArgumentNullException(nameof(phone));
        Email = email ?? throw new ArgumentNullException(nameof(email));

        ModifiedAt = DateTime.UtcNow;
        Validate();
    }

    public void UpdateJobInfo(
        decimal salary,
        int departmentId,
        int jobTitleId,
        int educationLevelId,
        string professionalProfile)
    {
        Salary = salary;
        DepartmentId = departmentId;
        JobTitleId = jobTitleId;
        EducationLevelId = educationLevelId;
        ProfessionalProfile = professionalProfile ?? string.Empty;
        ModifiedAt = DateTime.UtcNow;
    }

    public void ChangeStatus(EmployeeStatus newStatus)
    {
        Status = newStatus;
        ModifiedAt = DateTime.UtcNow;
    }

    public void ChangeDocument(string newDocument)
    {
        if (string.IsNullOrWhiteSpace(newDocument))
            throw new ArgumentException("El documento no puede estar vacío");

        Document = newDocument;
        ModifiedAt = DateTime.UtcNow;
    }
    
    private void UpdateEmployeeData(
        Employee employee,
        EmployeeExcelDto data,
        int departmentId,
        int jobTitleId,
        int educationLevelId,
        EmployeeStatus status)
    {
        // Actualizar datos personales
        employee.UpdatePersonalInfo(
            data.Nombres,
            data.Apellidos,
            data.FechaNacimiento,
            data.Direccion,
            data.Telefono,
            data.Email
        );

        // Actualizar información laboral
        employee.UpdateJobInfo(
            data.Salario,
            departmentId,
            jobTitleId,
            educationLevelId,
            data.PerfilProfesional
        );

        // Actualizar estado
        employee.ChangeStatus(status);
    }


    private void Validate()
    {
        if (BirthDate > DateTime.UtcNow.AddYears(-18))
            throw new InvalidOperationException("El empleado debe ser mayor de edad");

        if (!IsValidEmail(Email))
            throw new InvalidOperationException("El email no es válido");

        if (Salary < 0)
            throw new InvalidOperationException("El salario no puede ser negativo");

        if (HireDate > DateTime.UtcNow)
            throw new InvalidOperationException("La fecha de ingreso no puede ser futura");
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}

public enum EmployeeStatus
{
    Active = 1,
    Inactive = 2,
    OnVacation = 3
}