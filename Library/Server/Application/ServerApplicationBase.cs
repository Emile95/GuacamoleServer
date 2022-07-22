using Library.Server.EventHandler;

namespace Library.Server.Application
{
    public abstract class ServerApplicationBase
    {
        public ServerEventHandlerManager EventHandlerManager { get; set; }

        public abstract string GetName();
        public abstract string GetDescription();
        protected abstract void ValidateApplication();
        public abstract void Install();
        public abstract void Uninstall();
        public abstract void Initialize();
        public abstract void Uninitialize();
    }
}
