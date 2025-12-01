using System;

namespace Dan.Net
{
    /// <summary>
    /// Indicates that the function/method attributed is a DanNet event.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class DanNetEventAttribute : Attribute { }
}