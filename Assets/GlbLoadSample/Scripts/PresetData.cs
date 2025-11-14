using System;

namespace Thoracentes
{
    /// <summary>
    /// プリセットデータ。Jsonを解釈するためのクラス。
    /// </summary>
    [Serializable]
    public class PresetData
    {
        public string Version;
        
        /// <summary>
        /// Presetの表示名。
        /// </summary>
        public string Name;
        
        /// <summary>
        /// 各部位の情報の配列。
        /// </summary>
        public Preset[] Presets;
    }

    /// <summary>
    /// 色情報。
    /// </summary>
    [Serializable]
    public class MLut
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;
    }

    /// <summary>
    /// プリセットの詳細データ。
    /// </summary>
    [Serializable]
    public class Preset
    {
        /// <summary>
        /// 部位名。
        /// </summary>
        public string Name;
        
        /// <summary>
        /// 表示するかどうかのフラグ。0: 非表示, 1: 表示。
        /// </summary>
        public int Display;
        
        /// <summary>
        /// 色情報。
        /// </summary>
        public MLut MLut;
    }
}