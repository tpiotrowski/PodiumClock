using System;

namespace ItSoft.Command
{
    public interface ICommandFactory
    {
        object CreateInstance(Action<object> action, Func<object, bool> predicate);
    }
}