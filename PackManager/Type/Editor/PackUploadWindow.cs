using Codice.CM.Common;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Momos.Tools.PackManager
{
    internal class PackUploadWindow : EditorWindow
    {
        internal PackMgrConfigAsset config;
        private bool isCreateNew;
        private string packName;
        private string directoryName;
        private string description;
        private bool isMomos;
        private PackMgrFunction function;
        private PackMgrFunction Function => function ?? new PackMgrFunction();

        internal void UploadCreate()
        {
            isCreateNew = true;
        }
        internal void UploadUpdate(string packName,string directoryName)
        {
            isCreateNew = false;
            this.packName = packName;
            this.directoryName = directoryName;
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal(); // 1

            GUILayout.BeginVertical(); // 1.1
            GUILayout.Label("包名");
            GUILayout.Label("目录名");
            GUILayout.Label("说明");
            GUILayout.Label("UnionNamespace");
            GUILayout.EndVertical(); // 1.1

            GUILayout.BeginVertical(); // 1.2
            EditorGUI.BeginDisabledGroup(!isCreateNew); // 1.2.1
            packName = GUILayout.TextField(packName);
            directoryName = GUILayout.TextField(directoryName);
            EditorGUI.EndDisabledGroup(); // 1.2.1
            description = GUILayout.TextField(description);
            isMomos = EditorGUILayout.Toggle(isMomos);
            GUILayout.EndVertical(); // 1.2

            GUILayout.EndHorizontal(); // 1

            if (GUILayout.Button("创建"))
            {
                if (Ver())
                {
                    string localRootPath = GetOrCreateDirectory();
                    // 获得并修改config.packItems
                    PackItemBody packItem = GetPackItemBody();
                    // 添加packItem的uploadRecord
                    // 为Null报错是数据错误,整个存储库有报废的风险
                    packItem.uploadRecords.Add(CreateUploadRecord());
                    Function.OpenExplorer(localRootPath);
                    Function.SavePackMgrConfigAsset(config);
                    // 关闭窗口
                    Close();
                }
            }
        }

        #region 路径与文件夹
        // 得到项目的文件夹 地址
        private string GetItemRootPath()
        {
            string path = config.LocalConfigDirectoryPath;
            path = Path.Combine(path, $"{directoryName}");
            return path;
        }
        // 得到项目某版本的文件夹 地址
        private string GetFolderChildPath(string rootPath)
        {
            return Path.Combine(rootPath, Function.GetProjectMessage());
        }
        // 得到|创建 一个项目及其版本的文件夹
        private string GetOrCreateDirectory()
        {
            // 得到|创建Pack本地根文件夹
            string path = GetItemRootPath();
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            // 得到|创建Pack本地根文件夹 下的本次上传文件夹
            path = GetFolderChildPath(path);
            if (!Directory.Exists(path)) // 一般得不到
                Directory.CreateDirectory(path);
            return path;
        }
        #endregion 路径与文件夹

        #region 文件夹验证
        // 验证是否可以创建新的文件夹
        private bool Ver()
        {
            if (string.IsNullOrEmpty(packName) || string.IsNullOrEmpty(directoryName))
            {
                EditorUtility.DisplayDialog("错误", "包名或目录名为空", "确定");
                return false;
            }
            if (isCreateNew && !VerCreate())
            {
                return false;  
            }
            return true;
        }
        /// <summary> 验证当创建新项目时是否合法  </summary>
        private bool VerCreate()
        {
            foreach (PackItemBody item in config.packItems)
            {
                if (item.packName == packName)
                {
                    EditorUtility.DisplayDialog("错误", $"包名{packName}冲突", "确定");
                    return false;
                }
                else if (item.directoryName == directoryName)
                {
                    EditorUtility.DisplayDialog("错误", $"与包{item.packName}的目录冲突", "确定");
                    return false;
                }
            }
            return true;
        }
        #endregion 文件夹验证

        #region 获得信息类
        // 得到一个包的信息类
        private PackItemBody GetPackItemBody()
        {
            PackItemBody packItem = null;
            if (isCreateNew)
            {
                packItem = CreatePackItem();
                config.packItems.Add(packItem);
            }
            else
            {
                foreach (PackItemBody item in config.packItems)
                {
                    if (item.packName == packName)
                    {
                        packItem = item;
                        break;
                    }
                }
            }
            return packItem;
        }
        // 使用面板数据 创建一个PackItem
        private PackItemBody CreatePackItem()
        {
            PackItemBody item = new PackItemBody();
            item.packName = packName;
            item.directoryName = directoryName;
            item.isMomos = isMomos;
            return item;
        }
        // 使用面板数据 创建一个UploadRecord
        private UploadRecordBody CreateUploadRecord()
        { 
            UploadRecordBody item = new UploadRecordBody();
            item.dataTime = Function.GetTime();
            item.directoryName = item.projectMessage = Function.GetProjectMessage();
            item.description = description;
            return item;
        }
        #endregion 获得信息类
    }
}