using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;

public class FiledownloadHelper : MonoBehaviour
{
    //单例对象
    public static FiledownloadHelper Single;
    public static FiledownloadHelper Inst
    {
        get
        {
            if (Single == null)
            {
                GameObject obj = new GameObject("FiledownloadHelper");
                DontDestroyOnLoad(obj);
                Single = obj.AddComponent<FiledownloadHelper>();
            }
            return Single;
        }
    }
    public void UpLoadFile(string url, byte[] bytes, string name, Action<bool, string> act) {
        StartCoroutine(UploadFile(url,bytes,name,act));
    }
    
    public void GetFile(string url, Action<byte[]> actionResult)
    {
        StartCoroutine(DownloadFile(url, actionResult));
    }
    /// <summary>
    /// 下载图片
    /// </summary>
    /// <param name="url"></param>
    /// <param name="progress"></param>
    /// <param name="action"></param>
    public void GetTexture(string url, Action<float> progress, Action<Texture2D,byte[], bool> action)
    {
        StartCoroutine(DownLoadTexture(url, progress, action));
    }
    /// <summary>
    /// 下载模型
    /// </summary>
    /// <param name="url"></param>
    /// <param name="progress"></param>
    /// <param name="action"></param>
    public void GetAssetBundle(string url, Action<float> progress, Action<GameObject, bool> action)
    {
        string assetName = url.Substring(url.LastIndexOf("/", StringComparison.Ordinal) + 1);
        if (File.Exists(ConstantValue.BundlePathLocal + assetName))
        {
            url = ConstantValue.BundlePathLocal + assetName;
        }       
        StartCoroutine(DownLoadAssetBundle(url, progress, action));
    }
   
    IEnumerator DownLoadTexture(string url, Action<float> progress, Action<Texture2D,byte[], bool> action)
    {
        UnityWebRequest request = new UnityWebRequest(url);
        //UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        DownloadHandlerTexture handlerTexture = new DownloadHandlerTexture(true);

        request.downloadHandler = handlerTexture;
        if (progress != null)
        {
            StartCoroutine(DownLoadProgress(request, progress));
        }
        yield return request.SendWebRequest();

        if (!string.IsNullOrEmpty(request.error))
        {
            Debug.Log("图片下载错误：" + request.error + ":" + url);
            action?.Invoke(null, null, false);
        }
        else
        {
            Texture2D texture = handlerTexture.texture;
            Debug.Log(request.downloadHandler.data.Length.ToString());
            action?.Invoke(texture, request.downloadHandler.data, true);
        }
    }
    /// <summary>
    /// 加载assetbundle
    /// </summary>
    /// <param name="url">资源url</param>
    /// <param name="progress">进度条 </param>
    /// <param name="action">模型回调 </param>
    /// <returns></returns>
    IEnumerator DownLoadAssetBundle(string url, Action<float> progress, Action<GameObject, bool> action)
    {
        string assetName = url.Substring(url.LastIndexOf("/", StringComparison.Ordinal) + 1);
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            if (!string.IsNullOrEmpty(request.error))
            {
                Debug.Log(request.error + ":" + url);
                action?.Invoke(null, false);
            }
            else
            {
                if (request.isDone)
                {
                    AssetBundleCreateRequest ab = AssetBundle.LoadFromMemoryAsync(request.downloadHandler.data);
                    yield return ab;
                    AssetBundle bundle = ab.assetBundle;
                    AssetBundleRequest assetRequest = bundle.LoadAllAssetsAsync(typeof(GameObject));
                    yield return assetRequest;
                    GameObject obj = assetRequest.asset as GameObject;
                    action?.Invoke(obj, true);
                    byte[] buff = request.downloadHandler.data;
                    if (!File.Exists(ConstantValue.BundlePathLocal + assetName)) {
                        SaveAssetLocalFile(ConstantValue.BundlePathLocal, assetName, buff, buff.Length);
                    }                   
                    request.Dispose();
                    bundle.Unload(false);
                    Resources.UnloadUnusedAssets();
                    GC.Collect();
                    Caching.ClearCache();
                }
            }
        }
    }
   
    IEnumerator DownloadFile(string url, Action<byte[]> actionResult)
    {        
        var uwr = UnityWebRequest.Get(url);
        yield return uwr.SendWebRequest();
        if (uwr.isDone)
        {
            byte[] data = uwr.downloadHandler.data;
            actionResult?.Invoke(data);
        }
    }
    IEnumerator UploadFile(string url,byte[] bytes,string name,Action<bool,string> act) {
        WWWForm form = new WWWForm();
        form.AddBinaryData("image",bytes,name,"image/jpg");       
        form.AddField("vertex", PlayerPrefs.GetString("vertex"));       
        form.AddField("gender", PlayerPrefs.GetString("gender"));
        using (UnityWebRequest www = UnityWebRequest.Post(url,form)) {
         //   www.timeout = 5;
            yield return www.SendWebRequest();
            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.Log(www.error);
                act?.Invoke(false, www.error);
            }
            else {
                act?.Invoke(true,www.downloadHandler.text);
            }
        }
    }

   
    //保存图片到本地
    public void SaveAssetLocalFile(string filepath, string filename, byte[] info, int length)
    {
        if (!Directory.Exists(filepath))
        Directory.CreateDirectory(filepath);        
        Stream sw = null;
        Debug.Log("已保存本地：" + filepath + filename);
        FileInfo fileInfo = new FileInfo(filepath + "/" + filename);
        if (fileInfo.Exists)
        {
            fileInfo.Delete();
        }
        //如果此文件不存在则创建  
        sw = fileInfo.Create();       
        sw.Write(info, 0, length);
        sw.Flush();
        sw.Close();
        sw.Dispose();
        Debug.Log(filename + "成功保存到本地");

    }

    //回调下载进度
    IEnumerator DownLoadProgress(UnityWebRequest request, Action<float> action)
    {
        while (!request.isDone)
        {
            yield return null;
            Debug.Log(request.downloadProgress.ToString());
            action(request.downloadProgress);
        }
        request.Abort();
        Resources.UnloadUnusedAssets();
    }
}
