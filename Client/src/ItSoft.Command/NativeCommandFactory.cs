using System;

namespace ItSoft.Command
{
    public class NativeCommandFactory : ICommandFactory
    {
        public object CreateInstance(Action<object> action, Func<object, bool> predicate)
        {
            return new NativeCommand(action, predicate);
        }
    }
}