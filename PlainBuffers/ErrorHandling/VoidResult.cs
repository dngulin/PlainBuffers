using System;

namespace PlainBuffers.ErrorHandling {
  internal readonly struct VoidResult<TError> {
    private readonly TError _error;

    private VoidResult(TError error) {
      _error = error;
      HasError = true;
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

    public static VoidResult<TError> Ok() => new VoidResult<TError>();
    public static VoidResult<TError> Fail(TError err) => new VoidResult<TError>(err);
  }
}