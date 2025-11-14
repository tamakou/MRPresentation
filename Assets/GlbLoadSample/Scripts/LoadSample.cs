using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Thoracentes
{

    public class LoadSample : MonoBehaviour
    {
        private LoadModel _loadModel;
        private InitializeGlbModel _initializeModel;

        private readonly Dictionary<Type, string> _errorMessageDirectory = new()
        {
            { typeof(UnityWebRequestException), "ダウンロード中にエラーが発生しました" },
            { typeof(LoadException), "モデルの読み込み中にエラーが発生しました" },
            { typeof(InstantiateException), "モデルの生成中にエラーが発生しました" },
        };

        private async void Awake()
        {
            _loadModel = new LoadModel();
            _initializeModel = new InitializeGlbModel();

            // モデルの親配下のオブジェクトを削除しておく。
            ModelParent.Instance.DeleteModel();

            var getModelData = new GetModelDataFromPersistentDataPath();
            var data = await getModelData.GetModelDataAsync(this.GetCancellationTokenOnDestroy());
            StartProcess(data);
        }


        /// <summary>
        /// モデル生成処理を開始する。
        /// </summary>
        /// <param name="data">モデルデータ</param>
        private void StartProcess(ModelData data)
        {
            UniTask.Action(async () =>
            {
                var ct = this.GetCancellationTokenOnDestroy();

                if (data != null)
                {
                    try
                    {
                        await ProcessModelDataAsync(data, ModelParent.Instance, ct);
                    }
                    catch (OperationCanceledException)
                    {
                        // キャンセル時は何もしない
                    }
                    catch (UnityWebRequestException ex)
                    {
                        Exception(typeof(UnityWebRequestException), ex);
                    }
                    catch (LoadException ex)
                    {
                        Exception(typeof(LoadException), ex);
                    }
                    catch (InstantiateException ex)
                    {
                        Exception(typeof(InstantiateException), ex);
                    }
                }
            })();
        }

        /// <summary>
        /// モデル生成までの処理。
        /// </summary>
        /// <param name="data">モデルデータ</param>
        /// <param name="parent">生成先となるオブジェクトの親</param>
        /// <param name="ct">キャンセル用のトークン</param>
        private async UniTask ProcessModelDataAsync(
            ModelData data,
            ModelParent parent,
            CancellationToken ct)
        {
            var parentTransform = parent.GetModelParent();
            await InternalProcessModelDataAsync(data.FilePath, parentTransform, ct);

            // ロードした3Dモデルに対して初期化処理を行う。
            _initializeModel.Initialize(data);
        }

        /// <summary>
        /// Exception発生時の処理。
        /// </summary>
        /// <param name="exceptionType">発生したExceptionの種類</param>
        /// <param name="exception">発生したException</param>
        private void Exception(Type exceptionType, Exception exception)
        {
            // エラーメッセージを表示する。
            var message = GetMessageType(exceptionType);
            var internalMessage = $"{message}: {exception.InnerException?.Message ?? exception.Message}";
            Debug.LogError(internalMessage);

            // モデルを削除して選択画面に戻る。
            ModelParent.Instance.DeleteModel();
        }

        /// <summary>
        /// モデルのDL、ロード、生成処理。
        /// </summary>
        /// <param name="path">取得するモデルの配置先パス</param>
        /// <param name="parent">生成先となるオブジェクトの親Transform</param>
        /// <param name="ct">キャンセル用のトークン</param>
        private async UniTask InternalProcessModelDataAsync(
            string path,
            Transform parent,
            CancellationToken ct)
        {
            // URLの場合はDLしてローカルに保存。既にDL済みの場合はキャッシュを利用する。
            if (IsURL(path))
            {
                // ファイルが既に存在する場合はDL処理をスキップする。
                var fileName = Path.GetFileName(path);
                var localPath = Path.Combine(_loadModel.DownloadPath, fileName);
                if (!File.Exists(localPath))
                {
                    localPath = await _loadModel.DownloadAsync(path, fileName, ct);
                }

                await _loadModel.LoadAsync(localPath, ct);
                await _loadModel.InstantiateAsync(parent, ct);
            }
            else
            {
                // URLでない場合はDL処理を行わず、指定のパスのローカルファイルをロードする。
                await _loadModel.LoadAsync(path, ct);
                await _loadModel.InstantiateAsync(parent, ct);
            }
        }

        /// <summary>
        /// 辞書からメッセージを取得する。
        /// </summary>
        /// <param name="exceptionType">Exceptionの種類</param>
        /// <returns>メッセージ</returns>
        private string GetMessageType(Type exceptionType)
        {
            return _errorMessageDirectory[exceptionType];
        }

        /// <summary>
        /// URLかどうか判定する。
        /// </summary>
        /// <param name="path">パス</param>
        /// <returns>URLであればTrue</returns>
        private bool IsURL(string path)
        {
            if (!Uri.IsWellFormedUriString(path, UriKind.Absolute)) return false;
            var uri = new Uri(path);
            return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
        }
    }
}