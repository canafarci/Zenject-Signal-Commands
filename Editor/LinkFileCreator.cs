#if UNITY_EDITOR
using System.IO;
using UnityEditor;

namespace ZenjectSignalCommands.Editor
{
    public class LinkFileCreator
    {
        [InitializeOnLoadMethod]
        static void OnImportPackageCompleted()
        {
            CreateXMLFile();
        }

        static void CreateXMLFile()
        {
            string filePath = "Assets/Plugins/ZenjectSignalCommands/link.xml";
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