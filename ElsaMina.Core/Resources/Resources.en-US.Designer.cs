﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ElsaMina.Core.Resources {
    using System;
    
    
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
    internal class Resources_en_US {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources_en_US() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ElsaMina.Core.Resources.Resources.en-US", typeof(Resources_en_US).Assembly);
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
        ///   Looks up a localized string similar to The command already exist..
        /// </summary>
        internal static string addcommand_already_exist {
            get {
                return ResourceManager.GetString("addcommand_already_exist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The command&apos;s content cannot start with this character..
        /// </summary>
        internal static string addcommand_bad_first_char {
            get {
                return ResourceManager.GetString("addcommand_bad_first_char", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Command content is too long..
        /// </summary>
        internal static string addcommand_content_too_long {
            get {
                return ResourceManager.GetString("addcommand_content_too_long", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Command name is too long..
        /// </summary>
        internal static string addcommand_name_too_long {
            get {
                return ResourceManager.GetString("addcommand_name_too_long", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Successfully added new command : **{0}**..
        /// </summary>
        internal static string addcommand_success {
            get {
                return ResourceManager.GetString("addcommand_success", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An error occured while adding the badge : {0}.
        /// </summary>
        internal static string badge_add_failure_message {
            get {
                return ResourceManager.GetString("badge_add_failure_message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to New badge added successfully..
        /// </summary>
        internal static string badge_add_success_message {
            get {
                return ResourceManager.GetString("badge_add_success_message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Adds a new badge. Syntax : -add-badge &lt;name&gt;, &lt;image&gt;. Use -add-trophy to add a trophy..
        /// </summary>
        internal static string badge_help_message {
            get {
                return ResourceManager.GetString("badge_help_message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Shows suggested commands when someone tries to use a command that doesn&apos;t exist..
        /// </summary>
        internal static string dashboard_autocorrect_description {
            get {
                return ResourceManager.GetString("dashboard_autocorrect_description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Show command suggestions.
        /// </summary>
        internal static string dashboard_autocorrect_name {
            get {
                return ResourceManager.GetString("dashboard_autocorrect_name", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Shows the stacktrace of a command that crashed..
        /// </summary>
        internal static string dashboard_errors_description {
            get {
                return ResourceManager.GetString("dashboard_errors_description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Show commands errors.
        /// </summary>
        internal static string dashboard_errors_name {
            get {
                return ResourceManager.GetString("dashboard_errors_name", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The locale used by the bot in this room..
        /// </summary>
        internal static string dashboard_locale_description {
            get {
                return ResourceManager.GetString("dashboard_locale_description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Locale.
        /// </summary>
        internal static string dashboard_locale_name {
            get {
                return ResourceManager.GetString("dashboard_locale_name", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Room {0} not found..
        /// </summary>
        internal static string dashboard_room_doesnt_exist {
            get {
                return ResourceManager.GetString("dashboard_room_doesnt_exist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Elsa-Mina v7 : Bot in development.
        /// </summary>
        internal static string help {
            get {
                return ResourceManager.GetString("help", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An error occured while updating the configuration : {0}.
        /// </summary>
        internal static string room_config_failure {
            get {
                return ResourceManager.GetString("room_config_failure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not find locale &apos;{0}&apos;.
        /// </summary>
        internal static string room_config_locale_not_found {
            get {
                return ResourceManager.GetString("room_config_locale_not_found", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not find room &apos;{0}&apos;.
        /// </summary>
        internal static string room_config_room_not_found {
            get {
                return ResourceManager.GetString("room_config_room_not_found", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Configuration of room {0} has been updated..
        /// </summary>
        internal static string room_config_success {
            get {
                return ResourceManager.GetString("room_config_success", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}&apos;s dashboard.
        /// </summary>
        internal static string room_dashboard {
            get {
                return ResourceManager.GetString("room_dashboard", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Submit.
        /// </summary>
        internal static string submit {
            get {
                return ResourceManager.GetString("submit", resourceCulture);
            }
        }
    }
}