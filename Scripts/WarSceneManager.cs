using UnityEngine;
using System.Collections;

public class WarSceneManager : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		StartCoroutine("DestroyScene");
	}

	IEnumerator DestroyScene()
	{
		yield return new WaitForSeconds(6);
		Destroy(gameObject);
		TurnManager.instance.tacticScene.SetActive(true);
		TurnManager.instance.tacticSceneloaded = true;
	}
}
