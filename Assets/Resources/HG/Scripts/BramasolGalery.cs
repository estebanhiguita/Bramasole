using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BramasolGalery : MonoBehaviour, IBeginDragHandler, IDragHandler {

	public List<Galery> galeria;
	public UnityEngine.UI.Image imagen;
	public Text titulo;
	public Text descripcion;

	public float tol;

	public Galery current
	{
		get {return galeria [currentIndex];}
	}

	// Use this for initialization
	void Start () 
	{
		SetUI ();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void SetUI ()
	{
		imagen.sprite = current.imagen;
		titulo.text = current.titulo;
		descripcion.text = current.descripcion;
	}
	

	int currentIndex = 0;

	void Switch (int dir)
	{
		if (galeria.Count < 2)
			return;

		currentIndex = MathMod (currentIndex + dir, galeria.Count);

		SetUI ();
	}

	void Previous ()
	{

	}

	Vector3 initialPosition;
	bool drag = true;
	#region IBeginDragHandler implementation
	void IBeginDragHandler.OnBeginDrag (PointerEventData eventData)
	{
		initialPosition = transform.InverseTransformPoint (eventData.worldPosition);
		drag = true;

	}
	#endregion

	#region IDragHandler implementation
	void IDragHandler.OnDrag (PointerEventData eventData)
	{
		if (! drag)
			return;


		var nextPosition = transform.InverseTransformPoint (eventData.worldPosition);

		var delta = nextPosition.x - initialPosition.x;

		if (Mathf.Abs (delta) >= tol)
		{
			Switch ((int)Mathf.Sign (delta));
			drag = false;
		}
	}
	#endregion

	static int MathMod(int a, int b) 
	{
		return (Mathf.Abs(a * b) + a) % b;
	}
}

[System.Serializable]
public class Galery
{
	public Sprite imagen;
	public string titulo;
	public string descripcion;
}
