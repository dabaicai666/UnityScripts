
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using System.IO;

public class CreateAssetBunldesMain{
    [MenuItem("AssetBundle/Build_PC AssetBunldes")]
    [System.Obsolete]
    static void Build_PCAssetBunldes()
    {
        //获取在Project视图中选择的所有游戏对象
        Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        //遍历所有的游戏对象
        foreach (Object obj in SelectedAsset)
        {
            string sourcePath = AssetDatabase.GetAssetPath(obj);                    
            string targetPath = Application.dataPath + "/_AllBundle_PC/" + obj.name + ".assetbundle";
            if (BuildPipeline.BuildAssetBundle(obj, null, targetPath, BuildAssetBundleOptions.CollectDependencies, EditorUserBuildSettings.activeBuildTarget))
            {
                Debug.Log(obj.name + "资源打包成功");
            }
            else
            {
                Debug.Log(obj.name + "资源打包失败");
            }
        }
        //刷新编辑器
        AssetDatabase.Refresh ();	
	}
    [MenuItem("AssetBundle/Build_IOS AssetBunldes")]
    [System.Obsolete]
    static void Build_IOSAssetBunldes()
    {
        //获取在Project视图中选择的所有游戏对象
        Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        //遍历所有的游戏对象
        foreach (Object obj in SelectedAsset)
        {
            string sourcePath = AssetDatabase.GetAssetPath(obj);          
            string targetPath = Application.dataPath + "/_AllBundle_IOS/" + obj.name + ".assetbundle";          
            if (BuildPipeline.BuildAssetBundle(obj, null, targetPath, BuildAssetBundleOptions.CollectDependencies, BuildTarget.iOS))
            {
                Debug.Log(obj.name + "资源打包成功");
            }
            else
            {
                Debug.Log(obj.name + "资源打包失败");
            }
        }    
        AssetDatabase.Refresh();
    }
    [MenuItem("AssetBundle/BuildAssetBunldes")]
    static void BuildAssetBundles() {
        Object[] selectedAsset = Selection.GetFiltered(typeof(Object),SelectionMode.DeepAssets);       
        //备打文件集合，当前为单个
        AssetBundleBuild[] abb = new AssetBundleBuild[selectedAsset.Length];        
        for (int i = 0;i< selectedAsset.Length;i++) {
            string[] path = new string[1];
            path[0] = AssetDatabase.GetAssetPath(selectedAsset[i]);
            Debug.Log(AssetDatabase.GetAssetPath(selectedAsset[i]));
            abb[i].assetNames = path;
            abb[i].assetBundleName = selectedAsset[i].name;//设置bundle名字
            abb[i].assetBundleVariant = "assetbundle";   //设置bundle扩展名
            Debug.Log(i);
        }
        string targetPath;
#if UNITY_IOS
  targetPath = Application.dataPath + "/_AllBundle_iOS";
#elif UNITY_ANDROID
  targetPath = Application.dataPath + "/_AllBundle_Android";
#elif UNITY_EDITOR
         targetPath = Application.dataPath + "/_AllBundle_PC";
#endif
        if (!Directory.Exists(targetPath)) 
         Directory.CreateDirectory(targetPath);        
         BuildPipeline.BuildAssetBundles(targetPath, abb, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);       
         AssetDatabase.Refresh();
    }
  
}