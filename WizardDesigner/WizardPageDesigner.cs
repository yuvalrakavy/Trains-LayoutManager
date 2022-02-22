using System;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;

namespace Gui.Wizard {
    /// <summary>
    /// Summary description for WizardPageDesigner.
    /// </summary>
    public class WizardPageDesigner : ParentControlDesigner {
        public override DesignerVerbCollection Verbs {
            get {
                var verbs = new DesignerVerbCollection {
                    new DesignerVerb("Remove Page", new EventHandler(HandleRemovePage))
                };

                return verbs;
            }
        }

        private void HandleRemovePage(object? sender, EventArgs e) {
            IDesignerHost h = (IDesignerHost)GetService(typeof(IDesignerHost));
            IComponentChangeService c = (IComponentChangeService)GetService(typeof(IComponentChangeService));

            DesignerTransaction dt = h.CreateTransaction("Remove Page");

            if (this.Control is WizardPage page) {
                if (page.Parent is Wizard wiz) {
                    c.OnComponentChanging(wiz, null);
                    //Drop from wizard
                    wiz.Pages.Remove(page);
                    wiz.Controls.Remove(page);
                    c.OnComponentChanged(wiz, null, null, null);
                    h.DestroyComponent(page);
                }
                else {
                    c.OnComponentChanging(page, null);
                    //Mark for destruction
                    page.Dispose();
                    c.OnComponentChanged(page, null, null, null);
                }
            }
            dt.Commit();
        }
    }
}
