﻿#pragma checksum "..\..\..\MetadataWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "4762EE0B3281A07A48E0CB35CC08905DC43DB92FED5CBEC1D390DC7517111F1E"
//------------------------------------------------------------------------------
// <auto-generated>
//     Tento kód byl generován nástrojem.
//     Verze modulu runtime:4.0.30319.42000
//
//     Změny tohoto souboru mohou způsobit nesprávné chování a budou ztraceny,
//     dojde-li k novému generování kódu.
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
    /// MetadataWindow
    /// </summary>
    public partial class MetadataWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 12 "..\..\..\MetadataWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label previousMetadataLabel;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\..\MetadataWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label nextMetadataLabel;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\..\MetadataWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label indexLabel;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\..\MetadataWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox metadataLabel;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\..\MetadataWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button confirmMetadataButton;
        
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
            System.Uri resourceLocater = new System.Uri("/CDArcha_klient;component/metadatawindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\MetadataWindow.xaml"
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
            this.previousMetadataLabel = ((System.Windows.Controls.Label)(target));
            
            #line 14 "..\..\..\MetadataWindow.xaml"
            this.previousMetadataLabel.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.PreviousMetadataLabel_Clicked);
            
            #line default
            #line hidden
            
            #line 15 "..\..\..\MetadataWindow.xaml"
            this.previousMetadataLabel.MouseEnter += new System.Windows.Input.MouseEventHandler(this.Icon_MouseEnter);
            
            #line default
            #line hidden
            
            #line 15 "..\..\..\MetadataWindow.xaml"
            this.previousMetadataLabel.MouseLeave += new System.Windows.Input.MouseEventHandler(this.Icon_MouseLeave);
            
            #line default
            #line hidden
            return;
            case 2:
            this.nextMetadataLabel = ((System.Windows.Controls.Label)(target));
            
            #line 18 "..\..\..\MetadataWindow.xaml"
            this.nextMetadataLabel.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.NextMetadataLabel_Clicked);
            
            #line default
            #line hidden
            
            #line 19 "..\..\..\MetadataWindow.xaml"
            this.nextMetadataLabel.MouseEnter += new System.Windows.Input.MouseEventHandler(this.Icon_MouseEnter);
            
            #line default
            #line hidden
            
            #line 19 "..\..\..\MetadataWindow.xaml"
            this.nextMetadataLabel.MouseLeave += new System.Windows.Input.MouseEventHandler(this.Icon_MouseLeave);
            
            #line default
            #line hidden
            return;
            case 3:
            this.indexLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 4:
            this.metadataLabel = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.confirmMetadataButton = ((System.Windows.Controls.Button)(target));
            
            #line 27 "..\..\..\MetadataWindow.xaml"
            this.confirmMetadataButton.Click += new System.Windows.RoutedEventHandler(this.confirmMetadataButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

