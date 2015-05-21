﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UITesting;
using TestAutomation.Shared;
namespace TestAutomation.CodedUI
{
    class CodedUIWebElement : CodedUIWebObject, ITestableWebElement
    {

        public CodedUIWebElement(UITestControl baseObject)
        {
            _baseObject = baseObject;
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void Type(string text)
        {
            Keyboard.SendKeys(_baseObject, text);
        }

        public void Type(ITestableWebElement element, string text)
        {
            element.Type(text);
        }

        public void Click()
        {
            Mouse.Click(_baseObject);
        }

        public void Click(ITestableWebElement element)
        {
            element.Click();
        }

        public bool WaitForAppear(string targetSubElement, int timeout)
        {
            throw new NotImplementedException();
        }

        public bool WaitForDisappear(string targetSubElement, int timeout)
        {
            throw new NotImplementedException();
        }

        public void WaitForAttributeState(string targetSubElement, string attributeName, Func<string, bool> condition, int timeout)
        {
            throw new NotImplementedException();
        }

        public bool IsDisplayed()
        {
            throw new NotImplementedException();
        }

        public string GetAttribute(string attributeName)
        {
            throw new NotImplementedException();
        }

        public string InnerHtml()
        {
            throw new NotImplementedException();
        }

        public ITestableWebElement Parent(int levels = 1)
        {
            throw new NotImplementedException();
        }
    }
}