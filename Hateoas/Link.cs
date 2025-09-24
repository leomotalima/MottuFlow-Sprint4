namespace MottuFlow.Hateoas
{
    public class Link
    {
        public string Href { get; set; }  // O endereço do link (URL)
        public string Rel { get; set; }   // O tipo do link (ex: "self", "update", etc.)
        public string Method { get; set; } // O método HTTP, como 'GET', 'PUT', 'DELETE', etc.
    }
}
