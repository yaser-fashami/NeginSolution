
using DataAnnotationsExtensions;

namespace Negin.Core.Domain.Entities.Basic;

public class ShippingLineCompany: BaseEntity<uint>
{
	public string ShippingLineName { get; set; }
	public string EconomicCode { get; set; }
	public string NationalCode { get; set; }
	public string? Tel { get; set; }
	[Email]
	public string? Email { get; set; }
	public string? Address { get; set; }
	public string? City { get; set; }
	public string? Description { get; set; }
	public bool IsOwner { get; set; }
	public bool IsAgent { get; set; } 

	public virtual ICollection<AgentShippingLine>? Agents { get; set; }
	public virtual ICollection<Voyage>? OwnerVoyages { get; set; }
	public virtual ICollection<Voyage>? AgentVoyages { get; set; }

}
