﻿using UnityEngine;
using System.Collections;
using System;

public class MenuControl : MonoBehaviour
{
	enum Status
	{
		Menu,
		WaitingPlayers
	}

	public UnityEngine.EventSystems.EventSystem system;
	public GameObject buttons;
	public GameObject waiting;

	public GameObject[] wait = new GameObject[4];
	public GameObject[] canc = new GameObject[4];

	private Status m_status;
	public bool[] m_playerActive = new[] { false, false, false, false };

	public void PressedStart()
	{
		m_status = Status.WaitingPlayers;
	}

	public void PressedExit()
	{
		Application.Quit();
	}

	public void PressedBack()
	{
		m_status = Status.Menu;
		for (int i = 0; i < m_playerActive.Length; i++)
			m_playerActive[i] = false;

		m_status = Status.Menu;
	}

	public void Start()
	{
		m_status = Status.Menu;
	}

	public void Update()
	{
		if (system == null)
		{
			Debug.LogError("BUTTONS IS UNASIGNED");
			return;
		}

			if (buttons == null)
		{
			Debug.LogError("BUTTONS IS UNASIGNED");
			return;
		}

		if (waiting == null)
		{
			Debug.LogError("WAITING IS UNASIGNED");
			return;
		}

		switch (m_status)
		{
			case Status.Menu:
				buttons.SetActive(true);
				waiting.SetActive(false);
				break;
			case Status.WaitingPlayers:
				buttons.SetActive(false);
				waiting.SetActive(true);

				for (int i = 0; i < m_playerActive.Length; i++)
				{
					if (Input.GetButtonDown("A" + (i+1)))
					{
						if (!m_playerActive[i])
						{
							m_playerActive[i] = true;
						}
						else
							StartGame();
					}

					if (Input.GetButtonDown("B" + (i + 1)))
					{
						if (!m_playerActive[i])
							PressedBack();
						else
							m_playerActive[i] = false;
					}
				}

				for (int i = 0; i < m_playerActive.Length; i++)
				{
					wait[i].SetActive(!m_playerActive[i]);
					canc[i].SetActive(m_playerActive[i]);
				}

				break;
		}
	}

	private void StartGame()
	{
		
	}
}
