using Application.DataModel;
using Newtonsoft.Json;

namespace Application.Job
{
    public class JobStorage
    {
        private readonly string JOBSPATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jobs");

        private readonly Application.Logger.ILogger _logger;

        private readonly Dictionary<string, string> _jobDirectoryPaths;

        public JobStorage(Application.Logger.ILogger logger)
        {
            _logger = logger;
            _jobDirectoryPaths = new Dictionary<string, string>();

            string[] jobDirectoryPaths = Directory.GetDirectories(JOBSPATH);
            foreach (string jobDirectoryPath in jobDirectoryPaths)
            {
                string jobJson = File.ReadAllText(Path.Combine(jobDirectoryPath, "definition.json"));
                JobDefinition jobDefinition = JsonConvert.DeserializeObject<JobDefinition>(jobJson);
                _jobDirectoryPaths.Add(jobDefinition.Name, jobDirectoryPath);
            }
        }

        public void AddJob(JobDefinition jobDefinition)
        {
            string jobJson = JsonConvert.SerializeObject(jobDefinition);
            string jobDirectoryPath = Path.Combine(JOBSPATH, jobDefinition.Name);
            Directory.CreateDirectory(jobDirectoryPath);
            string jobDefinitionPath = Path.Combine(jobDirectoryPath, "definition.json");
            File.WriteAllText(jobDefinitionPath, jobJson);
            _jobDirectoryPaths.Add(jobDefinition.Name, jobDirectoryPath);
        }

        public void RemoveJob(string jobName)
        {
            Directory.Delete(_jobDirectoryPaths[jobName], true);
            _jobDirectoryPaths.Remove(jobName);
        }

        public JobDefinition GetJobDefinition(string jobName)
        {
            string jobDefinitionPath = Path.Combine(_jobDirectoryPaths[jobName], "definition.json");
            string jobJson = File.ReadAllText(jobDefinitionPath);
            return JsonConvert.DeserializeObject<JobDefinition>(jobJson);
        }

        public List<JobDefinition> GetAllJobDefinitions()
        {
            List<JobDefinition> jobDefinitions = new List<JobDefinition>();
            foreach (string jobName in _jobDirectoryPaths.Keys)
                jobDefinitions.Add(GetJobDefinition(jobName));
            return jobDefinitions;
        }
    }
}
