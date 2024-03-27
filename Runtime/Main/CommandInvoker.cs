using System;
using System.Collections.Generic;
using Zenject;
using ZenjectSignalCommands.Runtime.Internal;
using ZenjectSignalCommands.Runtime.Internal.Binders;

namespace ZenjectSignalCommands.Runtime.Main
{
    public class CommandInvoker 
    {
        private readonly CommandPool _commandPool;
        private readonly Dictionary<Type, CommandDeclaration> _localDeclarationMap;
        
        public CommandInvoker(SignalBus signalBus,
                              [Inject(Source = InjectSources.Local)] 
                              List<CommandDeclaration> commandDeclarations,
                              CommandPool commandPool)
        {
            _commandPool = commandPool;
            _localDeclarationMap = new();
            
            foreach (CommandDeclaration x in commandDeclarations)
            {
                _localDeclarationMap[x.SignalType] = x;
            }
        }
        
        public void OnSignalFired<TSignal>(TSignal signal)
        {
            Type t = signal.GetType();
            
            if (_localDeclarationMap.TryGetValue(t, out CommandDeclaration value))
            {
                Type commandType = value.CommandType;
                ICommand command = _commandPool.GetCommand(commandType, signal);

                command.Execute();
            }
        }
    }
}