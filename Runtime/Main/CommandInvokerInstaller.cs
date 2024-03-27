using Zenject;
using ZenjectSignalCommands.Runtime.Internal.Binders;

namespace ZenjectSignalCommands.Runtime.Main
{
    public class CommandInvokerInstaller : Installer<CommandInvokerInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<CommandInvoker>().AsSingle().CopyIntoAllSubContainers();
            Container.Bind<CommandPool>().AsSingle().CopyIntoAllSubContainers();
        }
    }
}