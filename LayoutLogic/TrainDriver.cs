using System;
using System.Xml;
using LayoutManager.Model;

#pragma warning disable IDE0051, IDE0060
#nullable enable
namespace LayoutManager.Logic {
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

        [LayoutEvent("enum-train-drivers")]
        private void enumTrainDrivers(LayoutEvent e) {
            XmlElement driversElement = Ensure.NotNull<XmlElement>(e.Info, "driversElement");

            XmlElement manualWithControllerElement = driversElement.OwnerDocument.CreateElement(E_Driver);

            manualWithControllerElement.SetAttribute(A_TypeName, "Manual (via extrnal controller)");
            manualWithControllerElement.SetAttribute(A_ComputerDriven, false);
            manualWithControllerElement.SetAttribute(A_Type, "ManualController");

            driversElement.AppendChild(manualWithControllerElement);

            XmlElement manuaOnScreenlDriverElement = driversElement.OwnerDocument.CreateElement(E_Driver);

            manuaOnScreenlDriverElement.SetAttribute(A_TypeName, "Manual (via on-screen dialog)");
            manuaOnScreenlDriverElement.SetAttribute(A_ComputerDriven, true);
            manuaOnScreenlDriverElement.SetAttribute(A_Type, "ManualOnScreen");

            driversElement.AppendChild(manuaOnScreenlDriverElement);

            XmlElement autoDriverElement = driversElement.OwnerDocument.CreateElement(E_Driver);

            autoDriverElement.SetAttribute(A_TypeName, "Automatic (by the computer)");
            autoDriverElement.SetAttribute(A_ComputerDriven, true);
            autoDriverElement.SetAttribute(A_Type, "Automatic");

            driversElement.AppendChild(autoDriverElement);
        }

        #region Common Object (model)

        #endregion

        #endregion

        #region On screen manual driver

        [LayoutEvent("driver-assignment", IfSender = "*[Driver/@Type='ManualOnScreen']")]
        private void manualDriverAssignment(LayoutEvent e) {
            var train = Ensure.NotNull<TrainCommonInfo>(e.Sender, "train"); ;

            EventManager.Event(new LayoutEvent("show-locomotive-controller", train));
            e.Info = true;
        }

        #endregion

        #region Manual controller

        [LayoutEvent("driver-assignment", IfSender = "*[Driver/@Type='ManualController']")]
        private void manualControllerDriverAssignment(LayoutEvent e) {
            e.Info = true;
        }

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

                set => SetAttribute("AutoDriverState", value.ToString());
            }

            public LocomotiveOrientation Direction {
                get => AttributeValue("Direction").Enum<LocomotiveOrientation>() ?? LocomotiveOrientation.Forward;

                set => SetAttribute("Direction", value.ToString());
            }
        }

        #endregion

        [LayoutEvent("driver-assignment", IfSender = "*[Driver/@Type='Automatic']")]
        private void automaticDriverAssignment(LayoutEvent e) {
            e.Info = true;
        }

        [LayoutEvent("query-driver-setting-dialog", IfSender = "*[@Type='Automatic']")]
        private void automaticDriverQueryDriverSettingDialog(LayoutEvent e) {
            e.Info = true;
        }

        [LayoutEvent("edit-driver-setting", IfSender = "*[@Type='Automatic']")]
        private void automaticDriverEditDriverSetting(LayoutEvent e) {
            var train = Ensure.NotNull<TrainCommonInfo>(e.Info, "train");

            EventManager.Event(new LayoutEvent("get-train-target-speed", train));
        }

        #region Event handlers for events taken care by the automatic driver

        [LayoutEvent("driver-emergency-stop", IfSender = "*[Driver/@Type='Automatic']")]
        private void autoDriverEmergencyStop(LayoutEvent e) {
            var train = Ensure.NotNull<TrainStateInfo>(e.Sender, "train");
            var _ = new TrainAutoDriverInfo(train) {
                State = AutoDriverState.Stop
            };

            train.Speed = 0;        // Stop at once!
        }

        [LayoutEvent("driver-stop", IfSender = "*[Driver/@Type='Automatic']")]
        private void autoDriverStop(LayoutEvent e) {
            var train = Ensure.NotNull<TrainStateInfo>(e.Sender, "train");
            var _ = new TrainAutoDriverInfo(train) {
                State = AutoDriverState.Stop
            };

            train.ChangeSpeed(0, train.StopRamp);
        }

        [LayoutEvent("driver-train-go", IfSender = "*[Driver/@Type='Automatic']")]
        private void autoDriverGo(LayoutEvent e) {
            var train = Ensure.NotNull<TrainStateInfo>(e.Sender, "train");

            TrainAutoDriverInfo driver = new TrainAutoDriverInfo(train);
            LocomotiveOrientation direction = (LocomotiveOrientation)e.Info;

            driver.State = AutoDriverState.Go;
            driver.Direction = direction;

            int effectiveSpeed = CalculateEffectiveTargetSpeed(train);

            if (direction == LocomotiveOrientation.Backward)
                effectiveSpeed = -effectiveSpeed;

            if (effectiveSpeed < train.Speed)
                train.ChangeSpeed(effectiveSpeed, train.DecelerationRamp);
            else
                train.ChangeSpeed(effectiveSpeed, train.AccelerationRamp);
        }

        [LayoutEvent("driver-prepare-stop", IfSender = "*[Driver/@Type='Automatic']")]
        private void autoDriverPrepareStop(LayoutEvent e) {
            var train = Ensure.NotNull<TrainStateInfo>(e.Sender, "train");
            TrainAutoDriverInfo driver = new TrainAutoDriverInfo(train) {
                State = AutoDriverState.SlowDown
            };

            int effectiveSpeed = CalculateEffectiveTargetSpeed(train);

            if (driver.Direction == LocomotiveOrientation.Backward)
                effectiveSpeed = -effectiveSpeed;

            train.ChangeSpeed(effectiveSpeed, train.SlowdownRamp);
        }

        [LayoutEvent("driver-update-speed", IfSender = "*[Driver/@Type='Automatic']")]
        [LayoutEvent("driver-target-speed-changed", IfSender = "*[Driver/@Type='Automatic']")]
        private void autoDriverUpdateSpeed(LayoutEvent e) {
            var train = Ensure.NotNull<TrainStateInfo>(e.Sender, "train");
            TrainAutoDriverInfo driver = new TrainAutoDriverInfo(train);

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
            TrainAutoDriverInfo driver = new TrainAutoDriverInfo(train);

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

