using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDT.Data.BaseEntity
{
    public abstract class BaseEntity<T> : IBaseEntity<T>
    {
        protected BaseEntity()
        {
            CreatedDate = DateTime.UtcNow;
        }

        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public T Id { get; set; }

        object IBaseEntity.Id
        {
            get { return Id; }
            set => Id = (T)value;
        }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? ModifiedDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? DeletedDate { get; set; }

        public int? CreatedBy { get; set; }

        object IBaseEntity.CreatedBy
        {
            get { return CreatedBy; }
            set => CreatedBy = (int?)value;
        }

        public int? ModifiedBy { get; set; }

        object IBaseEntity.ModifiedBy
        {
            get { return ModifiedBy; }
            set => ModifiedBy = (int?)value;
        }

        public int? DeletedBy { get; set; }

        object IBaseEntity.DeletedBy
        {
            get { return DeletedBy; }
            set => DeletedBy = (int?)value;
        }

        [Timestamp]
        public byte[]? Version { get; set; }
    }
}
