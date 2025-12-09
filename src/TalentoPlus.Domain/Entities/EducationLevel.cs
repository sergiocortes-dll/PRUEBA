namespace TalentoPlus.Domain.Entities;

public class EducationLevel : BaseEntity
{
    public string Name { get; private set; }
    public int Order { get; private set; } // Para ordenar niveles (1: Primaria, 2: Secundaria, etc.)

    // Navegación
    public ICollection<Employee> Employees { get; private set; } = new List<Employee>();
    
    protected EducationLevel() { }

    public EducationLevel(string name, int order = 0)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Order = order;
    }

    public void UpdateInfo(string name, int order = 0)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Order = order;
        ModifiedAt = DateTime.UtcNow;
    }
}