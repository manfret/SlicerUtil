﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Aura.Themes.Localization {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Errors_en_EN {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Errors_en_EN() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Aura.Themes.Localization.Errors-en-EN", typeof(Errors_en_EN).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to File reading error.
        /// </summary>
        public static string FileReading {
            get {
                return ResourceManager.GetString("FileReading", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Value must be in range {0} &lt;= value &lt;= {1}.
        /// </summary>
        public static string InTheRange {
            get {
                return ResourceManager.GetString("InTheRange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &quot;angle1°/angle2°&quot; format expected.
        /// </summary>
        public static string OnlyAngles {
            get {
                return ResourceManager.GetString("OnlyAngles", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Only doubles can be added.
        /// </summary>
        public static string OnlyDoubles {
            get {
                return ResourceManager.GetString("OnlyDoubles", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Only integers can be added.
        /// </summary>
        public static string OnlyIntegers {
            get {
                return ResourceManager.GetString("OnlyIntegers", resourceCulture);
            }
        }
    }
}