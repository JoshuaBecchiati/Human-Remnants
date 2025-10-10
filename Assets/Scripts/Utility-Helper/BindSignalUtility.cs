using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Timeline;

public static class BindSignalUtility
{
    private static List<SignalAsset> _signalAssets = new List<SignalAsset>();
    private static SignalReceiver _cachedReceiver;

    /// <summary>
    /// Ensure that there is a receiver
    /// </summary>
    /// <param name="receiver"></param>
    private static void EnsureSignalAssets(SignalReceiver receiver)
    {
        if (receiver == null)
        {
            Debug.LogWarning($"[{nameof(BindSignalUtility)}] Cannot ensure signals: receiver is null.");
            _cachedReceiver = null;
            _signalAssets.Clear();
            return;
        }

        if (_cachedReceiver != receiver || _signalAssets == null || _signalAssets.Count == 0)
        {
            _signalAssets = receiver.GetRegisteredSignals().ToList();
            _cachedReceiver = receiver;
        }
    }
    /// <summary>
    /// Bind a single UnityAction to a single SignalAsset
    /// </summary>
    /// <param name="call"></param>
    /// <param name="signalAssetName"></param>
    /// <param name="signalReceiver"></param>
    public static void BindSingleCallSingleAsset(UnityAction call, string signalAssetName, SignalReceiver signalReceiver)
    {
        EnsureSignalAssets(signalReceiver);

        SignalAsset sa = _signalAssets.Find(s => s.name == signalAssetName);
        if (sa == null)
        {
            Debug.LogWarning($"[{nameof(BindSignalUtility)}] Signal '{signalAssetName}' not found in receiver {signalReceiver.name}");
            return;
        }

        UnityEvent ue = signalReceiver.GetReaction(sa);
        if (ue == null)
        {
            Debug.LogWarning($"[{nameof(BindSignalUtility)}] Signal '{signalAssetName}' has no UnityEvent associated.");
            return;
        }

        ue.AddListener(call);
        Debug.Log($"[{nameof(BindSignalUtility)}] Bound '{signalAssetName}' → {call.Method.Name}");
    }

    /// <summary>
    /// Bind a single UnityAction to a single SignalAsset
    /// </summary>
    /// <param name="args"></param>
    public static void BindSingleCallSingleAsset(BindSignalArgs args)
    {
        EnsureSignalAssets(args.Receiver);

        SignalAsset sa = _signalAssets.Find(s => s.name == args.SignalAssetNames[0]);
        if (sa == null)
        {
            Debug.LogWarning($"[{nameof(BindSignalUtility)}] Signal '{args.SignalAssetNames[0]}' not found in receiver {args.Receiver.name}");
            return;
        }

        UnityEvent ue = args.Receiver.GetReaction(sa);
        if (ue == null)
        {
            Debug.LogWarning($"[{nameof(BindSignalUtility)}] Signal '{args.SignalAssetNames[0]}' has no UnityEvent associated.");
            return;
        }

        ue.AddListener(args.Calls[0]);
        Debug.Log($"[{nameof(BindSignalUtility)}] Bound '{args.SignalAssetNames[0]}' → {args.Calls[0].Method.Name}");
    }

    /// <summary>
    /// Bind multiple UnityActions to a single SignalAsset
    /// </summary>
    /// <param name="calls"></param>
    /// <param name="signalAssetName"></param>
    /// <param name="signalReceiver"></param>
    public static void BindMultiCallsToSingleAsset(UnityAction[] calls, string signalAssetName, SignalReceiver signalReceiver)
    {
        for (int i = 0; i < calls.Length; i++)
            BindSingleCallSingleAsset(calls[i], signalAssetName, signalReceiver);
    }

    /// <summary>
    /// Bind multiple UnityActions to a single SignalAsset
    /// </summary>
    /// <param name="args"></param>
    public static void BindMultiCallsToSingleAsset(BindSignalArgs args)
    {
        for (int i = 0; i < args.Calls.Length; i++)
            BindSingleCallSingleAsset(args);
    }

    /// <summary>
    /// Bind a single UnityAction to multiple SignalAssets
    /// </summary>
    /// <param name="call"></param>
    /// <param name="signalAssetNames"></param>
    /// <param name="signalReceiver"></param>
    public static void BindSingleCallToMultiAssets(UnityAction call, string[] signalAssetNames, SignalReceiver signalReceiver)
    {
        for (int i = 0; i < signalAssetNames.Length; i++)
            BindSingleCallSingleAsset(call, signalAssetNames[i], signalReceiver);
    }

    /// <summary>
    /// Bind a single UnityAction to multiple SignalAssets
    /// </summary>
    /// <param name="args"></param>
    public static void BindSingleCallToMultiAssets(BindSignalArgs args)
    {
        for (int i = 0; i < args.SignalAssetNames.Length; i++)
            BindSingleCallSingleAsset(args);
    }

    /// <summary>
    /// Bind multiple UnityActions to multiple SignalAssets, in 1:1 correspondence
    /// </summary>
    /// <param name="calls"></param>
    /// <param name="signalAssetNames"></param>
    /// <param name="signalReceiver"></param>
    public static void BindMultiCallsToMultiAssets(UnityAction[] calls, string[] signalAssetNames, SignalReceiver signalReceiver)
    {
        if (calls.Length != signalAssetNames.Length)
        {
            Debug.LogError($"[{nameof(BindSignalUtility)}] Mismatch: {calls.Length} calls for {signalAssetNames.Length} signals.");
            return;
        }

        for (int i = 0; i < calls.Length; i++)
            BindSingleCallSingleAsset(calls[i], signalAssetNames[i], signalReceiver);
    }

    /// <summary>
    /// Bind multiple UnityActions to multiple SignalAssets, in 1:1 correspondence
    /// </summary>
    /// <param name="args"></param>
    public static void BindMultiCallsToMultiAssets(BindSignalArgs args)
    {
        if (args.Calls.Length != args.SignalAssetNames.Length)
        {
            Debug.LogError($"[{nameof(BindSignalUtility)}] Mismatch: {args.Calls.Length} calls for {args.SignalAssetNames.Length} signals.");
            return;
        }

        for (int i = 0; i < args.Calls.Length; i++)
            BindSingleCallSingleAsset
            (
                args.Calls[i],
                args.SignalAssetNames[i],
                args.Receiver
            );
    }
}
