using System;
using Zenject;
using ZenjectSignalCommands.Runtime.Internal.Binders.DeclareCommand;

namespace ZenjectSignalCommands.Runtime.Internal
{
    public class CommandDeclaration
    {
        private readonly BindingId _bindingId;

        public CommandDeclaration(CommandDeclarationBindInfo bindInfo)
        {
            _bindingId = new BindingId(bindInfo.CommandType, bindInfo.Identifier);
            SignalType = bindInfo.SignalType;
            CommandType = bindInfo.CommandType;
        }

        public Type CommandType
        {
            get; private  set;
        }

        public BindingId BindingId
        {
            get { return _bindingId; }
        }

        public Type SignalType
        {
            get;
            private set;
        }
    }
}