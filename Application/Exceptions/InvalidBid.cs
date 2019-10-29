using System;
using System.Runtime.Serialization;

namespace Application.Exceptions
{
    [Serializable]
    public class InvalidResult : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public InvalidResult() { }

        public InvalidResult(string message) : base(message) { }

        public InvalidResult(string message, Exception inner) : base(message, inner) { }

        protected InvalidResult(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }
}