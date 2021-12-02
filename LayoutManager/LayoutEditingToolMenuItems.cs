using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.Xml;
using LayoutManager.Model;
using LayoutManager.CommonUI;

namespace LayoutManager {
    #region Context Menu items

    internal class MenuItemDeleteAllComponents : LayoutMenuItem {
        private readonly LayoutHitTestResult hitTestResult;

        public MenuItemDeleteAllComponents(LayoutHitTestResult hitTestResult) {
            // TODO: Get strings for resource manager...
            Text = "&Delete";
            this.hitTestResult = hitTestResult;
        }

        public class ZorderSorter : IComparer<ModelComponent> {
            #region IComparer<ModelComponent> Members

            public int Compare(ModelComponent? x, ModelComponent? y) => y!.ZOrder - x!.ZOrder;

            public bool Equals(ModelComponent x, ModelComponent y) => x == y;

            public int GetHashCode(ModelComponent obj) => obj.GetHashCode();

            #endregion
        }

        protected override void OnClick(EventArgs e) {
            base.OnClick(e);

            // Get all components in this layer
            var components = hitTestResult.Selection.Components.ToList();

            components.Sort(new ZorderSorter());

            LayoutCompoundCommand deleteCommand = new("delete");

            for (int i = components.Count - 1; i >= 0; i--) {
                EventManager.Event(new LayoutEvent("prepare-for-component-remove-command", components[i], deleteCommand));
                deleteCommand.Add(new LayoutComponentRemovalCommand(components[i], "Delete"));
            }

            LayoutController.Do(deleteCommand);
        }
    }

    internal class MenuItemCopyComponent : LayoutMenuItem {
        private readonly LayoutHitTestResult hitTestResult;

        public MenuItemCopyComponent(LayoutHitTestResult hitTestResult) {
            // TODO: Get strings for resource manager...
            Text = "&Copy";
            this.hitTestResult = hitTestResult;
        }

        protected override void OnClick(EventArgs e) {
            base.OnClick(e);
            Clipboard.SetDataObject(hitTestResult.Selection.GetDataObject(hitTestResult.ModelLocation));
        }
    }

    internal class MenuItemCutComponent : LayoutMenuItem {
        private readonly LayoutHitTestResult hitTestResult;

        public MenuItemCutComponent(LayoutHitTestResult hitTestResult) {
            // TODO: Get strings for resource manager...
            Text = "Cu&t";
            this.hitTestResult = hitTestResult;
        }

        protected override void OnClick(EventArgs e) {
            base.OnClick(e);

            Clipboard.SetDataObject(hitTestResult.Selection.GetDataObject(hitTestResult.ModelLocation));

            LayoutCompoundCommand cutCommand = new("cut component");
            foreach (ModelComponent c in hitTestResult.Selection) {
                LayoutComponentRemovalCommand removeCommand = new(c, "remove");

                cutCommand.Add(removeCommand);
            }

            LayoutController.Do(cutCommand);
        }
    }

    internal class MenuItemPasteComponents : LayoutMenuItem {
        private readonly LayoutHitTestResult hitTestResult;
        private readonly string componentsFormat = "LayoutManagerComponents";

        public MenuItemPasteComponents(LayoutHitTestResult hitTestResult) {
            this.hitTestResult = hitTestResult;
            this.Text = "&Paste";

            IDataObject dataObj = Clipboard.GetDataObject();

            this.Enabled = false;
            if (dataObj != null) {
                if (dataObj.GetDataPresent(componentsFormat))
                    this.Enabled = true;
            }
        }

        private void clearLocation(LayoutCompoundCommand pasteCommand, LayoutModelArea area, Point ml) {
            var spot = area[ml, LayoutPhase.All];

            if (spot != null) {
                IList<ModelComponent> components = spot.Components;

                for (int i = components.Count - 1; i >= 0; i--)
                    pasteCommand.Add(new LayoutComponentRemovalCommand(components[i], "Delete"));
            }
        }

        protected override void OnClick(EventArgs e) {
            IDataObject dataObj = Clipboard.GetDataObject();

            if (dataObj != null && dataObj.GetDataPresent(componentsFormat)) {
                System.IO.MemoryStream ms = new((byte[])dataObj.GetData(componentsFormat));
                XmlTextReader r = new LayoutXmlTextReader(ms, LayoutReadXmlContext.PasteComponents);
                ConvertableString GetAttribute(string name) => new(r.GetAttribute(name), $"Attribute {name}");
                LayoutCompoundCommand pasteCommand = new("paste components");

                // If there was a selection, then the pasted stuff replaces this selection
                if (LayoutController.UserSelection.Count > 0) {
                    foreach (ModelComponent component in LayoutController.UserSelection) {
                        pasteCommand.Add(new LayoutComponentDeselectCommand(LayoutController.UserSelection, component, "unselect"));
                        EventManager.Event(new LayoutEvent("prepare-for-component-remove-command", component, pasteCommand));
                        pasteCommand.Add(new LayoutComponentRemovalCommand(component, "delete"));
                    }
                }

                // Skip all kind of declaration stuff
                while (r.Read() && r.NodeType == XmlNodeType.XmlDeclaration)
                    ;

                //				r.Read();		// <Components>
                r.Read();       // <Component>

                while (r.NodeType == XmlNodeType.Element) {
                    Point ml = new((int)GetAttribute("X") + hitTestResult.ModelLocation.X,
                        (int)GetAttribute("Y") + hitTestResult.ModelLocation.Y);

                    clearLocation(pasteCommand, hitTestResult.Area, ml);

                    // Create the component
                    string className = Ensure.NotNull<string>(r.GetAttribute("Class"));
                    ModelComponent? component = LayoutModel.CreateModelComponent(className);

                    if (component != null) {
                        component.ReadXml(r);

                        LayoutComponentPlacmentCommand placeComponentCommand = new(hitTestResult.Area, ml, component, "Add component", hitTestResult.Area.Phase(ml));

                        pasteCommand.Add(placeComponentCommand);
                        ((LayoutSelectionWithUndo)LayoutController.UserSelection).Add(component, pasteCommand);
                    }
                }

                LayoutController.Do(pasteCommand);
            }
        }
    }

    internal class MenuItemDeleteComponent : LayoutMenuItem {
        private readonly ModelComponent component;

        public MenuItemDeleteComponent(String text, ModelComponent component) {
            this.component = component;
            this.Text = text;
        }

        protected override void OnClick(EventArgs e) {
            base.OnClick(e);

            LayoutCompoundCommand deleteCommand = new("remove " + component.ToString());

            EventManager.Event(new LayoutEvent("prepare-for-component-remove-command", component, deleteCommand));
            deleteCommand.Add(new LayoutComponentRemovalCommand(component, "remove " + component.ToString()));
            LayoutController.Do(deleteCommand);
        }
    }

    #endregion
}
