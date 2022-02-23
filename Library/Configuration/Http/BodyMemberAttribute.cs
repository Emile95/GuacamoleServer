namespace Library.Configuration.Http
{
    public  class BodyMemberAttribute : Attribute
    {
        public string Name { get; set; }
        public bool IsRequired { get; set; }
    }
}
