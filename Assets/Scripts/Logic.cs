using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Logic : MonoBehaviour {

	public static Logic ins;

	public int playersLayer;
	public int fireLayer;

	private bool[] m_players;
	public bool[] Players { get { return m_players; } }

	// Use this for initialization
	void Awake ()
	{
		if (ins == null)
		{
			DontDestroyOnLoad(gameObject);
			ins = this;
		}
		else
		{
			DestroyImmediate(gameObject);
		}
	}

	public void StartGame(bool[] players)
	{
		m_players = players;
		SceneManager.LoadScene(1);
	}

	public void EndGame()
	{
		SceneManager.LoadScene(0);
	}

	public void OnLevelLoad(int numscene)
	{
		switch(numscene)
		{
			case 0://menu
				Debug.Log("Menu started");
				break;
			case 1://game
				Debug.Log("Game started");
				break;
		}
	}
}
