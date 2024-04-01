#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.UnityLinker;
using UnityEngine;


namespace Editor
{
    public class LinkXmlInstaller : IUnityLinkerProcessor
    {
        
        int IOrderedCallback.callbackOrder => 0;
        string IUnityLinkerProcessor.GenerateAdditionalLinkXmlFile(BuildReport report, UnityLinkerBuildPipelineData data)
        {
            string metapath = "Packages/" + "com.canafarci.zenjectcommandsignals" + "/Editor/link.xml.meta";
            var assetPath = AssetDatabase.GetAssetPathFromTextMetaFilePath(metapath);
            
            return Path.GetFullPath(assetPath);
        }
    }
}
#endif