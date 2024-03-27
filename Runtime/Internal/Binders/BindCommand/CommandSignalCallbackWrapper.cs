using System;
using Zenject;
using ZenjectSignalCommands.Runtime.Main;

namespace ZenjectSignalCommands.Runtime.Internal.Binders.BindCommand
{
    public class CommandSignalCallbackWrapper  : IDisposable
    {
        readonly SignalBus _signalBus;
        private readonly CommandInvoker _commandInvoker;
        readonly Type _signalType;
        readonly object _identifier;
        
        public CommandSignalCallbackWrapper(
            SignalBindingBindInfo bindInfo,
            CommandInvoker commandInvoker,
            SignalBus signalBus)
        {
            _signalType = bindInfo.SignalType;
            _identifier = bindInfo.Identifier;
            _signalBus = signalBus;
            _commandInvoker = commandInvoker;

            signalBus.SubscribeId(bindInfo.SignalType, _identifier, OnSignalFired);
        }
        
        void OnSignalFired(object signal)
        {
            _commandInvoker.OnSignalFired(signal);
        }
        
        public void Dispose()
        {
            _signalBus.UnsubscribeId(_signalType, _identifier, OnSignalFired);
        }
    }
}