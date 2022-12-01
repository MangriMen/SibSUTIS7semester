namespace lab6.Contracts.Services;

public interface IActivationService
{
    Task ActivateAsync(object activationArgs);
}
