namespace lab4.Contracts.Services;

public interface IActivationService
{
    Task ActivateAsync(object activationArgs);
}
