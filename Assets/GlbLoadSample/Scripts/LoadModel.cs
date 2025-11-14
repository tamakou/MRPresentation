using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using GLTFast;
using UnityEngine;
using UnityEngine.Networking;

namespace Thoracentes
{
    public class LoadModel
    {
        private readonly GltfImport _gltfImport = new();
        private const string LogTemplate = "The GLB file {0} has {1}.";

        /// <summary>
        /// 3Dモデルのダウンロード先となるパス。
        /// </summary>
        public string DownloadPath => Application.temporaryCachePath;

        /// <summary>
        /// 3Dモデルのファイルをダウンロードし、ローカルに保存する。
        /// </summary>
        /// <param name="url">ファイルのDLリンク</param>
        /// <param name="fileName">保存する際のファイル名</param>
        /// <param name="ct">キャンセル用トークン</param>
        /// <returns>保存先のパス</returns>
        public async UniTask<string> DownloadAsync(string url, string fileName,CancellationToken ct)
        {
            Debug.Log("loading...");

            using UnityWebRequest request = UnityWebRequest.Get(url);
            await request.SendWebRequest().WithCancellation(ct);

            if (request.result != UnityWebRequest.Result.Success) return null;

            // ローカルにGLBを保存する。
            var fbxData = request.downloadHandler.data;
            var localPath = Path.Combine(DownloadPath, $"{fileName}");
            await File.WriteAllBytesAsync(localPath, fbxData, ct);
            Debug.Log("Saved glb at : " + localPath);
            return localPath;

        }

        /// <summary>
        /// ローカルに保存された3Dモデルのファイルを読み込む。
        /// </summary>
        /// <param name="path">保存先のパス</param>
        /// <param name="ct">キャンセル用トークン</param>
        /// <exception cref="LoadException">ロード中に発生した例外</exception>
        public async UniTask LoadAsync(string path, CancellationToken ct)
        {
            var data = await File.ReadAllBytesAsync(path, ct);

            try
            {
                var success = await _gltfImport.LoadGltfBinary(data, new Uri(path), cancellationToken: ct);
                if (success)
                {
                    Debug.Log($"Load glb from : {path}");
                    Debug.Log($"{CreateActionResultMessage("load", "success")}");
                }
                else
                {
                    throw new LoadException(CreateActionResultMessage("load", "failed"));
                }
            }
            catch (Exception e)
            {
                throw new LoadException(CreateActionResultMessage("load", "failed"), e);
            }
        }

        /// <summary>
        /// 読み込んだファイルをシーンに生成する。
        /// </summary>
        /// <param name="parent">親に設定したいGameObjectのTransform</param>
        /// <param name="ct">キャンセル用トークン</param>
        /// <exception cref="InstantiateException">生成時に発生した例外</exception>
        public async UniTask InstantiateAsync(Transform parent, CancellationToken ct)
        {
            try
            {
                var success = await _gltfImport.InstantiateMainSceneAsync(parent, ct);
                if (success)
                {
                    Debug.Log(CreateActionResultMessage("instantiate", "success"));
                }
                else
                {
                    throw new InstantiateException(CreateActionResultMessage("instantiate", "failed"));
                }
            }
            catch (Exception e)
            {
                throw new InstantiateException(CreateActionResultMessage("instantiate", "failed"), e);
            }
        }

        /// <summary>
        /// 処理内容と結果をメッセージとして整理した値を返す。
        /// </summary>
        /// <param name="action">処理内容</param>
        /// <param name="result">結果</param>
        /// <returns></returns>
        private string CreateActionResultMessage(string action, string result)
        {
            return string.Format(LogTemplate, action, result);
        }
    }
}