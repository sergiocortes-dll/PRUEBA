namespace TalentoPlus.Domain.Entities;

public class ProfessionalProfile : BaseEntity
{
    public string Name { get; private set; }

    // Navegación
    public ICollection<Employee> Employees { get; private set; } = new List<Employee>();
    
    protected ProfessionalProfile() { }

    public ProfessionalProfile(string name, string? skills = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public void UpdateInfo(string name, string? skills = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        ModifiedAt = DateTime.UtcNow;
    }
}