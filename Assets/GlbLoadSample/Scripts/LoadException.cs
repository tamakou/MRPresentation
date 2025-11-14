using System;

namespace Thoracentes
{
    /// <summary>
    /// 3Dモデルの読み込み処理中にエラーが発生した場合の例外。
    /// </summary>
    public class LoadException : Exception
    {
        public LoadException(string message) : base(message)
        {
        }

        public LoadException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}