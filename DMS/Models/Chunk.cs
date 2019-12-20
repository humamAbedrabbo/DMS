namespace DAS.Models
{
    public class Chunk
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public Document Document { get; set; }
        public int Version { get; set; }
        public int SortId { get; set; }
        public long Length { get; set; }
        public byte[] Contents { get; set; }
    }
}
