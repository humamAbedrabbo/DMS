namespace DMS.Exceptions
{
    public class InvalidEntityException : DmsException
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
