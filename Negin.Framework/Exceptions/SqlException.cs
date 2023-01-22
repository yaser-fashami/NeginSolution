
namespace Negin.Framework.Exceptions;

public struct SqlException
{
    public SqlExceptionMessages State { get; set; }
    public string? Message { get; set; }

    public enum SqlExceptionMessages
    {
        Fail = 0,
        Success = 1,
        DuplicateName = 2,
        AgentIsEmpty = 3,
    }
}
