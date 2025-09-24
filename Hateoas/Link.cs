namespace MottuFlow.Hateoas
{
    public class Link
    {
        public required string Href { get; set; } // Adicionando 'required'
        public required string Rel { get; set; }  // Adicionando 'required'
        public required string Method { get; set; }  // Adicionando 'required'
    }
}

