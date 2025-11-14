using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Thoracentes
{
    /// <summary>
    /// モデル情報をPersistentDataPathから取得する処理。
    /// </summary>
    public class GetModelDataFromPersistentDataPath
    {
        /// <summary>
        /// モデル群の情報を取得する。
        /// .../models/hoge/fuga.glbや.../models/hoge/model.jsonのような構造となる。
        /// </summary>
        /// <param name="ct">キャンセル用トークン</param>
        /// <returns>モデル群の情報</returns>
        public UniTask<ModelData[]> GetModelDataArrayAsync(CancellationToken ct)
        {
            // Application.persistentDataPath配下のフォルダ名をすべて取得
            var persistentPath = Path.Combine(Application.persistentDataPath, ConstantValues.ModelFolderName);
            var directories = Directory.GetDirectories(persistentPath);
            var modelDataList = new List<ModelData>();

            foreach (var directory in directories)
            {
                // フォルダの配下に存在するglbファイルのパスを取得
                var glbFilePath = Directory
                    .GetFiles(directory, "*.glb", SearchOption.TopDirectoryOnly)
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(glbFilePath))
                {
                    Debug.LogError($"glb file not found in {directory}");
                    continue;
                }

                var jsonFilePath = Path.Combine(directory, ConstantValues.ModelJsonFileName);
                if (!File.Exists(jsonFilePath))
                {
                    Debug.LogError($"Json file not found: {jsonFilePath}");
                    continue;
                }

                // Jsonを解釈してモデルデータを作成する。
                var json = File.ReadAllText(jsonFilePath);
                var jsonData = JsonUtility.FromJson<PatientData>(json);
                var modelData = new ModelData(
                    jsonData.Patient.Name,
                    jsonData.Patient.ID,
                    jsonData.Patient.Birthday,
                    jsonData.Patient.StudyDate,
                    jsonData.DefaultPreset,
                    glbFilePath,
                    directory);
                modelDataList.Add(modelData);
            }

            // モデルデータを作成日順にソートする。最も新しいデータが先頭に来るようにする。
            modelDataList.Sort((a, b) => string.Compare(b.StudyDate, a.StudyDate, StringComparison.Ordinal));

            return UniTask.FromResult(modelDataList.ToArray());
        }

        public async UniTask<ModelData> GetModelDataAsync(CancellationToken ct)
        {
            var modelDataArray = await GetModelDataArrayAsync(ct);

            if (modelDataArray == null || modelDataArray.Length == 0)
            {
                throw new InvalidOperationException("No model data found in persistent data path.");
            }

            // 最初のデータ（最新のデータ）を返す
            return modelDataArray[0];
        }
    }
}
