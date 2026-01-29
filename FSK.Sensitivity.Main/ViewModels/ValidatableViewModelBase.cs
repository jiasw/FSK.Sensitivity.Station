using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

public class ValidatableViewModelBase : INotifyPropertyChanged, INotifyDataErrorInfo
{
    // 存储每个属性的错误信息
    private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

    #region INotifyPropertyChanged 实现

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    #endregion

    #region INotifyDataErrorInfo 实现

    // 是否有任何验证错误
    public bool HasErrors => _errors.Any();

    // 当错误状态改变时触发
    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    // 获取指定属性的所有错误
    public IEnumerable GetErrors(string propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
        {
            // 返回所有错误
            return _errors.Values.SelectMany(e => e).ToList();
        }

        if (_errors.ContainsKey(propertyName))
        {
            return _errors[propertyName];
        }

        return null;
    }

    #endregion

    #region 验证辅助方法

    // 添加错误
    protected void AddError(string propertyName, string errorMessage)
    {
        if (!_errors.ContainsKey(propertyName))
        {
            _errors[propertyName] = new List<string>();
        }

        if (!_errors[propertyName].Contains(errorMessage))
        {
            _errors[propertyName].Add(errorMessage);
            OnErrorsChanged(propertyName);
        }
    }

    // 移除指定属性的指定错误
    protected void RemoveError(string propertyName, string errorMessage)
    {
        if (_errors.ContainsKey(propertyName))
        {
            _errors[propertyName].Remove(errorMessage);

            if (_errors[propertyName].Count == 0)
            {
                _errors.Remove(propertyName);
            }

            OnErrorsChanged(propertyName);
        }
    }

    // 清除指定属性的所有错误
    protected void ClearErrors(string propertyName)
    {
        if (_errors.ContainsKey(propertyName))
        {
            _errors.Remove(propertyName);
            OnErrorsChanged(propertyName);
        }
    }

    // 清除所有错误
    protected void ClearAllErrors()
    {
        var propertyNames = _errors.Keys.ToList();
        _errors.Clear();

        foreach (var propertyName in propertyNames)
        {
            OnErrorsChanged(propertyName);
        }
    }

    // 触发错误状态改变事件
    protected virtual void OnErrorsChanged(string propertyName)
    {
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        OnPropertyChanged(nameof(HasErrors));
    }

    #endregion
}