namespace courses.Middleware

{
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message = "Forbidden") 
            : base(message)
        { }
    }
}