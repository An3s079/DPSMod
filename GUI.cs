using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace DPSMod
{
	public static class GUI
	{
			// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
			public static bool Toggle()
			{
				bool flag = !GUI.GUIRoot.gameObject.activeSelf;
				GUI.GUIRoot.gameObject.SetActive(flag);
				return flag;
			}

			// Token: 0x06000002 RID: 2 RVA: 0x00002087 File Offset: 0x00000287
			public static void SetVisible(bool visible)
			{
				GUI.GUIRoot.gameObject.SetActive(visible);
			}

			// Token: 0x06000003 RID: 3 RVA: 0x0000209C File Offset: 0x0000029C
			public static void Init()
			{
				GUI.GUIController = new GameObject("GUIController").transform;
				UnityEngine.Object.DontDestroyOnLoad(GUI.GUIController.gameObject);
				GUI.CreateCanvas();
				GUI.GUIRoot = GUI.m_canvas.transform;
				GUI.GUIRoot.SetParent(GUI.GUIController);
			}

			// Token: 0x06000004 RID: 4 RVA: 0x000020F4 File Offset: 0x000002F4
			public static void CreateCanvas()
			{
				GameObject gameObject = new GameObject("Canvas");
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
				GUI.m_canvas = gameObject.AddComponent<Canvas>();
				GUI.m_canvas.renderMode = RenderMode.ScreenSpaceOverlay;
				GUI.m_canvas.sortingOrder = 100000;
				gameObject.AddComponent<CanvasScaler>();
				gameObject.AddComponent<GraphicRaycaster>();
			}

			// Token: 0x06000005 RID: 5 RVA: 0x0000214C File Offset: 0x0000034C
			public static Text CreateText(Transform parent, Vector2 offset, string text, TextAnchor anchor = TextAnchor.MiddleCenter, int font_size = 20)
			{
				GameObject gameObject = new GameObject("Text");
				gameObject.transform.SetParent((parent != null) ? parent : GUI.GUIRoot);
				RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
				rectTransform.SetTextAnchor(anchor);
				rectTransform.anchoredPosition = offset;
				//Tools.LogPropertiesAndFields<Font>(ResourceManager.LoadAssetBundle("shared_auto_001").LoadAsset<Font>("04B_03__"), "");
				Text text2 = gameObject.AddComponent<Text>();
				text2.horizontalOverflow = HorizontalWrapMode.Overflow;
				text2.verticalOverflow = VerticalWrapMode.Overflow;
				text2.alignment = anchor;
				text2.text = text;
				text2.font = ResourceManager.LoadAssetBundle("shared_auto_001").LoadAsset<Font>("04B_03__");
				text2.fontSize = font_size;
				text2.color = Color.grey;
				return text2;
			}

			// Token: 0x06000006 RID: 6 RVA: 0x0000220A File Offset: 0x0000040A
			public static void SetTextAnchor(this RectTransform r, TextAnchor anchor)
			{
				r.anchorMin = GUI.AnchorMap[anchor];
				r.anchorMax = GUI.AnchorMap[anchor];
				r.pivot = GUI.AnchorMap[anchor];
			}

			// Token: 0x04000001 RID: 1
			public static Font font;

			// Token: 0x04000002 RID: 2
			public static Transform GUIRoot;

			// Token: 0x04000003 RID: 3
			public static Transform GUIController;

			// Token: 0x04000004 RID: 4
			private static Canvas m_canvas;

			// Token: 0x04000005 RID: 5
			public static readonly Dictionary<TextAnchor, Vector2> AnchorMap = new Dictionary<TextAnchor, Vector2>
		{
			{
				TextAnchor.LowerLeft,
				new Vector2(0f, 0f)
			},
			{
				TextAnchor.LowerCenter,
				new Vector2(0.5f, 0f)
			},
			{
				TextAnchor.LowerRight,
				new Vector2(1f, 0f)
			},
			{
				TextAnchor.MiddleLeft,
				new Vector2(0f, 0.5f)
			},
			{
				TextAnchor.MiddleCenter,
				new Vector2(0.5f, 0.5f)
			},
			{
				TextAnchor.MiddleRight,
				new Vector2(1f, 0.5f)
			},
			{
				TextAnchor.UpperLeft,
				new Vector2(0f, 1f)
			},
			{
				TextAnchor.UpperCenter,
				new Vector2(0.5f, 1f)
			},
			{
				TextAnchor.UpperRight,
				new Vector2(1f, 1f)
			}
		};

			// Token: 0x04000006 RID: 6
			private static Color defaultTextColor = new Color(1f, 1f, 1f, 0.5f);
	}
}
