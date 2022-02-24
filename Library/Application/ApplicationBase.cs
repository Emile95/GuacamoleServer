using Library.EventHandler;

namespace Library.Application
{
    public abstract class ApplicationBase
    {
        public EventHandlerManager EventHandlerManager { get; set; }

        public abstract string GetName();
        public abstract string GetDescription();
        protected abstract void ValidateApplication();
        public abstract void Install(ApplicationContext context);
        public abstract void Uninstall(ApplicationContext context);
        public abstract void Initialize(ApplicationContext context);
        public abstract void Unitialize(ApplicationContext context);
    }
}
