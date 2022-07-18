namespace Application.DataModel
{
    public class JobDefinition
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<JobStep> JobSteps { get; set; }
    }
}
