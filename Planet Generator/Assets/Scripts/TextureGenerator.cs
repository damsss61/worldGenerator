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
		int width = heightMap.values.GetLength (0);
		int height = heightMap.values.GetLength (1);

        int inc = 0;
		Color[] colourMap = new Color[(width-2) * (height-2)];
		for (int x = 1; x < height-1; x++) {
			for (int y = 1; y < width-1; y++) {
                colourMap[inc] = Color.Lerp(Color.black, Color.white, Mathf.InverseLerp(heightMap.minValue, heightMap.maxValue, heightMap.values[y, x]));
                inc += 1;
			}
		}

		return TextureFromColourMap (colourMap, width-2, height-2);
	}

}
