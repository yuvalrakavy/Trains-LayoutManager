using LayoutManager.Model;
using MethodDispatcher;
using System;
using System.Collections.Generic;

#nullable enable
namespace LayoutManager.Logic {
    /// <summary>
    /// Handle train motion issues, such as acceleration
    /// </summary>
    [LayoutModule("Event Script Manager", UserControl = false)]
    public class EventScriptManagerManager : LayoutModuleBase {
        private readonly Dictionary<Guid, LayoutEventScript> activeScripts = new();

        [DispatchTarget]
        private void OnEventScriptReset(LayoutEventScript script, LayoutEventScriptTask task) {
            if (!activeScripts.ContainsKey(script.Id))
                activeScripts.Add(script.Id, script);
        }

        [DispatchTarget]
        private void OnEventScriptDispose(LayoutEventScript script) {
            OnEventScriptTerminated(script);
        }

        [DispatchTarget]
        private void OnEventScriptTerminated(LayoutEventScript script) {
            activeScripts.Remove(script.Id);
        }

        [DispatchTarget]
        private LayoutEventScript? GetActiveScript(Guid scriptId) {
            if (activeScripts.TryGetValue(scriptId, out var activeScript)) {
                return activeScript;
            }
            return null;
        }

        [DispatchTarget(Order = 1000)]
        private void OnEnteredOperationMode(OperationModeParameters settings) {
            foreach (LayoutPolicyInfo policy in LayoutModel.StateManager.LayoutPolicies) {
                if (policy.Apply && policy.EventScriptElement != null) {
                    LayoutEventScript runningScript = EventManager.EventScript($"Global policy {policy.Name}", policy.EventScriptElement, Array.Empty<Guid>(), null);

                    runningScript.Id = policy.Id;
                    runningScript.Reset();
                }
            }
        }

        [DispatchTarget(Order = -1000)]
        private void OnExitOperationMode() {
            foreach (LayoutEventScript script in activeScripts.Values)
                script.Dispose();
        }
    }
}
