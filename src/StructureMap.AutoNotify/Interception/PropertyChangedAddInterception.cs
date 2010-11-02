using System.ComponentModel;
using Castle.DynamicProxy;

namespace StructureMap.AutoNotify.Interception
{
    class PropertyChangedAddInterception : IInterception
    {
        readonly PropertyChangedInterceptor _propertyChangedInterceptor;
        readonly IInvocation _invocation;

        public PropertyChangedAddInterception(PropertyChangedInterceptor propertyChangedInterceptor, IInvocation invocation)
        {
            _propertyChangedInterceptor = propertyChangedInterceptor;
            _invocation = invocation;
        }

        public void Call()
        {
            var onPropertyChanged = (PropertyChangedEventHandler)_invocation.GetArgumentValue(0);
            _propertyChangedInterceptor.PropertyChanged += onPropertyChanged;
        }
    }
}