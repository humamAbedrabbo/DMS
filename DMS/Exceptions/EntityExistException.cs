using System;

namespace DMS.Exceptions
{
    public class EntityExistException : DmsException
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
