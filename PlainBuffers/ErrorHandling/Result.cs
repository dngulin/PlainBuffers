using System;

namespace PlainBuffers.ErrorHandling {
  internal readonly struct Result<TValue, TError> {
    private readonly TValue _value;
    private readonly TError _error;

    private Result(TValue value, TError error, bool isError) {
      _value = value;
      _error = error;
      HasError = isError;
    }

    public bool HasError { get; }

    public bool TryGetError(out TError error) {
      error = _error;
      return HasError;
    }

    public TError Error {
      get {
        if (!HasError)
          throw new InvalidOperationException();

        return _error;
      }
    }

    public TValue Unwrap() {
      if (HasError)
        throw new InvalidOperationException();

      return _value;
    }

    public static Result<TValue, TError> Ok(TValue value) => new Result<TValue, TError>(value, default, false);
    public static Result<TValue, TError> Fail(TError error) => new Result<TValue, TError>(default, error, true);
  }
}