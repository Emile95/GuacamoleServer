using Library.Logger;
using System.Text.Json;

namespace Library
{
    public interface IAction : IRunnable<JsonElement, ActionRunLogger>
    {
        bool HasParameter();
        string GetActionID();
    }
}
