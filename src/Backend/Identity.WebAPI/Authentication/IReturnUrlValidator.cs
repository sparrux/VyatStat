namespace Identity.WebAPI.Authentication;

public interface IReturnUrlValidator
{
    bool IsValidAuthorizeReturnUrl(string returnUrl);

    bool IsValidClientReturnUrl(string returnUrl);
}
