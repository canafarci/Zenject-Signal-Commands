#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.UnityLinker;
using UnityEngine;


namespace Editor
{
    [InitializeOnLoad]
    public class PackageImporter
    {
        static PackageImporter()
        {
            AssetDatabase.importPackageCompleted += OnImportPackageCompleted;
        }

        static void OnImportPackageCompleted(string packageName)
        {
            if (packageName == "com.canafarci.zenjectsignalcommands")
            {
                CreateXMLFile();
            }
        }

        static void CreateXMLFile()
        {
            string filePath = "Assets/Plugins/link.xml";
            if (!File.Exists(filePath))
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("<linker>");
                    writer.WriteLine("  <assembly fullname=\"ZenjectSignalCommands\" preserve=\"all\"/>");
                    writer.WriteLine("</linker>");
                }
                AssetDatabase.Refresh();
            }
        }
    }
}
#endif