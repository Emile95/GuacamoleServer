namespace Library.Job
{
    public abstract class JobAction
    {
        public abstract void Process(JobRunContext context);
    }
}
