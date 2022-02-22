using LayoutManager.CommonUI;
using LayoutManager.Model;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace LayoutManager.Tools {
    public class LayoutComponentMenuItem : LayoutMenuItem {
        public LayoutComponentMenuItem(ModelComponent component, string name, EventHandler onClick) : base(name, null, onClick) {
            this.Component = component;
        }

        public ModelComponent Component { get; }
    }

    /// <summary>
    /// Implement a properties menu item. Invoke the given dialog type
    /// </summary>
    public class MenuItemProperties : LayoutMenuItem {
        private readonly Type dialogType;
        private readonly ModelComponent component;

        public MenuItemProperties(ModelComponent component, Type dialogType) {
            this.component = component;
            this.dialogType = dialogType;

            string menuName = "&Properties...";

            if (LayoutController.IsOperationMode)
                menuName = Ensure.NotNull<string>(EventManager.Event(new LayoutEvent<ModelComponent, string, string>("get-component-operation-properties-menu-name", component, menuName)));
            else
                menuName = Ensure.NotNull<string>(EventManager.Event(new LayoutEvent<ModelComponent, string, string>("get-component-editing-properties-menu-name", component, menuName)));

            this.Text = menuName;
        }

        protected override void OnClick(EventArgs e) {
            var constructor = dialogType.GetConstructor(new Type[] { typeof(ModelComponent), typeof(PlacementInfo) });
            var dialogTypeFullName = Ensure.NotNull<string>(dialogType.FullName);
            ILayoutComponentPropertiesDialog? dialog;

            if (constructor != null) {
                dialog = (ILayoutComponentPropertiesDialog?)dialogType.Assembly.CreateInstance(
                    dialogTypeFullName, false, BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance, null,
                    new Object[] { component, new PlacementInfo(component) }, null, Array.Empty<object>());
            }
            else {
                constructor = dialogType.GetConstructor(new Type[] { typeof(ModelComponent) });

                if (constructor != null) {
                    dialog = (ILayoutComponentPropertiesDialog?)dialogType.Assembly.CreateInstance(
                        dialogTypeFullName, false, BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance, null,
                        new Object[] { component }, null, Array.Empty<object>());
                }
                else
                    throw new ArgumentException("Invalid dialog class constructor (should be cons(LayoutModel, ModelComponent, [XmlElement]))");
            }

            if (dialog != null && dialog.ShowDialog() == DialogResult.OK) {
                LayoutModifyComponentDocumentCommand modifyComponentDocumentCommand =
                    new(component, dialog.XmlInfo);

                LayoutController.Do(modifyComponentDocumentCommand);
            }
        }
    }
}
