﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Face : ScriptableObject {

	[SerializeField]
	private int[] vertexIndex;
	[SerializeField]
	private int[] finalVertexListIndexes;
	public int[] VertexIndex {
		get {
			return vertexIndex;
		}
		set {
			vertexIndex = value;
			
			finalVertexListIndexes = new int[vertexIndex.Length];
			for(int i = 0; i< vertexIndex.Length; i++){
				finalVertexListIndexes[i] = finalVertexList.Count;
				finalVertexList.Add(sharedVertex[vertexIndex[i]]);
			}
		}
	}

	[SerializeField]
	private Vector3[] sharedVertex;
	public Vector3[] SharedVertex {
		get {
			return sharedVertex;
		}
		set {
			sharedVertex = value;
		}
	}
	[SerializeField]
	private List<Vector3> finalVertexList;
	public List<Vector3> FinalVertexList {	
		get {
			return finalVertexList;
		}
		set {
			finalVertexList = value;
		}
	}

	[SerializeField]
	private Texture2D texture;
	public Texture2D Texture {
		get {
			return texture;
		}
		set {
			texture = value;
		}
	}
	[SerializeField]
	private IsoTexture textureMapping;
	public IsoTexture TextureMapping {
		get {
			return textureMapping;
		}
		set {
			textureMapping = value;
		}
	}
	[SerializeField]
	private int[] triangles;
	public int[] Triangles {
		get {
			return triangles;
		}
	}
	[SerializeField]
	private Vector2[] uvs;
	public Vector2[] Uvs {
		get {
			return uvs;
		}
	}
	
	public void regenerateTriangles(){
		
		this.triangles = new int[6];
		
		triangles[0] = finalVertexListIndexes[0];
		triangles[1] = finalVertexListIndexes[2];
		triangles[2] = finalVertexListIndexes[1];
		
		if(vertexIndex.Length == 4){
			triangles[3] = finalVertexListIndexes[2];
			triangles[4] = finalVertexListIndexes[0];
			triangles[5] = finalVertexListIndexes[3];
		}
		
	}
	
	public void regenerateUVs(Rect textureRect){
		
		this.uvs = new Vector2[vertexIndex.Length];
		
		if(vertexIndex.Length == 4){
			Vector2 topLeft = new Vector2(textureRect.x, textureRect.y + textureRect.height);
			Vector2 topRight = new Vector2(textureRect.x + textureRect.width, textureRect.y + textureRect.height);
			Vector2 botLeft = new Vector2(textureRect.x, textureRect.y);
			Vector2 botRight = new Vector2(textureRect.x + textureRect.width, textureRect.y);
			
			int cornerBotLeft 	= 0,
				cornerBotRight 	= 1,
				cornerTopRight	= 2,
				cornerTopLeft	= 3;
			
			if(textureMapping != null){
				cornerBotLeft = (cornerBotLeft + textureMapping.Rotation)%4;
				cornerBotRight = (cornerBotRight + textureMapping.Rotation)%4;
				cornerTopLeft = (cornerTopLeft + textureMapping.Rotation)%4;
				cornerTopRight = (cornerTopRight + textureMapping.Rotation)%4;
			}
			
			uvs[cornerBotLeft] = botLeft;
			uvs[cornerBotRight] = botRight;
			uvs[cornerTopRight] = topRight;
			uvs[cornerTopLeft] = topLeft;
			
			if(textureMapping != null){
				uvs[cornerBotLeft] += new Vector2(0,textureRect.height * (1f - (textureMapping.getYCorner() / (textureMapping.getTexture().height*1.0f))));
				uvs[cornerBotRight] -= new Vector2(textureRect.width * (1f - (textureMapping.getOppositeXCorner() / (textureMapping.getTexture().width*1.0f))), 0);
				uvs[cornerTopRight] -= new Vector2(0, textureRect.height * (textureMapping.getOppositeYCorner() / (textureMapping.getTexture().height*1.0f)));
				uvs[cornerTopLeft] += new Vector2(textureRect.width * (textureMapping.getXCorner() / (textureMapping.getTexture().width*1.0f)),0); 
			}
		}else{
			for(int i = 0; i<vertexIndex.Length; i++)
				uvs[i] = new Vector2(0,0);
		}
	}

	/* ##############################################
	 * 					Bounds related
	 * */

	[SerializeField]
	private bool boundsGenerated = false;
	[SerializeField]
	private Bounds bounds;
	private void generateBounds(){
		if(sharedVertex == null)
			return;
		
		Mesh meshecilla = new Mesh();
		Vector3[] puntos = new Vector3[vertexIndex.Length];
		for(int i = 0; i< puntos.Length; i++){
			puntos[i] = sharedVertex[vertexIndex[i]];
		}
		int[] trianglecillos;
		if(vertexIndex.Length == 3){
			trianglecillos = new int[3];
			trianglecillos[0] = 0;
			trianglecillos[1] = 2;
			trianglecillos[2] = 1;
		}else{
			trianglecillos = new int[6];
			
			trianglecillos[0] = 0;
			trianglecillos[1] = 2;
			trianglecillos[2] = 1;
			trianglecillos[3] = 2;
			trianglecillos[4] = 0;
			trianglecillos[5] = 3;
		}
		
		meshecilla.vertices = puntos;
		meshecilla.triangles = trianglecillos; 
		meshecilla.RecalculateBounds();
		
		bounds = meshecilla.bounds;
		boundsGenerated = true;
		
		/*bounds = new Bounds();
		foreach(int i in vertexIndex)
			bounds.Encapsulate(sharedVertex[i]);*/
	}
	
	public bool contains(Vector3 point){
		if(!boundsGenerated)
			generateBounds();
		return bounds.Contains(point);
	}


	/* #########################################################
	 * 					ENTITIES THINGS
	 * */

	[SerializeField]
	private List<Entity> entities;
	public List<Entity> Entities{get;set;}

	/* #########################################################
	 * 					DECORATION THINGS
	 * */

	[SerializeField]
	private List<Decoration> decorations;

	public void addDecoration(GameObject dec, IsoDecoration iDec, float x, float y, Cell father){
		if (decorations == null)
			decorations = new List<Decoration> ();

		Decoration dr = dec.GetComponent<Decoration> ();
		dr.X = x; dr.Y = y; dr.Father = father; dr.IsoDec = iDec;
		this.decorations.Add(dr);
	}

	public void removeDecoration(Decoration d){
		if (decorations == null)
			decorations = new List<Decoration> ();
		else {
			if(decorations.Contains(d))
				decorations.Remove(d);
		}
	}

	// ################## Scriptable object methods #####################

	public Face(){}

	public void OnLoad(){
		DontDestroyOnLoad(this);
	}

};

