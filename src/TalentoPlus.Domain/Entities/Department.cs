namespace TalentoPlus.Domain.Entities;

public class Department : BaseEntity
{
    public string Name { get; private set; }

    // Navegación
    public ICollection<Employee> Employees { get; private set; } = new List<Employee>();
    
    protected Department() { }

    public Department(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public void UpdateInfo(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        ModifiedAt = DateTime.UtcNow;
    }
}