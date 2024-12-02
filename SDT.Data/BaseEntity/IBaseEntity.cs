namespace SDT.Data.BaseEntity
{
    public interface IBaseEntity
    {
        object Id { get; set; }
        DateTime CreatedDate { get; set; }
        DateTime? ModifiedDate { get; set; }
        DateTime? DeletedDate { get; set; }
        object? CreatedBy { get; set; }
        object? ModifiedBy { get; set; }
        object? DeletedBy { get; set; }
        byte[] Version { get; set; }
    }

    public interface IBaseEntity<T> : IBaseEntity
    {
        new T Id { get; set; }
    }
}
