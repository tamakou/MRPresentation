namespace Thoracentes
{
    /// <summary>
    /// 3Dモデルを表示、取得するため必要な情報を詰め込んだデータ。
    /// シーケンス間の受け渡しに対応。
    /// </summary>
    public class ModelData
    {
        /// <summary>
        /// 患者名。
        /// </summary>
        public string PatientName { get; }

        /// <summary>
        /// 患者のID。
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// 誕生日。
        /// </summary>
        public string Birthday { get; }
        
        /// <summary>
        /// 作成日。
        /// </summary>
        public string StudyDate { get; }
        
        /// <summary>
        /// プリセットのファイル名。
        /// </summary>
        public string Preset { get; private set; }
        
        /// <summary>
        /// ファイルの配置先となるパス。
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// ファイル一式をまとめたフォルダのパス。
        /// </summary>
        public string RootFolderPath { get; }

        public ModelData(
            string patientName,
            string id,
            string birthday,
            string studyDate,
            string preset,
            string filePath,
            string rootFolderPath)
        {
            PatientName = patientName;
            Id = id;
            Birthday = birthday;
            StudyDate = studyDate;
            Preset = preset;
            FilePath = filePath;
            RootFolderPath = rootFolderPath;
        }
        
        /// <summary>
        /// 現在設定中のプリセットを変更する。
        /// </summary>
        /// <param name="presetFileName">プリセットのファイル名</param>
        public void UpdateCurrentPreset(string presetFileName)
        {
            Preset = presetFileName;
        }

        /// <summary>
        /// データに欠損がないかチェックするメソッド。
        /// </summary>
        /// <returns>欠損がなければtrue、欠損があればfalseを返す。</returns>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(PatientName) &&
                   !string.IsNullOrWhiteSpace(Id) &&
                   //!string.IsNullOrWhiteSpace(Birthday) && 観察研究用のデータにおいて、nullが返されるので許容する。
                   !string.IsNullOrWhiteSpace(StudyDate) &&
                   !string.IsNullOrWhiteSpace(Preset) &&
                   !string.IsNullOrWhiteSpace(FilePath);
        }
    }
}