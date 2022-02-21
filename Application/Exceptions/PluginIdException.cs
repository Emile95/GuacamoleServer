namespace App.Exceptions
{
    public class PluginIdException<T> : Exception
    {
        public PluginIdException(string actionID)
            : base(typeof(T).Name +  " id : '" + actionID + "' duplicated")
        {}
    }
}
