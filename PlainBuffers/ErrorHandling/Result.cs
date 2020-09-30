using System;

namespace PlainBuffers.ErrorHandling {
  internal readonly struct Result<TValue, TError> {
    private readonly TValue _value;
    private readonly TError _error;
    private readonly bool _isError;

    private Result(TValue value, TError error, bool isError) {
      _value = value;
      _error = error;
      _isError = isError;
    }

    public bool HasError(out TError error) {
      error = _error;
      return _isError;
    }

    public TValue Unwrap() {
      if (_isError)
        throw new InvalidOperationException();

      return _value;
    }

    public static Result<TValue, TError> Ok(TValue value) => new Result<TValue, TError>(value, default, false);
    public static Result<TValue, TError> Fail(TError error) => new Result<TValue, TError>(default, error, true);
  }
}