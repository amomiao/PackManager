using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Momos.Tools.PackManager
{
    internal class PackManagerConfigWindow : EditorWindow
    {
        [MenuItem("Tools/PackManagerConfig",priority = 0)]
        private static void ShowWindow()
        {
            PackManagerConfigWindow win = EditorWindow.GetWindow<PackManagerConfigWindow>();
            win.titleContent = new GUIContent("PackManagerConfig");
            win.position = new Rect(200, 200, 1000, 200);
            win.Show();
        }

        // private string searchString;
        private Vector2 sv;
        private PackMgrConfigLoader loader;
        private PackMgrConfigAsset config;
        private PackMgrAssistant function;
        private float textContentWidth;    // Max(300, position.width - 16 - LabelContentWidth), 其中16是竖滚动条

        private float LabelContentWidth => 560;
        private PackMgrConfigLoader Loader => loader ?? new PackMgrConfigLoader();
        private PackMgrConfigAsset Config => config;
        private PackMgrAssistant Function => function ?? new PackMgrAssistant();

        private void Save()
        {
            // 其他页面不会涉及'已安装'相关逻辑
            foreach (PackItemBody item in config.packItems)
            {
                // 被标记为已安装的包 不在记录列表里
                if (item.IsInstal && !config.installedPacks.Contains(item.packName))
                {
                    // 记录
                    config.installedPacks.Add(item.packName);
                    item.usageRecords.Add(new UsageRecordBody(Function.GetTime(), "添加:" + Function.GetProjectMessageNoTime()));
                }
                // 被标记为未安装的包 在记录列表里
                else if (!item.IsInstal && config.installedPacks.Contains(item.packName))
                {
                    // 删除
                    config.installedPacks.Remove(item.packName);
                    item.usageRecords.Add(new UsageRecordBody(Function.GetTime(), "移除:" + Function.GetProjectMessageNoTime()));
                }
            }
            Function.SavePackMgrConfigAssetData(config);
        }

        private void OnEnable()
        {
            Loader.TryLoad(ref config);
            // isInstal是private的 未持久化,需要时不时重写进内存。
            if (config != null)
            {
                foreach (var item in config.packItems)
                {
                    if (config.installedPacks.Contains(item.packName))
                        item.IsInstal = true;
                }
            }
        }

        private void OnDisable()
        {
            if(config != null)
                Save();
        }

        private void OnGUI()
        {
            if (config == null)
                OnNotConfigGUI();
            else
                OnDrawConfigGUI();
        }

        #region OnNotConfigGUI
        private void OnNotConfigGUI()
        {
            GUILayout.Label($"未加载到一个{nameof(PackMgrConfigAsset)}");

            GUILayout.BeginHorizontal();
            GUILayout.Label("检查任一Resources目录下是否存在");
            GUILayout.TextField(Loader.ResourcePath + ".asset");
            GUILayout.EndHorizontal();

            GUILayout.Label("或是进行创建操作");
            // .MPack是放置在项目外的文件
            if (GUILayout.Button("创建.MPack文件"))
            {
                string path = EditorUtility.SaveFilePanel(
                    "创建.MPack",
                    $"{Application.dataPath}",
                    $"{Loader.AssetName}",
                    "MPack");
                if (!string.IsNullOrEmpty(path))
                {
                    string dirPath = path.Substring(0, path.LastIndexOf("/"));
                    if ((Directory.GetFiles(dirPath).Length == 0 && Directory.GetDirectories(dirPath).Length == 0) ||
                        EditorUtility.DisplayDialog("非空文件夹", $"{dirPath}目录非空,是否创建?", "确定", "取消"))
                    { 
                        string jsonContent = new PackMgrConfigAsset.ConfigData(path).ToJson();
                        File.WriteAllText(path, jsonContent);
                    }
                }
            }
            if (GUILayout.Button("读入.MPack文件 创建ConfigAsset"))
            {
                string path = EditorUtility.OpenFilePanel(
                    "读入.MPack文件",
                    $"{Application.dataPath}",
                    $"MPack");
                string json = File.ReadAllText(path);
                path = EditorUtility.SaveFilePanelInProject(
                    $"创建{nameof(PackMgrConfigAsset)}",
                    $"{Loader.AssetName}",
                    "asset",
                    "1111");
                if (!string.IsNullOrEmpty(path))
                {
                    if (Loader.IsUsablePath(path) ||
                        EditorUtility.DisplayDialog("警告", Loader.IsUnusablePathWarning(path), "是", "否")
                        )
                    {
                        PackMgrConfigAsset asset = ScriptableObject.CreateInstance<PackMgrConfigAsset>();
                        asset.FromJson(json);
                        // 通过编辑器API 根据数据创建一个数据资源文件
                        AssetDatabase.CreateAsset(asset, path);
                        // 保存创建的资源
                        AssetDatabase.SaveAssets();
                        // 刷新界面
                        AssetDatabase.Refresh();
                        // 保存到对应位置后 尝试加载
                        Loader.TryLoad(ref config);
                        EditorUtility.SetDirty(Config);
                        AssetDatabase.SaveAssets();
                    }
                }
            }
        }
        #endregion OnNotConfigGUI

        private void OnDrawConfigGUI()
        {
            textContentWidth = Mathf.Max(300, position.width - 16 - LabelContentWidth);
            DrawConfigHeadRow();

            // 首行20pixel: 64 = 44 + 20
            GUI.Box(new Rect(4, 64, position.width - 8, position.height - 64), GUIContent.none);
            GUILayout.BeginArea(new Rect(0, 44, position.width, position.height - 44));
            DrawConfigTableHeadRow(); // 表头

            sv = GUI.BeginScrollView(
                new Rect(0, 20, position.width, position.height - 64),
                sv,
                new Rect(0, 0, position.width - 20, 20 * config.packItems.Count));
            for (int i = 0; i < config.packItems.Count; i++)
            {
                DrawConfigTableContentRow(i, config.packItems[i]);
            }
            GUI.EndScrollView();
            // DrawConfigTableContent();
            GUILayout.EndArea();
        }

        private void DrawConfigHeadRow()
        {
            // GUILayout.BeginHorizontal();
            GUILayout.Label($"LocalJsonPath:{Config.localJsonConfigPath}");
            // GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();

            //GUILayout.Label("搜索"); // 🕊
            //searchString = GUILayout.TextField(searchString);

            if (GUILayout.Button("保存并记录"))
            {
                Save();
            }
            if (GUILayout.Button("新建"))
            {
               Function.ShowPackUploadWindow(config).UploadCreate();
            }
            GUILayout.EndHorizontal();
        }
        private void DrawConfigTableHeadRow()
        {
            GUI.TextField(GetTableCellRect(0, 0), "包名");
            GUI.TextField(GetTableCellRect(0, 1), "目录名");
            GUI.TextField(GetTableCellRect(0, 2), "说明");

            GUILayout.BeginArea(new Rect(textContentWidth, 0, LabelContentWidth, 20));
            GUI.TextField(GetTableCellRect(0, 3), "更新时间");
            GUI.TextField(GetTableCellRect(0, 4), "依赖完整");
            GUI.TextField(GetTableCellRect(0, 5), "UnionNamespace");
            GUI.TextField(GetTableCellRect(0, 6), "HasReadme");
            GUI.TextField(GetTableCellRect(0, 7), "已安装");
            GUI.TextField(GetTableCellRect(0, 8), "详情");
            GUILayout.EndArea();
        }
        private void DrawConfigTableContentRow(int row, PackItemBody item)
        {
            GUI.Label(GetTableCellRect(row, 0), item.packName);
            GUI.Label(GetTableCellRect(row, 1), item.directoryName);
            GUI.Label(GetTableCellRect(row, 2), item.Description);

            GUILayout.BeginArea(new Rect(textContentWidth, row * 20, LabelContentWidth, 20)); // 1
            GUI.Label(GetTableCellRect(0, 3), item.LastUploadTime);
            EditorGUI.Toggle(GetTableCellRect(0, 4),item.IsCompletedDepends(Config));
            item.isMomos = EditorGUI.Toggle(GetTableCellRect(0, 5),item.isMomos);
            item.hasReadme = EditorGUI.Toggle(GetTableCellRect(0, 6), item.hasReadme);
            item.IsInstal = EditorGUI.Toggle(GetTableCellRect(0, 7), item.IsInstal);
            if (GUI.Button(GetTableCellRect(0, 8),"详情"))
            {
                Function.ShowPackDeltaWindow(config, item);
            }
            GUILayout.EndArea(); // 1
        }
        private Rect GetTableCellRect(int r,int c)
        {
            switch (c)
            {
                // 包名
                case 0: return new Rect(0, r * 20, textContentWidth * 0.25f, 20);
                // 目录名
                case 1: return new Rect(textContentWidth * 0.25f, r * 20, textContentWidth * 0.25f, 20);
                // 说明
                case 2: return new Rect(textContentWidth * 0.5f, r * 20, textContentWidth * 0.5f, 20);
                // 更新时间
                case 3: return new Rect(0, r * 20, 140, 20);
                // 依赖完整
                case 4: return new Rect(140, r * 20, 80, 20);
                // UnionNamespace
                case 5: return new Rect(220, r * 20, 120, 20);
                // HasReadme
                case 6: return new Rect(340, r * 20, 100, 20);
                // 已安装
                case 7: return new Rect(440, r * 20, 60, 20);
                // 详情
                case 8: return new Rect(500, r * 20, 60, 20);
            }
            return new Rect();
        }
    }
}