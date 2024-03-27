using Zenject;
using ZenjectSignalCommands.Runtime.Internal.Binders.BindCommand;

namespace ZenjectSignalCommands.Runtime.Internal.Binders
{
    public static class CommandSignalExtensions
    {
        public static BindSignalToCommandBinder<TSignal> SignalDeclare<TSignal>(this DiContainer container)
        {
            var signalBindInfo = new SignalBindingBindInfo(typeof(TSignal));

            return new BindSignalToCommandBinder<TSignal>(container, signalBindInfo);
        }
    }
}