using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Momos.Tools.PackManager
{
    internal class PackItemDependsWindow : EditorWindow
    {
        internal PackMgrConfigAsset config;
        internal PackItemBody packItem;
        private Vector2 sv;
        private PackMgrFunction function;
        private PackMgrFunction Function => function ?? new PackMgrFunction();

        private void OnGUI()
        {
            sv = GUILayout.BeginScrollView(sv); // 1
            GUILayout.BeginHorizontal();    // 1.0

            GUILayout.BeginVertical(); // 1.1
            foreach (var item in config.packItems)
                GUILayout.Label(item.packName);
            GUILayout.EndVertical();  // 1.1

            GUILayout.BeginVertical();  // 1.2
            foreach (var item in config.packItems)
            {
                if (packItem.packName == item.packName)
                    GUILayout.Label("-");
                // 不存在依赖 允许添加依赖
                else if (!packItem.depends1.Contains(item.packName))
                {
                    if (GUILayout.Button("添加"))
                    {
                        packItem.depends1.Add(item.packName);
                        Function.SavePackMgrConfigAsset(config);
                    }
                }
                // 存在依赖 允许移除依赖
                else
                {
                    if (GUILayout.Button("移除"))
                    {
                        packItem.depends1.Remove(item.packName);
                        Function.SavePackMgrConfigAsset(config);
                    }
                }
            }
            GUILayout.EndVertical();  // 1.2

            GUILayout.EndHorizontal();  // 1.0
            GUILayout.EndScrollView();  // 1
        }
    }
}