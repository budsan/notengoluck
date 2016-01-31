using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Logic : MonoBehaviour {

	public static Logic ins;

	public const int playersLayer = 8;
	public const int fireLayer = 9;

	public GameObject PlayerObject;

	private bool[] m_playersActive = new[] { false, false, false, false };
	private GameObject[] m_spawns = new GameObject[4];
	private GameObject[] m_players = new GameObject[4];

	public bool[] PlayersActive { get { return m_playersActive; } }
	public GameObject[] Players { get { return m_players; } }

	// Use this for initialization
	void Awake ()
	{
		if (ins == null)
		{
			DontDestroyOnLoad(gameObject);
			ins = this;
			OnLevelWasLoaded(SceneManager.GetActiveScene().buildIndex);
		}
		else
		{
			DestroyImmediate(gameObject);
		}
	}

	void GameStarts()
	{
		m_spawns = GameObject.FindGameObjectsWithTag("Spawn");
		m_players = new GameObject[m_playersActive.Length]; 
		for (int i = 0; i < m_playersActive.Length; i++)
		{
			if (m_playersActive[i])
			{
				m_players[i] = Instantiate(PlayerObject);
				m_players[i].transform.position = m_spawns[i].transform.position;
				PlayerMovement mov = m_players[i].GetComponent<PlayerMovement>();
				mov.setPlayerId(i+1);
			}
			else
			{
				m_players[i] = null;
			}
		}
	}

	public void StartGame(bool[] players)
	{
		m_playersActive = players;
		SceneManager.LoadScene(1);
	}

	public void EndGame()
	{
		SceneManager.LoadScene(0);
	}

	public void OnLevelWasLoaded(int numscene)
	{
		switch(numscene)
		{
			case 0://menu
				Debug.Log("Menu started");
				break;
			
			default://game
				Debug.Log("Game started");
				GameStarts();
				break;
		}
	}
}
