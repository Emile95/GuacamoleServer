using Library.Agent.EventHandler;

namespace Library.Agent.Application
{
    public abstract class AgentApplicationBase
    {
        public AgentEventHandlerManager EventHandlerManager { get; set; }

        public abstract string GetName();
        public abstract string GetDescription();
        protected abstract void ValidateApplication();
        public abstract void Install();
        public abstract void Uninstall();
        public abstract void Initialize();
        public abstract void Uninitialize();
    }
}
