namespace HITS.LIB.Sqlite
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DataAttributes : Attribute
    {
        public bool SkipData { get; set; } = false;

        public DataAttributes()
        {
        }
    }
}
