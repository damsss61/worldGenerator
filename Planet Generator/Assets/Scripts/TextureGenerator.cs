using UnityEngine;
using System.Collections;

public static class TextureGenerator {

	public static Texture2D TextureFromColourMap(Color[] colourMap, int width, int height) {
		Texture2D texture = new Texture2D (width, height);
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.SetPixels (colourMap);
		texture.Apply ();
		return texture;
	}


	public static Texture2D TextureFromHeightMap(HeightMap heightMap) {
		int width = heightMap.values.GetLength (0)-2;
		int height = heightMap.values.GetLength (1)-2;
        

		Color[] colourMap = new Color[width * height];
		for (int x = 0; x < height; x++) {
			for (int y = 0; y < width; y++) {
                colourMap[x * width + y] = Color.Lerp(Color.black, Color.white, Mathf.InverseLerp(heightMap.minValue, heightMap.maxValue, heightMap.values[width-y-1, width-x-1]));
			}
		}

		return TextureFromColourMap (colourMap, width, height);
	}

}
