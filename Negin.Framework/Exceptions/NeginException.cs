namespace Negin.Framework.Exceptions;

public class SqlException: BLMessage
{
    public SqlExceptionState SqlState { get { return SqlState; } set { SqlState = value; State = false; } }
    public Object? sqlResult { get; set; }
    public enum SqlExceptionState
    {
        DuplicateName = 0,
        AgentIsEmpty = 1,
    }
}

public class BLMessage
{
    public string? Message { get; set; }
    public bool State { get; set; }
}
