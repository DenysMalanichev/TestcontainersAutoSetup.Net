using Testcontainers.AutoSetup.Core.Common.Enums;

namespace Testcontainers.AutoSetup.Core.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class DbResetAttribute : Attribute
{
    public ResetScope Scope { get; }
    public DbResetAttribute(ResetScope scope = ResetScope.BeforeExecution) => Scope = scope;
}