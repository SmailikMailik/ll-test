using System;

namespace LL.UI.Windows
{
    internal abstract class Window<TParameter> : WindowBase
        where TParameter : class, IWindowParameters
    {
        internal override Type ParameterType => typeof(TParameter);
        internal override IWindowParameters CurrentParameters => Parameters;

        protected TParameter Parameters { get; private set; }

        internal override void Show(IWindowParameters parameters)
        {
            if (parameters is not TParameter typedParameters)
            {
                throw new ArgumentException(
                    $"Expected {typeof(TParameter).Name}, got {parameters?.GetType().Name ?? "null"}",
                    nameof(parameters));
            }

            Parameters = typedParameters;
            Show();
        }
    }
}