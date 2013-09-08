using System;
using System.Reflection;

namespace W3Developments.Auditor
{
    /// <summary>
    /// An attribute to decorate properties to be audited
    /// </summary>
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Field)]
    public class Audited : Attribute
    {
    }
}
