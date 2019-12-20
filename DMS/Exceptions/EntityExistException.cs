using System;

namespace DAS.Exceptions
{
    public class EntityExistException : DasException
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
