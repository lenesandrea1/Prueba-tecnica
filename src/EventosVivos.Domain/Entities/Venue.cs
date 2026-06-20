namespace EventosVivos.Domain.Entities;

public sealed class Venue
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int Capacity { get; private set; }
    public string City { get; private set; } = string.Empty;

    private Venue()
    {
    }

    public Venue(int id, string name, int capacity, string city)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Venue name is required.", nameof(name));
        }

        if (capacity <= 0)
        {
            throw new ArgumentException("Venue capacity must be positive.", nameof(capacity));
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            throw new ArgumentException("Venue city is required.", nameof(city));
        }

        Id = id;
        Name = name.Trim();
        Capacity = capacity;
        City = city.Trim();
    }
}
