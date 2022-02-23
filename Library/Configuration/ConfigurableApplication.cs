using Library.Application;

namespace Library.Configuration
{
    public abstract class ConfigurableApplication<Configuration> : ApplicationBase
    {
        public Configuration Config { get; set; }

        protected sealed override void ValidateApplication()
        {
            base.ValidateApplication();
            ValidateConfiguration();
        }

        protected abstract void ValidateConfiguration();
    }
}
