namespace DAS.Models
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
    }
}
