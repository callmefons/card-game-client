#define DT_DEBUG

using UnityEngine;
using System.Collections;

public sealed class DTDebug 
{
    public static void Log(object message)
    {
#if DT_DEBUG
        Debug.Log(message);
#endif
    }

    public static void Log(object message, Object context)
    {
#if DT_DEBUG
        Debug.Log(message, context);
#endif
    }

    public static void LogWarning(object message)
    {
#if DT_DEBUG
        Debug.LogWarning(message);
#endif
    }

    public static void LogWarning(object message, Object context)
    {
#if DT_DEBUG
        Debug.LogWarning(message, context);
#endif
    }
}
