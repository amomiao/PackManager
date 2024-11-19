using UnityEditor;
using UnityEngine;

namespace Momos.Tools.PackManager
{
    internal class PackDeltaWindow : EditorWindow
    {
        internal PackMgrConfigAsset config;
        internal PackItemBody packItem;
        private bool isChangable = false;
        private Vector2 usv;
        private Vector2 dsv;
        private PackMgrFunction function;
        private PackMgrFunction Function => function ?? new PackMgrFunction();

        private void OnGUI()
        {
            isChangable = EditorGUILayout.Toggle("准许修改", isChangable);
            EditorGUILayout.BeginHorizontal(); // 0
            if (GUILayout.Button("保存"))
            { 
                EditorUtility.SetDirty(config);
                AssetDatabase.SaveAssetIfDirty(config);
            }
            if (GUILayout.Button("发布新版"))
            {
                Function.ShowPackUploadWindow(config).UploadUpdate(packItem.packName, packItem.directoryName);
            }
            if (GUILayout.Button("打开目录"))
            {
                Function.OpenExplorer(config.GetPackItemDirectoryPath(packItem));
            }
            if (GUILayout.Button("打开Git目录"))
            {
                Function.OpenExplorer(packItem.gitDirectoryPath);
            }
            if (GUILayout.Button("跳转到Git页面"))
            {
                Function.OpenURL(packItem.gitUrl);
            }
            GUILayout.EndHorizontal(); // 0
            
            EditorGUILayout.BeginHorizontal(); // 1

            // 行名
            EditorGUILayout.BeginVertical(); // 1.1
            GUILayout.Label("包名:");
            GUILayout.Label("说明:");
            GUILayout.Label("UnionNamespace:");
            GUILayout.Label("HasReadme:");
            GUILayout.Label("Git目录:");
            GUILayout.Label("GitURL:");
            EditorGUILayout.EndVertical(); // 1.1

            // 行内容
            EditorGUILayout.BeginVertical(); // 1.2
            EditorGUI.BeginDisabledGroup(!isChangable);
            // 包名
            packItem.packName = GUILayout.TextField(packItem.packName);
            // 描述
            GUILayout.Label(packItem.Description);
            // 命名空间
            packItem.isMomos = EditorGUILayout.Toggle(packItem.isMomos);
            // 拥有Readme
            packItem.hasReadme = EditorGUILayout.Toggle(packItem.hasReadme);
            // git目录
            packItem.gitDirectoryPath = GUILayout.TextField(packItem.gitDirectoryPath);
            // gitURL
            packItem.gitUrl = GUILayout.TextField(packItem.gitUrl);
            // ...
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical(); // 1.2

            EditorGUILayout.EndHorizontal(); // 1 

            #region 依赖
            GUILayout.BeginHorizontal();    // 2

            GUILayout.Label("依赖1:在本管理器能直接访问到的包");
            if (GUILayout.Button("选择依赖"))
            {
                Function.ShowPackItemDependsWindow(config, packItem);
            }
            GUILayout.EndHorizontal();  // 2
            if (packItem.depends1.Count == 0)
                GUILayout.Label("无");
            else
            {
                foreach (var item in packItem.depends1)
                {
                    GUILayout.BeginHorizontal();    // 3
                    // 依赖名称
                    GUILayout.Label(item);
                    // 是否安装了依赖
                    EditorGUILayout.Toggle(config.installedPacks.Contains(item));
                    // ...
                    GUILayout.EndHorizontal();  // 3
                }
            }
            GUILayout.Label("依赖2:需要从外部导入的包");
            packItem.depends2 = GUILayout.TextArea(packItem.depends2);
            #endregion 依赖

            #region 上下记录
            GUILayout.Label("上传记录");
            usv = GUILayout.BeginScrollView(usv);
            foreach (UploadRecordBody record in packItem.uploadRecords)
            {
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("打开目录"))
                {
                    Function.OpenExplorer(config.GetPackItemDirectoryPath(packItem, record));
                }
                GUILayout.Label(record.dataTime);
                GUILayout.Label(record.projectMessage);
                GUILayout.Label(record.directoryName);
                GUILayout.Label(record.description);

                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();

            GUILayout.Label("下载记录");
            dsv = GUILayout.BeginScrollView(dsv);
            foreach (UsageRecordBody record in packItem.usageRecords)
            {
                EditorGUILayout.BeginHorizontal();

                GUILayout.Label(record.dataTime);
                GUILayout.Label(record.projectMessage);

                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            #endregion 上下记录
        }
    }
}