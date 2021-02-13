using System;

namespace Trayscout
{
    internal class NightscoutException : Exception
    {
        public NightscoutException(string message) : base(message)
        {
        }
    }
}