using BlinkSyncLib;
using GLib;
using HanSquirrel.ResourceManager;
using HSFrameWork.Common;
using HSFrameWork.Common.Editor;
using HSFrameWork.ConfigTable.Editor;
using HSFrameWork.ConfigTable.Editor.Impl;
using System.Text.RegularExpressions;
using HSFrameWork.Component;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    public class BlywdModEditor
    {
        [MenuItem("Tools/部弯MOD打包AB", false, 0)]
        public static void BuildMod()
        {
            TE.NoThreadExtention = true;
            MenuHelper.SafeWrapMenuAction("部弯资源MOD打包", _ => ModMenu.ModBuild(
#if UNITY_STANDALONE_WIN
                BuildTarget.StandaloneWindows64,
#elif UNITY_ANDROID
                BuildTarget.Android,
#elif UNITY_IPHONE
                BuildTarget.iOS,
#endif
                false));

            ModPathHelper.AllModNamesBuildTime.ForEach(x =>
            {
                var ma = new ModAgent(x, true);
                var saPath = ma.ABSDeployRoot.CreateDir();
                var saDst = ("../" + ModPathHelper.DeployABSFolderName).FullPathG();

                HSUtils.Log("[{0}] >> [{1}]", saPath, saDst);
                new Sync(saPath, saDst).Start(new InputParams
                {
                    DeleteFromDest = true,
                    DeleteExcludeFiles =
                    new[] { new Regex(@"^hsframework_cevalues*\.bytes$", RegexOptions.IgnoreCase) },
                });


                var va = "../Interface/_virtuals.txt".CreateDirForFile();
                if (ma.DeployVirtualAssetsPath.ExistsAsFile())
                {
                    HSUtils.Log("复制 _virtuals.txt");
                    ma.DeployVirtualAssetsPath.CopyFileTo(va);
                }
                else
                {
                    HSUtils.Log("删除 _virtuals.txt");
                    va.SafeDeleteFile();
                }
            });
        }
    }
}
