using UnityEngine;

public class TestComboBox : MonoBehaviour 
{
	public ComboBox comboBox;
	public Sprite image;

	private void Start() 
	{
		var itemMakeBig = new ComboBoxItem("Make me big!");
		var itemMakeNormal = new ComboBoxItem("Normal", image, true);
		var itemMakeSmall = new ComboBoxItem("Make me small!");
		itemMakeBig.OnSelect += () =>
		{
			comboBox.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 180);
			comboBox.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 40);
			comboBox.UpdateGraphics();
			itemMakeBig.Caption = "Big";
			itemMakeBig.IsDisabled = true;
			itemMakeNormal.Caption = "Make me normal!";
			itemMakeNormal.IsDisabled = false;
			itemMakeSmall.Caption = "Make me small!";
			itemMakeSmall.IsDisabled = false;
		};
		itemMakeNormal.OnSelect += () =>
		{
			comboBox.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 160);
			comboBox.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 30);
			comboBox.UpdateGraphics();
			itemMakeBig.Caption = "Make me big!";
			itemMakeBig.IsDisabled = false;
			itemMakeNormal.Caption = "Normal";
			itemMakeNormal.IsDisabled = true;
			itemMakeSmall.Caption = "Make me small!";
			itemMakeSmall.IsDisabled = false;
		};
		itemMakeSmall.OnSelect += () =>
		{
			comboBox.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 160);
			comboBox.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 20);
			comboBox.UpdateGraphics();
			itemMakeBig.Caption = "Make me big!";
			itemMakeBig.IsDisabled = false;
			itemMakeNormal.Caption = "Make me normal!";
			itemMakeNormal.IsDisabled = false;
			itemMakeSmall.Caption = "Small";
			itemMakeSmall.IsDisabled = true;
		};
		comboBox.AddItems(itemMakeBig, itemMakeNormal, itemMakeSmall);
		comboBox.SelectedIndex = 1;
		comboBox.OnSelectionChanged += (int index) =>
		{
			Camera.main.backgroundColor = new Color32((byte)Random.Range(0, 256), (byte)Random.Range(0, 256), (byte)Random.Range(0, 256), 255);
		};
	}
}
