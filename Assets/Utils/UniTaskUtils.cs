using System;
using Cysharp.Threading.Tasks;

public static class UniTaskUtils {
    public static Action<T> AsAction<T>(Func<T, UniTask> task) => arg => task(arg).Forget();
}