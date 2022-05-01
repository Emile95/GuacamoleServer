using Library.Configuration.Http;
using Library.EventHandler;
using Library.Http;

namespace Library.Application
{
    public abstract class ApplicationBase
    {
        public EventHandlerManager EventHandlerManager { get; set; }

        public abstract string GetName();
        public abstract string GetDescription();
        protected abstract void ValidateApplication();
        public abstract void Install();
        public abstract void Uninstall();
        public abstract void Initialize();
        public abstract void Uninitialize();
        public virtual void AddBaseHttpRequests(HttpRequestManager httpRequestManager) 
        {
            httpRequestManager.Add((context) =>
            {
                List<object> requests = new List<object>();
                foreach(HttpRequestAttribute httpRequestAttribute in ApplicationContext.HttpRequestAttributes)
                {
                    requests.Add(new
                    {
                        type = httpRequestAttribute.HttpRequestType.ToString(),
                        pattern = httpRequestAttribute.Pattern,
                    });
                }

                var descriptive = new {
                    name = GetName(),
                    description = GetDescription(),
                    requests = requests
                };
                context.ResponseBody = descriptive;

            }, new HttpRequestAttribute()
            {
                HttpRequestType = HttpRequestType.Get,
                Pattern = ApplicationContext.Guid + "/descriptive",
            });
        }
    }
}
