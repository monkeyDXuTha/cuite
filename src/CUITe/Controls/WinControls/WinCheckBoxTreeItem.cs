﻿using Microsoft.VisualStudio.TestTools.UITesting;
using CUITControls = Microsoft.VisualStudio.TestTools.UITesting.WinControls;

namespace CUITe.Controls.WinControls
{
    /// <summary>
    /// Wrapper class for WinCheckBoxTreeItem
    /// </summary>
    public class WinCheckBoxTreeItem : WinControl<CUITControls.WinCheckBoxTreeItem>
    {
        public WinCheckBoxTreeItem()
        {
        }

        public WinCheckBoxTreeItem(string searchParameters)
            : base(searchParameters)
        {
        }

        public bool Checked
        {
            get { return SourceControl.Checked; }
            set { SourceControl.Checked = value; }
        }

        public bool HasChildNodes
        {
            get { return UnWrap().HasChildNodes; }
        }

        public bool Indeterminate
        {
            get { return SourceControl.Indeterminate; }
            set { SourceControl.Indeterminate = value; }
        }

        public UITestControlCollection Nodes
        {
            get { return UnWrap().Nodes; }
        }

        public UITestControl ParentNode
        {
            get { return UnWrap().ParentNode; }
        }

        public bool Selected
        {
            get { return SourceControl.Selected; }
            set { SourceControl.Selected = value; }
        }
    }
}