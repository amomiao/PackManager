using System;
using System.Collections.Generic;

namespace Momos.Tools.PackManager
{
    [Serializable]
    public class PackItemBody
    {
        // 包名
        public string packName;
        // 默认排序权重
        // public int orderWeight;
        // 包的存储端目录名
        public string directoryName;
        // 上传数据
        public List<UploadRecordBody> uploadRecords = new List<UploadRecordBody>();
        // 使用数据
        public List<UsageRecordBody> usageRecords = new List<UsageRecordBody>();
        // 依赖1: 非官方的,在本管理器能直接访问到的包
        public List<string> depends1 = new List<string>();
        // 依赖2: 在管理器不能访问到的包
        public string depends2;
        // 是否继承了Momos命名空间
        public bool isMomos;
        // 是否拥有Readme
        public bool hasReadme;
        // git目录
        public string gitDirectoryPath;
        // git Url
        public string gitUrl;
        // 是否安装到了本项目
        private bool isInstal = false;

        // 描述
        public string Description
        {
            get
            {
                if (uploadRecords != null && uploadRecords.Count > 0 && uploadRecords[^1] != null)
                    return uploadRecords[^1].description;
                return "Mission";
            }
        }
        // 描述
        public string LastUploadTime
        {
            get
            {
                if (uploadRecords != null && uploadRecords.Count > 0 && uploadRecords[^1] != null)
                    return uploadRecords[^1].dataTime;
                return "Mission";
            }
        }
        // 是否安装到了本项目
        public bool IsInstal { get => isInstal; set => isInstal = value; }
        // 是否获得了所有的依赖
        public bool IsCompletedDepends(PackMgrConfigAsset config)
        {
            if (depends1.Count == 0)
                return true;
            else
            {
                foreach (string dpName in depends1)
                {
                    if(!config.installedPacks.Contains(dpName))
                        return false;
                }
            }
            return true;
        }

    }
}