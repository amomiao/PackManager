using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Momos.Tools.PackManager
{
    public class PackMgrConfigAsset : ScriptableObject
    {
        public class ConfigData
        {
            public string localJsonConfigPath;
            public List<PackItemBody> packItems;
            public ConfigData() { }
            public ConfigData(string path)
            {
                this.localJsonConfigPath = path;
                this.packItems = null;
            }
            public ConfigData(PackMgrConfigAsset asset)
            {
                this.localJsonConfigPath = asset.localJsonConfigPath;
                this.packItems = asset.packItems;
            }
            public string ToJson() => JsonUtility.ToJson(this);
        }

        /// <summary> 存储端Json配置文件路径 </summary>
        public string localJsonConfigPath;
        /// <summary> Json配置文件数据,存储端和使用端同步记录 </summary>
        public List<PackItemBody> packItems = new List<PackItemBody>();
        /// <summary> 项目中已安装的包 </summary>
        public List<string> installedPacks = new List<string>();

        public string LocalConfigDirectoryPath => localJsonConfigPath.Substring(0, localJsonConfigPath.LastIndexOf('/'));

        public string GetPackItemDirectoryPath(PackItemBody item) => Path.Combine(LocalConfigDirectoryPath, item.directoryName);
        public string GetPackItemDirectoryPath(PackItemBody item, UploadRecordBody record) => Path.Combine(LocalConfigDirectoryPath, item.directoryName, record.directoryName);

        public string ToJson() => new ConfigData(this).ToJson();

        public void FromJson(string json)
        {
            ConfigData cd = JsonUtility.FromJson<ConfigData>(json);
            this.localJsonConfigPath= cd.localJsonConfigPath;
            this.packItems = cd.packItems;
        }
    }
}