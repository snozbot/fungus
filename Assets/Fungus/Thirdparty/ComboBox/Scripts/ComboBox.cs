using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class ComboBox : MonoBehaviour 
{
	public Sprite Sprite_UISprite;
	public Sprite Sprite_Background;

	public Action<int> OnSelectionChanged;

	[SerializeField]
	private bool _interactable = true;
	public bool Interactable
	{
		get
		{
			return _interactable;
		}
		set
		{
			_interactable = value;
			var button = comboButtonRectTransform.GetComponent<Button>();
			button.interactable = _interactable;
			var image = comboImageRectTransform.GetComponent<Image>();
			image.color = image.sprite == null ? new Color(1.0f, 1.0f, 1.0f, 0.0f) : _interactable ? button.colors.normalColor : button.colors.disabledColor;
			if (!Application.isPlaying)
				return;
			if (!_interactable && overlayGO.activeSelf)
				ToggleComboBox(false);
		}
	}

	[SerializeField]
	private int _itemsToDisplay = 4;
	public int ItemsToDisplay
	{
		get
		{
			return _itemsToDisplay;
		}
		set
		{
			if (_itemsToDisplay == value)
				return;
			_itemsToDisplay = value;
			Refresh();
		}
	}

	[SerializeField]
	private bool _hideFirstItem;
	public bool HideFirstItem
	{
		get
		{
			return _hideFirstItem;
		}
		set
		{
			if (value)
				scrollOffset--;
			else
				scrollOffset++;
			_hideFirstItem = value;
			Refresh();
		}
	}

	[SerializeField]
	private int _selectedIndex = 0;
	public int SelectedIndex
	{
		get 
		{
			return _selectedIndex;
		}
		set
		{
			if (_selectedIndex == value)
				return;
			if (value > -1 && value < Items.Length)
			{
				_selectedIndex = value;
				RefreshSelected();
			}
		}
	}

	[SerializeField]
	private ComboBoxItem[] _items;
	public ComboBoxItem[] Items
	{
		get
		{
			if (_items == null)
				_items = new ComboBoxItem[0];
			return _items;
		}
		set
		{
			_items = value;
			Refresh();
		}
	}

	private GameObject overlayGO;
	private int scrollOffset;
	private float _scrollbarWidth = 20.0f;

	private RectTransform _rectTransform;
	private RectTransform rectTransform
	{
		get
		{
			if (_rectTransform == null)
				_rectTransform = GetComponent<RectTransform>();
			return _rectTransform;
		}
		set
		{
			_rectTransform = value;
		}
	}

	private RectTransform _buttonRectTransform;
	private RectTransform buttonRectTransform
	{
		get
		{
			if (_buttonRectTransform == null)
				_buttonRectTransform = rectTransform.Find("Button").GetComponent<RectTransform>();
			return _buttonRectTransform;
		}
		set
		{
			_buttonRectTransform = value;
		}
	}

	private RectTransform _comboButtonRectTransform;
	private RectTransform comboButtonRectTransform
	{
		get
		{
			if (_comboButtonRectTransform == null)
				_comboButtonRectTransform = buttonRectTransform.Find("ComboButton").GetComponent<RectTransform>();
			return _comboButtonRectTransform;
		}
		set
		{
			_comboButtonRectTransform = value;
		}
	}

	private RectTransform _comboImageRectTransform;
	private RectTransform comboImageRectTransform
	{
		get
		{
			if (_comboImageRectTransform == null)
				_comboImageRectTransform = comboButtonRectTransform.Find("Image").GetComponent<RectTransform>();
			return _comboImageRectTransform;
		}
		set
		{
			_comboImageRectTransform = value;
		}
	}

	private RectTransform _comboTextRectTransform;
	private RectTransform comboTextRectTransform
	{
		get
		{
			if (_comboTextRectTransform == null)
				_comboTextRectTransform = comboButtonRectTransform.Find("Text").GetComponent<RectTransform>();
			return _comboTextRectTransform;
		}
		set
		{
			_comboTextRectTransform = value;
		}
	}

	private RectTransform _comboArrowRectTransform;
	private RectTransform comboArrowRectTransform
	{
		get
		{
			if (_comboArrowRectTransform == null)
				_comboArrowRectTransform = buttonRectTransform.Find("Arrow").GetComponent<RectTransform>();
			return _comboArrowRectTransform;
		}
		set
		{
			_comboArrowRectTransform = value;
		}
	}

	private RectTransform _scrollPanelRectTransfrom;
	private RectTransform scrollPanelRectTransfrom
	{
		get
		{
			if (_scrollPanelRectTransfrom == null)
				_scrollPanelRectTransfrom = rectTransform.Find("Overlay/ScrollPanel").GetComponent<RectTransform>();
			return _scrollPanelRectTransfrom;
		}
		set
		{
			_scrollPanelRectTransfrom = value;
		}
	}

	private RectTransform _itemsRectTransfrom;
	private RectTransform itemsRectTransfrom
	{
		get
		{
			if (_itemsRectTransfrom == null)
				_itemsRectTransfrom = scrollPanelRectTransfrom.Find("Items").GetComponent<RectTransform>();
			return _itemsRectTransfrom;
		}
		set
		{
			_itemsRectTransfrom = value;
		}
	}

	private RectTransform _scrollbarRectTransfrom;
	private RectTransform scrollbarRectTransfrom
	{
		get
		{
			if (_scrollbarRectTransfrom == null)
				_scrollbarRectTransfrom = scrollPanelRectTransfrom.Find("Scrollbar").GetComponent<RectTransform>();
			return _scrollbarRectTransfrom;
		}
		set
		{
			_scrollbarRectTransfrom = value;
		}
	}

	private RectTransform _slidingAreaRectTransform;
	private RectTransform slidingAreaRectTransform
	{
		get
		{
			if (_slidingAreaRectTransform == null)
				_slidingAreaRectTransform = scrollbarRectTransfrom.Find("SlidingArea").GetComponent<RectTransform>();
			return _slidingAreaRectTransform;
		}
		set
		{
			_slidingAreaRectTransform = value;
		}
	}

	private RectTransform _handleRectTransfrom;
	private RectTransform handleRectTransfrom
	{
		get
		{
			if (_handleRectTransfrom == null)
				_handleRectTransfrom = slidingAreaRectTransform.Find("Handle").GetComponent<RectTransform>();
			return _handleRectTransfrom;
		}
		set
		{
			_handleRectTransfrom = value;
		}
	}

	private void Awake()
	{
		InitControl();
	}

	public void OnItemClicked(int index)
	{
		var selectionChanged = index != SelectedIndex;
		SelectedIndex = index;
		ToggleComboBox(true);
		if (selectionChanged && OnSelectionChanged != null)
			OnSelectionChanged(index);
	}

	public void AddItems(params object[] list)
	{
		var cbItems = new List<ComboBoxItem>();
		foreach (var obj in list)
		{
			if (obj is ComboBoxItem)
			{
				var item = (ComboBoxItem)obj;
				cbItems.Add(item);
				continue;
			}
			if (obj is string)
			{
				var item = new ComboBoxItem((string)obj, null, false, null);
				cbItems.Add(item);
				continue;
			}
			if (obj is Sprite)
			{
				var item = new ComboBoxItem(null, (Sprite)obj, false, null);
				cbItems.Add(item);
				continue;
			}
			throw new Exception("Only ComboBoxItem, string and Sprite types are allowed");
		}
		var newItems = new ComboBoxItem[Items.Length + cbItems.Count];
		Items.CopyTo(newItems, 0);
		cbItems.ToArray().CopyTo(newItems, Items.Length);
		Refresh();
		Items = newItems;
	}

	public void ClearItems()
	{
		Items = new ComboBoxItem[0];
	}

	public void CreateControl()
	{
		rectTransform = GetComponent<RectTransform>();

		var buttonGO = new GameObject("Button");
		buttonGO.transform.SetParent(transform, false);
		buttonRectTransform = buttonGO.AddComponent<RectTransform>();
		buttonRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rectTransform.sizeDelta.x);
		buttonRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rectTransform.sizeDelta.y);
		buttonRectTransform.anchoredPosition = Vector2.zero;

		var comboButtonGO = new GameObject("ComboButton");
		comboButtonGO.transform.SetParent(buttonRectTransform, false);
		comboButtonRectTransform = comboButtonGO.AddComponent<RectTransform>();
		comboButtonRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, buttonRectTransform.sizeDelta.x);
		comboButtonRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, buttonRectTransform.sizeDelta.y);
		comboButtonRectTransform.anchoredPosition = Vector2.zero;

		var comboButtonImage = comboButtonGO.AddComponent<Image>();
		comboButtonImage.sprite = Sprite_UISprite;
		comboButtonImage.type = Image.Type.Sliced;
		var comboButtonButton = comboButtonGO.AddComponent<Button>();
		comboButtonButton.targetGraphic = comboButtonImage;
		var comboButtonColors = new ColorBlock();
		comboButtonColors.normalColor = new Color32(255, 255, 255, 255);
		comboButtonColors.highlightedColor = new Color32(245, 245, 245, 255);
		comboButtonColors.pressedColor = new Color32(200, 200, 200, 255);
		comboButtonColors.disabledColor = new Color32(200, 200, 200, 128);
		comboButtonColors.colorMultiplier = 1.0f;
		comboButtonColors.fadeDuration = 0.1f;
		comboButtonButton.colors = comboButtonColors;

		var comboArrowGO = new GameObject("Arrow");
		comboArrowGO.transform.SetParent(buttonRectTransform, false);
		var comboArrowText = comboArrowGO.AddComponent<Text>();
		comboArrowText.color = new Color32(0, 0, 0, 255);
		comboArrowText.alignment = TextAnchor.MiddleCenter;
		comboArrowText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
		comboArrowText.text = "▼";
		comboArrowRectTransform.localScale = new Vector3(1.0f, 0.5f, 1.0f);
		comboArrowRectTransform.pivot = new Vector2(1.0f, 0.5f);
		comboArrowRectTransform.anchorMin = Vector2.right;
		comboArrowRectTransform.anchorMax = Vector2.one;
		comboArrowRectTransform.anchoredPosition = Vector2.zero;
		comboArrowRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, comboButtonRectTransform.sizeDelta.y);
		comboArrowRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, comboButtonRectTransform.sizeDelta.y);
		var comboArrowCanvasGroup = comboArrowGO.AddComponent<CanvasGroup>();
		comboArrowCanvasGroup.interactable = false;
		comboArrowCanvasGroup.blocksRaycasts = false;

		var comboImageGO = new GameObject("Image");
		comboImageGO.transform.SetParent(comboButtonRectTransform, false);
		var comboImageImage = comboImageGO.AddComponent<Image>();
		comboImageImage.color = new Color32(255, 255, 255, 0);
		comboImageRectTransform.pivot = Vector2.up;
		comboImageRectTransform.anchorMin = Vector2.zero;
		comboImageRectTransform.anchorMax = Vector2.up;
		comboImageRectTransform.anchoredPosition = new Vector2(4.0f, -4.0f);
		comboImageRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, comboButtonRectTransform.sizeDelta.y - 8.0f);
		comboImageRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, comboButtonRectTransform.sizeDelta.y - 8.0f);

		var comboTextGO = new GameObject("Text");
		comboTextGO.transform.SetParent(comboButtonRectTransform, false);
		var comboTextText = comboTextGO.AddComponent<Text>();
		comboTextText.color = new Color32(0, 0, 0, 255);
		comboTextText.alignment = TextAnchor.MiddleLeft;
		comboTextText.lineSpacing = 1.2f;
		comboTextText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
		comboTextRectTransform.pivot = Vector2.up;
		comboTextRectTransform.anchorMin = Vector2.zero;
		comboTextRectTransform.anchorMax = Vector2.one;
		comboTextRectTransform.anchoredPosition = new Vector2(10.0f, 0.0f);
		comboTextRectTransform.offsetMax = new Vector2(4.0f, 0.0f);
		comboTextRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, comboButtonRectTransform.sizeDelta.y);
	}

	public void InitControl()
	{
		var cbi = transform.Find("Button/ComboButton/Image");
		var cbt = transform.Find("Button/ComboButton/Text");
		var cba = transform.Find("Button/Arrow");
		if (cbi == null || cbt == null || cba == null)
		{
			foreach (Transform child in transform)
				Destroy(child);
			CreateControl();
		}

		comboButtonRectTransform.GetComponent<Button>().onClick.AddListener(() => { ToggleComboBox(false); });
		var dropdownHeight = comboButtonRectTransform.sizeDelta.y *  Mathf.Min(ItemsToDisplay, Items.Length - (HideFirstItem ? 1 : 0));

		overlayGO = new GameObject("Overlay");
		overlayGO.SetActive(false);
		var overlayImage = overlayGO.AddComponent<Image>();
		overlayImage.color = new Color32(0, 0, 0, 0);
		var canvasTransform = transform;
		while (canvasTransform.GetComponent<Canvas>() == null)
			canvasTransform = canvasTransform.parent;
		overlayGO.transform.SetParent(canvasTransform, false);
		var overlayRectTransform = overlayGO.GetComponent<RectTransform>();
		overlayRectTransform.anchorMin = Vector2.zero;
		overlayRectTransform.anchorMax = Vector2.one;
		overlayRectTransform.offsetMin = Vector2.zero;
		overlayRectTransform.offsetMax = Vector2.zero;
		overlayGO.transform.SetParent(transform, false);
		var overlayButton = overlayGO.AddComponent<Button>();
		overlayButton.targetGraphic = overlayImage;
		overlayButton.onClick.AddListener(() => { ToggleComboBox(false); });

		var scrollPanelGO = new GameObject("ScrollPanel");
		var scrollPanelImage = scrollPanelGO.AddComponent<Image>();
		scrollPanelImage.sprite = Sprite_UISprite;
		scrollPanelImage.type = Image.Type.Sliced;
		scrollPanelGO.transform.SetParent(overlayGO.transform, false);
		scrollPanelRectTransfrom.pivot = new Vector2(0.5f, 1.0f);
		scrollPanelRectTransfrom.anchorMin = Vector2.zero;
		scrollPanelRectTransfrom.anchorMax = Vector2.one;
		scrollPanelGO.transform.SetParent(transform, false);
		scrollPanelRectTransfrom.anchoredPosition = new Vector2(0.0f, -comboButtonRectTransform.sizeDelta.y);
		scrollPanelGO.transform.SetParent(overlayGO.transform, false);
		scrollPanelRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, comboButtonRectTransform.sizeDelta.x);
		scrollPanelRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dropdownHeight);
		var scrollPanelScrollRect = scrollPanelGO.AddComponent<ScrollRect>();
		scrollPanelScrollRect.horizontal = false;
		scrollPanelScrollRect.elasticity = 0.0f;
		scrollPanelScrollRect.movementType = ScrollRect.MovementType.Clamped;
		scrollPanelScrollRect.inertia = false;
		scrollPanelScrollRect.scrollSensitivity = comboButtonRectTransform.sizeDelta.y;
		scrollPanelGO.AddComponent<Mask>();

		var scrollbarWidth = Items.Length - (HideFirstItem ? 1 : 0) > _itemsToDisplay ? _scrollbarWidth : 0.0f;

		var itemsGO = new GameObject("Items");
		itemsGO.transform.SetParent(scrollPanelGO.transform, false);
		itemsRectTransfrom = itemsGO.AddComponent<RectTransform>();
		itemsRectTransfrom.pivot = Vector2.up;
		itemsRectTransfrom.anchorMin = Vector2.up;
		itemsRectTransfrom.anchorMax = Vector2.one;
		itemsRectTransfrom.anchoredPosition = Vector2.right;
		itemsRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scrollPanelRectTransfrom.sizeDelta.x - scrollbarWidth);
		var itemsContentSizeFitter = itemsGO.AddComponent<ContentSizeFitter>();
		itemsContentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
		itemsContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
		var itemsGridLayoutGroup = itemsGO.AddComponent<GridLayoutGroup>();
		itemsGridLayoutGroup.cellSize = new Vector2(comboButtonRectTransform.sizeDelta.x - scrollbarWidth, comboButtonRectTransform.sizeDelta.y);
		itemsGridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
		itemsGridLayoutGroup.constraintCount = 1;
		scrollPanelScrollRect.content = itemsRectTransfrom;

		var scrollbarGO = new GameObject("Scrollbar");
		scrollbarGO.transform.SetParent(scrollPanelGO.transform, false);
		var scrollbarImage = scrollbarGO.AddComponent<Image>();
		scrollbarImage.sprite = Sprite_Background;
		scrollbarImage.type = Image.Type.Sliced;
		var scrollbarScrollbar = scrollbarGO.AddComponent<Scrollbar>();
		var scrollbarColors = new ColorBlock();
		scrollbarColors.normalColor = new Color32(128, 128, 128, 128);
		scrollbarColors.highlightedColor = new Color32(128, 128, 128, 178);
		scrollbarColors.pressedColor = new Color32(88, 88, 88, 178);
		scrollbarColors.disabledColor = new Color32(64, 64, 64, 128);
		scrollbarColors.colorMultiplier = 2.0f;
		scrollbarColors.fadeDuration = 0.1f;
		scrollbarScrollbar.colors = scrollbarColors;
		scrollPanelScrollRect.verticalScrollbar = scrollbarScrollbar;
		scrollbarScrollbar.direction = Scrollbar.Direction.BottomToTop;
		scrollbarRectTransfrom.pivot = Vector2.one;
		scrollbarRectTransfrom.anchorMin = Vector2.one;
		scrollbarRectTransfrom.anchorMax = Vector2.one;
		scrollbarRectTransfrom.anchoredPosition = Vector2.zero;
		scrollbarRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scrollbarWidth);
		scrollbarRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dropdownHeight);

		var slidingAreaGO = new GameObject("SlidingArea");
		slidingAreaGO.transform.SetParent(scrollbarGO.transform, false);
		slidingAreaRectTransform = slidingAreaGO.AddComponent<RectTransform>();
		slidingAreaRectTransform.anchoredPosition = Vector2.zero;
		slidingAreaRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
		slidingAreaRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dropdownHeight - scrollbarRectTransfrom.sizeDelta.x);

		var handleGO = new GameObject("Handle");
		handleGO.transform.SetParent(slidingAreaGO.transform, false);
		var handleImage = handleGO.AddComponent<Image>();
		handleImage.sprite = Sprite_UISprite;
		handleImage.type = Image.Type.Sliced;
		handleImage.color = new Color32(255, 255, 255, 150);
		scrollbarScrollbar.targetGraphic = handleImage;
		scrollbarScrollbar.handleRect = handleRectTransfrom;
		handleRectTransfrom.pivot = new Vector2(0.5f, 0.5f);
		handleRectTransfrom.anchorMin = new Vector2(0.5f, 0.5f);
		handleRectTransfrom.anchorMax = new Vector2(0.5f, 0.5f);
		handleRectTransfrom.anchoredPosition = Vector2.zero;
		handleRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scrollbarWidth);
		handleRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scrollbarWidth);

		Interactable = Interactable;

		if (Items.Length < 1)
			return;
		Refresh();
	}

	public void Refresh()
	{
		var itemsGridLayoutGroup = itemsRectTransfrom.GetComponent<GridLayoutGroup>();
		var itemsLength = Items.Length - (HideFirstItem ? 1 : 0);
		var dropdownHeight = comboButtonRectTransform.sizeDelta.y *  Mathf.Min(_itemsToDisplay, itemsLength);
		var scrollbarWidth = itemsLength > ItemsToDisplay ? _scrollbarWidth : 0.0f;
		scrollPanelRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dropdownHeight);
		scrollbarRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scrollbarWidth);
		scrollbarRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dropdownHeight);
		slidingAreaRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dropdownHeight - scrollbarRectTransfrom.sizeDelta.x);
		itemsRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scrollPanelRectTransfrom.sizeDelta.x - scrollbarWidth);
		itemsGridLayoutGroup.cellSize = new Vector2(comboButtonRectTransform.sizeDelta.x - scrollbarWidth, comboButtonRectTransform.sizeDelta.y);
		for (var i = itemsRectTransfrom.childCount - 1; i > -1; i--)
			DestroyImmediate(itemsRectTransfrom.GetChild(0).gameObject);
		for (var i = 0; i < Items.Length; i++)
		{
			if (HideFirstItem && i == 0)
				continue;
			var item = Items[i];
			item.OnUpdate = Refresh;
			var itemTransform = Instantiate(comboButtonRectTransform) as Transform;
			itemTransform.SetParent(itemsRectTransfrom, false);
			itemTransform.GetComponent<Image>().sprite = null;
			var itemText = itemTransform.Find("Text").GetComponent<Text>();
			itemText.text = item.Caption;
			if (item.IsDisabled)
				itemText.color = new Color32(174, 174, 174, 255);
			var itemImage = itemTransform.Find("Image").GetComponent<Image>();
			itemImage.sprite = item.Image;
			itemImage.color = item.Image == null ? new Color32(255, 255, 255, 0) : item.IsDisabled ? new Color32(255, 255, 255, 147) : new Color32(255, 255, 255, 255);
			var itemButton = itemTransform.GetComponent<Button>();
			itemButton.interactable = !item.IsDisabled;
			var index = i;
			itemButton.onClick.AddListener(
				delegate()
				{
					OnItemClicked(index);
					if (item.OnSelect != null)
						item.OnSelect();
				}
			);
		}
		RefreshSelected();
		UpdateComboBoxImages();
		UpdateGraphics();
		FixScrollOffset();
	}

	public void RefreshSelected()
	{
		var comboButtonImage = comboImageRectTransform.GetComponent<Image>();
		var item = SelectedIndex > -1 && SelectedIndex < Items.Length ? Items[SelectedIndex] : null;
		var includeImage = item != null && item.Image != null;
		comboButtonImage.sprite = includeImage ? item.Image : null;
		var comboButtonButton = comboButtonRectTransform.GetComponent<Button>();
		comboButtonImage.color = includeImage ? (Interactable ? comboButtonButton.colors.normalColor : comboButtonButton.colors.disabledColor) : new Color(1.0f, 1.0f, 1.0f, 0);
		UpdateComboBoxImage(comboButtonRectTransform, includeImage);
		comboTextRectTransform.GetComponent<Text>().text = item != null ? item.Caption : "";
		if (!Application.isPlaying)
			return;
		var i = 0;
		foreach (Transform child in itemsRectTransfrom)
		{
			comboButtonImage = child.GetComponent<Image>();
			comboButtonImage.color = SelectedIndex == i + (HideFirstItem ? 1 : 0) ? comboButtonButton.colors.highlightedColor : comboButtonButton.colors.normalColor;
			i++;
		}
	}

	private void UpdateComboBoxImages()
	{
		var includeImages = false;
		foreach (var item in Items)
		{
			if (item.Image != null)
			{
				includeImages = true;
				break;
			}
		}
		foreach (Transform child in itemsRectTransfrom)
			UpdateComboBoxImage(child, includeImages);
	}

	private void UpdateComboBoxImage(Transform comboButton, bool includeImage)
	{
		comboButton.Find("Text").GetComponent<RectTransform>().offsetMin = Vector2.right * (includeImage ? comboImageRectTransform.rect.width + 8.0f : 10.0f);
	}

	private void FixScrollOffset()
	{
		var selectedIndex = SelectedIndex + (HideFirstItem ? 1 : 0);
		if (selectedIndex < scrollOffset)
			scrollOffset = selectedIndex;
		else
			if (selectedIndex > scrollOffset + ItemsToDisplay - 1)
				scrollOffset = selectedIndex - ItemsToDisplay + 1;
		var itemsCount = Items.Length - (HideFirstItem ? 1 : 0);
		if (scrollOffset > itemsCount - ItemsToDisplay)
			scrollOffset = itemsCount - ItemsToDisplay;
		if (scrollOffset < 0)
			scrollOffset = 0;
		itemsRectTransfrom.anchoredPosition = new Vector2(0.0f, scrollOffset * rectTransform.sizeDelta.y);
	}
	
	private void ToggleComboBox(bool directClick)
	{
		overlayGO.SetActive(!overlayGO.activeSelf);
		if (overlayGO.activeSelf)
		{
			var curTransform = transform;
			do
			{
				curTransform.SetAsLastSibling();
			}
			while ((curTransform = curTransform.parent) != null);
			FixScrollOffset();
		}
		else
			if (directClick)
				scrollOffset = (int)Mathf.Round(itemsRectTransfrom.anchoredPosition.y / rectTransform.sizeDelta.y);
	}

	public void UpdateGraphics()
	{
		if (overlayGO != null)
		{
			var scrollbarWidth = Items.Length - (HideFirstItem ? 1 : 0) > ItemsToDisplay ? _scrollbarWidth : 0.0f;
			handleRectTransfrom.offsetMin = -scrollbarWidth / 2 * Vector2.one;
			handleRectTransfrom.offsetMax = scrollbarWidth / 2 * Vector2.one;
		}
		if (rectTransform.sizeDelta != buttonRectTransform.sizeDelta && buttonRectTransform.sizeDelta == comboButtonRectTransform.sizeDelta)
		{
			buttonRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rectTransform.sizeDelta.x);
			buttonRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rectTransform.sizeDelta.y);
			comboButtonRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rectTransform.sizeDelta.x);
			comboButtonRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rectTransform.sizeDelta.y);
			comboArrowRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rectTransform.sizeDelta.y);
			comboImageRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, comboImageRectTransform.rect.height);
			comboTextRectTransform.offsetMax = new Vector2(4.0f, 0.0f);
			if (overlayGO == null)
				return;
			scrollPanelRectTransfrom.SetParent(transform, false);
			scrollPanelRectTransfrom.anchoredPosition = new Vector2(0.0f, -comboButtonRectTransform.sizeDelta.y);
			scrollPanelRectTransfrom.SetParent(overlayGO.transform, false);
			scrollPanelRectTransfrom.GetComponent<ScrollRect>().scrollSensitivity = comboButtonRectTransform.sizeDelta.y;
			UpdateComboBoxImage(comboButtonRectTransform, Items[SelectedIndex].Image != null);
			Refresh();
		}
	}
}