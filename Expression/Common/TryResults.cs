namespace Expression.Common;

public class TryResults<T, TErrorCode>
{
    public T? Value { get; }
    public bool Success { get; }
    public bool Failed => !Success;
    public string ErrorMessage { get; }
    public TErrorCode? ErrorCode { get; }

    public TryResults(T value)
    {
        Value = value;
        Success = true;
        ErrorMessage = string.Empty;
    }

    public TryResults(string errorMessage, TErrorCode? errorCode = default)
    {
        Value = default;
        Success = false;
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
    }
}

public class TryResults<T> : TryResults<T, int>
{
    public TryResults(T value) : base(value) { }

    public TryResults(string errorMessage, int errorCode = default) : base(errorMessage, errorCode) { }

    public static TryResults<T> Fail(string errorMessage, int errorCode = default)
    {
        return new TryResults<T>(errorMessage, errorCode);
    }

    public static TryResults<T> Succeed(T value)
    {
        return new TryResults<T>(value);
    }
}