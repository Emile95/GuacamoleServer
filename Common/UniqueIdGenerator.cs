namespace Common
{
    public static class UniqueIdGenerator
    {
        public static string Generate(IEnumerable<string> existingIds, List<string> withoutIds = null)
        {
            string id = null;
            do
            {
                id = Guid.NewGuid().ToString();
            } while (withoutIds == null ? existingIds.Contains(id) : existingIds.Contains(id) && withoutIds.Contains(id));
            return id;
        }
    }
}
