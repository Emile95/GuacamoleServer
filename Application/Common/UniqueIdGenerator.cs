namespace Application.Common
{
    public static class UniqueIdGenerator
    {
        public static string Generate(List<string> existingIds)
        {
            string id = null;
            do
            {
                id = Guid.NewGuid().ToString();
            } while (existingIds.Contains(id));
            return id;
        }

        public static string GenerateWithout(List<string> existingIds, List<string> withoutIds)
        {
            string id = null;
            do
            {
                id = Guid.NewGuid().ToString();
            } while (existingIds.Contains(id) && withoutIds.Contains(id));
            return id;
        }
    }
}
