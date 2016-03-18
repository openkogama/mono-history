using UnityEngine;

public class CubeGunBulletObject : MonoBehaviour
{
	[SerializeField]
	private Bullet bullet;

	[SerializeField]
	private CubeBullet cubeBullet;

	public Bullet Bullet => bullet;

	public CubeBullet CubeBullet => cubeBullet;
}
