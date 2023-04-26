using CookingPrototype.Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class StartWindow : MonoBehaviour
{
	[SerializeField] private Button StartButton = null;
	[SerializeField] private TMP_Text CountForWinText = null;
	
	
	private void Start() 
	{
		CountForWin();
		Time.timeScale = 0f;
		StartButton.onClick.AddListener(StartGame);
	}
	private void StartGame() 
	{
		gameObject.SetActive(false);
		Time.timeScale = 1f;
	}

	public void CountForWin() 
	{
		var gc = GameplayController.Instance;
		CountForWinText.text = $"{gc.OrdersTarget - 2}";
	}
}

