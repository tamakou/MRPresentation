using System.IO;
using UnityEngine;

namespace Thoracentes
{
    /// <summary>
    /// 3Dモデルの見た目を適用するクラス。
    /// </summary>
    public static class ApplyModelVisual
    {
        private static readonly int MainColor = Shader.PropertyToID("_MainColor");
        private static readonly int Alpha = Shader.PropertyToID("_Alpha");

        /// <summary>
        /// 適用処理。プリセットの情報を元に適用するので、モデルデータを引数として受け取る。
        /// </summary>
        /// <param name="modelData">モデルデータ</param>
        public static void Apply(ModelData modelData)
        {
            // モデルデータの配置先パスをチェック。
            var filePath = Path.GetDirectoryName(modelData.FilePath);
            if (filePath == null)
            {
                Debug.LogError($"File path is null. Path: {modelData.FilePath}");
                return;
            }

            // Jsonファイルの配置先パスをチェック。
            var jsonPath = Path.Combine(filePath, $"{modelData.Preset}.json");
            if (!File.Exists(jsonPath))
            {
                Debug.LogError($"Json file not found: {jsonPath}");
                return;
            }

            // Jsonファイルからプリセット情報を取得。
            var json = File.ReadAllText(jsonPath);
            var jsonData = JsonUtility.FromJson<PresetData>(json);

            // プリセット情報を適用。
            ApplyFromJsonData(jsonData);
        }

        public static void ApplyFromJsonData(PresetData presetData)
        {
            var root = ModelParent.Instance.GetModelRoot();
            var unTransparentShader = Shader.Find(ConstantValues.UnTransparentShaderName);
            var transparentShader = Shader.Find(ConstantValues.TransparentShaderName);

            // プリセット情報を元に名前の一致するオブジェクトを探し、シェーダーおよび設定値を適用する。
            foreach (var preset in presetData.Presets)
            {
                var modelName = preset.Name;
                var color = new Color32(preset.MLut.R, preset.MLut.G, preset.MLut.B, preset.MLut.A);
                var display = preset.Display;

                var modelObject = root.Find(modelName);
                if (modelObject)
                {
                    var modelRenderer = modelObject.GetComponent<Renderer>();
                    if (!modelRenderer) continue;

                    // Alphaを0から1の範囲に正規化
                    var alpha = Mathf.Clamp01(color.a / 255f);

                    // 不透明かどうかでシェーダーを切り替え。
                    modelRenderer.material.shader = alpha >= 1.0f ? unTransparentShader : transparentShader;

                    // シェーダーのプロパティを設定。
                    var materialPropertyBlock = new MaterialPropertyBlock();
                    modelRenderer.GetPropertyBlock(materialPropertyBlock);
                    materialPropertyBlock.SetColor(MainColor, color);
                    materialPropertyBlock.SetFloat(Alpha, alpha);
                    modelRenderer.SetPropertyBlock(materialPropertyBlock);

                    // 表示/非表示の設定。
                    modelObject.gameObject.SetActive(display != 0);
                }
                else
                {
                    Debug.LogError($"Model object not found: {modelName}");
                }
            }
        }
    }
}