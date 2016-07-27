//
// Authors:	 
//	  Jonathan Pobst (monkey@jpobst.com>)
// 	  Ivan N. Zlatev (contact@i-nZ.net)
//
// (C) 2007 Jonathan Pobst
// (C) 2008 Ivan N. Zlatev

//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Design;

namespace mwf_designer 
{
	public class ToolBoxList : Panel, IToolboxService 
	{

#region Fields
		private Dictionary<string, ToolBoxGroupPanel> group_panels = new Dictionary<string,ToolBoxGroupPanel> ();
#endregion

#region Public Constructors
		public ToolBoxList ()
		{
			AutoScroll = true;
		}
#endregion

#region Public Methods
		public void Clear ()
		{
			group_panels.Clear ();
			Controls.Clear ();
		}

		protected override void OnLayout (LayoutEventArgs args)
		{
			base.OnLayout (args);
			int top = AutoScrollPosition.Y;
			foreach (Control c in this.Controls) {
				c.Top = top;
				top += c.Height;
				c.Width = this.Width;
			}
		}
#endregion

#region Protected Methods
		protected override void OnControlAdded (ControlEventArgs e)
		{
			base.OnControlAdded (e);
			
			if (e.Control is ToolBoxGroupPanel) {
				ToolBoxGroupPanel gp = (ToolBoxGroupPanel)e.Control;
				gp.Visible = true;
				
				group_panels.Add (gp.Text, gp);
			}
			base.PerformLayout ();
		}

		protected override void OnControlRemoved (ControlEventArgs e)
		{
			base.OnControlRemoved (e);

			if (e.Control is ToolBoxGroupPanel) {
				ToolBoxGroupPanel gp = (ToolBoxGroupPanel)e.Control;

				group_panels.Remove (gp.Text);
			}
			base.PerformLayout ();
		}

		protected virtual void OnToolPicked (EventArgs e)
		{
			EventHandler eh = (EventHandler)(Events[ToolPickedEvent]);
			if (eh != null)
				eh (this, e);
		}
#endregion

#region Private Methods
		private void ClearSelected (ToolBoxListItem item)
		{
			foreach (ToolBoxGroupPanel p in group_panels.Values)
				foreach (ToolBoxListItem i in p.Items)
					if (i != item)
						i.Selected = false;
		}

		private void item_Click (object sender, EventArgs e)
		{
			ClearSelected ((ToolBoxListItem)sender);

			OnToolPicked (e);
		}
#endregion

#region Public Events
		static object ToolPickedEvent = new object ();

		public event EventHandler ToolPicked {
			add { Events.AddHandler (ToolPickedEvent, value);}
			remove { Events.RemoveHandler (ToolPickedEvent, value);}
		}
#endregion

#region IToolboxService implementation
		public CategoryNameCollection CategoryNames {
			get {
				List<string> categories = new List<string> ();

				foreach (string s in group_panels.Keys)
					categories.Add (s);

				CategoryNameCollection cnc = new CategoryNameCollection (categories.ToArray ());

				return cnc;
			}
		}

		public string SelectedCategory
		{
			get { return null;}
			set {}
		}

		public void AddCreator (System.Drawing.Design.ToolboxItemCreatorCallback creator, string format, System.ComponentModel.Design.IDesignerHost host)
		{
		}

		public void AddCreator (System.Drawing.Design.ToolboxItemCreatorCallback creator, string format)
		{
		}

		public void AddLinkedToolboxItem (System.Drawing.Design.ToolboxItem toolboxItem, string category, System.ComponentModel.Design.IDesignerHost host)
		{
		}

		public void AddLinkedToolboxItem (System.Drawing.Design.ToolboxItem toolboxItem, System.ComponentModel.Design.IDesignerHost host)
		{
		}

		public void AddToolboxItem (System.Drawing.Design.ToolboxItem toolboxItem, string category)
		{
			ToolBoxListItem item = new ToolBoxListItem (toolboxItem);

			if (!group_panels.ContainsKey (category)) {
				ToolBoxGroupPanel gp = new ToolBoxGroupPanel ();
				gp.Text = category;
				Controls.Add (gp);
			}

			item.Click += new EventHandler (item_Click);
			group_panels[category].Items.Add (item);
		}

		public void AddToolboxItem (System.Drawing.Design.ToolboxItem toolboxItem)
		{
			AddToolboxItem (toolboxItem, "All");
		}

		public System.Drawing.Design.ToolboxItem DeserializeToolboxItem (object serializedObject, System.ComponentModel.Design.IDesignerHost host)
		{
			return null;
		}

		public System.Drawing.Design.ToolboxItem DeserializeToolboxItem (object serializedObject)
		{
			return null;
		}

		public System.Drawing.Design.ToolboxItem GetSelectedToolboxItem (System.ComponentModel.Design.IDesignerHost host)
		{
			return this.GetSelectedToolboxItem ();
		}

		public System.Drawing.Design.ToolboxItem GetSelectedToolboxItem ()
		{
			foreach (ToolBoxGroupPanel p in group_panels.Values)
				foreach (ToolBoxListItem i in p.Items)
					if (i.Selected)
						return i.ToolBoxItem;

			return null;
		}

		public System.Drawing.Design.ToolboxItemCollection GetToolboxItems (string category, System.ComponentModel.Design.IDesignerHost host)
		{
			return this.GetToolboxItems ();
		}

		public System.Drawing.Design.ToolboxItemCollection GetToolboxItems (string category)
		{
			List<ToolboxItem> items = new List<ToolboxItem> ();

			ToolBoxGroupPanel p = group_panels[category];

			foreach (ToolBoxListItem i in p.Items)
				items.Add (i.ToolBoxItem);

			return new ToolboxItemCollection (items.ToArray ());
		}

		public System.Drawing.Design.ToolboxItemCollection GetToolboxItems (System.ComponentModel.Design.IDesignerHost host)
		{
			return this.GetToolboxItems ();
		}

		public System.Drawing.Design.ToolboxItemCollection GetToolboxItems ()
		{
			List<ToolboxItem> items = new List<ToolboxItem> ();

			foreach (ToolBoxGroupPanel p in group_panels.Values)
				foreach (ToolBoxListItem i in p.Items)
					items.Add (i.ToolBoxItem);

			return new ToolboxItemCollection (items.ToArray ());
		}

		public bool IsSupported (object serializedObject, System.Collections.ICollection filterAttributes)
		{
			return false;
		}

		public bool IsSupported (object serializedObject, System.ComponentModel.Design.IDesignerHost host)
		{
			return false;
		}

		public bool IsToolboxItem (object serializedObject, System.ComponentModel.Design.IDesignerHost host)
		{
			return false;
		}

		public bool IsToolboxItem (object serializedObject)
		{
			return false;
		}

		public void RemoveCreator (string format, System.ComponentModel.Design.IDesignerHost host)
		{
		}

		public void RemoveCreator (string format)
		{
		}

		public void RemoveToolboxItem (System.Drawing.Design.ToolboxItem item, string category)
		{
			ToolBoxGroupPanel group = null;
			if (category != null && group_panels.TryGetValue (category, out group)) {
				int toRemove = -1;
				for (int i=0; i < group.Items.Count; i++) {
					ToolBoxListItem listItem = (ToolBoxListItem) group.Items[i];
					if (listItem.ToolBoxItem == item) {
						listItem.Click -= new EventHandler (item_Click);
						toRemove = i;
						break;
					}
				}
				if (toRemove != -1)
					group.Items.RemoveAt (toRemove);
				if (group.Items.Count == 0)
					Controls.Remove (group);
			}
		}

		public void RemoveToolboxItem (System.Drawing.Design.ToolboxItem item)
		{
			this.RemoveToolboxItem (item, "All");
		}

		public void SelectedToolboxItemUsed ()
		{
			ClearSelected (null);
		}

		public object SerializeToolboxItem (System.Drawing.Design.ToolboxItem toolboxItem)
		{
			return null;
		}

		public bool SetCursor ()
		{
			return false;
		}

		public void SetSelectedToolboxItem (System.Drawing.Design.ToolboxItem toolboxItem)
		{
			foreach (ToolBoxGroupPanel p in group_panels.Values)
				foreach (ToolBoxListItem i in p.Items)
					if (i.ToolBoxItem == toolboxItem) {
						i.Selected = true;
						break;
					}
		}
#endregion
	}
}
