using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace AssetBundleBrowser
{
    internal class MessageList
    {
        private Vector2 m_ScrollPosition = Vector2.zero;

        private GUIStyle[] m_Style = new GUIStyle[2];

        IEnumerable<AssetBundleModel.AssetInfo> m_Selecteditems;
        List<MessageSystem.Message> m_Messages;

        Vector2 m_Dimensions = new Vector2(0, 0);
        const float k_ScrollbarPadding = 16f;
        const float k_BorderSize = 1f;

        List<AssetBundleManageTab.DependencyInfoData> m_dependencyInfo;
        IEnumerable<AssetBundleModel.BundleInfo> m_SelectItemByBundlelist;
        float testY = 0f;
        internal MessageList()
        {
            Init();
        }
        private void Init()
        {
            m_Style[0] = "OL EntryBackOdd";
            m_Style[1] = "OL EntryBackEven";
            m_Style[0].wordWrap = true;
            m_Style[1].wordWrap = true;
            m_Style[0].padding = new RectOffset(32, 0, 1, 4);
            m_Style[1].padding = new RectOffset(32, 0, 1, 4);
            m_Messages = new List<MessageSystem.Message>();

            m_dependencyInfo = AssetBundleBrowserMain.instance.m_ManageTab._dependencyInfo;
        }
        internal void OnGUI(Rect fullPos)
        {
            DrawOutline(fullPos, 1f);

            Rect pos = new Rect(fullPos.x + k_BorderSize, fullPos.y + k_BorderSize, fullPos.width - 2 * k_BorderSize, fullPos.height - 2 * k_BorderSize);

            if (m_Messages != null && m_Messages.Count > 0)
            {
                if (m_Dimensions.y == 0 || m_Dimensions.x != pos.width - k_ScrollbarPadding)
                {
                    //recalculate height.
                    m_Dimensions.x = pos.width - k_ScrollbarPadding;
                    m_Dimensions.y = 0;
                    foreach (var message in m_Messages)
                    {
                        m_Dimensions.y += m_Style[0].CalcHeight(new GUIContent(message.message), m_Dimensions.x);
                    }
                }

                m_ScrollPosition = GUI.BeginScrollView(pos, m_ScrollPosition, new Rect(0, 0, m_Dimensions.x, m_Dimensions.y));

                int counter = 0;
                float runningHeight = 0.0f;
                foreach (var message in m_Messages)
                {
                    int index = counter % 2;
                    var content = new GUIContent(message.message);
                    float height = m_Style[index].CalcHeight(content, m_Dimensions.x);

                    GUI.Box(new Rect(0, runningHeight, m_Dimensions.x, height), content, m_Style[index]);
                    GUI.DrawTexture(new Rect(0, runningHeight, 32f, 32f), message.icon);
                    //TODO - cleanup formatting issues and switch to HelpBox
                    //EditorGUI.HelpBox(new Rect(0, runningHeight, m_dimensions.x, height), message.message, (MessageType)message.severity);

                    counter++;
                    runningHeight += height;
                }

                GUI.EndScrollView();
            }
            else
            {
                if (m_Dimensions.y == 0 || m_Dimensions.x != pos.width - k_ScrollbarPadding || pos.height < testY)
                {
                    //recalculate height.
                    m_Dimensions.x = pos.width - k_ScrollbarPadding;
                    m_Dimensions.y = 0;


                    foreach (var msg in m_dependencyInfo)
                    {
                        var bunData = m_SelectItemByBundlelist.FirstOrDefault();

                        if (bunData != null)
                        {
                            if (!msg.checkSelectAssetName.Equals(bunData.displayName))
                            {
                                continue;
                            }

                            string msgs = $"[ {msg.assetFullname} ]\n[ {msg.aiFullName} ]";
                            m_Dimensions.y += m_Style[0].CalcHeight(new GUIContent(msgs), m_Dimensions.x);
                        }

                    }

                    // 칸이 딱 떨어지게 채워지지 않아 임시로 칸을 늘리는 코드..
                    if (m_dependencyInfo != null && m_dependencyInfo.Count > 0)
                    {
                        string tmsgs = $"[ {m_dependencyInfo[0].assetFullname} ]\n[ {m_dependencyInfo[0].aiFullName} ]";
                        m_Dimensions.y += m_Style[0].CalcHeight(new GUIContent(tmsgs), m_Dimensions.x);
                    }

                }

                m_ScrollPosition = GUI.BeginScrollView(pos, m_ScrollPosition, new Rect(0, 0, m_Dimensions.x, m_Dimensions.y));
                float testh = 0;
                int counter = 0;
                int curIndex = 0;
                foreach (var msg in m_dependencyInfo)
                {
                    var bunData = m_SelectItemByBundlelist.FirstOrDefault();

                    if (bunData != null)
                    {
                        if (!msg.checkSelectAssetName.Equals(bunData.displayName))
                        {
                            continue;
                        }

                        if(msg.dependencyAssetName.Contains("font"))
                        {
                            continue;
                        }

                        int index = counter % 2;
                        string msgs = $"[ {msg.assetFullname} ]\n[ {msg.aiFullName} ]";
                        var content = new GUIContent(msgs);
                        float height = m_Style[index].CalcHeight(content, m_Dimensions.x);

                        GUI.contentColor = Color.white;
                        GUI.TextArea(new Rect(0f, testh, m_Dimensions.x, height), msgs);
                        counter++;
                        testh += height;
                        curIndex += 1;
                    }

                }
                testY = testh;

                GUI.EndScrollView();
            }
        }

        internal void SetBundleListItem(IEnumerable<AssetBundleModel.BundleInfo> bundle)
        {
            m_SelectItemByBundlelist = bundle;
        }

        internal void SetItems(IEnumerable<AssetBundleModel.AssetInfo> items)
        {
            m_Selecteditems = items;
            CollectMessages();
        }

        internal void CollectMessages()
        {
            m_Messages.Clear();
            m_Dimensions.y = 0f;
            if (m_Selecteditems != null)
            {
                foreach (var asset in m_Selecteditems)
                {
                    m_Messages.AddRange(asset.GetMessages());
                }
            }
        }

        internal static void DrawOutline(Rect rect, float size)
        {
            Color color = new Color(0.6f, 0.6f, 0.6f, 1.333f);
            if (EditorGUIUtility.isProSkin)
            {
                color.r = 0.12f;
                color.g = 0.12f;
                color.b = 0.12f;
            }

            if (Event.current.type != EventType.Repaint)
                return;

            Color orgColor = GUI.color;
            GUI.color = GUI.color * color;
            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, size), EditorGUIUtility.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.yMax - size, rect.width, size), EditorGUIUtility.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.y + 1, size, rect.height - 2 * size), EditorGUIUtility.whiteTexture);
            GUI.DrawTexture(new Rect(rect.xMax - size, rect.y + 1, size, rect.height - 2 * size), EditorGUIUtility.whiteTexture);

            GUI.color = orgColor;
        }
    }
}
