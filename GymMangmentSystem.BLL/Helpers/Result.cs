using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMangmentSystem.BLL.Helpers
{
    public record Result(bool success , string? Error = null , ResultKind Kind = ResultKind.Ok)
    {
        public static Result Ok()
        {
            return new Result(true);
        }

        public static Result Fail(string error , ResultKind kind = ResultKind.Conflict)
        {
            return new Result(false, error, kind);
        }

        public static Result NotFound (string error = "Not Found")
        {
            return new Result(false , error , ResultKind.NotFound);
        }

        public static Result Validation (string error)
        {
            return new Result(false, error, ResultKind.ValidationFailed);
        }

    }

    public record Result<T>(bool success, T? Value , string? Error = null, ResultKind Kind = ResultKind.Ok)
    {
        public static Result<T> Ok(T value)
        {
            return new Result<T>(true , value);
        }

        public static Result<T> Fail(string error, ResultKind kind = ResultKind.Conflict)
        {
            return new Result<T>(false, default, error, kind);
        }

        public static Result<T> NotFound(string error = "Not Found")
        {
            return new Result<T>(false, default, error, ResultKind.NotFound);
        }

        public static Result Validation(string error)
        {
            return new Result(false, error, ResultKind.ValidationFailed);
        }

    }

    public enum ResultKind
    {
        Ok,
        NotFound,
        Conflict,
        ValidationFailed,
        Forbidden
    }
}
