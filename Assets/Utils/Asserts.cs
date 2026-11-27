using System.Diagnostics;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using Debug = UnityEngine.Debug;

public class Asserts {
    [Conditional("UNITY_ASSERTIONS")]
    static void AssertThread(bool expectMain, [CallerMemberName] string caller = "") {
        if (PlayerLoopHelper.IsMainThread != expectMain)
            Debug.LogError($"{caller}: expected main={expectMain}");
    }
}