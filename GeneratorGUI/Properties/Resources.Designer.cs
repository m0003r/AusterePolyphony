﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34011
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GeneratorGUI.Properties {
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("GeneratorGUI.Properties.Resources", typeof(Resources).Assembly);
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
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Не удалось завершить сочинение за указанное количество шагов!.
        /// </summary>
        internal static string CantFinish {
            get {
                return ResourceManager.GetString("CantFinish", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ошибка.
        /// </summary>
        internal static string Error {
            get {
                return ResourceManager.GetString("Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Total filtering time: {0}
        ///Total generation time: {1}.
        /// </summary>
        internal static string FilteringGenerationTime {
            get {
                return ResourceManager.GetString("FilteringGenerationTime", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to \version &quot;2.15.41&quot;
        ///
        ///\language &quot;deutsch&quot;
        ///
        ///\header {{
        ///  tagline = &quot;&quot;
        ///  composer = &quot;Seed: {0}&quot;
        ///}}
        ///
        ///\score {{
        ///  \new StaffGroup &lt;&lt;
        ///{1}
        ///  &gt;&gt;
        ///  
        ///  \layout {{ 
        ///    \context {{
        ///      \Voice
        ///      \remove &quot;Note_heads_engraver&quot;
        ///      \consists &quot;Completion_heads_engraver&quot;
        ///      \remove &quot;Rest_engraver&quot;
        ///      \consists &quot;Completion_rest_engraver&quot;
        ///    }}
        ///  }}
        ///  
        ///  \midi {{
        ///    \context {{
        ///      \Score
        ///      tempoWholesPerMinute = #(ly:make-moment 72 2)
        ///    }}
        ///  }}
        ///}}.
        /// </summary>
        internal static string ScoreTemplate {
            get {
                return ResourceManager.GetString("ScoreTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Total steps: {0}
        ///.
        /// </summary>
        internal static string TotalSteps {
            get {
                return ResourceManager.GetString("TotalSteps", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to \new Staff
        ///    {{
        ///      \set Staff.midiInstrument = #&quot;synth voice&quot;
        ///      \key {0} {1}
        ///      \clef {2}
        ///      \time {3}
        ///      
        ///      {4}
        ///    }}.
        /// </summary>
        internal static string VoiceTemplate {
            get {
                return ResourceManager.GetString("VoiceTemplate", resourceCulture);
            }
        }
    }
}
