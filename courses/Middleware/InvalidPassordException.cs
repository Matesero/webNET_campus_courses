namespace courses.Middleware

{
    public class InvalidPasswordException : Exception
    {
        public InvalidPasswordException() 
            : base("The password is incorrect.")
        { }
    }
}