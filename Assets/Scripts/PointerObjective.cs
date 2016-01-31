using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class PointerObjective : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
	public GameObject Pointer;

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (Pointer == null)
			return;

		Pointer.transform.SetParent(transform, false);
	}

	public void OnPointerDown(PointerEventData eventData)
	{

	}
}
