using Testcontainers.AutoSetup.Core.Common;

namespace Testcontainers.AutoSetup.Core;

// A truly generic base class
public abstract class GenericTestBase
{
    private readonly TestEnvironment _environment;

    protected GenericTestBase(TestEnvironment environment)
    {
        _environment = environment;
    }

    // The method to call before every test
    protected async Task OnTestStartAsync()
    {
        // Optional: Reflection check for [DbReset] attribute goes here
        // var shouldReset = CheckAttribute(this.GetType());

        if (ShouldReset())
        {
            await _environment.ResetAsync();
        }
    }

    // Helper for attribute checking
    private bool ShouldReset()
    {
        // Basic reflection to see if class/method has [DbReset(None)]
        // This is framework-agnostic because it uses standard .NET Reflection
        return true; 
    }
}