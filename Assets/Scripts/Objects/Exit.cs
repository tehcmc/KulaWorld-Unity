using UnityEngine;

public class Exit : BaseObject
{
	[SerializeField] int keysToUnlock = 1;

	Player player;

	RecolourMaterial recolourer;

	bool isOpen = false;

	private void Awake()
	{
		recolourer = GetComponentInChildren<RecolourMaterial>();
		Key.OnCollectEvent += TryOpen;
		recolourer.Recolour(Color.red);
	}



	private void Start()
	{
		player = FindObjectOfType<Player>();
	}

	void TryOpen(object sender, CollectibleArgs args)
	{
		if (isOpen) return;

		if (!player) return;

		if (args == null) return;

		if (args.Name != "key") return;

		if (player.KeyCount < keysToUnlock) return;

		Open();

	}

	void Open()
	{
		isOpen = true;
		Debug.Log("open");
		recolourer.Recolour(Color.green);
	}

	void ExitLevel()
	{
		//do exit stuff here
		Debug.Log("exit!");
		Destroy(gameObject);
	}


	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Ball")) return;
		if (!isOpen) return;

		ExitLevel();
		// change levels. If all fruit collected, store next "real" level and move on to bonus stage. else move to next level (in a list?) 
	}
}
