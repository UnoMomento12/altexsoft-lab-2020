using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace HomeTask4.Core.Exceptions
{
    public class EmptyFieldException : Exception
    {
        public EmptyFieldException()
        {
        }

        public EmptyFieldException(string message) : base(message)
        {
        }

        public EmptyFieldException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EmptyFieldException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
