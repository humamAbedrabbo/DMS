using System;

namespace DMS.Exceptions
{
    public class DmsException : ApplicationException
    {
        public DmsException()
            : base()
        {
        }

        public DmsException(string message)
            : base(message)
        {
        }
    }
}
