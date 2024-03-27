using System;
using Zenject;

namespace ZenjectSignalCommands.Runtime.Internal.Binders.DeclareCommand
{
    [NoReflectionBaking]
    public class CommandDeclarationBindInfo
    {
        public CommandDeclarationBindInfo(Type commandType, Type signalType)
        {
            CommandType = commandType;
            SignalType = signalType;
        }
        
        public object Identifier
        {
            get; set;
        }

        public Type CommandType
        {
            get;  private set;
        }

        public Type SignalType
        {
            get; private set;
        }
    }
}