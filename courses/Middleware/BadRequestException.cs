namespace courses.Middleware

{
    public class BadRequestException : Exception
    {
        public BadRequestException(string message = "Bad request") 
            : base(message)
        { }
    }
}