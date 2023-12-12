namespace AB460Proniya.Utilities.Exceptions
{
    public class WrongRequestException : Exception
    {
        public WrongRequestException(string message = "Wrong ") : base(message)
        {

        }
    }
}
