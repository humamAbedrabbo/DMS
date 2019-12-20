using System;

namespace DAS.Exceptions
{
    public class DasException : ApplicationException
    {
        public DasException()
            : base()
        {
        }

        public DasException(string message)
            : base(message)
        {
        }
    }
}
