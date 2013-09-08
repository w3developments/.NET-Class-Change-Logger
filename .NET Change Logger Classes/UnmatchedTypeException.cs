using System;

namespace W3Developments.Auditor
{
    [Serializable]
    internal class UnmatchedTypeException : Exception
    {
        public UnmatchedTypeException() : base("The object types do not match") { }
    }
}
