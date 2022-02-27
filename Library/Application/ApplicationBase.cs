using Library.EventHandler;
using Library.Http;

namespace Library.Application
{
    public abstract class ApplicationBase
    {
        public EventHandlerManager EventHandlerManager { get; set; }

        public abstract string GetName();
        public abstract string GetDescription();
        protected abstract void ValidateApplication(ApplicationContext context);
        public abstract void Install(ApplicationContext context);
        public abstract void Uninstall(ApplicationContext context);
        public abstract void Initialize(ApplicationContext context);
        public abstract void Uninitialize(ApplicationContext context);
        public virtual void AddBaseHttpRequests(ApplicationContext context, HttpRequestManager httpRequestManager) { }
    }
}
