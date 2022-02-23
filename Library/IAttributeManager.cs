namespace Library
{
    public interface IAttributeManager<T, Attribute>
    {
        public void Add(T t, IEnumerable<Attribute> attributes);
    }
}
