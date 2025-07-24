using System;
using UnityEngine;


[System.Flags]
public enum PlaceCondition
{
	PlaceAtTop = 1 << 0,
	SameColor = 1 << 1,
	FirstOnly = 1 << 2
}

[Serializable]
public class CubeConfig
{
	public CubeConfig(Sprite sprite)
	{
		Id = sprite.name;
		Sprite = sprite;
		AdditionalColor = Color.white;
		PlaceCondition = PlaceCondition.PlaceAtTop;
	}

	public string Id;
	public Sprite Sprite;
	public Color AdditionalColor = Color.white;
	public PlaceCondition PlaceCondition;

	public CubeConfig Copy(Sprite sprite)
	{
		return new CubeConfig(sprite)
		{
			Id = Id,
			AdditionalColor = AdditionalColor,
			PlaceCondition = PlaceCondition
		};
	}
}