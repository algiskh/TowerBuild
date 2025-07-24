using Sirenix.OdinInspector;
using UnityEngine;

public interface IGameValuesHolder
{
	float PlacementDeviation { get; }

#if UNITY_EDITOR
	void ApplyParsedData();
#endif
}

[CreateAssetMenu(fileName = "GameValuesHolder", menuName = "ScriptableObjects/GameValuesHolder")]
public class GameValuesHolder : ScriptableObject, IGameValuesHolder
{
	[InfoBox("Maximum allowed deviation for cube placement from the center of the previous cube.")]
	[PropertyRange(1, 100)]
	[SuffixLabel("%", true)]
	[LabelText("Placement Deviation")]
	[SerializeField]
	private float _placementDeviation = 10f;
	public float PlacementDeviation => _placementDeviation / 100;

#if UNITY_EDITOR
	public void ApplyParsedData()
	{
		// logic for parsed data 
		UnityEditor.EditorUtility.SetDirty(this);
	}
#endif
}
