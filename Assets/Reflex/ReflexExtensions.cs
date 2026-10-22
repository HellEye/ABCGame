using System;
using Reflex.Core;
using Reflex.Enums;

public static class ReflexExtensions {
    public static ContainerBuilder RegisterType<T>(this ContainerBuilder builder,
        Lifetime lifetime = Lifetime.Singleton, Resolution resolution = Resolution.Eager) =>
        builder.RegisterType(typeof(T), lifetime, resolution);

    public static ContainerBuilder
        RegisterValue(this ContainerBuilder builder, object value, params Type[] contracts) =>
        builder.RegisterValue(value, contracts);

    public static ContainerBuilder RegisterType<T>(this ContainerBuilder builder, Lifetime lifetime,
        Resolution resolution, params Type[] contracts) =>
        builder.RegisterType(typeof(T), contracts, lifetime, resolution);
}