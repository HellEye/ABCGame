using Reflex.Core;
using Reflex.Enums;

public static class ReflexExtensions {
    public static ContainerBuilder RegisterType<T>(this ContainerBuilder builder,
        Lifetime lifetime = Lifetime.Singleton, Resolution resolution = Resolution.Eager) =>
        builder.RegisterType(typeof(T), lifetime, resolution);
}