using UnityEngine;

public class Player : MonoBehaviour
{
	[SerializeField] Ball ball;
	MeshRenderer ballMesh;
	int keyCount = 0;
	int currentScore = 0;// score gained in the current stage. Added to total score if the player completes the stage, deleted if they fail.
	int totalScore = 0;// total score. When the player fails a stage, subtract from this. if it reaches 0, game over.
	int currentFruit = 0;// if the player collects x amount of fruit required to open a bonus stage, next level will be a bonus stage instead.
	bool isHeating = false;


	public int KeyCount { get => keyCount; set => keyCount = value; }
	public bool IsHeating { get => isHeating; set => isHeating = value; }
	public int CurrentFruit { get => currentFruit; set => currentFruit = value; }

	[SerializeField][Tooltip("Time in seconds before ball explodes due to overheating.")] float heatTime = 3f; // if player is on heat square, increment num until <= to this then kill player. If player moves off of heat square, decrement number till 0.
	float currentHeat = 1f;


	private void Awake()
	{
		ballMesh = ball.gameObject.GetComponentInChildren<MeshRenderer>();
	}


	void Update()
	{

		CheckTemperature(GameManager.Instance.gameTime);
	}






	// ---- slowly turn red until X seconds reached, then game over.
	//      if not heating transition from red back to white/clear.
	void CheckTemperature(float time)
	{
		if (!isHeating)
		{
			if (currentHeat > 0)
			{
				CoolDown(time);
			}
			else return;
		}
		else
		{
			HeatUp(time);

		}


		Debug.Log(Mathf.Round(currentHeat+1));
	}

	void HeatUp(float time)
	{
		if (currentHeat >= heatTime-1) return;
		if (!IsHeating) return;

		currentHeat += time;
		ballMesh.material.color = Color.Lerp(ballMesh.material.color, ball.HeatColor, time/heatTime);
	}
	void CoolDown(float time)
	{
		if (currentHeat < 0) { currentHeat = 0; return;}
		if (IsHeating) return;
		ballMesh.material.color = Color.Lerp(ballMesh.material.color, ball.DefaultColor, time / heatTime);
		currentHeat -= time;
	}
}
