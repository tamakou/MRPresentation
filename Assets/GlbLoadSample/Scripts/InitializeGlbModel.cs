using System.Linq;
using UnityEngine;

namespace Thoracentes
{
    /// <summary>
    /// モデルの初期化処理を担うクラス。
    /// </summary>
    public class InitializeGlbModel
    {
        /// <summary>
        /// モデルの初期化処理。
        /// </summary>
        public void Initialize(ModelData data)
        {
            // モデルの実体をここで保持させておく。初期化時以外には変更されない前提。
            var modelParent = ModelParent.Instance.GetModelParent();
            var modelEntity = modelParent.GetChild(0);
            ModelParent.Instance.SetModelRoot(modelEntity);

            var modelRoot = ModelParent.Instance.GetModelRoot();
            var renderers = modelParent.GetComponentsInChildren<Renderer>();

            AddCollider();
            AdjustTransform();
            ApplyVisual();
            return;

            void AddCollider()
            {
                // 全RendererのBoundsをまとめて囲むBoundsを作成
                var combinedBounds = renderers[0].bounds;
                for (var i = 1; i < renderers.Length; i++)
                {
                    combinedBounds.Encapsulate(renderers[i].bounds);
                }

                var boxCollider = modelRoot.gameObject.AddComponent<BoxCollider>();

                // BoxColliderに適用
                boxCollider.center = combinedBounds.center;
                boxCollider.size = combinedBounds.size;
            }

            // 子階層のTransformを調整する。
            void AdjustTransform()
            {
                // カメラの正面1メートル先に表示する。ただし、カメラの向きはY軸以外は考慮せず、位置を参照して正面に出す。
                var camera = Camera.main;
                if (!camera)
                {
                    Debug.LogError("Main camera not found. Cannot adjust model position.");
                    return;
                }

                var cameraForward = camera.transform.forward;
                cameraForward.y = 0;
                cameraForward.Normalize();
                var position = camera.transform.position + cameraForward * 1.0f;
                modelParent.position = position;
            }

            // モデルの見た目を変更する。
            void ApplyVisual()
            {
                var shader = Shader.Find(ConstantValues.UnTransparentShaderName);
                if (!shader)
                {
                    Debug.LogError($"Shader not found: {ConstantValues.UnTransparentShaderName}");
                    return;
                }

                var modelMaterial = new Material(shader);
                renderers.ToList().ForEach(r => r.material = modelMaterial);

                // モデルの見た目を変更する。
                ApplyModelVisual.Apply(data);
            }
        }
    }
}