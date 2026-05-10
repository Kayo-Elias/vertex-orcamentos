namespace VertexOrcamentos.Models
{
    public class Element
    {
        public int Number { get; set; }
        public string Sign { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Position { get; set; }
        public double Molar { get; set; }
        public string Group { get; set; } = string.Empty;
    }
}