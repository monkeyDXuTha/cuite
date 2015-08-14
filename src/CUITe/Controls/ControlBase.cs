﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using CUITe.Browsers;
using Microsoft.VisualStudio.TestTools.UITesting;
using CUITControls = Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;

namespace CUITe.Controls
{
    /// <summary>
    /// Base wrapper class for all CUITe controls
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ControlBase<T> : IControlBase
        where T : UITestControl
    {
        private readonly PropertyExpressionCollection searchProperties;

        protected ControlBase()
            : this(null)
        {
        }

        protected ControlBase(string searchProperties)
        {
            this.searchProperties = new PropertyExpressionCollection();
            
            SetSearchProperties(searchProperties);
        }

        #region IControlBase Members

        public virtual IControlBase Parent
        {
            get { return null; }
        }

        public virtual IControlBase PreviousSibling
        {
            get { return null; }
        }

        public virtual IControlBase NextSibling
        {
            get { return null; }
        }

        public virtual IControlBase FirstChild
        {
            get { return null; }
        }

        /// <summary>
        /// Wraps WaitForControlReady method and Enabled property for a UITestControl.
        /// </summary>
        public bool Enabled
        {
            get
            {
                SourceControl.WaitForControlReady();
                return SourceControl.Enabled;
            }
        }

        /// <summary>
        /// Wraps the Exists property for a UITestControl.
        /// </summary>
        public bool Exists
        {
            get
            {
                if (SourceControl == null)
                {
                    return false;
                }

                return SourceControl.Exists;
            }
        }

        /// <summary>
        /// Get the Coded UI base type that is being wrapped by CUITe
        /// </summary>
        /// <returns></returns>
        public Type SourceType
        {
            get { return typeof(T); }
        }

        /// <summary>
        /// Wraps the provided UITestControl in a CUITe object. 
        /// Fills the Coded UI control's search properties using values 
        /// set when the CUITe object was created.
        /// </summary>
        /// <param name="control"></param>
        public virtual void Wrap(object control)
        {
            SourceControl = control as T;
            SourceControl.SearchProperties.AddRange(searchProperties);
        }

        /// <summary>
        /// Wraps the provided UITestControl in a CUITe object.
        /// It does nothing with the control's search properties.
        /// </summary>
        /// <param name="control"></param>
        public void WrapReady(object control)
        {
            SourceControl = control as T;
        }

        /// <summary>
        /// Wraps the WaitForControlReady method for a UITestControl.
        /// </summary>
        public void WaitForControlReady()
        {
            SourceControl.WaitForControlReady();
        }

        /// <summary>
        /// Wraps WaitForControlReady and Click methods for a UITestControl.
        /// </summary>
        public void Click()
        {
            SourceControl.WaitForControlReady();
            Mouse.Click(SourceControl);
        }

        /// <summary>
        /// Wraps WaitForControlReady and DoubleClick methods for a UITestControl.
        /// </summary>
        public void DoubleClick()
        {
            SourceControl.WaitForControlReady();
            Mouse.DoubleClick(SourceControl);
        }

        /// <summary>
        /// Wraps WaitForControlReady and SetFocus methods for a UITestControl.
        /// </summary>
        public void SetFocus()
        {
            SourceControl.WaitForControlReady();
            SourceControl.SetFocus();
        }

        /// <summary>
        /// Wraps the adding of search properties for the UITestControl where
        /// the property expression is 'EqualTo'.
        /// </summary>
        /// <param name="sPropertyName"></param>
        /// <param name="sValue"></param>
        public void SetSearchProperty(string sPropertyName, string sValue)
        {
            SourceControl.SearchProperties.Add(sPropertyName, sValue, PropertyExpressionOperator.EqualTo);
        }

        /// <summary>
        /// Wraps the adding of search properties for the UITestControl where
        /// the property expression is 'Contains'.
        /// </summary>
        /// <param name="sPropertyName"></param>
        /// <param name="sValue"></param>
        public void SetSearchPropertyRegx(string sPropertyName, string sValue)
        {
            SourceControl.SearchProperties.Add(sPropertyName, sValue, PropertyExpressionOperator.Contains);
        }

        public virtual List<IControlBase> GetChildren()
        {
            return null;
        }

        #endregion
        
        public TControl Get<TControl>() where TControl : IControlBase
        {
            var control = Activator.CreateInstance<TControl>();
            var sourceControl = Activator.CreateInstance(control.SourceType, UnWrap());

            control.Wrap(sourceControl);
            return control;
        }

        /// <summary>
        /// Gets the CUITe UI control object from the descendants of this control using the search parameters are passed. 
        /// You don't have to create the object repository entry for this.
        /// </summary>
        /// <typeparam name="T">Pass the CUITe control you are looking for.</typeparam>
        /// <param name="searchParameters">In 'Key1=Value1;Key2=Value2' format. For example 'Id=firstname' 
        /// or use '~' for Contains such as 'Id~first'</param>
        /// <returns>CUITe control object</returns>
        public TControl Get<TControl>(string searchParameters) where TControl : IControlBase
        {
            TControl control = ControlBaseFactory.Create<TControl>(searchParameters);
            var sourceControl = Activator.CreateInstance(control.SourceType, UnWrap());

            control.Wrap(sourceControl);
            return control;
        }

        public void SetSearchProperties(string searchProperties)
        {
            // fill the UITestControl's search properties based on the search string provided

            // iterate through the class inheritance hierarchy to get a list of property names for the specific control
            // Note: Some properties may not be valid to use for search (ex. filter property names). MS does not provide and exact list
            List<FieldInfo> controlProperties = new List<FieldInfo>();

            Type nestedType = typeof(T);
            Type nestedPropertyNamesType = nestedType.GetNestedType("PropertyNames");

            while (nestedType != typeof(object))
            {
                if (nestedPropertyNamesType != null)
                {
                    controlProperties.AddRange(nestedPropertyNamesType.GetFields());
                }

                nestedType = nestedType.BaseType;
                nestedPropertyNamesType = nestedType.GetNestedType("PropertyNames");
            }

            if (searchProperties == null)
            {
                return;
            }

            // Split on groups of key/value pairs
            string[] saKeyValuePairs = searchProperties.Split(
                new[] { ';' },
                StringSplitOptions.RemoveEmptyEntries);

            foreach (string sKeyValue in saKeyValuePairs)
            {
                PropertyExpressionOperator compareOperator = PropertyExpressionOperator.EqualTo;

                // If split on '=' does not work, then try '~'
                // Split at the first instance of '='. Other instances are considered part of the value.
                string[] saKeyVal = sKeyValue.Split(
                    new[] { '=' },
                    2);
                if (saKeyVal.Length != 2)
                {
                    // Otherwise try to split on '~'. If it works then compare type is Contains
                    // Split at the first instance of '~'. Other instances are considered part of the value.
                    saKeyVal = sKeyValue.Split(
                        new[] { '~' },
                        2);
                    if (saKeyVal.Length == 2)
                    {
                        compareOperator = PropertyExpressionOperator.Contains;
                    }
                    else
                    {
                        throw new InvalidSearchParameterFormatException(searchProperties);
                    }
                }

                // Find the first property in the list of known values
                string valueName = saKeyVal[0];

                if ((typeof(T).IsSubclassOf(typeof(CUITControls.HtmlControl))) && (valueName.Equals("Value", StringComparison.OrdinalIgnoreCase)))
                {
                    //support for backward compatibility where search properties like "Value=Log In" are used
                    valueName += "Attribute";
                }

                FieldInfo foundField = controlProperties.Find(
                    searchProperty => searchProperty.Name.Equals(valueName, StringComparison.OrdinalIgnoreCase));

                if (foundField == null)
                {
                    throw new InvalidSearchKeyException(valueName, searchProperties, controlProperties.Select(x => x.Name).ToList());
                }

                // Add the search property, value and type
                this.searchProperties.Add(foundField.GetValue(null).ToString(), saKeyVal[1], compareOperator);
            }
        }

        /// <summary>
        /// UnWraps the CUITe controls to expose the underlying UITestControl.
        /// This helps when you want to use any methods/properties of the underlying UITestControl.
        /// CUITe controls are wrappers/abstractions which hides complexity. UnWrap() helps you break the abstraction.
        /// </summary>
        /// <returns>The underlying UITestControl instance. For example, returns HtmlEdit in case of HtmlEdit.</returns>
        public T UnWrap()
        {
            return SourceControl;
        }
        
        /// <summary>
        /// Clicks on the center of the UITestControl based on its point on the screen.
        /// This may "work-around" Coded UI tests (on third-party controls) that throw the following exception:
        /// Microsoft.VisualStudio.TestTools.UITest.Extension.FailedToPerformActionOnBlockedControlException: Another control is blocking the control. Please make the blocked control visible and retry the action.
        /// </summary>
        public void PointAndClick()
        {
            SourceControl.WaitForControlReady();
            int x = SourceControl.BoundingRectangle.X + SourceControl.BoundingRectangle.Width / 2;
            int y = SourceControl.BoundingRectangle.Y + SourceControl.BoundingRectangle.Height / 2;
            Mouse.Click(new Point(x, y));
        }
        
        protected T SourceControl { get; set; }

        protected PropertyExpressionCollection SearchProperties
        {
            get { return searchProperties; }
        }

        /// <summary>
        /// Run/evaluate JavaScript code in the DOM context.
        /// </summary>
        /// <param name="code">The JavaScript code.</param>
        protected void RunScript(string code)
        {
            BrowserWindow browserWindow = (BrowserWindow)SourceControl.TopParent;
            InternetExplorer.RunScript(browserWindow, code);
        }
    }
}