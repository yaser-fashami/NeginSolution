using System.ComponentModel.DataAnnotations;

namespace Negin.Core.Domain.Entities;

public abstract class BaseEntity: BaseEntity<ulong>
{
}

public abstract class BaseEntity<TKey> 
{
    [Key]
    [Required]
    public TKey Id { get; set; }
	public string? CreatedById { get; set; }
	public User? CreatedBy { get; set; }
	public DateTime CreateDate { get; set; }
	public string? ModifiedById { get; set; }
	public User? ModifiedBy { get; set; }
	public DateTime? ModifiedDate { get; set; }
	public bool IsDelete { get; set; } = false;

}

public abstract class BaseBasicInformation<TKey>
{
	[Key]
	[Required]
	public TKey Id { get; set; }
}
