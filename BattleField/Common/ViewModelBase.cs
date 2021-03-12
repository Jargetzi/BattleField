using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;

namespace BattleField
{
    public abstract class ViewModelBase : NotifyPropertyChangedBase, IDataErrorInfo, IDisposable
    {
        #region Fields
        private Dictionary<string, string> _errors = new Dictionary<string, string>();
        //private ServiceLocator serviceLocator = new ServiceLocator();
        protected List<string> _cleanProperties = new List<string>() { "ErrorsExist", "IsDirty", "IgnoreDirty", "Error", "ServiceLocator" };
        #endregion Fields

        #region Properties
        public bool ErrorsExist { get { return _errors.Count > 0; } }

        /// <summary>
        /// IsDirty is flagged every time the OnPropertyChanged method is called unless the property that has changed is included in the list of clean properties.
        /// It is up to the sub class to reset IsDirty whenever the object is refreshed, saved, initialized, etc.
        /// </summary>
        public bool IsDirty { get; set; }

        public bool IgnoreDirty { get; set; }

        public string Error { get { return "Error"; } }

        public string this[string columnName]
        {
            get
            {
                if (!PropertyHasError(columnName))
                    return "";
                return GetPropertyError(columnName);
            }
        }

        /// <summary>
        /// Gets the service locator 
        /// </summary>
        //public ServiceLocator ServiceLocator
        //{
        //    get
        //    {
        //        return serviceLocator;
        //    }
        //}

        public bool IsDisposed { get; set; }
        #endregion Properties

        #region Methods
        public new void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (!IgnoreDirty && !IsDirty && !_cleanProperties.Contains(propertyName))
                IsDirty = true;
            base.OnPropertyChanged(propertyName);
        }

        private void OnPropertyChangedIgnoringDirty(string propertyName)
        {
            var oldIgnoreDirty = IgnoreDirty;
            IgnoreDirty = true;
            OnPropertyChanged(propertyName);
            IgnoreDirty = oldIgnoreDirty;
        }

        public void AddCleanProperties(params string[] cleanProperties)
        {
            foreach (var property in cleanProperties)
                CheckProperty(property);
            _cleanProperties.AddRange(cleanProperties);
        }

        public bool ValidateRule(bool rule, string ruleProperty, string ruleDescription)
        {
            if (!rule)
            {
                AddError(ruleProperty, ruleDescription);
            }
            return rule;
        }

        public void AddError(string propertyName, string error)
        {
            CheckProperty(propertyName);
            if (_errors.ContainsKey(propertyName))
            {
                _errors[propertyName] = error;
            }
            else
            {
                _errors.Add(propertyName, error);
            }
            try
            {
                //Trigger property to catch exception 
                OnPropertyChangedIgnoringDirty(propertyName);
            }
            catch { }
        }

        public void RemoveError(string propertyName)
        {
            _errors.Remove(propertyName);
        }

        /// <summary>
        /// Clears all errors attached to class properties.
        /// </summary>
        /// <param name="removeErrorTemplates">
        /// This should be true if the UI controls with error templates applied need to be removed.
        /// </param>
        public void ClearErrors(bool removeErrorTemplates)
        {
            _errors.Clear();
            if (!removeErrorTemplates)
                return;

            var properties = GetType().GetProperties();
            foreach (var property in properties)
            {
                if (!_cleanProperties.Contains(property.Name))
                    OnPropertyChangedIgnoringDirty(property.Name);
            }
        }

        public void ClearError(string propertyName, bool removeErrorTemplate)
        {
            CheckProperty(propertyName);
            _errors.Remove(propertyName);
            if (removeErrorTemplate)
                OnPropertyChangedIgnoringDirty(propertyName);
        }

        public List<String> GetErrors()
        {
            return new List<string>(_errors.Values);
        }

        public bool PropertyHasError([CallerMemberName] string propertyName = null)
        {
            return _errors.ContainsKey(propertyName);
        }

        public string GetPropertyError([CallerMemberName] string propertyName = null)
        {
            return _errors[propertyName];
        }

        public void Dispose()
        {
            _errors.Clear();
            _cleanProperties.Clear();
            IsDisposed = true;
        }

        /// <summary>
        /// Gets a service from the service locator
        /// </summary>
        /// <typeparam name="T">The type of service to return</typeparam>
        /// <returns>Returns a service that was registered with the Type T</returns>
        //public T GetService<T>()
        //{
        //    return serviceLocator.GetService<T>();
        //}

        private void CheckProperty(string propertyName)
        {
            var property = GetType().GetProperty(propertyName);
            if (property == null)
                throw new Exception(propertyName + " is not a valid property");
        }

        //public event PropertyChangedEventHandler PropertyChanged = new PropertyChangedEventHandler();
        //protected void RaisePropertyChangedEvent(string propertyName)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChangedEventArgs e = new PropertyChangedEventArgs(propertyName);
        //        PropertyChanged(this, e);
        //    }
        //}
        #endregion Methods
    }

    public class ErrorBinding : Binding
    {
        public ErrorBinding() : base()
        {
            Initiate();
        }

        public ErrorBinding(string path) : base(path)
        {
            Initiate();
        }

        private void Initiate()
        {
            ValidatesOnDataErrors = true;
            NotifyOnValidationError = true;
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        }
    }
}
