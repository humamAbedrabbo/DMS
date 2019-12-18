using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Exceptions
{
    public class EntityNotFoundException : ApplicationException
    {
        public EntityNotFoundException()
            :base()
        {
        }

        public EntityNotFoundException(string entityType, string message)
            :base($"{entityType} Not Found, {message}")
        {
        }
    }
}
