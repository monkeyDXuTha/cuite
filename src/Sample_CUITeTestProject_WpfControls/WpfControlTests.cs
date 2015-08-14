﻿using System;
using System.Diagnostics;
using System.IO;
using CUITe.Controls.WpfControls;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sample_CUITeTestProject_WpfControls.ObjectLibrary;
using CUITControls = Microsoft.VisualStudio.TestTools.UITesting.WpfControls;

namespace Sample_CUITeTestProject_WpfControls
{
    /// <summary>
    /// Summary description for WpfControlTests
    /// </summary>
    [CodedUITest]
#if DEBUG
    [DeploymentItem(@"..\..\..\ControlTemplateExamples\bin\Debug")]
#else
    [DeploymentItem(@"..\..\..\ControlTemplateExamples\bin\Release")]
#endif
    public class WpfControlTests
    {
        private static readonly string TestProcess = "ControlTemplateExamples";
        private readonly WinWpfControls mainWindow = new WinWpfControls();
        private TestContext testContextInstance;
        private ApplicationUnderTest testApp;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            // Close the app after all tests are finished
            Playback.Initialize();
            try
            {
                Process[] processes = Process.GetProcessesByName(TestProcess);

                foreach (Process process in processes)
                {
                    ApplicationUnderTest app = ApplicationUnderTest.FromProcess(process);
                    app.Close();
                }
            }
            finally
            {
                Playback.Cleanup();
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            if (Process.GetProcessesByName(TestProcess).Length == 0)
            {
                testApp = ApplicationUnderTest.Launch(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TestProcess + ".exe"));
            }
        }

        [TestCleanup]
        public void TestCleanup()
        {
            if (testContextInstance.CurrentTestOutcome != UnitTestOutcome.Passed)
            {
                testApp.Close();
            }
        }

        [TestMethod]
        public void WpfButton_Click_Succeeds()
        {
            mainWindow.SetFocus();
            mainWindow.btnDefault.Click();
            Assert.AreEqual("Default", mainWindow.btnDefault.DisplayText);
        }

        [TestMethod]
        public void WpfCheckBox_Checked_Succeeds()
        {
            mainWindow.SetFocus();
            Assert.AreEqual(false, mainWindow.chkNormal.Checked);
            Assert.AreEqual(true, mainWindow.chkChecked.Checked);
            Assert.AreEqual(true, mainWindow.chkIndeterminate.Indeterminate);
        }

        [TestMethod]
        public void WpfTable_GetRowAndCellByAutomationId_CanGetCellValue()
        {
            WpfRow row0 = mainWindow.dg1.Get<WpfRow>("AutomationId=Row0");
            WpfCell row0cell0 = row0.Get<WpfCell>("AutomationId=cellRow0Col0");
            Assert.AreEqual("XML in Action", row0cell0.Value);
        }

        [TestMethod]
        public void WpfListView_GetRow_CanClickOnRow()
        {
            WpfTable listView = mainWindow.Get<WpfTable>("AutomationId=lv1");
            Assert.AreEqual(8, listView.RowCount);
            WpfControl<CUITControls.WpfControl> control = listView.Get<WpfControl<CUITControls.WpfControl>>("AutomationId=Item0");
            control.Click();
        }

        [TestMethod]
        public void WpfListView_GetRow_CanClickOnCell()
        {
            WpfTable listView = mainWindow.Get<WpfTable>("AutomationId=lv1");
            Assert.AreEqual(8, listView.RowCount);
            WpfControl<CUITControls.WpfControl> control = listView.Get<WpfControl<CUITControls.WpfControl>>("AutomationId=Item0");
            WpfCell cell = control.Get<WpfCell>("ColumnHeader=Content");
            cell.Click();
        }
    }
}