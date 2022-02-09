using System.Xml;
using Microsoft.Win32.SafeHandles;

using MethodDispatcher;
using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager {
    static public class ModelDispatchSources {
        [DispatchSource]
        public static System.IO.FileStream OpenSerialCommunicationDeviceRequest(this Dispatcher d, XmlElement setupElement) {
            return d[nameof(OpenSerialCommunicationDeviceRequest)].Call<System.IO.FileStream>(setupElement);
        }

        [DispatchSource]
        public static SafeFileHandle CreateNamedPipeRequest(this Dispatcher d, string pipeName, bool overlapppedIo) {
            return d[nameof(CreateNamedPipeRequest)].Call<SafeFileHandle>(pipeName, overlapppedIo);
        }

        [DispatchSource]
        public static System.IO.FileStream WaitNamedPipeClientToConnectRequest(this Dispatcher d, SafeFileHandle handle, bool overlappedIo) {
            return d[nameof(WaitNamedPipeClientToConnectRequest)].Call<System.IO.FileStream>(handle, overlappedIo);
        }

        [DispatchSource]
        public static void DisconnectNamedPipeRequest(this Dispatcher d, object handle) {
            d[nameof(DisconnectNamedPipeRequest)].CallVoid(handle);
        }

        [DispatchSource]
        public static LayoutComponentConnectionPoint? GetLocomotiveFront(this Dispatcher d, LayoutBlockDefinitionComponent blockDefinition, object trainNameObject) {
            return d[nameof(GetLocomotiveFront)].CallNullable<LayoutComponentConnectionPoint>(blockDefinition, trainNameObject);
        }

        [DispatchSource]
        public static System.IO.FileStream WaitNamedPipeRequest(this Dispatcher d, string pipeName, bool overlappedIo) {
            return d[nameof(WaitNamedPipeRequest)].Call<System.IO.FileStream>(pipeName, overlappedIo);
        }

        [DispatchSource]
        public static LayoutComponentConnectionPoint? GetWaypointFront(this Dispatcher d, LayoutStraightTrackComponent track) {
            return d[nameof(GetWaypointFront)].CallNullable<LayoutComponentConnectionPoint?>(track);
        }

        [DispatchSource]
        public static TrainFrontAndLength? GetTrainFrontAndLength(this Dispatcher d, LayoutBlockDefinitionComponent blockDefinition, XmlElement collectionElement) {
            return d[nameof(GetTrainFrontAndLength)].CallNullable<TrainFrontAndLength?>(blockDefinition, collectionElement);
        }

        [DispatchSource]
        public static ILayoutTopologyServices GetTopologyServices(this MethodDispatcher.Dispatcher d) {
            return d[nameof(GetTopologyServices)].Call<ILayoutTopologyServices>();
        }
    }
}