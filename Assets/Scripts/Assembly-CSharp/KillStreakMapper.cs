using UnityEngine;

public class KillStreakMapper : MonoBehaviour
{
	public Renderer killstreakRenderer;

	public UIAtlas killstreakAtlas;

	private Vector2 atlasSize;

	public Animator killstreakAnimator;

	private void Start()
	{
		atlasSize = new Vector2(killstreakAtlas.texture.width, killstreakAtlas.texture.height);
	}

	public void GetStreak(string spriteName)
	{
		int x = killstreakAtlas.GetSprite(spriteName).x;
		int y = killstreakAtlas.GetSprite(spriteName).y;
		int height = killstreakAtlas.GetSprite(spriteName).height;
		int width = killstreakAtlas.GetSprite(spriteName).width;
		float y2 = (float)height / (float)width;
		killstreakRenderer.transform.localScale = new Vector3(1f, y2, 1f);
		killstreakRenderer.material.mainTextureScale = new Vector2((float)width / atlasSize.x, (float)height / atlasSize.y);
		killstreakRenderer.material.mainTextureOffset = new Vector2((float)x / atlasSize.x, (atlasSize.y - (float)(y + height)) / atlasSize.y);
		if (killstreakAnimator != null)
		{
			killstreakAnimator.SetTrigger("play");
		}
	}
}
