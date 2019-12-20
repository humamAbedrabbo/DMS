namespace DAS.Exceptions
{
    public class InvalidEntityException : DasException
    {
        public InvalidEntityException()
            : base()
        {
        }

        public InvalidEntityException(string entity, string message)
            : base($"Invalid {entity}, {message}")
        {
        }
    }
}
