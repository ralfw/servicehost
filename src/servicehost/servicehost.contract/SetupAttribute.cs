using System;

namespace servicehost.contract
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SetupAttribute : Attribute { }
    
}