using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoidScript;

public class BoidCreator : MonoBehaviour
{
    public int nbOfBoids;
    public int PredatorCount;

    public Vector2 speedLim;
    public Vector2 Frame;

    public Vector2 FlockStat;
    public Vector2 AlignStat;
    public Vector2 AvoidStat;
    public Vector2 PredatorStat;
    public Vector2 BounceStat;

    public Sprite boidSprite;
    public Material boidMat;
    public Material predMat;

    private Boid[] boidsGroup;

    // Start is called before the first frame update
    void Start()
    {
        boidsGroup = new Boid[nbOfBoids];
        for (int i = 0; i < nbOfBoids; i++)
        {
            Boid boid = new Boid(i, Frame, boidSprite, boidMat);
            boidsGroup[i] = boid;
        }
        foreach (Boid boid in boidsGroup)
        {
            boid.StartBoid(boidsGroup, PredatorCount, predMat);
        }

    }

    // Update is called once per frame
    void Update()
    {
        foreach (Boid boid in boidsGroup)
        {
            boid.UpdateBoid(speedLim, FlockStat, AlignStat, AvoidStat, PredatorStat, BounceStat);
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(0,30, 50, 30), "Marge: ");
        BounceStat.x = GUI.HorizontalSlider(new Rect(35, 25, 200, 30), BounceStat.x, 0, 1);
    }
}
