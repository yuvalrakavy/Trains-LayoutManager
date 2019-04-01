using System;
using System.Windows.Forms;
using System.Reflection;

using LayoutManager.Model;

namespace LayoutManager.Tools {
    public class LayoutComponentMenuItem : MenuItem {
        readonly ModelComponent component;

        public LayoutComponentMenuItem(ModelComponent component, string name, EventHandler onClick) : base(name, onClick) {
            this.component = component;
        }

        public ModelComponent Component => component;
    }

    /// <summary>
    /// Implement a properties menu item. Invoke the given dialog type
    /// </summary>
    public class MenuItemProperties : MenuItem {
        readonly Type dialogType;
        readonly ModelComponent component;

        public MenuItemProperties(ModelComponent component, Type dialogType) {
            this.component = component;
            this.dialogType = dialogType;

            string menuName = "&Properties...";

            if (LayoutController.IsOperationMode)
                menuName = (string)EventManager.Event(new LayoutEvent<ModelComponent, string, string>("get-component-operation-properties-menu-name", component, menuName));
            else
                menuName = (string)EventManager.Event(new LayoutEvent<ModelComponent, string, string>("get-component-editing-properties-menu-name", component, menuName));

            this.Text = menuName;
        }

        protected override void OnClick(EventArgs e) {
            ConstructorInfo constructor = dialogType.GetConstructor(new Type[] { typeof(ModelComponent), typeof(PlacementInfo) });
            ILayoutComponentPropertiesDialog dialog;

            if (constructor != null) {
                dialog = (ILayoutComponentPropertiesDialog)dialogType.Assembly.CreateInstance(
                    dialogType.FullName, false, BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance, null,
                    new Object[] { component, new PlacementInfo(component) }, null, new Object[] { });
            }
            else {
                constructor = dialogType.GetConstructor(new Type[] { typeof(ModelComponent) });

                if (constructor != null) {
                    dialog = (ILayoutComponentPropertiesDialog)dialogType.Assembly.CreateInstance(
                        dialogType.FullName, false, BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance, null,
                        new Object[] { component }, null, new Object[] { });
                }
                else
                    throw new ArgumentException("Invalid dialog class constructor (should be cons(LayoutModel, ModelComponent, [XmlElement]))");
            }

            if (dialog.ShowDialog() == DialogResult.OK) {
                LayoutModifyComponentDocumentCommand modifyComponentDocumentCommand =
                    new LayoutModifyComponentDocumentCommand(component, dialog.XmlInfo);

                LayoutController.Do(modifyComponentDocumentCommand);
            }
        }
    }
}
