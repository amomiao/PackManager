using System;

namespace Momos.Tools.PackManager
{
    [Serializable]
    public class UsageRecordBody
    {
        // 使用发生的时间
        public string dataTime;
        // 项目信息
        public string projectMessage;

        public UsageRecordBody(string dataTime, string projectMessage)
        {
            this.dataTime = dataTime;
            this.projectMessage = projectMessage;
        }
    }
}