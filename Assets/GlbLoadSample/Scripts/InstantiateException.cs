using System;

namespace Thoracentes
{
    /// <summary>
    /// 3Dモデルの生成処理中にエラーが発生した場合の例外。
    /// </summary>
    public class InstantiateException : Exception
    {
        public InstantiateException(string message) : base(message)
        {
        }

        public InstantiateException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}