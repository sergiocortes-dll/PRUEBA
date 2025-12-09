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
        Document = document ?? throw new ArgumentNullException(nameof(document));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Surname = surname ?? throw new ArgumentNullException(nameof(surname));
        BirthDate = birthDate;
        Address = address ?? throw new ArgumentNullException(nameof(address));
        Phone = phone ?? throw new ArgumentNullException(nameof(phone));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        
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

    public void ChangeDocument(string newDocument)
    {
        if (string.IsNullOrWhiteSpace(newDocument))
            throw new ArgumentException("El documento no puede estar vacío");

        Document = newDocument;
        ModifiedAt = DateTime.UtcNow;
    }

    private void Validate()
    {
        if (BirthDate > DateTime.UtcNow.AddYears(-18))
            throw new InvalidOperationException("El empleado debe ser mayor de edad");

        if (!IsValidEmail(Email))
            throw new InvalidOperationException("El email no es válido");
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