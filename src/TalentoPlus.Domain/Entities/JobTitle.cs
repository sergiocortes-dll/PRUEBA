namespace TalentoPlus.Domain.Entities;

public class JobTitle : BaseEntity
{
    public string Name { get; private set; }

    // Navegación
    public ICollection<Employee> Employees { get; private set; } = new List<Employee>();
    
    protected JobTitle() { }

    public JobTitle(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public void UpdateInfo(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        ModifiedAt = DateTime.UtcNow;
    }
}