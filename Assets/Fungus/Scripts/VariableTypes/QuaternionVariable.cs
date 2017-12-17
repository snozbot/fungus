using UnityEngine;


namespace Fungus
{
    /// <summary>
    /// Quaternion variable type.
    /// </summary>
    [VariableInfo("Other", "Quaternion", isPreviewedOnly:true)]
    [AddComponentMenu("")]
	[System.Serializable]
	public class QuaternionVariable : VariableBase<Quaternion>
	{ }

	/// <summary>
	/// Container for a Quaternion variable reference or constant value.
	/// </summary>
	[System.Serializable]
	public struct QuaternionData
	{
		[SerializeField]
		[VariableProperty("<Value>", typeof(QuaternionVariable))]
		public QuaternionVariable quaternionRef;

		[SerializeField]
		public Quaternion quaternionVal;

		public static implicit operator Quaternion(QuaternionData QuaternionData)
		{
			return QuaternionData.Value;
		}

		public QuaternionData(Quaternion v)
		{
			quaternionVal = v;
			quaternionRef = null;
		}

		public Quaternion Value
		{
			get { return (quaternionRef == null) ? quaternionVal : quaternionRef.Value; }
			set { if (quaternionRef == null) { quaternionVal = value; } else { quaternionRef.Value = value; } }
		}

		public string GetDescription()
		{
			if (quaternionRef == null)
			{
				return quaternionVal.ToString();
			}
			else
			{
				return quaternionRef.Key;
			}
		}
	}
}