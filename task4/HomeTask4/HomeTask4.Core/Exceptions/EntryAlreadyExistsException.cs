using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace HomeTask4.Core.Exceptions
{
    public class EntryAlreadyExistsException : Exception
    {
        public EntryAlreadyExistsException()
        {
        }

        public EntryAlreadyExistsException(string message) : base(message)
        {
        }

        public EntryAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EntryAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
