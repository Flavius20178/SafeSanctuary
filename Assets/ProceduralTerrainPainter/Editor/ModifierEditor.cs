// Procedural Terrain Painter by Staggart Creations http://staggart.xyz
// Copyright protected under Unity Asset Store EULA

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace sc.terrain.proceduralpainter
{
    public class ModifierEditor
    {
        public static Type[] ModifierTypes;
        public static string[] ModifierNames;

        public static GUIContent[] blendModesList;

        public static Type GetType(string name)
        {
            for (var i = 0; i < ModifierNames.Length; i++)
                if (ModifierNames[i] == name)
                    return ModifierTypes[i];

            return null;
        }

        public static void RefreshModifiers()
        {
            //No need to refresh, if scripts were recompiled, yes. In this case the static list will be null anyway
            if (ModifierTypes != null) return;

            var enums = Enum.GetNames(typeof(Modifier.BlendMode));
            blendModesList = new GUIContent[enums.Length];

            for (var i = 0; i < enums.Length; i++) blendModesList[i] = new GUIContent(enums[i]);

            if (ModifierTypes == null)
            {
                var exts = new List<Type>();
                var names = new List<string>();

                var allTypes = new List<Type>();
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                foreach (var assembly in assemblies)
                {
                    var types = assembly.GetTypes();
                    foreach (var type in types)
                    {
                        if (type.IsAbstract) continue;

                        if (type.IsSubclassOf(typeof(Modifier)))
                            allTypes.Add(type);
                    }
                }

                foreach (var t in allTypes)
                {
                    exts.Add(t);

                    //Insert blank space in between camel case strings
                    var name = Regex.Replace(Regex.Replace(t.Name, "([a-z])([A-Z])", "$1 $2", RegexOptions.Compiled),
                        "([A-Z])([A-Z][a-z])", "$1 $2", RegexOptions.Compiled);

                    names.Add(name);
                }

                ModifierTypes = exts.ToArray();
                ModifierNames = names.ToArray();
            }
        }
    }
}