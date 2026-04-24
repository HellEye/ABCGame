using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEngine.UIElements;

public class MainMenuOptionsData : IDataSourceViewHashProvider, INotifyBindablePropertyChanged
{
    private long viewVersion;
    public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;
    public long GetViewHashCode() => viewVersion;
    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        viewVersion++;
        propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(propertyName));
    }
}