using System;

namespace Exceptions
{
    public class InvalidMapDataException : Exception
    {
        public InvalidMapDataException(string message) : base(message)
        {
            
        }
    }
}