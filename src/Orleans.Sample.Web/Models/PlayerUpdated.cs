namespace Orleans.Sample.Web
{
    public class PlayerUpdated
    {
        public string Name { get; }

        public PlayerUpdated(string name)
        {
            Name = name;
        }
    }
}