using System;

namespace servicehost.contract
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TeardownAttribute : Attribute { }
}