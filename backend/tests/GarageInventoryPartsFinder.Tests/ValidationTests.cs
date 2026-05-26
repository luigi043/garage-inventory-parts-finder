using GarageInventoryPartsFinder.Application.Authentication;
using GarageInventoryPartsFinder.Application.Validation;

namespace GarageInventoryPartsFinder.Tests;

public class ValidationTests
{
    [Fact]
    public void RegisterValidationRequiresValidEmail()
    {
        var errors = AuthRequestValidator.ValidateRegister(new RegisterRequest("invalid", "Passw0rd!", "Demo"));

        Assert.Contains("email", errors.Keys);
    }

    [Fact]
    public void NameValidationRejectsBlankWorkflowItemName()
    {
        var errors = AuthRequestValidator.ValidateName("");

        Assert.Contains("name", errors.Keys);
    }
}
