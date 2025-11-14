using UnityEngine;

namespace Thoracentes
{
    /// <summary>
    /// 3Dモデルのルートオブジェクトとして機能する。
    /// </summary>
    public class ModelParent : MonoBehaviour
    {
        private static Transform _modelRoot;
        private static GameObject _centerNode;
        private static ModelParent _instance;

        /// <summary>
        /// インスタンス。
        /// 既に存在していれば既存のものを、まだ存在していなければ新たに生成して返す。
        /// </summary>
        public static ModelParent Instance
        {
            get
            {
                if (_instance) return _instance;

                var obj = new GameObject("ModelParent");
                _instance = obj.AddComponent<ModelParent>();
                return _instance;
            }
        }

        /// <summary>
        /// 3Dモデルを削除する。
        /// </summary>
        public void DeleteModel()
        {
            var root = GetModelRoot();
            if (root)
            {
                Destroy(root.gameObject);
            }

            ResetTransform();
        }

        /// <summary>
        /// Transformの値をリセットする。
        /// </summary>
        private void ResetTransform()
        {
            Instance.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        /// <summary>
        /// 3Dモデルのルートオブジェクトを設定する。
        /// </summary>
        /// <param name="modelRoot">3DモデルのルートとなるTransform(親ではない)</param>
        public void SetModelRoot(Transform modelRoot)
        {
            _modelRoot = modelRoot;
        }

        /// <summary>
        /// 3Dモデルの親オブジェクト(自身)を取得する。
        /// </summary>
        public Transform GetModelParent() => Instance.transform;

        /// <summary>
        /// 3Dモデルのルートオブジェクトを取得する。
        /// </summary>
        public Transform GetModelRoot() => _modelRoot;

        /// <summary>
        /// 中心としての振る舞いを担うノードを取得する。
        /// </summary>
        public Transform GetCenterNode()
        {
            if (!_centerNode)
            {
                CreateCenterNode();
            }

            return _centerNode.transform;
        }

        /// <summary>
        /// 3Dモデルの親オブジェクト(自身)のアクティブ状態を変更する。
        /// </summary>
        /// <param name="active">アクティブかどうか</param>
        public void ModelParentSetActive(bool active)
        {
            GetModelParent()?.gameObject.SetActive(active);
        }

        /// <summary>
        /// 3Dモデルの中心として振る舞うノードを作成する。
        /// Pivotの定義次第では回転軸が3Dモデルの中心にならないため、その対策として作成する。
        /// </summary>
        public void CreateCenterNode()
        {
            var modelParent = GetModelParent();
            var modelRoot = GetModelRoot();

            var renderers = modelParent.GetComponentsInChildren<Renderer>();
            var bounds = renderers[0].bounds;
            for (var i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            var center = bounds.center;
            _centerNode = new GameObject("ModelCenter")
            {
                transform =
                {
                    position = center
                }
            };

            _centerNode.transform.SetParent(modelParent);
            _centerNode.transform.localRotation = _modelRoot.localRotation;
            modelRoot.transform.SetParent(_centerNode.transform);
        }

        public void DestroyCenterNode()
        {
            var modelParent = GetModelParent();
            var modelRoot = GetModelRoot();

            modelRoot.SetParent(modelParent);

            if (_centerNode)
            {
                Destroy(_centerNode);
            }
        }
    }
}