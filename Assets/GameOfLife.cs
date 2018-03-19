using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

[Serializable]
public class LifeCell
{
	public GameObject root;
	public bool ifAlive;
	public bool gettingAlive;
	SpriteRenderer rend;

	public LifeCell(GameObject _root)
	{
		root = _root;
		ifAlive = false;
		gettingAlive = false;
		rend = root.GetComponent<SpriteRenderer>();
	}

	public void ToggleLife()
	{
		ifAlive = !ifAlive;
		gettingAlive = ifAlive;
		rend.color = (ifAlive) ? Color.black : Color.white;
	}
}

public class GameOfLife : MonoBehaviour
{
	public GameObject cell;
	public int automationScale;
	
	List<LifeCell> cells = new List<LifeCell>();
	int Generation = 0;
	
	void Start()
	{		
		Vector3 camPos = Camera.main.transform.position;
		Camera.main.transform.position = new Vector3(camPos.x + automationScale/2, camPos.y + automationScale/2, camPos.z);
		for(int i=0; i<automationScale; i++)
		{
			for(int j=0; j<automationScale; j++)
			{
				Vector2 pos = new Vector2(i, j);
				GameObject gb = GameObject.Instantiate(cell, pos, Quaternion.identity);
				gb.transform.parent = transform;
				gb.name = "cell" + cells.Count;
				cells.Add(new LifeCell(gb));
			}
		}
	}

	void OnGUI()
	{
		string lbl = "Automation Scale: " + automationScale + " Generation: " + Generation;
		GUIStyle style = new GUIStyle();
		style.alignment = TextAnchor.UpperRight;
		style.fontSize = 32;
		style.normal.textColor = Color.white;
		GUI.Label(new Rect(0, 10, Screen.width - 10, Screen.height - 10), lbl, style);
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Vector3 coord = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
			Vector2 pos = Camera.main.ScreenToWorldPoint(coord);
			int cX = Mathf.RoundToInt(pos.x);
			int cY = Mathf.RoundToInt(pos.y);
			if(cX>=0 && cX<automationScale && cY>=0 && cY<automationScale)
			{
				int sC = cX*automationScale + cY;
				cells[sC].ToggleLife();
			}
		}

		if(Input.GetKeyDown(KeyCode.Escape)) SceneManager.LoadScene(SceneManager.GetActiveScene().name);

		if(Input.GetKeyDown(KeyCode.Space))
		{
			Generation++;
			
			//Birth phase
			for(int j=0; j<automationScale; j++)
			{
				for(int i=0; i<automationScale; i++)
				{
					int sC = i*automationScale + j;					
					if(!cells[sC].ifAlive)
					{
						int n = GetNeighbours(i, j);						
						if(n == 3) cells[sC].gettingAlive = !cells[sC].ifAlive;
					}
				}
			}

			//Death phase
			for(int j=0; j<automationScale; j++)
			{
				for(int i=0; i<automationScale; i++)
				{
					int sC = i*automationScale + j;
					if(cells[sC].ifAlive)
					{
						int n = GetNeighbours(i, j);
						if(n < 2 || n > 3) cells[sC].gettingAlive = !cells[sC].ifAlive;
					}
				}
			}

			//Finalizing phase
			for(int j=0; j<automationScale; j++)
			{
				for(int i=0; i<automationScale; i++)
				{
					int sC = i*automationScale + j;
					if(cells[sC].ifAlive != cells[sC].gettingAlive) cells[sC].ToggleLife();
				}
			}
		}
	}

	int GetNeighbours(int x, int y)
	{
		int count = 0;
		try	{ if(cells[(x-1)*automationScale + y].ifAlive && x>0) count++; } catch {}
		try	{ if(cells[(x+1)*automationScale + y].ifAlive && x<automationScale-1) count++; } catch {}
		try	{ if(cells[x*automationScale + (y-1)].ifAlive && y>0) count++; } catch {}
		try	{ if(cells[x*automationScale + (y+1)].ifAlive && y<automationScale-1) count++; } catch {}
		try	{ if(cells[(x+1)*automationScale + (y+1)].ifAlive && x<automationScale-1 && y<automationScale-1) count++; } catch {}
		try	{ if(cells[(x-1)*automationScale + (y+1)].ifAlive && x>0 && y<automationScale-1) count++; } catch {}
		try	{ if(cells[(x+1)*automationScale + (y-1)].ifAlive && x<automationScale-1 && y>0) count++; } catch {}
		try	{ if(cells[(x-1)*automationScale + (y-1)].ifAlive && x>0 && y>0) count++; } catch {}

		return count;
	}
}