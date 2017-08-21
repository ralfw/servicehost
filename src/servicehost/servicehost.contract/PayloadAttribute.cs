using System;
namespace servicehost.contract
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class PayloadAttribute : Attribute { }
}
