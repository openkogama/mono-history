using UnityEngine;

public class NPCHealth : MonoBehaviour
{
	private float curHealth;

	public float MaxHealth = 10f;

	public bool isAlive => curHealth > 0f;

	public float Health => curHealth;

	private void Start()
	{
		curHealth = MaxHealth;
	}

	public void Interact(float amount)
	{
		curHealth += amount;
		if (curHealth > MaxHealth)
		{
			curHealth = MaxHealth;
		}
		if (curHealth < 0f)
		{
			curHealth = 0f;
		}
		Debug.Log("hit: " + curHealth);
	}
}
