using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Arixen.ScriptSmith;
using Object = UnityEngine.Object;

public class AddressablesManager : MonoGenericLazySingleton<AddressablesManager>
{
    private Dictionary<string, AsyncOperationHandle<object>> loadedAssets = new Dictionary<string, AsyncOperationHandle<object>>();

    protected override void Awake()
    {
        base.Awake();
        // Ensure Addressables are initialized
        Addressables.InitializeAsync().Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log("Addressables initialized successfully.");
            }
            else
            {
                Debug.LogError($"Addressables initialization failed. Reason: {handle.OperationException}");
            }
        };
    }

    public async Task<T> LoadAssetAsync<T>(string key)
    {
        var handle = Addressables.LoadAssetAsync<T>(key);
        return await handle.Task;
    }

    public async Task<T> LoadAssetAsync<T>(AssetReferenceT<T> assetReference) where T : Object
    {
        var handle = assetReference.LoadAssetAsync<T>();
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log($"Successfully loaded - {assetReference.SubObjectName} {assetReference.AssetGUID}");
            return handle.Result;
        }
        else
        {
            Debug.LogError($"Failed to load Addressable: {assetReference.AssetGUID} with status {handle.Status}");
            return null;
        }
    }

    
    public void LoadAsset<T>(AssetReferenceT<T> labelReference, Action<T> onLoaded) where T : Object
    {
        labelReference.LoadAssetAsync<T>().Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                T loadedAsset = handle.Result;
                onLoaded?.Invoke(loadedAsset);
                Debug.Log($"Successfully loaded - {labelReference.SubObjectName} {labelReference.AssetGUID}");
            }
            else
            {
                Debug.LogError($"Failed to load Addressable: {labelReference.AssetGUID} with status {handle.Status}");
            }
        };
    }

    public void LoadScriptable<T>(AssetLabelReference labelReference, Action<T> onLoaded) where T : ScriptableObject
    {
        AsyncOperationHandle<object> loadHandle = Addressables.LoadAssetAsync<object>(labelReference.labelString);

        loadedAssets.Add(labelReference.labelString, loadHandle);
        loadHandle.Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                T loadedScriptable = (T)handle.Result;
                onLoaded?.Invoke(loadedScriptable);
                Debug.Log($"Successfully loaded {labelReference.labelString}");
            }
            else
            {
                Debug.LogError($"Failed to load Addressable: {labelReference.labelString} with status {handle.Status}");
            }
        };
    }

    public void LoadPrefab(AssetLabelReference labelReference, Action<GameObject> onLoaded)
    {
        AsyncOperationHandle<object> loadHandle = Addressables.LoadAssetAsync<object>(labelReference.labelString);
        loadedAssets.Add(labelReference.labelString, loadHandle);

        loadHandle.Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject loadedPrefab = (GameObject)handle.Result;
                onLoaded?.Invoke(loadedPrefab);
                Debug.Log($"Successfully loaded {labelReference.labelString}");
            }
            else
            {
                Debug.LogError($"Failed to load Addressable: {labelReference.labelString} with status {handle.Status}");
            }
        };
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        // Release all loaded assets when the manager is destroyed
        foreach (var entry in loadedAssets)
        {
            Addressables.Release(entry.Value);
        }

        loadedAssets.Clear();
    }
}