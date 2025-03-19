namespace courses.Middleware;

public class NotFoundException : Exception
{
    public NotFoundException()
        : base("Requested entity does not exist")
    { }

    public NotFoundException(string propertyValue, string entity, string property = "id")
        : base($"{entity} with {property} {propertyValue} does not exist")
    { }
    
    public NotFoundException(string message)
        : base(message)
    { }
}