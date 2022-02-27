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
        protected abstract void ValidateApplication(ApplicationContext context);
        public abstract void Install(ApplicationContext context);
        public abstract void Uninstall(ApplicationContext context);
        public abstract void Initialize(ApplicationContext context);
        public abstract void Uninitialize(ApplicationContext context);
        public virtual void AddBaseHttpRequests(ApplicationContext applicationContext, HttpRequestManager httpRequestManager) 
        {
            httpRequestManager.Add((context) =>
            {
                List<object> requests = new List<object>();
                foreach(HttpRequestAttribute httpRequestAttribute in applicationContext.HttpRequestAttributes)
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
                Pattern = applicationContext.Guid + "/descriptive",
            });
        }
    }
}
