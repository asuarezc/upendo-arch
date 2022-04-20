using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using DryIoc;
using upendo.CrossCutting.Localization;
using upendo.Managers;

namespace upendo.Models
{
    public abstract class BaseModel : INotifyPropertyChanged, IDisposable
    {
        private bool disposedValue;
        
        private readonly ITranslationManager translationManager;

        protected ITranslationManager TranslationManager => translationManager;

        public event PropertyChangedEventHandler PropertyChanged;

        protected DryIoc.IContainer Container => DependencyServiceManager.Instance.Container;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void NotifyPropertyChanged<T>(Expression<Func<T>> propertyLambda)
        {
            if (propertyLambda == null)
                throw new ArgumentNullException(nameof(propertyLambda));

            if (propertyLambda.Body is not MemberExpression memberExpression)
                throw new ArgumentException($"Expression \"{propertyLambda}\" refers to a method, not a property.");

            if (memberExpression.Member is not PropertyInfo propertyInfo)
                throw new ArgumentException($"Expression \"{propertyLambda}\" refers to a field, not a property.");

            NotifyPropertyChanged(propertyInfo.Name);
        }

        public BaseModel()
        {
            translationManager = Container.Resolve<ITranslationManager>();
        }

        public string GetResource(StringKey key) => translationManager.GetResource(key);

        public string GetResource(string key) => translationManager.GetResource(key);

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
                return;

            disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
