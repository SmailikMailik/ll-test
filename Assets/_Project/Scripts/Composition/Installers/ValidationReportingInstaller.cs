using LL.Infrastructure.Validation;
using LL.Validation.Reporting;
using VContainer;
using VContainer.Unity;

namespace LL.Composition.Installers
{
    internal sealed class ValidationReportingInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterInstance<IValidationIssueFormatter>(new ValidationIssueFormatter());
            builder.Register<UnityConsoleValidationReporter>(Lifetime.Singleton).As<IValidationReporter>();
        }
    }
}