namespace Orleans.Sample.Web.Models
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