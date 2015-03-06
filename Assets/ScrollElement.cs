using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollElement : MonoBehaviour, IDragHandler, IPointerDownHandler
{

	ScrollRect scrollRect;
	Vector2 pointerOffset;

	// Use this for initialization
	void Start () 
	{
		scrollRect = GetComponentInParent<ScrollRect> ();
	}

	public void OnPointerDown (PointerEventData data) 
	{
		RectTransformUtility.ScreenPointToLocalPointInRectangle 
		(
				scrollRect.content,
				data.position, 
				data.pressEventCamera, 
				out pointerOffset
		);
	}
	
	public void OnDrag (PointerEventData data) {
		
		Vector2 localPointerPosition;
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle
		(
			scrollRect.transform as RectTransform, data.position, data.pressEventCamera, out localPointerPosition
		)) 
		{
			scrollRect.content.localPosition = WithY(scrollRect.content.localPosition,(localPointerPosition - pointerOffset).y);

		}
	}

	public static Vector3 WithY (Vector3 vector, float value) {
		return new Vector3(
			vector.x,
			value,
			vector.z);
	}
}
