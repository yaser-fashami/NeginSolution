
namespace Negin.Core.Domain.Interfaces;
public interface IAppVersionService
{
	string Version { get; }
    public Beneficiary Beneficiary { get; }

}

public enum Beneficiary
{
	Negin, SinaOil
}
