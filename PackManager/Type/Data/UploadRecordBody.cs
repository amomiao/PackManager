using System;

namespace Momos.Tools.PackManager
{
    [Serializable]
    public class UploadRecordBody
    {
        // 上传发生的时间
        public string dataTime;
        // 项目信息
        public string projectMessage;
        // 上传到的目录位置
        public string directoryName;
        // 描述
        public string description;
    }
}