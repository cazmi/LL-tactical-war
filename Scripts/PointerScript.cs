using UnityEngine;
using System.Collections;

public class PointerScript : MonoBehaviour {
	public float moveSpeed = 20f;
	public float tileSize = 4f;

	private enum Orientation {
		Horizontal,
		Vertical
	};
	private Orientation gridOrientation = Orientation.Horizontal;
	private Vector2 input;
	private bool isMoving = false;
	private Vector3 startPosition;
	private Vector3 endPosition;
	private float t;
	private float factor;
	
	void Start()
	{
		factor = 1f;
	}
	
	public void AdjustPosition(Vector3 position)
	{
		Vector3 temp = position;
		temp.y = 10f;
		transform.position = temp;
	}
	
	public void Update() {
		if (!isMoving) {
			input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
			
			if (Mathf.Abs(input.x) > Mathf.Abs(input.y)) {
				input.y = 0;
			} else {
				input.x = 0;
			}
			
			
			if (input != Vector2.zero) {
				StartCoroutine(move(transform));
			}
		}
	}
	
	public IEnumerator move(Transform transform) {
		isMoving = true;
		startPosition = transform.position;
		t = 0;
		
		if(gridOrientation == Orientation.Horizontal) {
			endPosition = new Vector3(startPosition.x + System.Math.Sign(input.x) * tileSize,
			                          startPosition.y, startPosition.z + System.Math.Sign(input.y) * tileSize);
		} else {
			endPosition = new Vector3(startPosition.x + System.Math.Sign(input.x) * tileSize,
			                          startPosition.y + System.Math.Sign(input.y) * tileSize, startPosition.z);
		}
		
		while (t < 1f) {
			t += Time.deltaTime * (moveSpeed/tileSize) * factor;
			transform.position = Vector3.Lerp(startPosition, endPosition, t);
			yield return null;
		}
		
		isMoving = false;
		yield return 0;
	}
}
