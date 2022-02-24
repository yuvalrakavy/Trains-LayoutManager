using LayoutManager.Model;
using MethodDispatcher;
using System.Xml;

#nullable enable
namespace LayoutManager.Logic {

    static class DriverFilters {
        static private bool DriverTypeFilter(object? filterValue, object? targetObject, object? parameterValue) {
            if (filterValue is string driverTypeValue) {
                if (parameterValue is TrainStateInfo train) {
                    return filterValue switch {
                        "ManualOnScreen" or "ManualController" or "Automatic" => train.Driver.Type == driverTypeValue,
                        _ => throw new DispatchFilterException($"Invalid IsTrainDriver Value {filterValue ?? "(missing)"} - Can be ManualOnScreen, ManualController or Automatic")
                    };
                }
                else
                    throw new DispatchFilterException($"IsTrainDriver dispatch filter can be only applied on type {nameof(TrainStateInfo)}");
            }
            else
                throw new DispatchFilterException("DriverType value must be string");
        }

        static bool IsAutomaticTrainDriver(object? filterValue, object? targetObject, object? parameterValue) {
            if (filterValue != null)
                throw new DispatcherFilterHasValueException();
            if (parameterValue is TrainStateInfo train)
                return train.Driver.Type == "Automatic";
            else
                throw new DispatchFilterException($"IsAutomaticTrainDriver dispatch filter can be only applied on type {nameof(TrainStateInfo)}");

        }

        [DispatchTarget]
        static void AddDispatcherFilters() {
            Dispatch.AddCustomParameterFilter("IsTrainDriver", DriverTypeFilter);
            Dispatch.AddCustomParameterFilter("IsAutomaticTrainDriver", IsAutomaticTrainDriver);
        }

    }
    /// <summary>
    /// Summary description for TripPlanner.
    /// </summary>
    [LayoutModule("Standard Train Drivers")]
    internal class TrainDrivers : LayoutModuleBase {
        private const string A_TypeName = "TypeName";
        private const string A_ComputerDriven = "ComputerDriven";
        private const string A_Type = "Type";
        private const string E_Driver = "Driver";

        #region Code common to all driver types

        [DispatchTarget]
        private void EnumTrainDrivers(XmlElement driversElement) {
            XmlElement manualWithControllerElement = driversElement.OwnerDocument.CreateElement(E_Driver);

            manualWithControllerElement.SetAttribute(A_TypeName, "Manual (via external controller)");
            manualWithControllerElement.SetAttributeValue(A_ComputerDriven, false);
            manualWithControllerElement.SetAttribute(A_Type, "ManualController");

            driversElement.AppendChild(manualWithControllerElement);

            XmlElement manuaOnScreenlDriverElement = driversElement.OwnerDocument.CreateElement(E_Driver);

            manuaOnScreenlDriverElement.SetAttribute(A_TypeName, "Manual (via on-screen dialog)");
            manuaOnScreenlDriverElement.SetAttributeValue(A_ComputerDriven, true);
            manuaOnScreenlDriverElement.SetAttribute(A_Type, "ManualOnScreen");

            driversElement.AppendChild(manuaOnScreenlDriverElement);

            XmlElement autoDriverElement = driversElement.OwnerDocument.CreateElement(E_Driver);

            autoDriverElement.SetAttribute(A_TypeName, "Automatic (by the computer)");
            autoDriverElement.SetAttributeValue(A_ComputerDriven, true);
            autoDriverElement.SetAttribute(A_Type, "Automatic");

            driversElement.AppendChild(autoDriverElement);
        }

        #region Common Object (model)

        #endregion

        #endregion

        #region On screen manual driver

        [DispatchTarget]
        private bool DriverAssignment_ManualOnScreen([DispatchFilter(Type = "IsTrainDriver", Value = "ManualOnScreen")] TrainStateInfo train) {
            Dispatch.Call.ShowLocomotiveController(train);
            return true;
        }

        #endregion

        #region Manual controller

        [DispatchTarget]
        private bool DriverAssignment_ManualController([DispatchFilter(Type = "IsTrainDriver", Value = "ManualController")] TrainStateInfo train) => true;

        #endregion

        #region Automatic Driver

        #region Data structures

        private enum AutoDriverState {
            Stop,                   // Train is stopped
            Go,                     // Train is moving
            SlowDown                // Train is slowing down (prepare to stop)
        }

        private class TrainAutoDriverInfo : TrainDriverInfo {
            public TrainAutoDriverInfo(XmlElement element) : base(element) {
            }

            public TrainAutoDriverInfo(TrainCommonInfo train) : base(train) {
            }

            public AutoDriverState State {
                get => AttributeValue("AutoDriverState").Enum<AutoDriverState>() ?? AutoDriverState.Stop;

                set => SetAttributeValue("AutoDriverState", value.ToString());
            }

            public LocomotiveOrientation Direction {
                get => AttributeValue("Direction").Enum<LocomotiveOrientation>() ?? LocomotiveOrientation.Forward;

                set => SetAttributeValue("Direction", value.ToString());
            }
        }

        #endregion

        [DispatchTarget]
        private bool DriverAssignment_Automatic([DispatchFilter(Type = "IsAutomaticTrainDriver")] TrainStateInfo train) => true;


        [DispatchTarget]
        private bool QueryDriverDialog_AutomaticDriver([DispatchFilter(Type = "XPath", Value = "*[@Type='Automatic']")] XmlElement driverElement) => true;


        [DispatchTarget]
        private void EditDriverSettings_AutomaticDriver(TrainCommonInfo train, [DispatchFilter(Type = "XPath", Value = "*[@Type='Automatic']")] XmlElement driverElement) {
            Dispatch.Call.GetTrainTargetSpeed(train);
        }

        #region Event handlers for events taken care by the automatic driver

        [DispatchTarget]
        private void DriverEmergencyStop([DispatchFilter(Type = "IsAutomaticTrainDriver")] TrainStateInfo train) {
            var _ = new TrainAutoDriverInfo(train) {
                State = AutoDriverState.Stop
            };

            train.Speed = 0;        // Stop at once!
        }

        [DispatchTarget]
        private void DriverStop([DispatchFilter(Type = "IsAutomaticTrainDriver")] TrainStateInfo train) {
            var _ = new TrainAutoDriverInfo(train) {
                State = AutoDriverState.Stop
            };

            train.ChangeSpeed(0, train.StopRamp);
        }

        [DispatchTarget]
        private void DriverTrainGo([DispatchFilter(Type = "IsAutomaticTrainDriver")] TrainStateInfo train, LocomotiveOrientation direction) {
            int effectiveSpeed = CalculateEffectiveTargetSpeed(train);

            if (direction == LocomotiveOrientation.Backward)
                effectiveSpeed = -effectiveSpeed;

            if (effectiveSpeed < train.Speed)
                train.ChangeSpeed(effectiveSpeed, train.DecelerationRamp);
            else
                train.ChangeSpeed(effectiveSpeed, train.AccelerationRamp);
        }

        [DispatchTarget]
        private void DriverPrepareStop([DispatchFilter(Type = "IsAutomaticTrainDriver")] TrainStateInfo train) {
            TrainAutoDriverInfo driver = new(train) {
                State = AutoDriverState.SlowDown
            };

            int effectiveSpeed = CalculateEffectiveTargetSpeed(train);

            if (driver.Direction == LocomotiveOrientation.Backward)
                effectiveSpeed = -effectiveSpeed;

            train.ChangeSpeed(effectiveSpeed, train.SlowdownRamp);
        }

        [DispatchTarget]
        private void OnDriverTargetSpeedChanged([DispatchFilter(Type = "IsAutomaticTrainDriver")] TrainStateInfo train) {
            AutoDriverUpdateSpeed(train);
        }

        [DispatchTarget]
        private void DriverUpdateSpeed([DispatchFilter(Type = "IsAutomaticTrainDriver")] TrainStateInfo train) {
            AutoDriverUpdateSpeed(train);
        }

        private void AutoDriverUpdateSpeed(TrainStateInfo train) {
            TrainAutoDriverInfo driver = new(train);

            if (driver.State == AutoDriverState.Go) {
                int effectiveSpeed = CalculateEffectiveTargetSpeed(train);

                if (driver.Direction == LocomotiveOrientation.Backward)
                    effectiveSpeed = -effectiveSpeed;

                if (effectiveSpeed < train.Speed)
                    train.ChangeSpeed(effectiveSpeed, train.DecelerationRamp);
                else
                    train.ChangeSpeed(effectiveSpeed, train.AccelerationRamp);
            }
        }

        #endregion

        #region Utility methods

        private static int CalculateEffectiveTargetSpeed(TrainStateInfo train) {
            int effectiveSpeed;
            TrainAutoDriverInfo driver = new(train);

            if (driver.State == AutoDriverState.SlowDown)
                effectiveSpeed = train.CurrentSlowdownSpeed;
            else if (driver.State == AutoDriverState.Stop)
                effectiveSpeed = 0;
            else
                effectiveSpeed = train.TargetSpeed;

            if (effectiveSpeed > train.CurrentSpeedLimit)
                effectiveSpeed = train.CurrentSpeedLimit;

            return effectiveSpeed;
        }

        #endregion

        #endregion

    }
}

