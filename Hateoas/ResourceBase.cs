namespace MottuFlow.Hateoas
{
    public class ResourceBase
    {
        public int Id { get; set; }
        public List<Link> Links { get; set; } = new List<Link>();

        public void AddLink(Link link)
        {
            Links.Add(link);  // Método para adicionar um link à lista de links
        }
    }
}
