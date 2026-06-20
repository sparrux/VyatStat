using Microsoft.AspNetCore.Identity;

namespace Identity.WebAPI.Extensions;

static class ErrorExtensions
{
    extension(IEnumerable<IdentityError> errors)
    {
        public IEnumerable<string> Stringify() => 
            errors.Select(err => $"{err.Code}: {err.Description}");
    }
}