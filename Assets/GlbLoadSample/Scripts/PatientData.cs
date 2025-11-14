using System;

namespace Thoracentes
{
    /// <summary>
    /// 患者データ。Jsonを解釈するためのクラス。
    /// </summary>
    [Serializable]
    public class PatientData
    {
        public string Version;
        public PatientInfo Patient;
        public string DefaultPreset;
    }

    /// <summary>
    /// 患者の詳細データ。
    /// </summary>
    [Serializable]
    public class PatientInfo
    {
        public string Name;
        public string ID;
        public string Birthday;
        public string StudyDate;
    }
}