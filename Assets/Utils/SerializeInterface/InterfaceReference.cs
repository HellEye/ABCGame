using System;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class InterfaceReference<TInterface, TObject> where TObject : Object where TInterface : class {
    [SerializeField] [HideInInspector] TObject underlyingValue;

    public InterfaceReference() { }

    public InterfaceReference(TObject target) => underlyingValue = target;

    public InterfaceReference(TInterface @interface) => underlyingValue = @interface as TObject;

    public TInterface Value {
        get {
            if (underlyingValue == null) return null;
            if (underlyingValue is TInterface @interface) return @interface;

            throw new InvalidOperationException(
                $"{underlyingValue} needs to implement interface {nameof(TInterface)}.");
        }
        set {
            if (value == null) underlyingValue = null;
            else if (value is TObject obj) underlyingValue = obj;
            else throw new ArgumentException($"{value} needs to be of type {typeof(TObject)}");
        }
    }


    public TObject UnderlyingValue { get => underlyingValue; set => underlyingValue = value; }


    public static implicit operator TInterface(InterfaceReference<TInterface, TObject> obj) => obj.Value;
}

[Serializable]
public class InterfaceReference<TInterface> : InterfaceReference<TInterface, Object> where TInterface : class { }