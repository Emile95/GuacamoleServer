using Library.EventHandler;

namespace Library.Application
{
    public abstract class ApplicationBase
    {
        public EventHandlerManager EventHandlerManager { get; set; }

        public abstract string GetName();
        public abstract string GetDescription();

        protected virtual void ValidateApplication()
        {

        }
    }
}
