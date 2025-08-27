using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Camera.MAUI.Internals;

/// <summary>
/// Provides base impl for supporting bindings via <see cref="System.ComponentModel.INotifyPropertyChanged"/>
/// </summary>
public abstract class ObservableBase : System.ComponentModel.INotifyPropertyChanged,
                                       System.ComponentModel.INotifyPropertyChanging
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;

    /// <summary>
    /// Call this from a property setter BEFORE setting the backing field,
    /// calling <see cref="SetProperty{T}(ref T, T, string)"/>,
    /// or calling <see cref="OnPropertyChanged(string)"/>
    /// </summary>
    /// <param name="propertyName"></param>
    virtual protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
    }

    virtual protected void OnPropertyChanging([CallerMemberName] string propertyName = null)
    {
        PropertyChanging?.Invoke(this, new System.ComponentModel.PropertyChangingEventArgs(propertyName));
    }

    virtual protected bool SetProperty<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingField, value))
            return false;

        // call now, no performance hit if no event subscribers.
        OnPropertyChanging(propertyName);

        backingField = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    /// <summary>
    /// Redirects to <see cref="SetProperty{T}(ref T, T, string)"/>, makes translating from bindable properties easier
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="backingField"></param>
    /// <param name="value"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    protected bool SetValue<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null) =>
        SetProperty(ref backingField, value, propertyName);
}