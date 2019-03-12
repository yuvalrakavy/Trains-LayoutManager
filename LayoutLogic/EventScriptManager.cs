using System;
using System.Collections;
using System.Collections.Specialized;
using LayoutManager;
using LayoutManager.Model;

#pragma warning disable IDE0051, IDE0060
#nullable enable
namespace LayoutManager.Logic {
    /// <summary>
    /// Handle train motion issues, such as acceleration
    /// </summary>
    [LayoutModule("Event Script Manager", UserControl = false)]
    public class EventScriptManagerManager : LayoutModuleBase {
        readonly IDictionary activeScripts = new HybridDictionary();

        [LayoutEvent("event-script-reset")]
        private void eventScriptReset(LayoutEvent e) {
            var script = Ensure.NotNull<LayoutEventScript>(e.Sender, "script");

            if (!activeScripts.Contains(script.Id))
                activeScripts.Add(script.Id, script);
        }

        [LayoutEvent("event-script-dispose")]
        [LayoutEvent("event-script-terminated")]
        private void eventScriptDispose(LayoutEvent e) {
            var script = Ensure.NotNull<LayoutEventScript>(e.Sender, "script");

            activeScripts.Remove(script.Id);
        }

        [LayoutEvent("get-active-event-script")]
        private void isEventScriptActive(LayoutEvent e) {
            Guid scriptID = (Guid)e.Sender;

            e.Info = activeScripts[scriptID];
        }

        [LayoutEvent("get-active-event-scripts")]
        private void getActiveEventScripts(LayoutEvent e) {
            LayoutEventScript[] result = new LayoutEventScript[activeScripts.Count];

            int i = 0;
            foreach (LayoutEventScript script in activeScripts.Values)
                result[i++] = script;

            e.Info = result;
        }

        [LayoutEvent("enter-operation-mode", Order = 1000)]
        private void enterOperationMode(LayoutEvent e) {
            foreach (LayoutPolicyInfo policy in LayoutModel.StateManager.LayoutPolicies) {
                if (policy.Apply && policy.EventScriptElement != null) {
                    LayoutEventScript runningScript = EventManager.EventScript("Global policy " + policy.Name, policy.EventScriptElement, new Guid[] { }, null);

                    runningScript.Id = policy.Id;
                    runningScript.Reset();
                }
            }
        }

        [LayoutEvent("exit-operation-mode", Order = -1000)]
        private void exitOperationalMode(LayoutEvent e) {
            LayoutEventScript[] scripts = (LayoutEventScript[])EventManager.Event(new LayoutEvent("get-active-event-scripts", this))!;

            foreach (LayoutEventScript script in scripts)
                script.Dispose();
        }
    }
}
