﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using System.Runtime.InteropServices;
using SHDocVw;
using mshtml;
using System.Diagnostics;

namespace CUITe.Controls.HtmlControls
{
    public class CUITe_BrowserDialog : CUITe_BrowserWindow
    {
        public CUITe_BrowserDialog() 
        {
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "IEFrame";
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "Internet Explorer_TridentDlgFrame";
            this.WindowTitles.Add(this.sWindowTitle);
        }

        //public new void RunScript(string sCode)
        //{
        //    HtmlDocument document = new HtmlDocument(this);
        //    mshtml.IHTMLBodyElement idoc = (mshtml.IHTMLBodyElement)document.NativeElement;
        //    idoc.parentWindow.execScript(sCode);
        //}
    }
}