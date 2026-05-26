using GarageInventoryPartsFinder.Application.Authentication;

namespace GarageInventoryPartsFinder.Application.Validation;

public static class AuthRequestValidator
{
    public static Dictionary<string, string[]> ValidateRegister(RegisterRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains('@'))
        {
            errors["email"] = ["A valid email is required."];
        }

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
        {
            errors["password"] = ["Password must be at least 8 characters."];
        }

        if (string.IsNullOrWhiteSpace(request.DisplayName))
        {
            errors["displayName"] = ["Display name is required."];
        }

        return errors;
    }

    public static Dictionary<string, string[]> ValidateLogin(LoginRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            errors["email"] = ["Email is required."];
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            errors["password"] = ["Password is required."];
        }

        return errors;
    }

    public static Dictionary<string, string[]> ValidateName(string name)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(name))
        {
            errors["name"] = ["Name is required."];
        }

        if (!string.IsNullOrWhiteSpace(name) && name.Length > 160)
        {
            errors["name"] = ["Name must be 160 characters or fewer."];
        }

        return errors;
    }
}
