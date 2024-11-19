using UnityEngine;

namespace Momos.Tools
{
    public abstract class ConfigLoader<T> where T : ScriptableObject
    {
        public abstract string AssetName { get; }
        public virtual string ResourcePath => $"Tools/Config/{AssetName}";
        public virtual string LocalFullPath => $"{nameof(Resources)}/{ResourcePath}.asset";

        /// <summary> 尝试加载 </summary>
        public bool TryLoad(ref T configAsset)
        {
            T config = Get();
            if (config != null)
            {
                configAsset = config;
                return true;
            }
            return false;
        }

        /// <summary> 
        /// 验证一条路径(带文件名及其后缀),
        /// 是否能正确被加载.
        /// </summary>
        /// <param name="path"> 路径从Assets开始 </param>
        public bool IsUsablePath(string path)
        {
            int resIndex = path.IndexOf(nameof(Resources));
            if (resIndex != -1)
            {
                string suffixPath = path.Substring(resIndex, path.Length - resIndex);
                return suffixPath == LocalFullPath;
            }
            return false;
        }
        /// <summary> 路径验证失败后将打印的警告文本 </summary>
        public string IsUnusablePathWarning(string path)
        {
            return $"当前保存路径:{path} 没有指向{LocalFullPath} 不能被读取。 \n是否继续创建?";
        }

        private T Get()
        {
            return Resources.Load<T>(ResourcePath);
        }
    }
}