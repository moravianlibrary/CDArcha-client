﻿#pragma checksum "..\..\..\CreateNewUnitWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "CBA7A942B91C0F0F77E08A3B0FAB436E41E6157F"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace CDArcha_klient {
    
    
    /// <summary>
    /// CreateNewUnitWindow
    /// </summary>
    public partial class CreateNewUnitWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 6 "..\..\..\CreateNewUnitWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TabItem monographTabItem;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\..\CreateNewUnitWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox monographIdentifierComboBox;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\..\CreateNewUnitWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox monographIdentifierTextBox;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\..\CreateNewUnitWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button monographNewUnitButton;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\..\CreateNewUnitWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TabItem periodicalTabItem;
        
        #line default
        #line hidden
        
        
        #line 49 "..\..\..\CreateNewUnitWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox periodicalIdentifierComboBox;
        
        #line default
        #line hidden
        
        
        #line 60 "..\..\..\CreateNewUnitWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox periodicalIdentifierTextBox;
        
        #line default
        #line hidden
        
        
        #line 63 "..\..\..\CreateNewUnitWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button periodicalNewUnitButton;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/CDArcha_klient;component/createnewunitwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\CreateNewUnitWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 5 "..\..\..\CreateNewUnitWindow.xaml"
            ((System.Windows.Controls.TabControl)(target)).SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.TabControl_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 2:
            this.monographTabItem = ((System.Windows.Controls.TabItem)(target));
            return;
            case 3:
            this.monographIdentifierComboBox = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 4:
            this.monographIdentifierTextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 26 "..\..\..\CreateNewUnitWindow.xaml"
            this.monographIdentifierTextBox.KeyDown += new System.Windows.Input.KeyEventHandler(this.IdentifierTextBox_KeyDown);
            
            #line default
            #line hidden
            
            #line 26 "..\..\..\CreateNewUnitWindow.xaml"
            this.monographIdentifierTextBox.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.monographIdentifierTextBox_TextChanged);
            
            #line default
            #line hidden
            return;
            case 5:
            this.monographNewUnitButton = ((System.Windows.Controls.Button)(target));
            
            #line 28 "..\..\..\CreateNewUnitWindow.xaml"
            this.monographNewUnitButton.Click += new System.Windows.RoutedEventHandler(this.NewUnitButton_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.periodicalTabItem = ((System.Windows.Controls.TabItem)(target));
            return;
            case 7:
            this.periodicalIdentifierComboBox = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 8:
            this.periodicalIdentifierTextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 61 "..\..\..\CreateNewUnitWindow.xaml"
            this.periodicalIdentifierTextBox.KeyDown += new System.Windows.Input.KeyEventHandler(this.IdentifierTextBox_KeyDown);
            
            #line default
            #line hidden
            
            #line 61 "..\..\..\CreateNewUnitWindow.xaml"
            this.periodicalIdentifierTextBox.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.periodicalIdentifierTextBox_TextChanged);
            
            #line default
            #line hidden
            return;
            case 9:
            this.periodicalNewUnitButton = ((System.Windows.Controls.Button)(target));
            
            #line 63 "..\..\..\CreateNewUnitWindow.xaml"
            this.periodicalNewUnitButton.Click += new System.Windows.RoutedEventHandler(this.NewUnitButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

