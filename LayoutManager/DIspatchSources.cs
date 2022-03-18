using LayoutManager.Model;
using MethodDispatcher;
using System.Windows.Forms;
using System.Xml;

namespace LayoutManager {
    public static class LayoutControllerDispatchSources {
        [DispatchSource]
        public static void CloseAllFrameWindows(this Dispatcher d) {
            d[nameof(CloseAllFrameWindows)].CallVoid();
        }

        static DispatchSource? ds_ActivateLearnLayout = null;
        [DispatchSource]
        public static Form? ActivateLearnLayout(this Dispatcher d) {
            ds_ActivateLearnLayout ??= d[nameof(ActivateLearnLayout)];
            return ds_ActivateLearnLayout.Call<Form?>();
        }

        static DispatchSource? ds_ExitDesignMode = null;
        [DispatchSource]
        public static void ExitDesignMode(this Dispatcher d) {
            ds_ExitDesignMode ??= d[nameof(ExitDesignMode)];
            ds_ExitDesignMode.CallVoid();
        }

        static DispatchSource? ds_QueryPoliciesDefinitionDialog = null;
        [DispatchSource]
        public static Form? QueryPoliciesDefinitionDialog(this Dispatcher d) {
            ds_QueryPoliciesDefinitionDialog ??= d[nameof(QueryPoliciesDefinitionDialog)];
            return ds_QueryPoliciesDefinitionDialog.Call<Form?>();
        }

        static DispatchSource? ds_OnBeginingTrainsAnalysisPhase = null;
        [DispatchSource]
        public static void OnBeginingTrainsAnalysisPhase(this Dispatcher d) {
            ds_OnBeginingTrainsAnalysisPhase ??= d[nameof(OnBeginingTrainsAnalysisPhase)];
            ds_OnBeginingTrainsAnalysisPhase.CallVoid();
        }

        static DispatchSource? ds_OnEndingTrainsAnalysisPhase = null;
        [DispatchSource]
        public static void OnEndingTrainsAnalysisPhase(this Dispatcher d) {
            ds_OnEndingTrainsAnalysisPhase ??= d[nameof(OnEndingTrainsAnalysisPhase)];
            ds_OnEndingTrainsAnalysisPhase.CallVoid();
        }

        static DispatchSource? ds_OnManualDispatchRegionStatusChanged = null;
        [DispatchSource]
        public static void OnManualDispatchRegionStatusChanged(this Dispatcher d, ManualDispatchRegionInfo manualDispatchRegion, bool active) {
            ds_OnManualDispatchRegionStatusChanged ??= d[nameof(OnManualDispatchRegionStatusChanged)];
            ds_OnManualDispatchRegionStatusChanged.CallVoid(manualDispatchRegion, active);
        }

        static DispatchSource? ds_OnAreaAdded = null;
        [DispatchSource]
        public static void OnAreaAdded(this Dispatcher d, LayoutModelArea area) {
            ds_OnAreaAdded ??= d[nameof(OnAreaAdded)];
            ds_OnAreaAdded.CallVoid(area);
        }

        static DispatchSource? ds_OnAreaRemoved = null;
        [DispatchSource]
        public static void OnAreaRemoved(this Dispatcher d, LayoutModelArea area) {
            ds_OnAreaRemoved ??= d[nameof(OnAreaRemoved)];
            ds_OnAreaRemoved.CallVoid(area);
        }

        static DispatchSource? ds_OnAreaRenamed = null;
        [DispatchSource]
        public static void OnAreaRenamed(this Dispatcher d, LayoutModelArea area) {
            ds_OnAreaRenamed ??= d[nameof(OnAreaRenamed)];
            ds_OnAreaRenamed.CallVoid(area);
        }

        static DispatchSource? ds_SaveLayout = null;
        [DispatchSource]
        public static void SaveLayout(this Dispatcher d) {
            ds_SaveLayout ??= d[nameof(SaveLayout)];
            ds_SaveLayout.CallVoid();
        }

        static DispatchSource? ds_ChangePhase = null;
        [DispatchSource]
        public static void ChangePhase(this Dispatcher d, LayoutSelection? selection, LayoutPhase phase) {
            ds_ChangePhase ??= d[nameof(ChangePhase)];
            ds_ChangePhase.CallVoid(selection, phase);
        }

        static DispatchSource? ds_EditLocomotiveCollectionItem = null;
        [DispatchSource]
        public static void EditLocomotiveCollectionItem(this Dispatcher d, XmlElement element) {
            ds_EditLocomotiveCollectionItem ??= d[nameof(EditLocomotiveCollectionItem)];
            ds_EditLocomotiveCollectionItem.CallVoid(element);
        }

        static DispatchSource? ds_DeleteLocomotiveCollectionItem = null;
        [DispatchSource]
        public static void DeleteLocomotiveCollectionItem(this Dispatcher d, XmlElement element) {
            ds_DeleteLocomotiveCollectionItem ??= d[nameof(DeleteLocomotiveCollectionItem)];
            ds_DeleteLocomotiveCollectionItem.CallVoid(element);
        }

    }
}