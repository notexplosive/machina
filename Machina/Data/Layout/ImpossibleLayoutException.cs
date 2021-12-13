using System;

namespace Machina.Data.Layout
{
    public class ImpossibleLayoutException : Exception
    {
        public ImpossibleLayoutException(string message) : base(message)
        {
        }

        public ImpossibleLayoutException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
