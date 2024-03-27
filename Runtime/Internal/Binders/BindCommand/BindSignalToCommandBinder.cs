using System;
using System.Reflection;
using ModestTree;
using Zenject;
using ZenjectSignalCommands.Runtime.Internal.Binders.DeclareCommand;
using ZenjectSignalCommands.Runtime.Main;

namespace ZenjectSignalCommands.Runtime.Internal.Binders.BindCommand
{
    public class BindSignalToCommandBinder<TSignal> : BindSignalToBinder<TSignal>
    {
        public SignalCopyBinder WithCommand<TCommand>()
        {
            GetContainerAndBindStatement<TCommand>(out BindStatement bindStatement, out DiContainer container);
            
            Assert.That(!bindStatement.HasFinalizer);
            bindStatement.SetFinalizer(new NullBindingFinalizer());
            
            DeclareSignal(container);
            BindCommand<TCommand>(container);

            var bindInfo = container.Bind<IDisposable>()
                                    .To<CommandSignalCallbackWrapper>()
                                    .AsCached()
                                    .WithArguments(SignalBindInfo)
                                    .NonLazy().BindInfo;

            return new SignalCopyBinder(bindInfo);
        }

        private static void DeclareSignal(DiContainer container)
        {
            container.DeclareSignal<TSignal>();
        }

        private static void BindCommand<TCommand>(DiContainer container)
        {
            CommandDeclarationBindInfo commandBindInfo = new CommandDeclarationBindInfo(typeof(TCommand), typeof(TSignal));

            container.Bind<CommandDeclaration>().AsCached()
                     .WithArguments(commandBindInfo).WhenInjectedInto(typeof(CommandInvoker));
        }

        private void GetContainerAndBindStatement<TCommand>(out BindStatement bindStatement, out DiContainer container)
        {
            FieldInfo containerField = typeof(BindSignalToCommandBinder<TSignal>).BaseType
                                                                                 .GetField("_container", BindingFlags.Instance | BindingFlags.NonPublic);
            
            FieldInfo bindStatementField = typeof(BindSignalToCommandBinder<TSignal>).BaseType
                .GetField("_bindStatement", BindingFlags.Instance | BindingFlags.NonPublic);

            container =  (DiContainer)containerField.GetValue(this);
            bindStatement = (BindStatement)bindStatementField.GetValue(this);
        }

        public BindSignalToCommandBinder(DiContainer container, SignalBindingBindInfo signalBindInfo) : base(container, signalBindInfo)
        {
        }
    }
}