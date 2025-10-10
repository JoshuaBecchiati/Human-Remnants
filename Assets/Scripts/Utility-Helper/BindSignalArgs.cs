using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Timeline;

/// <summary>
/// Container class for signal binding data.
/// </summary>
public class BindSignalArgs
{
    public UnityAction[] Calls { get; private set; }
    public string[] SignalAssetNames { get; private set; }
    public SignalReceiver Receiver { get; private set; }

    // --- COSTRUTTORI ---

    /// <summary>
    /// Single call → single signal.
    /// </summary>
    public BindSignalArgs(UnityAction call, string signalAssetName, SignalReceiver receiver)
    {
        Calls = new[] { call };
        SignalAssetNames = new[] { signalAssetName };
        Receiver = receiver;
    }

    /// <summary>
    /// Multiple calls → single signal.
    /// </summary>
    public BindSignalArgs(UnityAction[] calls, string signalAssetName, SignalReceiver receiver)
    {
        Calls = calls;
        SignalAssetNames = new[] { signalAssetName };
        Receiver = receiver;
    }

    /// <summary>
    /// Single call → multiple signals.
    /// </summary>
    public BindSignalArgs(UnityAction call, string[] signalAssetNames, SignalReceiver receiver)
    {
        Calls = new[] { call };
        SignalAssetNames = signalAssetNames;
        Receiver = receiver;
    }

    /// <summary>
    /// Multiple calls → multiple signals (1:1).
    /// </summary>
    public BindSignalArgs(UnityAction[] calls, string[] signalAssetNames, SignalReceiver receiver)
    {
        Calls = calls;
        SignalAssetNames = signalAssetNames;
        Receiver = receiver;
    }

    /// <summary>
    /// Basic validation before use.
    /// </summary>
    public bool IsValid(out string message)
    {
        if (Receiver == null)
        {
            message = "Receiver is null.";
            return false;
        }

        if (Calls == null || Calls.Length == 0)
        {
            message = "No UnityActions provided.";
            return false;
        }

        if (SignalAssetNames == null || SignalAssetNames.Length == 0)
        {
            message = "No SignalAsset names provided.";
            return false;
        }

        if (Calls.Length > 1 && SignalAssetNames.Length > 1 && Calls.Length != SignalAssetNames.Length)
        {
            message = "Mismatch between number of calls and signal names.";
            return false;
        }

        message = null;
        return true;
    }
}