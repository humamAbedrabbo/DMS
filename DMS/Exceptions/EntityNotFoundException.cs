using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAS.Exceptions
{
    public class EntityNotFoundException : DasException
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
