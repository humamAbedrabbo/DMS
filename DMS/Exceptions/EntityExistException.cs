using System;

namespace DMS.Exceptions
{
    public class EntityExistException : ApplicationException
    {
        public EntityExistException()
            : base()
        {
        }

        public EntityExistException(string entityType, string message)
            : base($"{entityType} Already exists, {message}")
        {
        }
    }
}
